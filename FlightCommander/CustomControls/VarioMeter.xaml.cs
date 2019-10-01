using System;
using System.Collections.Generic;
using System.Linq;
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

namespace FlightCommander.CustomControls
{
    /// <summary>
    /// SpeedIndicator.xaml 的交互逻辑
    /// </summary>
    public partial class VarioMeter : UserControl
    {
        public VarioMeter()
        {
            InitializeComponent();
        }

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                rotationAngle = 180.0f * speed / topSpeed;
            }
        }

        public float RotationAngle
        {
            get
            {
                return rotationAngle;
            }
        }

        private float speed = 0;
        private float rotationAngle = 15.0f;
        private const float topSpeed = 25;
    }
}
