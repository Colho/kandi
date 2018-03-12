using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandi
{
    /// <summary>
    /// Rajapinta pääikkunalle, jonka avulla käytetään observer-mallia.
    /// </summary>
    interface IObserver
    {
        void update(int value, string key);
    }
}
