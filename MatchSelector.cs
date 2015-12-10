using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class MatchSelector<C, G> : ISelector<C, G>
    {

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
            Random r = new Random();
            List<C> newpool = new List<C>();

            for (int i = 0; i < pool.Count / 2; i++)
            {
                C cBest = default(C);
                for (int j = 0; j < MatchCount; j++)
                {
                    double currFit = 0;
                    double rnd = r.NextDouble() * sum;
                    foreach (var c in pool)
                    {
                        currFit += fitness(c);
                        if (currFit > rnd && (j == 0 || fitness(cBest) < fitness(c)))
                        {
                            cBest = c;
                        }
                    }
                }
                newpool.Add(cBest);
            }
            return newpool;
        }
    }
}
