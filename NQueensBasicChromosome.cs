using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    class NQueensBasicChromosome : IChromosome<int>
    {
        private static Random r = new Random();
        private int[] genes;
        private int geneCount;

        public NQueensBasicChromosome(int geneCount, bool empty = false) {
            this.geneCount = geneCount;
            genes = new int[geneCount];
            if (!empty)
            {
                List<int> possible = new List<int>();
                for (int i = 0; i < geneCount; i++)
                {
                    possible.Add(i);
                }
                
                for (int i = 0; i < geneCount; i++)
                {
                    int index = r.Next(0, possible.Count);
                    this.genes[i] = possible[index];
                    possible.RemoveAt(index);
                }
            }
        }

        private NQueensBasicChromosome(int geneCount, int[] genes) {
            this.geneCount = geneCount;
            this.genes = new int[geneCount];
            for (int i = 0; i < geneCount; i++)
            {
                this.genes[i] = genes[i];
            }
        }

        public int this[int i] {
            get {
                return genes[i];
            }
            set {
                genes[i] = value;
            }
        }

        public IChromosome<int> Mutate(double p) {
            Random r = new Random();
            NQueensBasicChromosome newC = new NQueensBasicChromosome(geneCount, genes);
            /*for (int i = 0; i < GeneCount; i++) // Give a chance to all genes to mutate
            {
                if (r.NextDouble() < p) // Mutate the gene with the given probability
                {
                    newC[i] = r.Next(0, geneCount); // Upper bound is exclusive

                    //if (i < GeneCount - 1 && r.Next(0, 2) == 1)
                    //    newC[i] = newC[i + 1];//r.Next(0, geneCount); // Upper bound is exclusive
                    //else if (i > 0) 
                    //    newC[i] = newC[i - 1];
                }
            }*/
            for (int i = 0; i < GeneCount; i++) // Give a chance to all genes to mutate
            {
                if (r.NextDouble() < p) // Mutate two genes with the given probability
                {
                    int i2 = i;
                    while (i == i2)
                    {
                        i2 = r.Next(0, GeneCount);
                    }

                    int temp = newC[i];
                    newC[i] = newC[i2];
                    newC[i2] = temp;
                }
            }
            return newC;
        }

        public int GeneCount {
            get {
                return geneCount;
            }
        }
    }
}
