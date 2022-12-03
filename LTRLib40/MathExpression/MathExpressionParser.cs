/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LTRLib.MathExpression;

public class MathExpressionParser : IMathExpressionParser
{
    private static readonly Dictionary<string, string> BinaryOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        { "pow", "Power" },
        { "^", "Power" },
        { "+", "AddChecked" },
        { "-", "SubtractChecked" },
        { "*", "MultiplyChecked" },
        { "/", "Divide" },
        { "mod", "Modulo" },
        { "%", "Modulo" },
        { "&", "And" },
        { "|", "Or" },
        { ",", "," }
    };

    private static readonly Dictionary<string, string> UnaryPrefixOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        { "-", "NegateChecked" },
        { "neg", "NegateChecked" },
        { "!", "Not" }
    };

    private static readonly Dictionary<string, string> UnarySuffixOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        { "!", "Fac" }
    };

    public CultureInfo FormatInfo { get; } = CultureInfo.CurrentCulture;

    public Type[] ProviderTypes = { typeof(Math), typeof(MathFunctions) };

    public MathExpressionParser()
    {
    }

    public MathExpressionParser(CultureInfo cultureInfo)
    {
        FormatInfo = cultureInfo;
    }

    public MathExpressionParser(params Type[] extraProviderTypes)
    {
        ProviderTypes = ProviderTypes.Concat(extraProviderTypes).ToArray();
    }

    public MathExpressionParser(CultureInfo cultureInfo, params Type[] extraProviderTypes)
    {
        FormatInfo = cultureInfo;
        ProviderTypes = ProviderTypes.Concat(extraProviderTypes).ToArray();
    }

    public Expression ParseExpression(string line, out ParameterExpression[] parameters)
    {
        var subExpr = new Dictionary<string, Expression>(StringComparer.Ordinal);
        line = ParseExpression(line, subExpr);
        parameters = subExpr.Values.OfType<ParameterExpression>().ToArray();
        return subExpr[line];
    }

    public LambdaExpression ParseExpression(string line) => Expression.Lambda(ParseExpression(line, out var parameters), parameters);

    public TDelegate ParseExpression<TDelegate>(string line) => Expression.Lambda<TDelegate>(ParseExpression(line, out var parameters), parameters).Compile();

    private static string FriendlyString(IEnumerable<string> operands, Dictionary<string, Expression> subExpr) => operands
        .Select(o => subExpr.ContainsKey(o) ? subExpr[o].ToString() : o)
        .Join(" ")
        .ToLowerInvariant();

    private static readonly string[] prioOperators = { "^", "*", "/", "%", "Power", "pow", "MultiplyChecked", "Multiply", "Divide", "Modulo", "mod" };

    private string ParseExpression(string line, Dictionary<string, Expression> subExpr)
    {
        line = line.ToLowerInvariant().Replace("**", "^");

        foreach (var op in UnaryPrefixOperators.Keys
            .Concat(UnarySuffixOperators.Keys)
            .Concat(BinaryOperators.Keys))
        {
            line = line.Replace(op, $" {op} ");
        }

        line = line.Replace("(", " ( ").Replace(")", " ) ");

        // Split line into operands
        var operands = line
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (operands.Count == 0)
        {
            operands.Add("0");
        }

        // First parse expressions within explicit parentheses
        for (; ; )
        {
            var endidx = operands.IndexOf(")");
            if (endidx < 0)
            {
                if (operands.IndexOf("(") >= 0)
                {
                    throw new InvalidOperationException($"Mismatched left parenthesis: '{FriendlyString(operands, subExpr)}'");
                }

                break;
            }

            var startidx = operands.LastIndexOf("(", endidx - 1);
            
            if (startidx < 0)
            {
                throw new InvalidOperationException($"Mismatched right parenthesis: '{FriendlyString(operands, subExpr)}'");
            }
            
            var count = endidx - startidx + 1;
            var sub_operands = operands.Skip(startidx + 1).Take(count - 2).ToList();
            string? key = null;
            
            if (startidx >= 1 &&
                sub_operands.Count >= 1 &&
                sub_operands
                .Where((op, i) => (i & 1) == 1)
                .All(op => op.Equals(",", StringComparison.Ordinal)))
            {
                startidx--;
                count++;

                key = ParseOperation(
                    operands[startidx],
                    subExpr,
                    sub_operands
                    .Where((op, i) => (i & 1) == 0)
                    .Select(op =>
                        {
                            var operand = new List<string>(1) { op };
                            var subkey = ParseExpression(operand, subExpr);
                            if (subkey is null)
                            {
                                throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operand, subExpr)}'");
                            }

                            return subExpr[subkey];
                        })
                    .ToArray());
            }
            else
            {
                key = ParseExpression(sub_operands, subExpr);
            }

            if (key is null)
            {
                throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operands, subExpr)}'");
            }

            operands.RemoveRange(startidx, count);
            operands.Insert(startidx, key);
        }

        // Priority operators with implicit parentheses
        while (operands.Count > 3)
        {
            var prioIdx = prioOperators
                .Select(o => new int?(operands.IndexOf(o)))
                .FirstOrDefault(i => i!.Value >= 0);

            if (prioIdx.HasValue &&
                prioIdx.Value >= 1 &&
                prioIdx.Value <= operands.Count - 1)
            {
                var startIdx = prioIdx.Value - 1;
                var count = 3 + operands.Skip(prioIdx.Value + 1).TakeWhile(o => IsKnownOperator(o, 1)).Count();
                if (startIdx + count <= operands.Count)
                {
                    var key = ParseExpression(operands.Skip(startIdx).Take(count).ToList(), subExpr);
                    if (key is null)
                    {
                        throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operands, subExpr)}'");
                    }

                    operands.RemoveRange(startIdx, count);
                    operands.Insert(startIdx, key);
                    continue;
                }
            }

            break;
        }

        // Priority unary suffix operators
        if (operands.Count >= 2)
        {
            if (UnarySuffixOperators.TryGetValue(operands[1], out var op))
            {
                operands[1] = operands[0];
                operands[0] = op;
            }
        }

        var newLine = ParseExpression(operands, subExpr);

        if (newLine is null)
        {
            throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operands, subExpr)}'");
        }

        return newLine;
    }

    private string? ParseExpression(List<string> operands, Dictionary<string, Expression> subExpr)
    {
        for (; ; )
        {
            string? key;

            // Operators with single operand
            if ((operands.Count >= 2) &&
                IsKnownOperator(operands[0], 1))
            {
                if (!subExpr.ContainsKey(operands[1]))
                {
                    key = ParseExpression(operands.Skip(1).ToList(), subExpr);
                    
                    if (key is null)
                    {
                        throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operands, subExpr)}'");
                    }
                    
                    operands.RemoveRange(1, operands.Count - 1);
                    operands.Insert(1, key);
                    
                    continue;
                }
                
                key = ParseOperation(operands[0], subExpr, subExpr[operands[1]]);
                
                if (key is not null)
                {
                    operands.RemoveRange(0, 2);
                    operands.Insert(0, key);
                    
                    continue;
                }
            }

            // Then, other operators with two operands
            if ((operands.Count >= 3) &&
                IsKnownOperator(operands[1], 2))
            {
                if (!subExpr.ContainsKey(operands[2]))
                {
                    key = ParseExpression(operands.Skip(2).ToList(), subExpr);
                    if (key is null)
                    {
                        throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operands, subExpr)}'");
                    }

                    operands.RemoveRange(2, operands.Count - 2);
                    operands.Insert(2, key);
                    continue;
                }

                if (!subExpr.ContainsKey(operands[0]))
                {
                    key = ParseExpression(operands.Take(1).ToList(), subExpr);
                    if (key is null)
                    {
                        throw new NotSupportedException($"Unsupported operator in expression: '{FriendlyString(operands, subExpr)}'");
                    }

                    operands.RemoveAt(0);
                    operands.Insert(0, key);
                    continue;
                }

                key = ParseOperation(operands[1], subExpr, subExpr[operands[0]], subExpr[operands[2]]);
                if (key is not null)
                {
                    operands.RemoveRange(0, 3);
                    operands.Insert(0, key);
                    continue;
                }
            }

            // Already evaluated subexpressions
            if ((operands.Count >= 1) &&
                !subExpr.ContainsKey(operands[0]))
            {
                key = ParseOperation(operands[0], subExpr);   // Single op, parameter or constant field
                if (key is not null)
                {
                    operands.RemoveAt(0);
                    operands.Insert(0, key);
                    continue;
                }

                subExpr[operands[0]] = Expression.Parameter(typeof(double), operands[0]);
                continue;
            }

            // Single expression item
            if (operands.Count == 1)
            {
                return operands[0];
            }

            // Two expression items, implicit multiplication, insert * and reparse
            if (operands.Count >= 2)
            {
                operands.Insert(1, "*");
                continue;
            }

            return null;
        }
    }

    private static MethodInfo? GetExpressionFactoryMethod(string method, int paramCount)
    {
        if (paramCount == 1 && UnaryPrefixOperators.TryGetValue(method, out var operatorname))
        {
            method = operatorname;
        }
        else if (paramCount == 2 && BinaryOperators.TryGetValue(method, out operatorname))
        {
            method = operatorname;
        }

        var paramList = Enumerable.Repeat(typeof(Expression), paramCount).ToArray();
        return typeof(Expression).GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase, null, paramList, null);
    }

    private MethodInfo? GetMathMethod(string method, int paramCount) => ProviderTypes
            .Select(type => type.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase, null, Enumerable.Repeat(typeof(double), paramCount).ToArray(), null))
            .FirstOrDefault(m => m is not null && m.ReturnType == typeof(double));

    private FieldInfo? GetMathConstant(string constant) => ProviderTypes
            .Select(type => type.GetField(constant, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase))
            .FirstOrDefault(f => (f is not null) && f.FieldType == typeof(double));

    private bool IsKnownOperator(string method, int argumentCount) => GetExpressionFactoryMethod(method, argumentCount) is not null ||
            GetMathMethod(method, argumentCount) is not null;

    private string? ParseOperation(string method, Dictionary<string, Expression> subExpr, params Expression[] operands)
    {
        Expression? expr = null;

        if (operands.Length == 0)
        {
            if (subExpr.TryGetValue(method, out expr))
            {
            }
            else if (double.TryParse(method, NumberStyles.Any, FormatInfo, out var d))
            {
                expr = Expression.Constant(d);
            }
            else
            {
                var fieldInfo = GetMathConstant(method);
                if (fieldInfo is not null)
                {
                    expr = Expression.Constant(fieldInfo.GetValue(null));
                }
            }
        }
        else
        {
            var methodInfo = GetExpressionFactoryMethod(method, operands.Length);
            if (methodInfo is not null)
            {
                expr = methodInfo.Invoke(null, operands) as Expression;
            }
            else
            {
                methodInfo = GetMathMethod(method, operands.Length);
                if (methodInfo is not null)
                {
                    expr = Expression.Call(methodInfo, operands);
                }
            }
        }

        if (expr is null)
        {
            return null;
        }

        var key = expr.NodeType == ExpressionType.Constant
            ? $"#{BitConverter.DoubleToInt64Bits((double)((ConstantExpression)expr!).Value!):X8}"
            : $"$({expr})";

        subExpr.AddOrReplace(key, expr);

        return key;
    }
}

#endif
