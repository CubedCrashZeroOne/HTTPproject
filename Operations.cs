using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HTTPproject
{
    public static class Operations
    {
        public static double Add(params double[] par)
        {
            double result = 0;
            foreach (var d in par)
            {
                result += d;
            }
            return result;
        }
        public static double Multiply(params double[] par)
        {
            double result = 1;
            foreach (var d in par)
            {
                result *= d;
            }
            return result;
        }
        public static double Subtract(params double[] par)
        {
            if(par.Length < 1)
            {
                throw new ArgumentException("Can't subtract with an empty array");
            }
            double result = par[0];
            foreach (var d in par.Skip(1))
            {
                result -= d;
            }
            return result;
        }

        public static double Divide(params double[] par)
        {
            if (par.Length < 1)
            {
                throw new ArgumentException("Can't divide with an empty array");
            }
            double result = par[0];
            foreach (var d in par.Skip(1))
            {
                result /= d;
            }
            return result;
        }
    }
}
