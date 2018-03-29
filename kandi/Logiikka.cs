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

        // Vaihdettavissa
        public async void loop(double kp, double ki, double kd, int nopeus)
        {
            int midpoint = (Int32.Parse(_maxwhite) + Int32.Parse(_maxblack)) / 2;
            myController.setParam(kp, ki, kd, nopeus);
            lasterror = 0;
            double sensor;
            double error = 0;
            double integral = 0;
            double derivative = 0;
            int i = 0;
            jatka = true;
            while (jatka)
            {

                //sensor = Int32.Parse(brick.Ports[InputPort.Three].RawValue.ToString());
                sensor = brick.Ports[InputPort.Three].RawValue;
                error = midpoint - sensor;
                integral = (0.1 * error) + integral;
                if (integral > 100) integral = 100;
                if (integral < -100) integral = -100;
                derivative = error - lasterror;
                //kp = 0.07, ki = 0.3, kd = 1
                //saato = (kp * error + ki * integral + kd * derivative); //P SAATIMESSA P = 3 IS OKAY
                saato = myController.control(midpoint, (int)sensor);
                //Console.WriteLine(i + ":" + saato + ":" + integral);
                i++;
                //this.Dispatcher.Invoke(() => { saatoBox.Text = saato.ToString(); });
                notifyObservers((int)saato, "saato");
                double saato2 = saato / 2;
                if (sensor < midpoint)
                //if (saato > 0)
                {
                    try
                    {
                        brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus - (int)saato), 10, false);
                        brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus + (int)saato), 10, false);
                        await brick.BatchCommand.SendCommandAsync();
                    }
                    catch
                    {
                        jatka = false;
                    }
                    System.Threading.Thread.Sleep(10);
                }
                else
                {
                    try
                    {
                        brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus - (int)saato), 10, false);
                        brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus + (int)saato), 10, false);
                        await brick.BatchCommand.SendCommandAsync();
                    }
                    catch
                    {
                        jatka = false;
                    }
                    System.Threading.Thread.Sleep(10);
                }
                lasterror = error;
            }
        }

        public void stop()
        {
            jatka = false;
            brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, false);
        }

        public void eteen()
        {
            brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _forward, _time, false);
        }

        public void vasen()
        {
            brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.C, _forward + 25, _time, false);
            brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B, _forward, _time, false);
        }

        public void oikea()
        {
            brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.C, _forward, _time, false);
            brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B, _forward + 25, _time, false);
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
