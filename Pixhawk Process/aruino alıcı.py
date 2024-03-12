import serial
import threading
import time

# Seri port ve baud hızı
serPort = "COM19"  # Arduino port
baudRate = 9600

# Seri port bağlantısını başlat
ser = serial.Serial(serPort, baudRate)
print("Seri port {} {} baud hızıyla açıldı".format(serPort, baudRate))

# Seri port üzerinden veri okuma işlemi için fonksiyon
def read_serial():
    while True:
      
            veri = ser.readline().decode().strip()
            print("Arduino'dan gelen veri:", veri)
            time.sleep(1)  # Gerektiğinde uygun bir zaman aralığı

# İş parçacığını başlat
serial_thread = threading.Thread(target=read_serial)

# İş parçacığını başlat
serial_thread.start()

# İş parçacığının tamamlanmasını bekle
serial_thread.join()
