using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public struct WeightedEdge<T>
    {
        public T start;
        public T end;
        public float weight;
    }
}
