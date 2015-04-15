using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Edge
{
    public int start;
    public int end;
    public float cost;
}

public class BellmanFord
{
    private static T[][] DuplicateSquareMat<T>(T[][] src)
    {
        T[][] output = new T[src.Length][];
        for (int i = 0; i < src.Length; i++)
        {
            output[i] = new T[src.Length];

            for (int j = 0; j < src.Length; j++)
            {
                output[i][j] = src[i][j];
            }
        }

        return output;
    }
    private static T[][] SquareMat<T>(int size, T def)
    {
        T[][] output = new T[size][];
        for (int i = 0; i < size; i++)
        {
            output[i] = new T[size];

            for (int j = 0; j < size; j++)
            {
                output[i][j] = def;
            }
        }

        return output;
    }
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
    private static float[][] ComputeCostMatrix(Edge[] edges, int size)
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

        for (int i = 0; i < edges.Length; i++)
        {
            output[edges[i].start][edges[i].end] = edges[i].cost;
            output[edges[i].start][edges[i].end] = edges[i].cost;
        }

        return output;
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
    public static float[][] RunAlgorithm(Edge[] edges, int size)
    {
        float[][] c = ComputeCostMatrix(edges, size);
        float[][] d = DuplicateSquareMat(c);
        float[][] prevD = SquareMat(size, 0.0f);

        bool same = false;
        while (!same)
        {
            float[][] temp = prevD;
            prevD = d;
            d = temp;

            same = false;
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

                    same |= d[i][j] != prevD[i][j];
                }
            }
            same = !same;
        }

        return d;
    }
}
