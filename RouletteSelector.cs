using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class RouletteSelector<C, G> : ISelector<C, G>
    {
        public List<C> GetNewPopulation(List<C> pool, Func<C, double> fitness) {
            double sum = 0.0;
            foreach (var c in pool)
            {
                sum += fitness(c);
            }
            Random r = new Random();
            List<C> newpool = new List<C>();

            for (int i = 0; i < pool.Count / 2; i++)
            {
                double currFit = 0;
                double rnd = r.NextDouble() * sum;
                foreach (var c in pool)
                {
                    currFit += fitness(c);
                    if (currFit > rnd)
                    {
                        newpool.Add(c);
                    }
                }
            }
            return newpool;
        }
    }
}
