using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;

namespace SimpleChat
{
    public partial class FormForEnter : Form
    {
        Thread WaitingForConnecting;
        public FormForEnter()
        {
            InitializeComponent();
            WaitingForConnecting = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ClassSocketWorker.CreateSocket));
        }

        // Указывает на тип приложения
        private static bool AppCheckedTypeServer = true;

        private void FormForEnter_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Если подключились - закрываем окно настроек
            if (ClassSocketWorker.IsConnected)
            {
                try
                {
                    // Пытаемся завершить поток подключений
                    WaitingForConnecting.Abort();
                }
                catch { }
                // Закрываем окно настроек
                this.Close();
            }
            if (radioButtonServer.Checked)
            {
                // Блокируем поле ввода IP-адреса
                textBoxIP.ReadOnly = true;
                // Получаем свой IP
                textBoxIP.Text = GetMyIP();
                AppCheckedTypeServer = true;
            }
            else
            {
                AppCheckedTypeServer = false;
                textBoxIP.ReadOnly = false;
            }
        }

        private string GetMyIP()
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            string url = "http://checkip.dyndns.org:8245/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    sb.Append(Encoding.Default.GetString(buf, 0, count));
                }
            }
            while (count > 0);
            string [] container = new string [4];
            container = sb.ToString().Split(' ');// get IP ***.***.***.***
            container = container[5].Split('<');// seperate IP from html tegs
            return container[0];
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Enter_Click(object sender, EventArgs e)
        {
            // Блокируем кнопку от повторного нажатия
            Enter.Enabled = false;

            // Настраиваем основной игровой объект
            ChatWorker.CreateGameObject(AppCheckedTypeServer, textBoxNick.Text);

            // Запускаем поток подключения
            if (AppCheckedTypeServer)
            {
                // 0.0.0.0 -для прослушки внешней сети, не используйте 127.0.0.1 или localhost!
                WaitingForConnecting.Start(new Object[] { "0.0.0.0", "12123", AppCheckedTypeServer });
            }
            else
            {
                // 12123 –порт подключения
                WaitingForConnecting.Start(new Object[] { textBoxIP.Text.ToString(), "12123", AppCheckedTypeServer });
            }

        }
    }
}
