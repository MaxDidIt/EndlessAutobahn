using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SplineMath
{ 
    public static int Factorial(int n)
    {
        if(n < 0)
        {
            throw new ArgumentException("Parameter of factorial can't be smaller than zero.");
        }

        if(n == 0)
        {
            return 1;
        }

        int result = 1;
        for(int i = 2; i <= n; i++)
        {
            result *= i;
        }

        return result;
    }

    public static int BinomialCoefficient(int n, int k)
    {
        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }

    public static double BezierSplineWeight(int n, double t)
    {
        double result = 0;

        for(int i = 0; i < n; i++)
        {
            result += BinomialCoefficient(n, i) * Math.Pow((1 - t), n - i) * Math.Pow(t, i);
        }

        return result;
    }
}
