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

        public List<C> GetNewPopulation(List<C> pool, Func<C, double> fitness) {
            double sum = 0.0;
            foreach (var c in pool)
            {
                sum += fitness(c);
            }
            List<C> newpool = new List<C>();

            for (int i = 0; i < pool.Count; i++)
            {
                double currSumFit = 0;
                double rnd = r.NextDouble() * sum;
                foreach (var c in pool)
                {
                    currSumFit += fitness(c);
                    if (currSumFit > rnd)
                    {
                        newpool.Add(c);
                        break;
                    }
                }
            }
            return newpool;
        }
    }
}
