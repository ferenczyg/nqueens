using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class MatchSelector<C, G> : ISelector<C, G>
        where C : class, IChromosome<G>
    {
        private Random r = new Random();
        public int MatchCount { get; set; }

        public MatchSelector(int matchCount) {
            MatchCount = matchCount;
        }

        public List<C> GetNewPopulation(List<C> pool, Func<C, double> fitness) {
            double sum = 0.0;
            foreach (var c in pool)
            {
                sum += fitness(c);
            }
            
            List<C> newpool = new List<C>();

            C cBest = default(C);
            for (int i = 0; i < pool.Count; i++)
            {
                double bestFit = 0;
                for (int j = 0; j < MatchCount; j++)
                {
                    double currSumFit = 0;
                    double rnd = r.NextDouble() * sum;
                    foreach (var c in pool)
                    {
                        double currFit = fitness(c);
                        currSumFit += currFit;
                        if (currSumFit > rnd && (j == 0 || bestFit < currFit))
                        {
                            cBest = c;
                            bestFit = currFit;
                            break;
                        }
                    }
                }
                newpool.Add(cBest);
            }
            return newpool;
        }
    }
}
