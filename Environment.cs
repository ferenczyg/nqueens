using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class Environment<C, G>
        where C : class, IChromosome<G>
    {
        List<C> pool;
        List<double> fitValues;
        ISelector<C, G> selector;
        Func<C, double> fitnessFunction;
        Func<double, double, bool> triggerFunction;
        Func<C, C, Tuple<C, C>> crossoverFunction;

        double pcross, pmut;
        Random r;
        int generation = 0;

        public Environment(
            int poolsize,
            Func<C> constructor,
            ISelector<C, G> selector,
            Func<C, C, Tuple<C, C>> crossoverFunction,
            Func<C, double> fitnessFunction,
            Func<double, double, bool> triggerFunction,
            double pcrossover,
            double pmutate) {
            if (poolsize % 2 == 1 || poolsize == 0) throw new ArgumentException("Poolsize cannot be 0 or an odd number!");

            this.selector = selector;
            this.fitnessFunction = fitnessFunction;
            this.triggerFunction = triggerFunction;
            this.crossoverFunction = crossoverFunction;
            pcross = pcrossover;
            pmut = pmutate;

            pool = new List<C>(poolsize);
            fitValues = new List<double>(poolsize);
            for (int i = 0; i < poolsize; i++) pool.Add(constructor());
            for (int i = 0; i < poolsize; i++) fitValues.Add(fitnessFunction(pool[i]));

            r = new Random();

            //PrintFitnesses(generation++);
        }

        public void StepOne() {
            List<C> newpool = new List<C>(pool.Count);
            for (int i = 0; i < pool.Count - 1; i += 2)
            {
                C parentA = selector.GetSelected(pool, fitValues);
                C parentB = selector.GetSelected(pool, fitValues, parentA);

                double d = r.NextDouble();
                if (pcross > d)
                {
                    Tuple<C, C> children = crossoverFunction(parentA, parentB);
                    newpool.Add(children.Item1);
                    newpool.Add(children.Item2);
                }
                else
                {
                    newpool.Add(parentA);
                    newpool.Add(parentB);
                }
            }
            pool = newpool;
            for (int i = 0; i < pool.Count; i += 2)
            {
                pool[i] = (C)pool[i].Mutate(pmut);
            }
            fitValues = new List<double>(pool.Count);
            for (int i = 0; i < pool.Count; i++) fitValues.Add(fitnessFunction(pool[i]));

            generation++;
            //PrintFitnesses(generation);
        }

        public void StepToTrigger(out C bestChromosome, out double bestFitness, out double averageFitness, out int reachedGeneration, int timeout = 0) {
            TimeSpan startTime = new TimeSpan(DateTime.Now.Ticks);
            
            bestChromosome = null;
            bestFitness = 0;
            double sumFit = 0;
            foreach (double f in fitValues) sumFit += f;
            averageFitness = sumFit / fitValues.Count;
            reachedGeneration = generation = 0;

            CheckTrigger(averageFitness, ref bestChromosome, ref bestFitness);
            while (bestChromosome == null)
            {
                sumFit = 0;
                foreach (double f in fitValues) sumFit += f;
                averageFitness = sumFit / fitValues.Count;
                reachedGeneration = generation;
                CheckTrigger(averageFitness, ref bestChromosome, ref bestFitness);

                StepOne();
                
                if (timeout > 0)
                {
                    TimeSpan currTime = new TimeSpan(DateTime.Now.Ticks);
                    if (currTime.TotalSeconds > startTime.TotalSeconds + timeout)
                    {
                        for (int i = 0; i < pool.Count; i++)
                        {
                            if (bestFitness < fitValues[i])
                            {
                                bestChromosome = pool[i];
                                bestFitness = fitValues[i];
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void CheckTrigger(double averageFitness, ref C chromosome, ref double fitness) {
            for (int i = 0; i < pool.Count; i++)
            {
                if (triggerFunction(fitValues[i], averageFitness)){
                    chromosome = pool[i];
                    fitness = fitValues[i];
                }
            }
        }

        private void PrintFitnesses(int generation) {
            Console.Clear();
            Console.Out.WriteLine("Generation: " + generation);
            double maxFit = 0;
            int maxIndex = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                double fitness = fitValues[i];
                if (maxFit < fitness)
                {
                    maxFit = fitness;
                    maxIndex = i;
                }

                /*if (i < pool.Count - 1)
                {
                    Console.Out.Write("{0:0.00} ", fitness);
                }
                else
                {
                    Console.Out.Write("{0:0.00}\n\n", fitness);
                }*/
            }
            Console.Out.WriteLine("Best fitness: {0:0.00} ", maxFit);
            /*for (int i = 0; i < pool.Count; i++)
            {
                for (int j = 0; j < pool[i].GeneCount; j++)
                {
                    if (j == pool[i].GeneCount - 1)
                    {
                        Console.Out.Write("{0,2}\n", pool[i][j]);
                    }
                    else
                    {
                        Console.Out.Write("{0,2}, ", pool[i][j]);
                    }
                }
            }
            for (int i = 0; i < pool[maxIndex].GeneCount; i++)
            {
                for (int j = 0; j < pool[maxIndex].GeneCount; j++)
                {
                    if (pool[maxIndex][j].ToString() == i.ToString()){
                        Console.Out.Write("X");
                    }
                    else
                    {
                        Console.Out.Write(".");
                    }
                }
                Console.Out.Write("\n");
            }*/
        }
    }
}
