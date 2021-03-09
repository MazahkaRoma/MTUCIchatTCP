using MTUCIchatTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool blnStartStop;
        ServiceHost host;
        ChatService cs = new ChatService();
        
        public MainWindow()
        {
            InitializeComponent();
            blnStartStop = true;
        }

        void cs_ChatListOfNames(List<string> names, object sender)
        {
            lstUser.Items.Clear();
            foreach (string s in names)
            {
                lstUser.Items.Add(s);
            }
        }

        private void Start_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (blnStartStop)
            {
                host = new ServiceHost(typeof(ChatService));
                host.Open();
                Start_Stop.Content = "Stop";
                ChatService.ChatListOfNames += new ListOfNames(cs_ChatListOfNames);
            }
            else
            {
                cs.Close();
                host.Close();
                Start_Stop.Content = "Start";
            }

            blnStartStop = !blnStartStop;
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
