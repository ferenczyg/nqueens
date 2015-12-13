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

            C cBest = null;
            double bestFit = 0;
            while (cBest == null)
            {
                for (int k = 0; k < MatchCount; k++)
                {
                    double currSumFit = 0;
                    double rnd = r.NextDouble() * sum;
                    for (int i = 0; i < pool.Count; i++)
                    {
                        currSumFit += fitValues[i];
                        if (currSumFit > rnd && bestFit < fitValues[i] && pool[i] != excluded)
                        {
                            cBest = pool[i];
                            bestFit = fitValues[i];
                            break;
                        }
                    }
                }
            }
            return cBest;
        }
    }
}
