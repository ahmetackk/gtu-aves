using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Sunucu bilgileri
        string serverIP = "127.0.0.1"; //IP adres python ile aynı olmalı
        int serverPort = 65432;

        // Soket oluşturma
        using (TcpClient client = new TcpClient(serverIP, serverPort))
        using (NetworkStream stream = client.GetStream())
        {
            Console.WriteLine($"Sunucuya bağlanıldı: {serverIP}:{serverPort}");

            // Sürekli iletişim
            while (true)
            {
                // Gönderilecek veri
                string message = "Hello from C#"; // gönderilen mesaj python terminalinde sürekli basılıyor

                // Veriyi ASCII kodlaması ile byte dizisine dönüştürme
                byte[] data = Encoding.ASCII.GetBytes(message);

                // Veriyi sunucuya gönderme
                stream.Write(data, 0, data.Length);

                // Sunucudan yanıtı alma
                data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);
                string response = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine($"Sunucudan gelen yanıt: {response}"); //pythondan gelen dönüş sürekli terminalde basılıyor

                // İstemciyi belirli bir süre bekletme
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
