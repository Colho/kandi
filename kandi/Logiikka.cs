using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Threading;

namespace kandi
{
    class Logiikka
    {
        Brick brick;
        private object m_lockObject = null;
        private HashSet<IObserver> m_observers = null;
        private PIDKontrolleri myController;
        private PIDSäädin myBetterController;

        int _forward = -40;
        uint _time = 300;
        string _maxwhite;
        string _maxblack;
        bool jatka;
        double saato = 0;
        double saatolul = 0;
        int nopeus1;
        double sensorStored;
        ManualResetEvent[] events = new ManualResetEvent[1];
        AutoResetEvent waiter = new AutoResetEvent(false);
        WaitHandle[] waiters = new WaitHandle[]
        {
            new AutoResetEvent(false)
        };



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
                brick = new Brick(new NetworkCommunication("192.168.1.34"));
                //brick = new Brick(new BluetoothCommunication("COM3"));
                //brick = new Brick(new UsbCommunication());
                brick.BrickChanged += Brick_BrickChanged;
                await brick.ConnectAsync();
                await brick.DirectCommand.PlayToneAsync(5, 800, 200);
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
            //Console.WriteLine(DateTime.Now);
            //Console.WriteLine(DateTime.Now + " + " +  DateTime.Now.Millisecond);
            sensorStored = brick.Ports[InputPort.Four].SIValue;
            notifyObservers((int)brick.Ports[InputPort.Four].SIValue, "realtime");
        }

        public async void vasen( Object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus1 - (int)saatolul), 100, false);
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus1), 100, false);
            brick.BatchCommand.SendCommandAsync();
            //Thread.Sleep(1);
            are.Set();
        }

        public async void oikea(Object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus1 - (int)saatolul), 100, false);
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus1), 100, false);
            brick.BatchCommand.SendCommandAsync();
            //Thread.Sleep(1);
            are.Set();
        }

        // Vaihdettavissa
        public async void loop(double kp, double ki, double kd, int nopeus)
        {
            int midpoint = (Int32.Parse(_maxwhite) + Int32.Parse(_maxblack)) / 2;
            myController.setParam(kp, ki, kd, nopeus);
            myBetterController = new PIDSäädin(kp, ki, kd, Convert.ToDouble(_maxwhite), Convert.ToDouble(_maxblack), -(nopeus-12), nopeus-12);
            double sensor;
            nopeus1 = nopeus;
            //Task t = Task.Run(() => brick.DirectCommand.OutputReadyAsync(OutputPort.B | OutputPort.C));
            var task = new Task(() => brick.DirectCommand.OutputReadyAsync(OutputPort.B | OutputPort.C));
            var tisk = new Task(() => brick.BatchCommand.SendCommandAsync());
            task.Start();
            jatka = true;
            while (jatka)
            {

                //sensor = Int32.Parse(brick.Ports[InputPort.Three].RawValue.ToString());
                //sensor = brick.Ports[InputPort.Four].SIValue;
                //saato = myController.control(midpoint, (int)sensor);
                saatolul = myBetterController.Compute(sensorStored, Convert.ToDouble(midpoint));
                notifyObservers((int)saatolul, "saato");
                //if (sensor < midpoint)
                if (saatolul > 0)
                {
                    try
                    {
                        if (task.IsCompleted)
                        {
                            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus - (int)saatolul), 100, false);
                            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus), 100, false);
                            //await brick.BatchCommand.SendCommandAsync();
                            tisk.Start();
                            tisk = new Task(() => brick.BatchCommand.SendCommandAsync());
                            task = new Task(() => brick.DirectCommand.OutputReadyAsync(OutputPort.B | OutputPort.C));
                            task.Start();
                        }
                        //await brick.DirectCommand.OutputReadyAsync(OutputPort.B | OutputPort.C);
                        //brick.BatchCommand.TurnMotorAtPower(OutputPort.B, -(nopeus - (int)saatolul));
                        //brick.BatchCommand.TurnMotorAtPower(OutputPort.C, -(nopeus));
                        //await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, -(nopeus + (int)saatolul));
                        //await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, -(nopeus - (int)saatolul));
                        //brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus - (int)saatolul), 100, false);
                        //brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus), 100, false);
                        //await brick.BatchCommand.SendCommandAsync();
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(vasen), waiters[0]);
                        //WaitHandle.WaitAll(waiters);
                        
                    }
                    catch
                    {
                        jatka = false;
                    }
                    //Thread.Sleep(100);
                }
                else
                {
                    try
                    {
                        saatolul = -saatolul;
                        if (task.IsCompleted)
                        {
                            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus - (int)saatolul), 100, false);
                            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus), 100, false);
                            //await brick.BatchCommand.SendCommandAsync();
                            tisk.Start();
                            tisk = new Task(() => brick.BatchCommand.SendCommandAsync());
                            task = new Task(() => brick.DirectCommand.OutputReadyAsync(OutputPort.B | OutputPort.C));
                            task.Start();
                        }
                        //await brick.DirectCommand.OutputReadyAsync(OutputPort.B | OutputPort.C);
                        //brick.BatchCommand.TurnMotorAtPower(OutputPort.C, -(nopeus - (int)saatolul));
                        //brick.BatchCommand.TurnMotorAtPower(OutputPort.B, -(nopeus));
                        //await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, -(nopeus + (int)saatolul));
                        //await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, -(nopeus - (int)saatolul));
                        //brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, -(nopeus - (int)saatolul), 100, false);
                        //brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, -(nopeus), 100, false);
                        //await brick.BatchCommand.SendCommandAsync();
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(oikea), waiters[0]);
                        //WaitHandle.WaitAll(waiters);
                    }
                    catch
                    {
                        jatka = false;
                    }
                    //Thread.Sleep(100);
                }
                await Task.Delay(100);
                //Thread.Sleep(200);
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
            _maxwhite = brick.Ports[InputPort.Four].RawValue.ToString();
            notifyObservers(Int32.Parse(_maxwhite), "maxwhite");
        }

        public void calBlack()
        {
            _maxblack = brick.Ports[InputPort.Four].RawValue.ToString();
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
