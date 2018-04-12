using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandi
{
    class PIDKontrolleri
    {
        double Kp;
        double Ki;
        double Kd;
        double integrator;
        double derivative;
        int maxLimit;
        int lowLimit = 0;
        int diff;
        int lastDiff;
        double unlimitedControl;
        double actualControl;
        int sign = 0;
        int oldSign = 0;


        public PIDKontrolleri()
        {

        }

        public void setParam(double P, double I, double D, int nopeus)
        {
            Kp = P;
            Ki = I;
            Kd = D;
            integrator = 0;
            unlimitedControl = 0;
            actualControl = 0;
            //maxLimit = nopeus - 5;
            maxLimit = 30;
        }


        public double control(int goal, int current)
        {
            diff = goal - current;
            if (Math.Sign(diff) != sign)
            {
                integrator = 0;
                sign = Math.Sign(diff);
            }
            if (diff < 0)
            {
                diff = -diff;
            }
            derivative = diff - lastDiff;
            //Console.WriteLine("diff: " + diff);
            unlimitedControl = Kp * diff + integrator + Kd * derivative;
            //Console.WriteLine("unlimited: " + unlimitedControl);

            if (unlimitedControl < lowLimit)
            {
                //Console.WriteLine("too low");
                actualControl = lowLimit;
            }
            else if (unlimitedControl > maxLimit)
            {
                //Console.WriteLine("too high");
                actualControl = maxLimit;
            }
            else
            {
                actualControl = unlimitedControl;
            }
            //Console.WriteLine("actual: " + actualControl);
            integrator = integrator + (Ki / 200) * diff;
            lastDiff = diff;
            if (integrator > 30) integrator = 30;
            //Console.WriteLine("Integrator: " + integrator);
            //Console.WriteLine(DateTime.Now);

            return actualControl;

        }
    }
}
