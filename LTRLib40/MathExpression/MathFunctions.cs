/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using static System.Math;

namespace LTRLib.MathExpression;

public static class MathFunctions
{
    public static double Sec(double x) => 1.0 / Cos(x);
    public static double Cosec(double x) => 1.0 / Sin(x);
    public static double Cotan(double x) => 1.0 / Tan(x);
    public static double Arcsin(double x) => Asin(x);
    public static double Arccos(double x) => Acos(x);
    public static double Arctan(double x) => Atan(x);
    public static double Atn(double x) => Atan(x);
    public static double Sqr(double x) => Sqrt(x);
    public static double Sgn(double x) => Sign(x);
    public static double Sign(double x) => Math.Sign(x);
    public static double Arcsec(double x) => Atan(x / Sqrt(x * x - 1.0)) + Sign(x - 1.0) * (2.0 * Atan(1.0));
    public static double Arccosec(double x) => Atan(x / Sqrt(x * x - 1.0)) + checked(Sign(x) - 1) * (2.0 * Atan(1.0));
    public static double Arccotan(double x) => Atan(x) + 2.0 * Atan(1.0);
    public static double HSin(double x) => Sinh(x);
    public static double HCos(double x) => Cosh(x);
    public static double HTan(double x) => Tanh(x);
    public static double HSec(double x) => 2.0 / (Exp(x) + Exp(-x));
    public static double HCosec(double x) => 2.0 / (Exp(x) - Exp(-x));
    public static double HCotan(double x) => (Exp(x) + Exp(-x)) / (Exp(x) - Exp(-x));
    public static double HArcsin(double x) => Log(x + Sqrt(x * x + 1.0));
    public static double HArccos(double x) => Log(x + Sqrt(x * x - 1.0));
    public static double HArctan(double x) => Log((1.0 + x) / (1.0 - x)) / 2.0;
    public static double HArcsec(double x) => Log((Sqrt(-x * x + 1.0) + 1.0) / x);
    public static double HArccosec(double x) => Log((Sign(x) * Sqrt(x * x + 1.0) + 1.0) / x);
    public static double HArccotan(double x) => Log((x + 1.0) / (x - 1.0)) / 2.0;
    public static double Ln(double x) => Log(x);
    public static double LogN(double x, double N) => Log(x, N);
    public static double Fac(double x)
    {
        var R = 1.0;
        for (var C = 2.0; C <= x; C += 1.0)
        {
            R *= C;
        }

        return R;
    }
    internal static double _(double x) => x;
}
