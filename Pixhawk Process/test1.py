import socket
from dronekit import VehicleMode, connect, Command
import dronekit
import time
from pymavlink import mavutil, ardupilotmega

class UAV:
    """
    This class represents a single drone in a network of drones. It provides
    methods for connecting to the drone, arming and disarming the motors, taking
    off and landing, and uploading and downloading missions. It also provides
    methods for getting the current location and attitude of the drone, and for
    adding waypoints to the mission.
    """

    def __init__(self, id_number, port, rank):
        """
        Initialize a new UAV object.

        Args:
            id_number (int): The ID number of the drone.
            port (int): The port number used for connecting to the drone.
            rank (int): The rank of the drone in the network.
        """
        self.id_number = id_number
        self.port = port
        self.rank = rank
        self.vehicle = None

        self._ekf_poshorizabs = False
        self._ekf_constposmode = False
        self._ekf_predposhorizabs = False
        self.properties = {}
        self.connect(port)
        self.add_statustext_listener()
        self.add_ekf_status_report_listener()

    def set_property(self, key, value):
        """
        Set a property of the UAV.

        Args:
            key (str): The key of the property.
            value (object): The value of the property.
        """
        self.properties[key] = value

    def get_property(self, key):
        """
        Get the value of a property of the UAV.

        Args:
            key (str): The key of the property.

        Returns:
            The value of the property, or None if the property does not exist.
        """
        return self.properties.get(key, None)

    def connect(self, port):
        """
        Connect to the drone on the specified port.

        Args:
            port (int): The port number used for connecting to the drone.

        Raises:
            ConnectionError: If there is an error connecting to the drone.
        """
        try:
            self.vehicle = connect(port, heartbeat_timeout=30)
        except socket.error:
            raise ConnectionError("No server exists at the specified port!")
        except dronekit.APIException:
            raise ConnectionError("Timeout while trying to connect to the drone!")
        except Exception as e:
            raise ConnectionError(f"An unexpected error occurred: {e}")

    def change_mode(self, mode):
        """
        Change the mode of the drone.

        Args:
            mode (str): The new mode of the drone.
        """
        self.vehicle.mode = VehicleMode(mode)

    def get_location(self):
        """
        Get the current location of the drone.

        Returns:
            The current location of the drone as a string.
        """
        return "Location" + str(self.vehicle.location.global_frame.lat) + "," + str(self.vehicle.location.global_frame.lon) + "," + str(self.vehicle.location.global_frame.alt)

    def disarm(self):
        """
        Disarm the motors of the drone.
        """
        print("Disarming motors")
        self.vehicle.armed = False

    def land(self):
        """
        Land the drone.
        """
        print("Landing...")
        self.vehicle.mode = VehicleMode("LAND")
        while self.vehicle.armed:
            print(" Waiting for landing...")
            time.sleep(1)
        print("Landed!")

    def return_to_launch(self):
        """
        Return the drone to its launch point.
        """
        print("Returning to launch...")
        self.vehicle.mode = VehicleMode("RTL")
        while self.vehicle.armed:
            print(" Returning to launch...")
            time.sleep(1)
        print("Returned to launch!")

    def get_attitude(self):
        """
        Get the current attitude (roll, pitch, yaw) of the drone.

        Returns:
            A tuple containing the current roll, pitch, and yaw angles of the drone.
        """
        return self.vehicle.attitude

    def arm(self):
        """
        Arm the motors of the drone.
        """
        print("Arming motors")
        self.vehicle.armed = True
        while not self.vehicle.armed:
            print(" Waiting for arming...")
            time.sleep(1)
        print("Armed!")
    
    def ekf_ok(self):
        return self._ekf_poshorizabs and self._ekf_predposhorizabs and not self._ekf_constposmode
    
    def get_ekf_ok(self):
        return self.ekf_ok()
    
    def get_ekf_status(self):
        return "ekf_poshorizabs: "+str(self._ekf_poshorizabs)+", ekf_constposmode: "+str(self._ekf_constposmode)+", ekf_predposhorizabs: "+str(self._ekf_predposhorizabs)
    
    def goto_location(self, latitude, longitude, altitude):
        """
        Fly the drone to a specified location.

        Args:
            latitude (float): The latitude of the location.
            longitude (float): The longitude of the location.
            altitude (float): The altitude of the location.
        """
        print("Going to location", latitude, longitude, altitude)
        self.vehicle.simple_goto(latitude, longitude, altitude)
        while True:
            print(" Altitude: ", self.vehicle.location.global_relative_frame.alt)
            if self.vehicle.location.global_relative_frame.alt >= altitude * 0.95:
                print("Reached target altitude")
                break
            time.sleep(0.5)
    
    def goto_waypoint(self, wp_Target_Latitude, wp_Target_Longtitude, wp_Target_Altitude):
        """
        Fly the drone to a specified waypoint.

        Args:
            wp_Target_Latitude (float): The latitude of the waypoint.
            wp_Target_Longtitude (float): The longitude of the waypoint.
            wp_Target_Altitude (float): The altitude of the waypoint.
        """
        print("Going to waypoint", wp_Target_Latitude, wp_Target_Longtitude, wp_Target_Altitude)
        self.vehicle.simple_goto(wp_Target_Latitude, wp_Target_Longtitude, wp_Target_Altitude)
        while True:
            print(" Altitude: ", self.vehicle.location.global_relative_frame.alt)
            if self.vehicle.location.global_relative_frame.alt >= wp_Target_Altitude * 0.95:
                print("Reached target altitude")
                break   
            time.sleep(0.5)
    



    def takeoff(self, aTargetAltitude):
        """
        Take the drone off the ground.

        Args:
            aTargetAltitude (float): The target altitude for the takeoff.
        """
        print("Taking off!")
        self.vehicle.simple_takeoff(aTargetAltitude)
        while True:
            print(" Altitude: ", self.vehicle.location.global_relative_frame.alt)
            if self.vehicle.location.global_relative_frame.alt >= aTargetAltitude * 0.95:
                print("Reached target altitude")
                break
            time.sleep(1)

    def readmission(self, aFileName):
        """
        Read a mission from a file.

        Args:
            aFileName (str): The name of the file containing the mission.

        Returns:
            A list of commands that make up the mission.
        """
        print(f"\nReading mission from file: {aFileName}")
        cmds = self.vehicle.commands
        missionlist = []
        with open(aFileName) as f:
            for i, line in enumerate(f):
                if i == 0:
                    if not line.startswith('QGC WPL 110'):
                        raise Exception('File is not supported WP version')
                else:
                    linearray = line.split('\t')
                    ln_index = int(linearray[0])
                    ln_currentwp = int(linearray[1])
                    ln_frame = int(linearray[2])
                    ln_command = int(linearray[3])
                    ln_param1 = float(linearray[4])
                    ln_param2 = float(linearray[5])
                    ln_param3 = float(linearray[6])
                    ln_param4 = float(linearray[7])
                    ln_param5 = float(linearray[8])
                    ln_param6 = float(linearray[9])
                    ln_param7 = float(linearray[10])
                    ln_autocontinue = int(linearray[11].strip())
                    cmd = Command(0, 0, 0, ln_frame, ln_command, ln_currentwp, ln_autocontinue, ln_param1, ln_param2, ln_param3, ln_param4, ln_param5, ln_param6, ln_param7)
                    missionlist.append(cmd)
        return missionlist
    

    def upload_mission(self, aFileName):
        """
        Upload a mission from a file to the drone.

        Args:
            aFileName (str): The name of the file containing the mission.

        Raises:
            IOError: If there is an error reading or writing the file.
        """
        missionlist = self.readmission(aFileName)
        print(f"\nUpload mission from a file: {aFileName}")
        print(" Clear mission")
        cmds = self.vehicle.commands
        cmds.clear()
        for command in missionlist:
            cmds.add(command)
        print(" Upload mission")
        self.vehicle.commands.upload()

    def download_mission(self):
        """
        Download the current mission from the drone.

        Returns:
            A list of commands that make up the mission.
        """
        print(" Download mission from vehicle")
        missionlist = []
        cmds = self.vehicle.commands
        cmds.download()
        cmds.wait_ready()
        for cmd in cmds:
            missionlist.append(cmd)
        return missionlist
    
    def save_mission(self,aFileName):
        print("\nSave mission from Vehicle to file: %s" % aFileName)    
        missionlist = self.download_mission()
        output='QGC WPL 110\n'
        home = self.vehicle.home_location
        output+="%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\n" % (0,1,0,16,0,0,0,0,home.lat,home.lon,home.alt,1)
        for cmd in missionlist:
            commandline="%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\n" % (cmd.seq,cmd.current,cmd.frame,cmd.command,cmd.param1,cmd.param2,cmd.param3,cmd.param4,cmd.x,cmd.y,cmd.z,cmd.autocontinue)
            output+=commandline
        with open(aFileName, 'w') as file_:
            print(" Write mission to file")
            file_.write(output)

    def printfile(aFileName):
        print("\nMission file: %s" % aFileName)
        with open(aFileName) as f:
            for line in f:
                print(' %s' % line.strip())
    
    def add_statustext_listener(self):
        @self.vehicle.on_message('STATUSTEXT')
        def message_handler(self,name,message):
            severity = message.severity
            text = message.text
            print("Severity:", severity)
            print("Text:", text)
            print()  # Bir sonraki mesaj için boş bir satır ekleyin

    def add_ekf_status_report_listener(self):
        @self.vehicle.on_message('EKF_STATUS_REPORT')
        def listener(self, name, message):
            self._ekf_poshorizabs = (message.flags & ardupilotmega.EKF_POS_HORIZ_ABS) > 0
            self._ekf_constposmode = (message.flags & ardupilotmega.EKF_CONST_POS_MODE) > 0
            self._ekf_predposhorizabs = (message.flags & ardupilotmega.EKF_PRED_POS_HORIZ_ABS) > 0
            self.notify_attribute_listeners('ekf_ok', self.ekf_ok, cache=True)
    
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
    
    def clear_mission(self):
        cmds = self.vehicle.commands
        cmds.clear()
        cmds.upload()
        cmds.download()
        cmds.wait_ready()
    
    def close(self): 
        self.vehicle.close()
    
    def __del__(self):
        self.close()
    
    def __str__(self):
        return "UAV id_number: "+str(self.id_number)+", port: "+str(self.port)+", rank: "+str(self.rank))