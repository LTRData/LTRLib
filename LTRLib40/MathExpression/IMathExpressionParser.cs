
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System.Globalization;
using System.Linq.Expressions;

namespace LTRLib.MathExpression;

public interface IMathExpressionParser
{
    CultureInfo FormatInfo { get; }

    Expression ParseExpression(string value, out ParameterExpression[] parameters);
}

#endif
