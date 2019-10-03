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

        public float Rotation { get; set; }

        public float Roll
        {
            get
            {
                return Rotation;
            }
            set
            {
                Rotation = value;
            }
        }

        public float Yaw { get; set; }

    }
}
