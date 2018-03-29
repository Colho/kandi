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
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace kandi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        Brick _brick;
        private Logiikka myLogiikka;
        int _forward = -40;
        int _backward = 30;
        uint _time = 300;
        string _maxwhite;
        string _maxblack;
        double lasterror = 0;
        bool jatka;
        double saato = 0;


        public MainWindow()
        {
            InitializeComponent();
            myLogiikka = new Logiikka();
            myLogiikka.addObserver(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        //eteen
        private void eteenButtonClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.eteen());
            task.Start();
        }



        //kalibroi valkoinen
        private void karibroiValkoinenClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.calWhite());
            task.Start();
        }

        //kalibroi musta
        private void kalibroiMustaClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.calBlack());
            task.Start();
        }


        public void update(int value, string key)
        {
            Dispatcher.BeginInvoke((Action)(() => updateUI(value, key)));
        }

        public void updateUI(int value, string key)
        {
            if (key == "maxwhite")
            {
                maxWhite.Text = value.ToString();
            }
            if (key == "maxblack")
            {
                maxBlack.Text = value.ToString();
            }
            if (key == "realtime")
            {
                realtime.Text = value.ToString();
            }
            if (key == "numberError")
            {

            }
            if (key == "saato")
            {
                saatoBox.Text = value.ToString();
            }

        }

        //aloita PID
        private void startButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                jatka = true;
                //int midpoint = (Int32.Parse(_maxwhite) + Int32.Parse(_maxblack)) / 2;
                double kp = Convert.ToDouble(kpBox.Text);
                double ki = Convert.ToDouble(kiBox.Text);
                double kd = Convert.ToDouble(kdBox.Text);
                double nopeus = Convert.ToDouble(nopeusBox.Text);
                int _nopeus = (int)nopeus;

                var task = new Task(() => myLogiikka.loop(kp, ki, kd, _nopeus));
                task.Start();
                //await Task.Run(() => myLogiikka.loop(midpoint, kp, ki, kd));
            }
            catch (Exception ec)
            {
                // Error
                update(0, "numberError");
            }
            


        }

        private void lopetaButtonClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.stop());
            task.Start();
            //_brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, false);
        }

        private void connectBTButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.connect());
            task.Start();
        }

        private void vasenButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.vasen());
            task.Start();
        }

        private void oikeaButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.oikea());
            task.Start();
        }
    }
}
