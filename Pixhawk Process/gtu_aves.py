import socket
from dronekit import Vehicle, Command, connect, LocationGlobalRelative, VehicleMode
import time
from pymavlink import mavutil
from pymavlink.dialects.v10 import ardupilotmega
import threading
from functools import partial

import paho.mqtt.client as mqtt

class MqttCommunication:
    
    def __init__(self):
        self.message = None
        self.mqttc = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2)
        self.mqttc.on_connect = self.on_connect
        self.mqttc.on_message = self.on_message

    def on_connect(self, client, userdata, flags, reason_code, properties):
        print(f"Connected with result code {reason_code}")
        client.subscribe("connection")

    def on_message(self, client, userdata, msg):
        self.message = str(msg.payload.decode())
        print(self.message)

    def start_communication(self):
        threading.Thread(target=self.communication_thread).start()

    def communication_thread(self):
        self.mqttc.connect("localhost", 1883, 60)
        self.mqttc.loop_forever()
        
    def send_message(self, topic, payload):
        self.mqttc.publish(topic, payload)

    def stop_communication(self):
        # Bu metodu kullanarak iletişim threadini durdurabilirsiniz.
        self.mqttc.disconnect()
    
    def reset_message(self):
        self.message = None
        
    def wait_for_message(self):
        self.message = None
        while(self.message == None):
            time.sleep(0.5)


def listener(self, name, value, conn):
        conn.send_message("listener", f"{name}: {value}")     

