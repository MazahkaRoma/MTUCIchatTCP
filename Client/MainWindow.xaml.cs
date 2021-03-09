using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.ChatService;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Interop;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReceiveClient rc = null;
        string myName;
        MsgData NewMessage = null;
        bool SendImage = false;
        System.Drawing.Image image = null;
        private void SendMessage()
        {
            if (!SendImage)
            {
                byte[] buffer = new byte[256];

                buffer = Controller.Coding(txtSend.Text, DataTypes.String);

                MsgData NewMsg = new MsgData(null, myName + ">" + txtSend.Text);


                lstMsgs.Items.Add(NewMsg);

                //buffer = Encoding.Unicode.GetBytes(txtSend.Text);
                rc.SendMessage(buffer, myName, null);
                txtSend.Clear();
            }
            else
            {
                byte[] buffer = new byte[1024];
                buffer = Controller.Coding(NewMessage.imgData,DataTypes.Image);
                lstMsgs.Items.Add(NewMessage);
                rc.SendMessage(buffer, myName, null);
                SendImage = false;
                img_preview.Source = null;
            }
        }

        void rc_NewNames(object sender, List<string> names)
        {
            lstClients.Items.Clear();
            foreach (string name in names)
            {
                if (name != myName)
                    lstClients.Items.Add(name);
            }
        }

        void rc_ReceiveMsg(string sender, byte[] msg)
        {
            string test = Encoding.Unicode.GetString(msg);

            MsgData Test = Controller.Decoder(msg);

            Test.strData = sender + ">" + Test.strData;


            lstMsgs.Items.Add(Test);
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text.Length > 0)
            {
                lstMsgs.IsEnabled = true;
                txtSend.IsEnabled = true;
                btnSend.IsEnabled = true;

                myName = txtUserName.Text.Trim();

                rc = new ReceiveClient();
                rc.Start(rc, myName);

                rc.NewNames += new GotNames(rc_NewNames);
                rc.ReceiveMsg += new ReceviedMessage(rc_ReceiveMsg);
            }
            else
            {
                lstMsgs.IsEnabled = false;
                txtSend.IsEnabled = false;
                btnSend.IsEnabled = false;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            int keyValue = (int)e.Key;

            if (keyValue == 13)
                SendMessage();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            rc.Stop(myName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstMsgs.IsEnabled = false;
            txtSend.IsEnabled = false;
            btnSend.IsEnabled = false;
            NewMessage = new MsgData(null, null);
        }

        private void label1_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Image fileContent = null;
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            MsgData test = new MsgData(null,"Ntcn");
           
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                SendImage = true;


                filePath = openFileDialog.FileName;

                    
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = System.Drawing.Image.FromStream(fileStream);
                }
                
                NewMessage.imgData = fileContent;

                img_preview.Source = NewMessage.ImageRecource;
                
            }
            
        }
    }
}
