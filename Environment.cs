using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class Environment<C, G>
        where C : class, IChromosome<G>
    {
        List<C> pool;
        ISelector<C, G> sel;
        Func<C, double> fit;
        Func<C, bool> trig;
        Func<C, C, Tuple<C, C>> cross;

        double pcross, pmut;
        Random r;
        int generation = 0;

        public Environment(
            uint poolsize,
            Func<C> constructor,
            ISelector<C, G> selector,
            Func<C, C, Tuple<C, C>> crosser,
            Func<C, double> fitness,
            Func<C, bool> trigger,
            double pcrossover,
            double pmutate) {
            if (poolsize % 2 == 1 || poolsize == 0) throw new ArgumentException("Poolsize cannot be 0 or an odd number!");

            pool = new List<C>();
            for (int i = 0; i < poolsize; i++) pool.Add(constructor());
            
            sel = selector;
            fit = fitness;
            trig = trigger;
            cross = crosser;
            pcross = pcrossover;
            pmut = pmutate;
            r = new Random();

            PrintFitnesses(generation++);
        }
        
        public C stepOne() {
            pool = sel.GetNewPopulation(pool, fit);

            List<C> newpool = new List<C>(pool.Count);
            for (int i = 0; i < pool.Count - 1; i += 2)
            {
                double d = r.NextDouble();
                if (pcross > d)
                {
                    Tuple<C, C> children = cross(pool[i], pool[i + 1]);
                    newpool.Add(children.Item1);
                    newpool.Add(children.Item2);
                }
                else
                {
                    newpool.Add(pool[i]);
                    newpool.Add(pool[i + 1]);
                }

            }
            pool = newpool;
            for (int i = 0; i < pool.Count; i += 2)
            {
                pool[i] = (C)pool[i].Mutate(pmut);
            }

            PrintFitnesses(generation++);

            foreach (C c in pool)
            {
                if (trig(c)) return c;
            }
            return null;
        }

        public C stepToTrigger() {//ide meg diagnosztika kell
            C c = null;
            while (c == null)
            {
                c = stepOne();
            }
            return c;
        }

        private void PrintFitnesses(int generation){
            Console.Clear();
            Console.Out.WriteLine("Generation: " + generation);
            double maxFit = 0;
            int maxIndex = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                double fitness = fit(pool[i]);
                if (maxFit < fitness){
                    maxFit = fitness;
                    maxIndex = i;
                }

                if (i < pool.Count - 1)
                {
                    Console.Out.Write("{0:0.00} ", fitness);
                }
                else
                {
                    Console.Out.Write("{0:0.00}\n\n", fitness);
                }
            }
            Console.Out.WriteLine("Best fitness: {0:0.00} ", maxFit);

            for (int i = 0; i < pool[maxIndex].GeneCount; i++)
            {
                for (int j = 0; j < pool[maxIndex].GeneCount; j++)
                {
                    if (pool[maxIndex][j].ToString() == i.ToString()){
                        Console.Out.WriteLine("{0," + (j + 1) + ":}", "X");
                    }
                }
            }
        }
    }
}
