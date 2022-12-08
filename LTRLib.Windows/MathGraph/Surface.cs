#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.MathExpression;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LTRLib.MathGraph;

public class Surface : IDisposable
{
    // Paths for the current graphs to be drawn when surface picturebox is painted
    private readonly GraphicsPath GraphPath = new();
    private readonly GraphicsPath DerivPath = new();
    private readonly GraphicsPath IntegPath = new();

    // Cache these values in variable for performance reasons
    public double Xmin;
    public double Xmax;
    public double Ymin;
    public double Ymax;

    public Rectangle Area = new();

    public void Clear()
    {
        GraphPath.Reset();
        DerivPath.Reset();
        IntegPath.Reset();
    }

    public void DrawAxis(Graphics Graphics, Pen Pen)
    {
        Graphics.DrawLine(Pen, VirtualToScreenX(Xmin), VirtualToScreenY(0d), VirtualToScreenX(Xmax), VirtualToScreenY(0d));
        Graphics.DrawLine(Pen, VirtualToScreenX(0d), VirtualToScreenY(Ymin), VirtualToScreenX(0d), VirtualToScreenY(Ymax));
    }

    public void DrawGraph(Graphics Graphics, Pen Pen)
    {
        Graphics.DrawPath(Pen, GraphPath);
    }

    public void DrawDerivative(Graphics Graphics, Pen Pen)
    {
        Graphics.DrawPath(Pen, DerivPath);
    }

    public void DrawIntegral(Graphics Graphics, Pen Pen)
    {
        Graphics.DrawPath(Pen, IntegPath);
    }

    public void Refresh(ScriptControl ScriptControl)
    {
        // The (to-be-calculated) Y values as init as Nothing. Later in code that is used
        // as the value for "undefined Y".
        double? X;
        var Y = default(double?);
        var oldX = default(double?);
        var oldY = default(double?);

        var old_dYdX = default(double?);
        double? old_FdX = 0;

        for (int Tracker = Area.Left, loopTo = Area.Right; Tracker <= loopTo; Tracker++)
        {
            X = ScreenToVirtualX(Tracker);
            try
            {
                Y = ScriptControl.Eval(X.Value, Y ?? 0d);
            }

            catch
            {
                // Set Y to 0 in case no Y is defined for this X value
                Y = default;

            }

            // Only go into plotting in case both this Y and the last one was defined.
            // In that case draw a line from the last point.
            if (Y.HasValue && oldY.HasValue)
            {
                // No line drawing in case curve passed +/- infinity (e.g. y=1/x)
                if ((oldY > Ymin && oldY < Ymax) || (Y > Ymin && Y < Ymax))
                {
                    GraphPath.StartFigure();
                    try
                    {
                        GraphPath.AddLine(Tracker - 1, VirtualToScreenY(oldY.Value), Tracker, VirtualToScreenY(Y.Value));
                    }
                    catch
                    {
                    }

                    GraphPath.CloseFigure();
                }

                // Calcualte derivate by it's definition
                var dYdX = (Y - oldY) / (X - oldX);

                // Only plot derivate curve if last derivate point was defined
                if (old_dYdX.HasValue)
                {
                    // Same here, don't draw "through infinity"
                    if (dYdX.HasValue
                        && ((old_dYdX > Ymin && old_dYdX < Ymax) || (dYdX > Ymin && dYdX < Ymax)))
                    {
                        DerivPath.StartFigure();
                        try
                        {
                            DerivPath.AddLine(Tracker - 1, VirtualToScreenY(old_dYdX.Value), Tracker, VirtualToScreenY(dYdX.Value));
                        }
                        catch
                        {
                        }

                        DerivPath.CloseFigure();
                    }
                }

                // Calculate antiderivative
                var FdX = old_FdX + Y * (X - oldX);
                if (FdX < Ymin)
                {
                    FdX = Ymax;
                }
                else if (FdX > Ymax)
                {
                    FdX = Ymin;
                }
                else if (old_FdX.HasValue && FdX.HasValue
                    && ((old_FdX > Ymin && old_FdX < Ymax) || (FdX > Ymin && FdX < Ymax)))
                {
                    IntegPath.StartFigure();
                    try
                    {
                        IntegPath.AddLine(Tracker - 1, VirtualToScreenY(old_FdX.Value), Tracker, VirtualToScreenY(FdX.Value));
                    }
                    catch
                    {
                    }

                    IntegPath.CloseFigure();
                }

                old_dYdX = dYdX;
                old_FdX = FdX;
            }
            else
            {
                old_FdX = 0;
            }

            oldX = X;
            oldY = Y;
        }
    }

    public double ScreenToVirtualX(int X)
    {
        return (X - Area.Left) * (Xmax - Xmin) / Area.Width + Xmin;
    }

    public double ScreenToVirtualY(int Y)
    {
        return (Area.Height - Y + Area.Top) * (Ymax - Ymin) / Area.Height + Ymin;
    }

    public int VirtualToScreenX(double X)
    {
        var ScreenCoord = (int)Math.Round((X - Xmin) * Area.Width / (Xmax - Xmin));

        return ScreenCoord switch
        {
            < 0 => Area.Left - 1,
            var case1 when case1 > Area.Width => Area.Right + 1,
            _ => Area.Left + ScreenCoord,
        };
    }

    public int VirtualToScreenY(double Y)
    {
        var ScreenCoord = (int)Math.Round(Area.Height - (Y - Ymin) * Area.Height / (Ymax - Ymin));
        
        return ScreenCoord switch
        {
            < 0 => Area.Top - 1,
            var case1 when case1 > Area.Height => Area.Bottom + 1,
            _ => Area.Top + ScreenCoord,
        };
    }
    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                GraphPath?.Dispose();
                DerivPath?.Dispose();
                IntegPath?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.

            // TODO: set large fields to null.
        }

        disposedValue = true;
    }

    // ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    ~Surface()
    {
        // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(false);
    }

    // This code added by Visual Basic to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}

#endif
