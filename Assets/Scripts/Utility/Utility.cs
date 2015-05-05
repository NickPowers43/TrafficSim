using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Globalization;

namespace Utility
{
    class Utility
    {
        public static void PrintMatrix(float[][] mat)
        {
            string temp = "";
            for (int i = 0; i < mat.Length; i++)
            {
                temp += "[" + mat[i][0].ToString("F", CultureInfo.InvariantCulture);
                for (int j = 1; j < mat[i].Length; j++)
                {
                    temp += ", " + mat[i][j].ToString("F", CultureInfo.InvariantCulture);
                }
                temp += "]\n";
            }
            Debug.Log(temp);
        }
        public static void PrintLQMatrix(LaneQueue[][] mat)
        {
            string temp = "";
            for (int i = 0; i < mat.Length; i++)
            {
                temp += "[" + mat[i][0].Index.ToString("D", CultureInfo.InvariantCulture);
                for (int j = 1; j < mat[i].Length; j++)
                {
                    temp += ", " + mat[i][j].Index.ToString("D", CultureInfo.InvariantCulture);
                }
                temp += "]\n";
            }
            Debug.Log(temp);
        }

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
