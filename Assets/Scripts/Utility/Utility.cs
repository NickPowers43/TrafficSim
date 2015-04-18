using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    class Utility
    {
        public static T[][] DuplicateSquareMat<T>(T[][] src)
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
        public static T[][] SquareMat<T>(int size, T def)
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
    }
}
