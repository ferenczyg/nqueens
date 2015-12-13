using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class RouletteSelector<C, G>  : ISelector<C, G>
        where C : class, IChromosome<G>
    {
        private Random r = new Random();

        public C GetSelected(List<C> pool, List<double> fitValues, C excluded = null) {
            double sum = 0.0;

            if (pool.Count != fitValues.Count)
            {
                throw new ArgumentException("Pool and fitness list counts must match.");
            }

            for (int i = 0; i < pool.Count; i++)
            {
                sum += fitValues[i];
            }

            C c = null;
            while (c == null)
            {
                double currSumFit = 0;
                double rnd = r.NextDouble() * sum;
                for (int i = 0; i < pool.Count; i++)
                {
                    currSumFit += fitValues[i];
                    if (currSumFit > rnd && pool[i] != excluded)
                    {
                        c = pool[i];
                        break;
                    }
                }
            }
            return c;
        }
    }
}
