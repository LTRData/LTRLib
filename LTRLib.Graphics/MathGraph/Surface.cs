#if NET6_OR_GREATER

using LTRLib.MathExpression;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System;

namespace LTRLib.Graphics.MathGraph;

public class Surface : IDisposable
{
    // Paths for the current graphs to be drawn when surface picturebox is painted
    private IPath? GraphPath;
    private IPath? DerivPath;
    private IPath? IntegPath;

    // Cache these values in variable for performance reasons
    public double Xmin;
    public double Xmax;
    public double Ymin;
    public double Ymax;

    public Rectangle Area;

    public void Clear()
    {
        GraphPath = null;
        DerivPath = null;
        IntegPath = null;
    }

    public void DrawAxis(IImageProcessingContext Image, Pen Pen)
    {
        Image.DrawLine(Pen, new PointF(VirtualToScreenX(Xmin), VirtualToScreenY(0d)), new PointF(VirtualToScreenX(Xmax), VirtualToScreenY(0d)));
        Image.DrawLine(Pen, new PointF(VirtualToScreenX(0d), VirtualToScreenY(Ymin)), new PointF(VirtualToScreenX(0d), VirtualToScreenY(Ymax)));
    }

    public void DrawGraph(IImageProcessingContext Image, Pen Pen) => Image.Draw(Pen, GraphPath ?? throw new InvalidOperationException("Not initialized"));

    public void DrawDerivative(IImageProcessingContext Image, Pen Pen) => Image.Draw(Pen, DerivPath ?? throw new InvalidOperationException("Not initialized"));

    public void DrawIntegral(IImageProcessingContext Image, Pen Pen) => Image.Draw(Pen, IntegPath ?? throw new InvalidOperationException("Not initialized"));

    public void Refresh(ScriptControl ScriptControl)
    {
        // The (to-be-calculated) Y values as init as Nothing. Later in code that is used
        // as the value for "undefined Y".
        double? X;
        double? Y = null;
        double? oldX = null;
        double? oldY = null;

        double? old_dYdX = null;
        double old_FdX = 0;

        var graphPathBuilder = new PathBuilder();
        var derivPathBuilder = new PathBuilder();
        var integPathBuilder = new PathBuilder();

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
                    graphPathBuilder.StartFigure();
                    try
                    {
                        graphPathBuilder.AddLine(Tracker - 1, VirtualToScreenY(oldY.Value), Tracker, VirtualToScreenY(Y.Value));
                    }
                    catch
                    {
                    }

                    graphPathBuilder.CloseFigure();
                }

                // Calcualte derivate by it's definition
                var dYdX = (Y - oldY) / (X - oldX);

                // Only plot derivate curve if last derivate point was defined
                if (old_dYdX.HasValue)
                {
                    // Same here, don't draw "through infinity"
                    if ((old_dYdX > Ymin && old_dYdX < Ymax) || (dYdX > Ymin && dYdX < Ymax))
                    {
                        derivPathBuilder.StartFigure();
                        try
                        {
                            derivPathBuilder.AddLine(Tracker - 1, VirtualToScreenY(old_dYdX.Value), Tracker, VirtualToScreenY(dYdX!.Value));
                        }
                        catch
                        {
                        }

                        derivPathBuilder.CloseFigure();
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
                else if ((old_FdX > Ymin && old_FdX < Ymax) || (FdX > Ymin && FdX < Ymax))
                {
                    integPathBuilder.StartFigure();
                    try
                    {
                        integPathBuilder.AddLine(Tracker - 1, VirtualToScreenY(old_FdX), Tracker, VirtualToScreenY(FdX!.Value));
                    }
                    catch
                    {
                    }

                    integPathBuilder.CloseFigure();
                }

                old_dYdX = dYdX;
                old_FdX = FdX!.Value;
            }
            else
            {
                old_FdX = 0;
            }

            oldX = X;
            oldY = Y;
        }

        GraphPath = graphPathBuilder.Build();
        DerivPath = derivPathBuilder.Build();
        IntegPath = integPathBuilder.Build();
    }

    public double ScreenToVirtualX(int X) => (X - Area.Left) * (Xmax - Xmin) / Area.Width + Xmin;

    public double ScreenToVirtualY(int Y) => (Area.Height - Y + Area.Top) * (Ymax - Ymin) / Area.Height + Ymin;

    public int VirtualToScreenX(double X)
    {
        var ScreenCoord = (int)Math.Round((X - Xmin) * Area.Width / (Xmax - Xmin));
        if (ScreenCoord < 0)
        {
            return Area.Left - 1;
        }
        else if (ScreenCoord > Area.Width)
        {
            return Area.Right + 1;
        }
        else
        {
            return Area.Left + ScreenCoord;
        }
    }

    public int VirtualToScreenY(double Y)
    {
        var ScreenCoord = (int)Math.Round(Area.Height - (Y - Ymin) * Area.Height / (Ymax - Ymin));
        if (ScreenCoord < 0)
        {
            return Area.Top - 1;
        }
        else if (ScreenCoord > Area.Height)
        {
            return Area.Bottom + 1;
        }
        else
        {
            return Area.Top + ScreenCoord;
        }
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
