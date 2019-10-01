using System.Windows.Controls;

namespace FlightCommander.CustomControls
{
    /// <summary>
    /// ArtificialHorizon.xaml 的交互逻辑
    /// </summary>
    public partial class ArtificialHorizon : UserControl
    {
        public ArtificialHorizon()
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
