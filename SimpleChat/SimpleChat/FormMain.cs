using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SimpleChat
{
    public partial class FormMain : Form
    {
        Thread WaitingForMessage;
        public FormMain()
        {
            InitializeComponent();
            WaitingForMessage = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ChatWorker.GetMessages));
            // Запускаем, в параметрах передаем листбокс (история чата)
            WaitingForMessage.Start(new Object[] { ChatHistory });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ChatWorker.GlobalAppObject.ApplicationTypeProp == AppCore.AppType.Server)
            {
                // Посылаем клиенту новое сообщение
                ClassSocketWorker.SendDataToClient(ChatText.Text, ChatWorker.GlobalAppObject.ChatNickProp);
            }
            else
            {
                ClassSocketWorker.SendDataToServer(ChatText.Text, ChatWorker.GlobalAppObject.ChatNickProp);
            }
            // Добавляем в историю свое же сообщение + ник + время написания
            ChatHistory.Items.Add(DateTime.Now.ToShortTimeString() + " " + ChatWorker.GlobalAppObject.ChatNickProp + ": " + ChatText.Text.ToString());
            // Автопрокрутка истории чата
            ChatHistory.TopIndex = ChatHistory.Items.Count - 1;
            // Убираем текст из поля ввода
            ChatText.Text = "";

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
