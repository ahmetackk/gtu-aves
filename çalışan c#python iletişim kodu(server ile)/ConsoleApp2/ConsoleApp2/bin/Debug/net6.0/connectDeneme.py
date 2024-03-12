import socket
from dronekit import connect, VehicleMode
import time
import threading

HOST = '127.0.0.1'  # localhost
PORT = 65430        # Port numarası

msg = ""

def communication():
    global msg
    # Soket oluşturma
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))  # Belirtilen HOST ve PORT'a bağlanma
        s.listen()  # Bağlantıları dinleme
        print(f"Sunucu {HOST}:{PORT} üzerinde dinleniyor...")

        conn, addr = s.accept()  # Bağlantıyı kabul etme
        with conn:
            print('Bağlantı adresi:', addr)
            while True:
                data = conn.recv(1024)  # İstemciden veri alma
                if not data:
                    break
                print(f"Gelen veri: {data.decode()}")  # Veriyi ekrana yazdırma
                msg = data.decode()

                # # Python kodunu çalıştırma ve yanıtı gönderme
                # result = deneme(3, 6) # c# koduna gönderilen cevap
                # conn.sendall(result.encode())

def pixhawk():
    while True:
        if msg != "":   
            print("Connecting")
            try:
                vehicle = connect(msg, timeout=60, baud=57600)
                time.sleep(2)
                
                while True:
                    print(vehicle.mode)
            except:
                print("Cannot connected to vehicle")
                
            
            

def main():
    
    th1 = threading.Thread(target=communication)
    th1.start()
    
    th2 = threading.Thread(target=pixhawk)
    th2.start()
    
    
    
main()