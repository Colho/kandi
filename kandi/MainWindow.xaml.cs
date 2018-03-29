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
            /*_brick = new Brick(new UsbCommunication());
            _brick = new Brick(new BluetoothCommunication("COM3"));
            _brick.BrickChanged += _brick_BrickChanged;
            await _brick.ConnectAsync();
            await _brick.DirectCommand.PlayToneAsync(10, 1000, 300);*/
            //await _brick.DirectCommand.SetMotorPolarityAsync(OutputPort.B | OutputPort.C, Polarity.Backward);
            //await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);
            //_brick.Ports[InputPort.Three].SetMode(ColorMode.Reflective);
        }

        /*private void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            //realtime.Text = _brick.Ports[InputPort.Three].RawValue.ToString();
            //saatoBox.Text = saato.ToString();
        }*/


        /*private string sensorData()
        {
            return _brick.Ports[InputPort.Three].RawValue.ToString();
        }*/

        //eteen
        private void eteenButtonClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.eteen());
            task.Start();
            //_brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _forward, _time, false);
        }



        //kalibroi valkoinen
        private void karibroiValkoinenClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.calWhite());
            task.Start();
            /*_maxwhite = _brick.Ports[InputPort.Three].RawValue.ToString();
            maxWhite.Text = _brick.Ports[InputPort.Three].RawValue.ToString();*/
        }

        //kalibroi musta
        private void kalibroiMustaClick(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => myLogiikka.calBlack());
            task.Start();
            /*_maxblack = _brick.Ports[InputPort.Three].RawValue.ToString();
            maxBlack.Text = _brick.Ports[InputPort.Three].RawValue.ToString();*/
        }

        //looppi?
        /*private async void loop(int midpoint, double kp, double ki, double kd)
        {
            double sensor;
            double error;
            double integral = 0;
            double derivative;
            int i = 0;
            while (jatka) {
                sensor = Int32.Parse(_brick.Ports[InputPort.Three].RawValue.ToString());
                error = midpoint - sensor;
                integral = error + integral;
                derivative = error - lasterror;
                //kp = 0.07, ki = 0.3, kd = 1
                saato = (kp * error + ki * integral + kd * derivative); //P SAATIMESSA P = 3 IS OKAY
                Console.WriteLine(i + ":" + saato + ":" + integral);
                i++;
                //this.Dispatcher.Invoke(() => { saatoBox.Text = saato.ToString(); });
                if (saato > 0)
                {
                    //_brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, -(int)saato);
                    //_brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, -80);

                    /*_brick.BatchCommand.TurnMotorAtPower(OutputPort.B, -(int)saato);
                    _brick.BatchCommand.TurnMotorAtPower(OutputPort.C, -80);
                    try
                    {
                        await _brick.BatchCommand.SendCommandAsync();
                    }
                    catch
                    {
                        jatka = false;
                    }*/

                    /*_brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(75-(int)saato), 5, false);
                    _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -75, 5, false);
                    try
                    {
                        await _brick.BatchCommand.SendCommandAsync();
                    }
                    catch
                    {
                        jatka = false;
                    }
                    
                    System.Threading.Thread.Sleep(5);

                }
                else
                {
                    //_brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, -Math.Abs((int)saato));
                    //_brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, -80);

                    /*_brick.BatchCommand.TurnMotorAtPower(OutputPort.C, (int)saato);
                    _brick.BatchCommand.TurnMotorAtPower(OutputPort.B, -80);
                    try
                    {
                        await _brick.BatchCommand.SendCommandAsync();
                    }
                    catch
                    {
                        jatka = false;
                    }*/

                    /*_brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -75-(int)saato, 5, false);
                    _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -75, 5, false);
                    try {
                        await _brick.BatchCommand.SendCommandAsync();
                    }
                    catch {
                        jatka = false;
                    }
                    System.Threading.Thread.Sleep(5);
                }
                lasterror = error;
            }
        }*/


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
            

            

            //await _brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, false);

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
