using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    interface ISelector<C,G>
        where C : class, IChromosome<G>
    {
        C GetSelected(List<C> pool, List<double> fitValues, C excluded = null);
    }
}
