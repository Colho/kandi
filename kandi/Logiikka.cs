using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace kandi
{
    class Logiikka
    {
        Brick brick;
        private object m_lockObject = null;
        private HashSet<IObserver> m_observers = null;
        private PIDKontrolleri myController;

        int _forward = -40;
        int _backward = 30;
        uint _time = 300;
        string _maxwhite;
        string _maxblack;
        double lasterror = 0;
        bool jatka;
        double saato = 0;


        public Logiikka()
        {
            m_lockObject = new object();
            m_observers = new HashSet<IObserver>();
            myController = new PIDKontrolleri();
        }

        public async void connect()
        {
            try
            {
                brick = new Brick(new BluetoothCommunication("COM3"));
                brick.BrickChanged += Brick_BrickChanged;
                await brick.ConnectAsync();
                await brick.DirectCommand.PlayToneAsync(10, 1000, 300);
            }
            catch (Exception e)
            {
                //error viesti
                //notifyObservers();
                Console.WriteLine("fug");
            }
        }

        private void Brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            //throw new NotImplementedException();
            notifyObservers(brick.Ports[InputPort.Three].RawValue, "realtime");
        }

        public async void loop(int midpoint, double kp, double ki, double kd)
        {
            double sensor;
            double error;
            double integral = 0;
            double derivative;
            int i = 0;
            while (jatka)
            {
                sensor = Int32.Parse(brick.Ports[InputPort.Three].RawValue.ToString());
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

                    brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(75 - (int)saato), 5, false);
                    brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -75, 5, false);
                    try
                    {
                        await brick.BatchCommand.SendCommandAsync();
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

                    brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -75 - (int)saato, 5, false);
                    brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -75, 5, false);
                    try
                    {
                        await brick.BatchCommand.SendCommandAsync();
                    }
                    catch
                    {
                        jatka = false;
                    }
                    System.Threading.Thread.Sleep(5);
                }
                lasterror = error;
            }
        }

        public void stop()
        {
            jatka = false;
            brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, false);
        }

        public void calWhite()
        {
            _maxwhite = brick.Ports[InputPort.Three].RawValue.ToString();
            notifyObservers(Int32.Parse(_maxwhite), "maxwhite");
        }

        public void calBlack()
        {
            _maxblack = brick.Ports[InputPort.Three].RawValue.ToString();
            notifyObservers(Int32.Parse(_maxblack), "maxblack");
        }

        public void addObserver(IObserver observer)
        {
            m_observers.Add(observer);
        }

        public void removeObserver(IObserver observer)
        {
            m_observers.Remove(observer);
        }

        private void notifyObservers(int value, string key)
        {
            int valueCopy = 0;
            string keyCopy = "asd";
            IObserver[] observersCopy = null;

            lock (m_lockObject)
            {
                valueCopy = value;
                keyCopy = key;
                observersCopy = new IObserver[m_observers.Count];
                m_observers.CopyTo(observersCopy);
            }

            foreach (IObserver o in observersCopy)
            {
                o.update(valueCopy, keyCopy);
            }
        }
    }
}
