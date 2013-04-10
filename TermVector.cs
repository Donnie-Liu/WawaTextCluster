using System;
using System.Collections.Generic;
using System.Text;

namespace WawaSoft.Search.Common
{
    public class TermVector
    {
        public static double ComputeCosineSimilarity(double[] vector1, double[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new Exception("DIFER LENGTH");


            double denom = (VectorLength(vector1) * VectorLength(vector2));
            if (denom == 0D)
                return 0D;
            else
                return (InnerProduct(vector1, vector2) / denom);

        }

        public static double InnerProduct(double[] vector1, double[] vector2)
        {

            if (vector1.Length != vector2.Length)
                throw new Exception("DIFFER LENGTH ARE NOT ALLOWED");


            double result = 0D;
            for (int i = 0; i < vector1.Length; i++)
                result += vector1[i] * vector2[i];

            return result;
        }

        public static double VectorLength(double[] vector)
        {
            double sum = 0.0D;
            for (int i = 0; i < vector.Length; i++)
                sum = sum + (vector[i] * vector[i]);

            return (double)Math.Sqrt(sum);
        }

    }
}
