using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class Program
    {
        static Random r = new Random();
        static int N = 10;

        static void Main(string[] args) {
            uint poolsize = 50;
            RouletteSelector<NQueensBasicChromosome, int> rs = new RouletteSelector<NQueensBasicChromosome, int>();
            Environment<NQueensBasicChromosome, int> e = new Environment<NQueensBasicChromosome, int>(poolsize, constructor, rs, crossConstraint, fitnessSimple, triggerSimple, 0.6, 0.001);

            NQueensBasicChromosome nqc = e.stepToTrigger();
        }

        static NQueensBasicChromosome constructor() {
            return new NQueensBasicChromosome(N);
        }

        static Tuple<NQueensBasicChromosome, NQueensBasicChromosome> cross1Point(NQueensBasicChromosome n1, NQueensBasicChromosome n2) {

            int point = r.Next(1, n1.GeneCount);
            for (int i = point; i < n1.GeneCount; i++)
            {
                int temp = n1[i];
                n1[i] = n2[i];
                n2[i] = temp;
            }
            return new Tuple<NQueensBasicChromosome, NQueensBasicChromosome>(n1, n2);
        }

        static Tuple<NQueensBasicChromosome, NQueensBasicChromosome> crossRand(NQueensBasicChromosome n1, NQueensBasicChromosome n2) {
            for (int i = 0; i < n1.GeneCount; i++)
            {
                double p = r.NextDouble();
                if (p > 0.5)
                {
                    int temp = n1[i];
                    n1[i] = n2[i];
                    n2[i] = temp;
                }
            }
            return new Tuple<NQueensBasicChromosome, NQueensBasicChromosome>(n1, n2);
        }

        static Tuple<NQueensBasicChromosome, NQueensBasicChromosome> crossConstraint(NQueensBasicChromosome n1, NQueensBasicChromosome n2) {
            NQueensBasicChromosome[] childs = {new NQueensBasicChromosome(n1.GeneCount, true), new NQueensBasicChromosome(n1.GeneCount, true)};
            List<int> possible = new List<int>();
            
            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    possible.Add(i);
                }
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    double p = r.NextDouble();
                    if (p > 0.5 && possible.Contains(n1[i]))
                    {
                        childs[k][i] = n1[i];
                        possible.Remove(n1[i]);
                    }
                    else if (possible.Contains(n2[i]))
                    {
                        childs[k][i] = n2[i];
                        possible.Remove(n2[i]);
                    }
                    else
                    {
                        int index = r.Next(0, possible.Count);
                        childs[k][i] = possible[index];
                        possible.RemoveAt(index);
                    }
                }
            }
            return new Tuple<NQueensBasicChromosome, NQueensBasicChromosome>(childs[0], childs[1]);
        }

        static double fitnessSimple(NQueensBasicChromosome nqc) {
            int hitCount = 0;
            for (int i = 0; i < nqc.GeneCount; i++)
            {
                for (int j = 0; j < nqc.GeneCount; j++) // Check all the other columns for collision
                {
                    if (i != j)
                    {
                        if (nqc[i] == nqc[j] || nqc[i] == nqc[j] + (j - i) || nqc[i] == nqc[j] - (j - i))
                        {
                            hitCount++;
                        }
                    }
                }
            }
            hitCount /= 2;
            int max = (nqc.GeneCount * (nqc.GeneCount - 1)) / 2;
            return (double)(max-hitCount) / max;
        }

        static bool triggerSimple(NQueensBasicChromosome nqc) {
            return fitnessSimple(nqc) > 0.99;
        }
    }
}
