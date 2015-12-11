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
        List<C> GetNewPopulation(List<C> pool, Func<C, double> fitness);
    }
}
