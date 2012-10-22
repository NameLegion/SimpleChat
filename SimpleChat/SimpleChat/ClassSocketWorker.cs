using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets; // Для работы с сокетами нужно подключить это пространство имен
using System.Windows.Forms;

namespace SimpleChat
{
    class ClassSocketWorker
    {
        // ----------------------------------------------------------------------
        private static IPHostEntry ipHost; // Класс для сведений об адресе веб-узла
        private static IPAddress ipAddr; // Предоставляет IP-адрес
        private static IPEndPoint ipEndPoint; // Локальная конечная точка

        private static Socket Server; // Создаем объект сокета-сервера
        private static Socket Client; // Создаем объект сокета-клиента
        private static Socket Handler; // Создаем объект вспомогательного сокета
        // ----------------------------------------------------------------------
        // Деструктор
        ~ClassSocketWorker()
        {
            // Вместо проверки сокетов на подключение, просто используем блок try-catch
            try
            {
                // Сразу обрываем соединения
                Server.Shutdown(SocketShutdown.Both);
                // А потом закрываем сокет!
                Server.Close();

                Client.Shutdown(SocketShutdown.Both);
                Client.Close();

                Handler.Shutdown(SocketShutdown.Both);
                Handler.Close();
            }
            catch { }
        }
        // ----------------------------------------------------------------------
        // Создание сокета
        public static bool IsConnected = false;
        public static void CreateSocket(Object obj)
        {
            Object[] TempObject = (Object[])obj;
            // IP-адрес сервера, для подключения
            string HostName = (string)TempObject[0];
            // Порт подключения
            string Port = (string)TempObject[1];
            bool ServerApp = (bool)TempObject[2];

            // Разрешает DNS-имя узла или IP-адрес в экземпляр IPHostEntry.
            ipHost = Dns.Resolve(HostName);
            // Получаем из списка адресов первый (адресов может быть много)
            ipAddr = ipHost.AddressList[0];
            // Создаем конечную локальную точку подключения на каком-то порту
            ipEndPoint = new IPEndPoint(ipAddr, int.Parse(Port));

            if (!ServerApp)
            {
                try
                {
                    // Создаем сокет на текущей машине
                    Client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                    while (true)
                    {
                        // Пытаемся подключиться к удаленной точке
                        Client.Connect(ipEndPoint);
                        if (Client.Connected)
                            IsConnected = true;
                        break;
                    }
                }
                catch (SocketException error)
                {
                    // 10061 - порт подключения занят/закрыт
                    if (error.ErrorCode == 10061)
                    {
                        MessageBox.Show("Порт подключения закрыт!");
                        Application.Exit();
                    }
                }
            }
            else
            {
                try
                {

                    // Создаем сокет сервера на текущей машине
                    Server = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                }
                catch (SocketException error)
                {
                    // 10061 - порт подключения занят/закрыт
                    if (error.ErrorCode == 10061)
                    {
                        MessageBox.Show("Порт подключения закрыт!");
                        Application.Exit();
                    }
                }

                // Ждем подключений
                try
                {
                    // Связываем удаленную точку с сокетом
                    Server.Bind(ipEndPoint);
                    // Не более 10 подключения на сокет
                    Server.Listen(10);

                    // Начинаем "прослушивать" подключения
                    while (true)
                    {
                        Handler = Server.Accept();
                        if (Handler.Connected)
                            IsConnected = true;
                        break;
                    }
                }
                catch
                {
                    throw new Exception("Проблемы с подключением");
                }
            }
        }
        // ----------------------------------------------------------------------
        // Получение данных от сервера
        public static string GetDataFromServer()
        {
            string GetInformation = "";

            // Создаем пустое «хранилище» байтов, куда будем получать информацию
            byte[] GetBytes = new byte[1024];
            int BytesRec = Client.Receive(GetBytes);

            // Переводим из массива битов в строку
            GetInformation += Encoding.Unicode.GetString(GetBytes, 0, BytesRec);

            return GetInformation;
        }
        // ----------------------------------------------------------------------
        // Получение информации от клиента
        public static string GetDataFromClient()
        {
            string GetInformation = "";

            byte[] GetBytes = new byte[1024];
            int BytesRec = Handler.Receive(GetBytes);

            GetInformation += Encoding.Unicode.GetString(GetBytes, 0, BytesRec);

            return GetInformation;
        }
        // ----------------------------------------------------------------------
        // Отправка информации на сервер
        public static void SendDataToServer(string Data, string Name)
        {
            // Используем unicode, чтобы не было проблем с кодировкой, при приеме информации
            byte[] SendMsg = Encoding.Unicode.GetBytes(Name + " : " + Data);
            Client.Send(SendMsg);
        }
        // ----------------------------------------------------------------------
        // Отправка информации на клиент
        public static void SendDataToClient(string Data, string Name)
        {
            byte[] SendMsg = Encoding.Unicode.GetBytes(Name + " : " + Data);
            Handler.Send(SendMsg);
        }
        // ----------------------------------------------------------------------
    }
}
