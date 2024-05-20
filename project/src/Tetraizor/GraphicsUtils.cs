using System;
using System.Collections.Generic;
using Godot;

public class GraphicsUtils
{
    public static Vector2I[] GetBresenhamsPoints(Vector2I start, Vector2I end)
    {
        int x0 = start.X;
        int y0 = start.Y;
        int x1 = end.X;
        int y1 = end.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int n = Math.Max(dx, dy) + 1; // Number of points in the line
        Vector2I[] points = new Vector2I[n];
        int index = 0;

        bool steep = dy > dx;

        if (steep)
        {
            // Swap x and y
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
            Swap(ref dx, ref dy);
        }

        if (x0 > x1)
        {
            // Swap start and end points
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int error = dx / 2;
        int ystep = (y0 < y1) ? 1 : -1;
        int y = y0;

        for (int x = x0; x <= x1; x++)
        {
            if (steep)
            {
                points[index++] = new Vector2I(y, x);
            }
            else
            {
                points[index++] = new Vector2I(x, y);
            }

            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }

        return points;
    }

    private static void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}