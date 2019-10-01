using System.Windows.Controls;

namespace FlightCommander.CustomControls
{
    /// <summary>
    /// SpeedIndicator.xaml 的交互逻辑
    /// </summary>
    public partial class SpeedIndicator : UserControl
    {
        public SpeedIndicator()
        {
            InitializeComponent();
        }

        public float Rotation
        {
            set
            {
                rotation = value;
            }
        }

        public float Roll
        {
            set
            {
                rotation = value;
            }
        }

        public float Yaw
        {
            set
            {
                yaw = value;
            }
        }

        private float yaw = 0.0f;
        private float rotation = 0.0f;
    }
}
