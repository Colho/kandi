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
            maxLimit = nopeus - 5;
        }


        public double control(int goal, int current)
        {
            diff = goal - current;
            if (Math.Sign(diff) != sign)
            {
                // Integraattorin nollaus jos säädön merkki vaihtuu
                integrator = 0;
                sign = Math.Sign(diff);
            }
            if (diff < 0)
            {
                // Tarkistetaan että saadaan vain positiivisia ulostuloja
                diff = -diff;
            }
            derivative = diff - lastDiff;
            unlimitedControl = Kp * diff + integrator + Kd * derivative;
            if (unlimitedControl < lowLimit)
            {
                actualControl = lowLimit;
            }
            else if (unlimitedControl > maxLimit)
            {
                actualControl = maxLimit;
            }
            else
            {
                actualControl = unlimitedControl;
            }
            integrator = integrator + (Ki / 200) * diff;
            lastDiff = diff;
            if (integrator > 30) integrator = 30;
            return actualControl;

        }
    }
}
