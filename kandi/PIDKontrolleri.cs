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
        double integrator;
        int maxLimit = 100;
        int lowLimit = 0;
        int diff;
        double unlimitedControl;
        double actualControl;

        /// <summary>
        /// Luokan oletusrakentaja, ei alusta mitään.
        /// </summary>
        public PIDKontrolleri()
        {

        }

        /// <summary>
        /// Nollaa säätimen ja asettaa uudet P ja I arvot
        /// </summary>
        /// <param name="P">Säätimen P-arvo</param>
        /// <param name="I">Säätimen I-arvo</param>
        public void setParam(double P, double I)
        {
            Kp = P;
            Ki = I;
            integrator = 0;
            unlimitedControl = 0;
            actualControl = 0;
        }

        /// <summary>
        /// Funnktio joka toteuttaa itse PI-säätimen
        /// </summary>
        /// <param name="goal">Tavoite arvo</param>
        /// <param name="current">Tämänhetkinen arvo</param>
        /// <returns>säädön määrän</returns>
        public double control(int goal, int current)
        {
            diff = goal - current;
            if (diff < 0)
            {
                diff = -diff;
            }
            Console.WriteLine("diff: " + diff);
            unlimitedControl = Kp * diff + integrator;
            Console.WriteLine("unlimited: " + unlimitedControl);

            if (unlimitedControl < lowLimit)
            {
                Console.WriteLine("too low");
                actualControl = lowLimit;
            }
            else if (unlimitedControl > maxLimit)
            {
                Console.WriteLine("too high");
                actualControl = maxLimit;
            }
            else
            {
                actualControl = unlimitedControl;
            }
            Console.WriteLine("actual: " + actualControl);
            integrator = integrator + Kp * (Ki / 200) * diff;
            Console.WriteLine("Integrator: " + integrator);

            return actualControl;

        }
    }
}