class UAV:
    
    # HOST = '127.0.0.1'  # localhost
    # PORT = 65449        # Port numarası
    
    ##################################################################
    
    def __init__(self, connection_string, connection):
        try:
            self.vehicle = connect(connection_string, timeout=30, baud=57600)
            self.add_listeners(connection)
            self.add_ekf_status_report_listener(connection)
            self.add_statustext_listener(connection)
            self.add_heartbeat_listener(connection)
        except:
            exit(1)
    
    ##################################################################
    
    def mqtt_communication(self):
        while True:
            connection.wait_for_message()
            message = connection.message
            if message == "arm":
                self.arm()   
            elif message == "disarm":
                self.disarm()
            else:
                print("Invalid message")
    
    ##################################################################
        
    def add_statustext_listener(self, connection):
        @self.vehicle.on_message('STATUSTEXT')
        def message_handler(self, name, message):
            severity = message.severity
            text = message.text
            
            # Write the data to MQTT
            payload = f"Severity: {severity}, Text: {text}"
            print(payload)
            connection.send_message("listener", payload)
            
    ##################################################################

    def add_ekf_status_report_listener(self, connection):
        @self.vehicle.on_message('EKF_STATUS_REPORT')
        def listener(self, name, message):
            _ekf_poshorizabs = (message.flags & ardupilotmega.EKF_POS_HORIZ_ABS) > 0
            _ekf_constposmode = (message.flags & ardupilotmega.EKF_CONST_POS_MODE) > 0
            _ekf_predposhorizabs = (message.flags & ardupilotmega.EKF_PRED_POS_HORIZ_ABS) > 0
            self.notify_attribute_listeners('ekf_ok', self.ekf_ok, cache=True)
            
            # Write the data to MQTT
            payload = f"ekf_poshorizabs: {_ekf_poshorizabs}, ekf_constposmode: {_ekf_constposmode}, ekf_predposhorizabs: {_ekf_predposhorizabs}"
            connection.send_message("listener", payload)
            
    ##################################################################
            
    def add_heartbeat_listener(self, connection):
        @self.vehicle.on_message('HEARTBEAT')
        def listener(self, name, m):                    
            armed = m.base_mode & mavutil.mavlink.MAV_MODE_FLAG_SAFETY_ARMED != 0
        
            # Determine if the vehicle is armed or disarmed
            arm_status = "Armed" if armed else "Disarmed"
            
            # Retrieve and format flight mode
            flight_mode = mavutil.mode_string_v10(m)
            
            payload = f"Status:{arm_status}, Flight Mode:{flight_mode}"
            
            connection.send_message("listener", payload)
        
        
            # armed = m.base_mode & mavutil.mavlink.MAV_MODE_FLAG_SAFETY_ARMED != 0
            # autopilot_type = mavutil.mavlink.enums["MAV_AUTOPILOT"].get(m.autopilot)
            # vehicle_type = mavutil.mavlink.enums["MAV_TYPE"].get(m.type)
            # flight_mode = mavutil.mavlink.enums["MAV_MODE_FLAG"].get(m.base_mode)
            # system_status = mavutil.mavlink.enums["MAV_STATE"].get(m.system_status)

            # payload = f"Armed: {armed}, Autopilot Type: {autopilot_type}, Vehicle Type: {vehicle_type}, Flight Mode: {flight_mode}, System Status: {system_status}"
            
            # print(payload)
            # connection.send_message("listener", payload)
            
    ##################################################################
    
    def add_listeners(self, connection):
        listener_mqtt = partial(listener, conn=connection)
    
        self.vehicle.add_attribute_listener('attitude', listener_mqtt)
        self.vehicle.add_attribute_listener('velocity', listener_mqtt)
        self.vehicle.add_attribute_listener('groundspeed', listener_mqtt)
        self.vehicle.add_attribute_listener('airspeed', listener_mqtt)
        self.vehicle.add_attribute_listener('heading', listener_mqtt)
        self.vehicle.add_attribute_listener('location.global_relative_frame', listener_mqtt)
        self.vehicle.add_attribute_listener('location.global_frame', listener_mqtt)
                
    ##################################################################  
    
    def ekf_ok(self):
        return self._ekf_poshorizabs and self._ekf_predposhorizabs and not self._ekf_constposmode
    
    def get_ekf_ok(self):
        return self.ekf_ok()
    
    ##################################################################  
    
    def arm(self):
        print("Arming motors")
        # Copter should arm in GUIDED mode
        self.vehicle.mode = VehicleMode("GUIDED")
        self.vehicle.armed = True
        
    def disarm(self):
        print("Disarming motors")
        self.vehicle.armed = False
                
    def arm_and_takeoff(self,aTargetAltitude):
        """
        Arms vehicle and fly to aTargetAltitude.
        """

        print("Basic pre-arm checks")
        # Don't try to arm until autopilot is ready
        while not self.vehicle.is_armable:
            print(" Waiting for vehicle to initialise...")
            time.sleep(1)

        print("Arming motors")
        # Copter should arm in GUIDED mode
        self.vehicle.mode = VehicleMode("GUIDED")
        self.vehicle.armed = True

        # Confirm vehicle armed before attempting to take off
        while not self.vehicle.armed:
            print(" Waiting for arming...")
            time.sleep(1)

        print("Taking off!")
        self.vehicle.simple_takeoff(aTargetAltitude)  # Take off to target altitude

        # Wait until the vehicle reaches a safe height before processing the goto
        #  (otherwise the command after Vehicle.simple_takeoff will execute
        #   immediately).
        while True:
            print(" Altitude: ", self.vehicle.location.global_relative_frame.alt)
            # Break and return from function just below target altitude.
            if self.vehicle.location.global_relative_frame.alt >= aTargetAltitude * 0.95:
                print("Reached target altitude")
                break
            time.sleep(1)
            
    ##################################################################
    
    def download_mission(self):
        """
        Download the current mission from the vehicle.
        """
        cmds = self.vehicle.commands
        cmds.download()
        cmds.wait_ready() # wait until download is complete.
    
    ##############################################################################################

    def add_waypoint(self,wp_Target_Latitude, wp_Target_Longtitude, wp_Target_Altitude):
        add_wp_flag = False
        cmds = self.vehicle.commands
        cmds.download()
        cmds.wait_ready()

        cmd = Command(0, 0, 0, mavutil.mavlink.MAV_FRAME_GLOBAL_RELATIVE_ALT, mavutil.mavlink.MAV_CMD_NAV_WAYPOINT, 0, 0, 
                    0, 0, 0, 0, wp_Target_Latitude, wp_Target_Longtitude, wp_Target_Altitude)

        cmds.add(cmd)
        cmds.upload()
                
        return
    
    ##############################################################################################

    def clear_mission(self):
        """
        Clear the current mission.
        """
        cmds = self.vehicle.commands
        cmds.clear()
        cmds.upload()

        # After clearing the mission you MUST re-download the mission from the vehicle
        # before vehicle.commands can be used again
        # (see https://github.com/dronekit/dronekit-python/issues/230)

        cmds.download()
        cmds.wait_ready()
    
    ##############################################################################################
    
    def close(self):
        self.vehicle.close()
        
    ##############################################################################################
            


