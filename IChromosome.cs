﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nqueenshazi
{
    interface IChromosome<T>
    {
        T this[int i] { get; set; }
        IChromosome<T> Mutate(double p);
        int GeneCount { get; }
    }
}
