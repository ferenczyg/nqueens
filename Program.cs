using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class Program
    {
        static Random r = new Random();
        static int N = 16;
        static int poolsize = 50;
        static int sequenceCount = 20;
        static int timeout = 30;
        static double[] pms = { 0.001, 0.005, 0.01, 0.05 };
        static double pc = 0.6;

        static void Main(string[] args) {
            RouletteSelector<NQueensBasicChromosome, int> rs = new RouletteSelector<NQueensBasicChromosome, int>();
            MatchSelector<NQueensBasicChromosome, int> ms = new MatchSelector<NQueensBasicChromosome, int>(5);
            
            createStatistics("cross_constraint", rs, crossConstraint, fitnessSimple, triggerSimple);
            createStatistics("cross_rand", rs, crossRand, fitnessSimple, triggerSimple);
            createStatistics("cross_1point", rs, cross1Point, fitnessSimple, triggerSimple);
        }

        static void createStatistics(
            string filename, 
            ISelector<NQueensBasicChromosome, int> selector,
            Func<NQueensBasicChromosome, NQueensBasicChromosome, Tuple<NQueensBasicChromosome, NQueensBasicChromosome>> crossoverFunction, 
            Func<NQueensBasicChromosome, double> fitnessFunction,
            Func<double, double, bool> triggerFunction)
        {
            Environment<NQueensBasicChromosome, int> e;
            StreamWriter file;
            NQueensBasicChromosome bestC;
            double bestFit, avgFit;
            int generation;

            CultureInfo huHU = CultureInfo.CreateSpecificCulture("hu-HU");
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            using (file = new StreamWriter(String.Format("{0}.csv", filename)))
            {
                for (int k = 5; k <= 16; k++)
                {
                    N = k;
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < sequenceCount; i++)
                        {
                            e = new Environment<NQueensBasicChromosome, int>(poolsize, constructor, selector, crossoverFunction, fitnessFunction, triggerFunction, pc, pms[j]);
                            e.StepToTrigger(out bestC, out bestFit, out avgFit, out generation, timeout);
                            if (triggerFunction(bestFit, avgFit))
                            {
                                file.WriteLine(String.Format(huHU, "{0} {1} {2} {3} {4:0.000} {5:0.000}", N, pc, pms[j], generation, bestFit, avgFit));
                                Console.Out.WriteLine(String.Format(enUS, "{0}: N={1}, pc={2}, pm={3}, g={4}, best={5:0.000}, avg={6:0.000}", filename, N, pc, pms[j], generation, bestFit, avgFit));
                            }
                            else
                            {
                                file.WriteLine(String.Format(huHU, "{0} {1} {2} {3} {4:0.000} {5:0.000} timeout", N, pc, pms[j], generation, bestFit, avgFit));
                                Console.Out.WriteLine(String.Format(enUS, "{0}: N={1}, pc={2}, pm={3}, g={4}, best={5:0.000}, avg={6:0.000}, timeout", filename, N, pc, pms[j], generation, bestFit, avgFit));
                            }
                        }
                    }
                }
            }
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
            NQueensBasicChromosome[] childs = { new NQueensBasicChromosome(n1.GeneCount, true), new NQueensBasicChromosome(n1.GeneCount, true) };
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
                    else if (possible.Contains(n1[i]))
                    {
                        childs[k][i] = n1[i];
                        possible.Remove(n1[i]);
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

        static Tuple<NQueensBasicChromosome, NQueensBasicChromosome> crossCommonLazy(NQueensBasicChromosome n1, NQueensBasicChromosome n2) {
            NQueensBasicChromosome[] childs = { new NQueensBasicChromosome(n1.GeneCount, true), new NQueensBasicChromosome(n1.GeneCount, true) };
            List<int> possible = new List<int>();

            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    possible.Add(i);
                }

                bool[] common = new bool[n1.GeneCount];
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    if (n1[i] == n2[i])
                    {
                        childs[k][i] = n1[i];
                        possible.Remove(n1[i]);
                        common[i] = true;
                    }
                }
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    if (common[i] == false)
                    {
                        int index = r.Next(0, possible.Count);
                        childs[k][i] = possible[index];
                        possible.RemoveAt(index);
                    }
                }
            }
            return new Tuple<NQueensBasicChromosome, NQueensBasicChromosome>(childs[0], childs[1]);
        }

        static Tuple<NQueensBasicChromosome, NQueensBasicChromosome> crossCommonStrict(NQueensBasicChromosome n1, NQueensBasicChromosome n2) {
            NQueensBasicChromosome[] childs = { new NQueensBasicChromosome(n1.GeneCount, true), new NQueensBasicChromosome(n1.GeneCount, true) };
            List<int> possible = new List<int>();

            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    possible.Add(i);
                }

                bool[] common = new bool[n1.GeneCount];
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    if (n1[i] == n2[i])
                    {
                        childs[k][i] = n1[i];
                        possible.Remove(n1[i]);
                        common[i] = true;
                    }
                }
                for (int i = 0; i < n1.GeneCount; i++)
                {
                    if (common[i] == false)
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
                        else if (possible.Contains(n1[i]))
                        {
                            childs[k][i] = n1[i];
                            possible.Remove(n1[i]);
                        }
                        else
                        {
                            int index = r.Next(0, possible.Count);
                            childs[k][i] = possible[index];
                            possible.RemoveAt(index);
                        }
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
            return (double)(max - hitCount) / max;
        }

        static double fitnessWeighted(NQueensBasicChromosome nqc) {
            double hitCount = 0;
            for (int i = 0; i < nqc.GeneCount; i++)
            {
                for (int j = 0; j < nqc.GeneCount; j++) // Check all the other columns for collision
                {
                    if (i != j)
                    {
                        if (nqc[i] == nqc[j] || nqc[i] == nqc[j] + (j - i) || nqc[i] == nqc[j] - (j - i))
                        {
                            hitCount += 1.0 / Math.Abs(i - j);
                        }
                    }
                }
            }
            hitCount /= 2;
            int max = (nqc.GeneCount * (nqc.GeneCount - 1)) / 2;
            return (double)(max - hitCount) / max;
        }

        static bool triggerSimple(double fitValue, double avgFitValue) {
            return fitValue > 0.999;
        }
    }
}
