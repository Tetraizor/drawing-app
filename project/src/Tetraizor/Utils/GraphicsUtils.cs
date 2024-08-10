using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tetraizor.Utils;

public class GraphicsUtils
{
    public static Vector2I[] GetBresenhamsPoints(Vector2I start, Vector2I end, int step = 0)
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
        int yStep = (y0 < y1) ? 1 : -1;
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
                y += yStep;
                error += dx;
            }
        }

        if (step <= 0) return points;

        int stepCount = Mathf.CeilToInt((float)points.Length / step);

        Vector2I[] result = new Vector2I[stepCount];
        for (int i = 0; i < stepCount; i += 1)
        {
            int current = Math.Min(i * step, points.Length - 1);

            result[i] = points[current];
        }

        return result;
    }

    public static bool[] GetFilledCirclePoints(int radius)
    {
        int diameter = 2 * radius + 1;
        bool[] points = new bool[diameter * diameter];

        int x = radius;
        int y = 0;
        int p = 1 - radius;

        while (x >= y)
        {
            DrawScanLine(points, radius, x, y);
            y++;
            if (p <= 0)
            {
                p = p + 2 * y + 1;
            }
            else
            {
                x--;
                p = p + 2 * y - 2 * x + 1;
            }
        }

        return points;
    }

    private static void DrawScanLine(bool[] points, int radius, int x, int y)
    {
        PutPixelLine(points, radius - x, radius + x, radius + y, radius); // Horizontal line at y
        PutPixelLine(points, radius - y, radius + y, radius + x, radius); // Horizontal line at x
        PutPixelLine(points, radius - x, radius + x, radius - y, radius); // Horizontal line at -y
        PutPixelLine(points, radius - y, radius + y, radius - x, radius); // Horizontal line at -x
    }

    static void PutPixelLine(bool[] points, int xStart, int xEnd, int y, int radius)
    {
        int diameter = 2 * radius + 1;
        for (int x = xStart; x <= xEnd; x++)
        {
            points[y * diameter + x] = true;
        }
    }

    private static void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}