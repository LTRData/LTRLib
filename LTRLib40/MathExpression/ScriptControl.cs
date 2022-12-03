#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System.Linq;
using System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;
using System.Threading;

namespace LTRLib.MathExpression;

public class ScriptControl
{
    protected delegate double ExpressionMethodDelegate(double X, double Y);

    protected ExpressionMethodDelegate? ExpressionMethod;

    protected readonly IMathExpressionParser MathExpressionParser;

    public ScriptControl(IMathExpressionParser ExpressionParser)
    {
        MathExpressionParser = ExpressionParser;
    }

    protected string? m_Expression;

    public string? Expression
    {
        get
        {
            return m_Expression;
        }
        set
        {
            if (value == m_Expression)
            {
                return;
            }

            if (value is null)
            {
                ExpressionMethod = null;
                m_Expression = null;
                return;
            }

            var expr = MathExpressionParser.ParseExpression(value, out var parameters);

            var @params = new[] {
                parameters.SingleOrDefault(p => "x" == p.Name),
                parameters.SingleOrDefault(p => "y" == p.Name)
            };
            
            if (@params[0] is null)
            {
                @params[0] = LinqExpression.Parameter(typeof(double), "x");
            }
            if (@params[1] is null)
            {
                @params[1] = LinqExpression.Parameter(typeof(double), "y");
            }

            ExpressionMethod = LinqExpression.Lambda<ExpressionMethodDelegate>(expr, @params!).Compile();

            var currentThCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = MathExpressionParser.FormatInfo;
            Thread.CurrentThread.CurrentUICulture = MathExpressionParser.FormatInfo;

            m_Expression = expr.ToString().ToLowerInvariant();

            Thread.CurrentThread.CurrentCulture = currentThCulture;
            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }
    }

    public double? Eval(double X, double Y)
    {
        if (ExpressionMethod is null)
        {
            return default;
        }

        try
        {
            return ExpressionMethod(X, Y);
        }

        catch
        {
            return default;

        }
    }

}

#endif
