using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleChat
{
    class ChatWorker
    {// Глобальный объект, характеризирующий персону в чате --------------------
        public static AppCore GlobalAppObject;
        // Настраивает объект выше
        public static void CreateGameObject(bool IsServer, string ChatNick)
        {
            GlobalAppObject = new AppCore();
            // Назначаем ник
            GlobalAppObject.ChatNickProp = ChatNick;

            // Тип приложения
            if (IsServer)
            {
                GlobalAppObject.ApplicationTypeProp = AppCore.AppType.Server;
            }
            else
            {
                GlobalAppObject.ApplicationTypeProp = AppCore.AppType.Client;
            }
        }
        // Ф-ция, работающая в новом потоке: получение новых сообщенй -------------
        public static void GetMessages(Object obj)
        {
            // Получаем объект истории чата (лист бокс)
            Object[] Temp = (Object[])obj;
            System.Windows.Forms.ListBox ChatListBox =
            (System.Windows.Forms.ListBox)Temp[0];

            // В бесконечном цикле получаем сообщения
            while (true)
            {
                // Ставим паузу, чтобы на время освобождать порт для отправки сообщений
                Thread.Sleep(50);
                if (GlobalAppObject.ApplicationTypeProp == AppCore.AppType.Server)
                {
                    try
                    {
                        // Получаем сообщение от клиента
                        string Message = ClassSocketWorker.GetDataFromClient();
                        // Добавляем в историю + текущее время
                        ChatListBox.Items.Add(DateTime.Now.ToShortTimeString() + " " + Message);
                        // Автопрокрутка списка
                        ChatListBox.TopIndex = ChatListBox.Items.Count - 1;
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        string Message = ClassSocketWorker.GetDataFromServer();
                        ChatListBox.Items.Add(DateTime.Now.ToShortTimeString() + " " + Message);
                        ChatListBox.TopIndex = ChatListBox.Items.Count - 1;
                    }
                    catch { }
                }
            }
        }
        // -----------------------------------------------------------------------

    }
}
