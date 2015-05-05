using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utility
{
    public class BellmanFord
    {
        private static void CopyData<T>(T[][] src, T[][] dst)
        {
            for (int i = 0; i < src.Length; i++)
            {
                for (int j = 0; j < src.Length; j++)
                {
                    dst[i][j] = src[i][j];
                }
            }
        }

        private static bool CompMatrix(float[][] a, float[][] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a.Length; j++)
                {
                    if (a[i][j] != b[i][j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static float MinimumOfTheSum(float[][] c, float[][] d, int row, int column)
        {
            float smallest = float.PositiveInfinity;
            //int index = 0;
            for (int i = 0; i < c.Length; i++)
            {
                float a = c[i][column];
                float b = d[row][i];
                float sum = a + b;
                if (sum < smallest)
                {
                    smallest = sum;
                }
            }

            return smallest;
        }
        private static float[][] ComputeCostMatrix(ICollection<WeightedEdge<LaneQueue>>[] edges, int size)
        {
            float[][] output = new float[size][];
            for (int i = 0; i < size; i++)
            {
                output[i] = new float[size];
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                        output[i][j] = 0.0f;
                    else
                        output[i][j] = float.PositiveInfinity;
                }
            }

            foreach (var list in edges)
            {
                foreach (var edge in list)
                {
                    //TODO: Verify this is correct
                    output[edge.start.Index][edge.end.Index] = edge.weight;
                }
            }
            return output;
        }
        public static float[][] RunAlgorithm(ICollection<WeightedEdge<LaneQueue>>[] edges, int size)
        {
            if(size < 1)
            {
                throw new ArgumentException();
            }

            //Debug.Log("Computing bellman ford algorithm\n|V| = " + size.ToString() + "\n|E| = " + edges.Count.ToString());

            float[][] c = ComputeCostMatrix(edges, size);
            float[][] d = Utility.DuplicateSquareMat(c);
            float[][] prevD = Utility.SquareMat(size, 0.0f);

            int iterations = 0;

            //Debug.Log("Printing C");
            //Utility.PrintMatrix(c);

            bool diff = true;
            while (diff && iterations < size)
            {
                iterations++;

                float[][] temp = prevD;
                prevD = d;
                d = temp;

                diff = false;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            d[i][j] = MinimumOfTheSum(c, prevD, i, j);
                        }
                        else
                        {
                            d[i][j] = 0;
                        }

                        diff |= d[i][j] != prevD[i][j];
                    }
                }

                //Debug.Log("Printing D");
                //Utility.PrintMatrix(d);
            }

            return d;
        }
    } 
}
