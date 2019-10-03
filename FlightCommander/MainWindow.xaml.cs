using FlightCommander.SerialCommunication;
using System.Windows;
using System.Windows.Controls;

namespace FlightCommander
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public int FrameCount
        {
            get
            {
                FrameParser fp = SerialBoss.Instance.FrameParser;
                return fp?.FrameCount ?? 0;
            }
        }

        public int ErrorCount
        {
            get
            {
                FrameParser fp = SerialBoss.Instance.FrameParser;
                return fp?.ErrorCount ?? 0;
            }
        }

        private void BtnScanClick(object sender, RoutedEventArgs e)
        {
            SerialBoss s = SerialBoss.Instance;
            s.SerialPortScanFinished += (_, list) =>
            {
                comboPorts.Dispatcher.Invoke(() => comboPorts.Items.Clear());
                if (list.PortNames.Count > 0)
                {
                    list.PortNames.ForEach((it) =>
                    {
                        comboPorts.Dispatcher.Invoke(() => comboPorts.Items.Add(it));
                    });
                    comboPorts.Dispatcher.Invoke(() => comboPorts.SelectedIndex = 0);
                }
            };
            s.StartScanAsync();
        }

        private void BtnConnectClick(object sender, RoutedEventArgs e)
        {
            string port = (string)comboPorts.SelectedValue;
            ComboBoxItem b = (ComboBoxItem) comboBaud.SelectedItem;
            if (string.IsNullOrWhiteSpace(port) || b == null)
            {
                MessageBox.Show("串口参数设定有误");
                return;
            }
            int baud = int.Parse(b.Content.ToString());
            SerialBoss.Instance.OpenPort(port, baud);
            lblConnectionStatus.Content = Properties.Resources.lblStatusConnected;
            btnConnect.IsEnabled = false;
            btnDisconnect.IsEnabled = true;
        }

        private void BtnDisconnectClick(object sender, RoutedEventArgs e)
        {
            SerialBoss.Instance.ClosePort();
            lblConnectionStatus.Content = Properties.Resources.lblStatusDisconnected;
            btnDisconnect.IsEnabled = false;
            btnConnect.IsEnabled = true;
        }

        private void BtnRecordClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStopClick(object sernder, RoutedEventArgs e)
        {

        }
    }
}
