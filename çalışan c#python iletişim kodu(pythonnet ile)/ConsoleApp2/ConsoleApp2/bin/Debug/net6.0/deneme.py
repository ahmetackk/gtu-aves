import os

klasor_adi = "Yeni_Klasor"

# Eğer klasör zaten mevcut değilse oluştur
if not os.path.exists(klasor_adi):
    os.makedirs(klasor_adi)
    print(f"{klasor_adi} adında bir klasör oluşturuldu.")
else:
    print(f"{klasor_adi} adında bir klasör zaten mevcut.")



# import socket
# from dronekit import connect, VehicleMode
# import time
# import threading

# class UAV:
    
#     HOST = '127.0.0.1'  # localhost
#     PORT = 65432        # Port numarası

#     msg = None
#     conn = None
#     adrr = None
    
#     def __init__(self):
#         self.connection_start(self.HOST, self.PORT)
#         connection_string = self.take_message()
#         try:
#             self.vehicle = connect(connection_string, timeout=60, baud=57600)
#         except:
#             print("PixHawk'a bağlanılamadı")
        
#     def connection_start(self, host, port):
#         # Soket oluşturma
#         with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
#             s.bind((host, port))  # Belirtilen HOST ve PORT'a bağlanma
#             s.listen()  # Bağlantıları dinleme
#             print(f"Sunucu {host}:{port} üzerinde dinleniyor...")

#             self.conn, self.addr = s.accept()  # Bağlantıyı kabul etme

#     def take_message(self):
#         with self.conn:
#             print('Bağlantı adresi:', self.addr)
#             while True:
#                 data = self.conn.recv(1024)  # İstemciden veri alma
#                 if data != None:
#                     print(f"Gelen veri: {data.decode()}")  # Veriyi ekrana yazdırma
#                     self.msg = data.decode()
#                     return self.msg
                
#     def print(self):
#         return self.vehicle.armed

    