if __name__ == "__main__":

    connection = MqttCommunication()
    connection.start_communication()
    connection.wait_for_message()
    connection_string = connection.message
    uav = UAV(connection_string, connection)
    
    # uav = UAV("COM22", connection)
    
    communication_thread = threading.Thread(target=uav.mqtt_communication)
    communication_thread.start()
    
    
    uav.clear_mission()
    print("CLEARED")
    
    
    # # ÇALIŞIYOR
    # while True:
    #     comm.send_message("12")
    #     time.sleep(3)
    
    # comm.wait_for_message()
    # message = comm.message
    # splmessage = message.split()
    # total = splmessage[0]
    
    
    # # BU DECODE KISMI DÜZELTİLECEK
    # index = 1

    # comm.stop_communication()
    # uav.close()
    
    # message = uav.take_message()
    # uav.clear_mission()
    
    # total = (len(message)-2) / 4
    
    # for i in range(int(total)):
    #     lat_str = message[4 + i*4]
    #     lat_str = lat_str.replace(',', '.')
    #     lon_str = message[5 + i*4]
    #     lon_str = lon_str.replace(',', '.')
        
    #     lat = float(lat_str)
    #     lon = float(lon_str)
    #     print(lat, lon)
    #     uav.add_waypoint(lat, lon, 0)

    ##################################################################
        
    # def connection_start(self): 
    #     # Soket oluşturma
    #     with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    #         s.bind((self.HOST, self.PORT))  # Belirtilen HOST ve PORT'a bağlanma
    #         s.listen()  # Bağlantıları dinleme
    #         print(f"Sunucu {self.HOST}:{self.PORT} üzerinde dinleniyor...")
    #         self.conn, self.addr = s.accept()  # Bağlantıyı kabul etme
            
    # ##################################################################
            
    # def close_connection(self):
    #     if self.conn:
    #         self.conn.close()
    #         print("Bağlantı kapatıldı.")
            
    # ##################################################################

    # def take_message(self):
    #     self.connection_start()
    #     with self.conn:
    #         print('Bağlantı adresi:', self.addr)
    #         while True:
    #             data = self.conn.recv(1024)  # İstemciden veri alma
    #             print(f"Gelen veri: {data.decode()}")  # Veriyi ekrana yazdırma
    #             message = data.decode()
    #             if "COM" in message:
    #                 self.close_connection()
    #                 return message
    #             elif "Total" in message:
    #                 splitted = message.split()
    #                 self.close_connection()
    #                 return splitted
                    
                
    ##################################################################    
    
    # # The callback for when the client receives a CONNACK response from the server.
    # def on_connect(self, client, userdata, flags, reason_code, properties):
    #     print(f"Connected with result code {reason_code}")
    #     # Subscribing in on_connect() means that if we lose the connection and
    #     # reconnect then subscriptions will be renewed.
    #     client.subscribe("Test")
    
    # ##################################################################    

    # # The callback for when a PUBLISH message is received from the server.
    # def on_message(self, client, userdata, msg):
    #     self.message = str(msg.payload.decode())
    #     print(self.message)
    
    # ##################################################################    
        
    # def communication(self):
    #     mqttc = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2)
    #     mqttc.on_connect = self.on_connect
    #     mqttc.on_message = self.on_message

    #     mqttc.connect("localhost", 1883, 60)
    #     mqttc.loop_forever()
    
    ##################################################################    
        
    # communication()
