namespace Tetraizor.Utils;

using System;
using System.Text;
using Godot;

public class ColorUtils
{
    public enum BlendMode
    {
        Normal,
        Multiply,
        Screen,
        Overlay,
        Darken,
        Lighten,
        Difference,
        Addition,
        Subtract,
        Divide,
        Exclusion,
        Erase,
        HardErase,
        Alpha
    }

    public static Vector3 RGBtoHSV(Color color)
    {
        float r = color.R;
        float g = color.G;
        float b = color.B;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));

        float h = 0;
        if (max == min) // Check for division by zero
        {
            h = 0;
        }
        else
        {
            if (max == r)
                h = 60 * (g - b) / (max - min) + (g < b ? 360 : 0);
            else if (max == g)
                h = 60 * (b - r) / (max - min) + 120;
            else if (max == b)
                h = 60 * (r - g) / (max - min) + 240;
        }

        float s = max == 0 ? 0 : 1 - min / max;
        float v = max;

        return new Vector3(h / 360, s, 1.0f - v);
    }

    public static Color HSVtoRGB(Vector3 hsv)
    {
        hsv.X = hsv.X * 360.0f;

        float h = hsv.X;
        float s = hsv.Y;
        float v = hsv.Z;

        float c = v * s;
        float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        float m = v - c;

        float r = 0;
        float g = 0;
        float b = 0;

        if (h >= 0 && h < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h >= 60 && h < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h >= 120 && h < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h >= 180 && h < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h >= 240 && h < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else if (h >= 300 && h < 360)
        {
            r = c;
            g = 0;
            b = x;
        }

        return new Color(r + m, g + m, b + m);
    }

    public static Color BlendColors(Color background, Color overlay, BlendMode blendMode)
    {
        Color blended = background;

        switch (blendMode)
        {
            case BlendMode.Normal:
                blended = Normal(background, overlay);
                break;
            case BlendMode.Multiply:
                blended = Multiply(background, overlay);
                break;
            case BlendMode.Screen:
                blended = Screen(background, overlay);
                break;
            case BlendMode.Overlay:
                blended = Overlay(background, overlay);
                break;
            case BlendMode.Darken:
                blended = Darken(background, overlay);
                break;
            case BlendMode.Lighten:
                blended = Lighten(background, overlay);
                break;
            case BlendMode.Difference:
                blended = Difference(background, overlay);
                break;
            case BlendMode.Addition:
                blended = Addition(background, overlay);
                break;
            case BlendMode.Subtract:
                blended = Subtract(background, overlay);
                break;
            case BlendMode.Divide:
                blended = Divide(background, overlay);
                break;
            case BlendMode.Exclusion:
                blended = Exclusion(background, overlay);
                break;
            case BlendMode.Erase:
                blended = Erase(background, overlay);
                break;
            case BlendMode.HardErase:
                blended = HardErase(background, overlay);
                break;
            case BlendMode.Alpha:
                blended = AlphaBlend(background, overlay);
                break;
        }

        return blended;
    }

    private static Color Normal(Color background, Color overlay)
    {
        return new Color(
            overlay.R,
            overlay.G,
            overlay.B,
            overlay.A);
    }

    private static Color Multiply(Color bottom, Color top)
    {
        return new Color(bottom.R * top.R, bottom.G * top.G, bottom.B * top.B, bottom.A * top.A);
    }

    private static Color Screen(Color bottom, Color top)
    {
        return new Color(
            1 - (1 - bottom.R) * (1 - top.R),
            1 - (1 - bottom.G) * (1 - top.G),
            1 - (1 - bottom.B) * (1 - top.B),
            bottom.A * top.A);
    }

    private static Color Overlay(Color bottom, Color top)
    {
        return new Color(
            bottom.R < 0.5 ? 2 * bottom.R * top.R : 1 - 2 * (1 - bottom.R) * (1 - top.R),
            bottom.G < 0.5 ? 2 * bottom.G * top.G : 1 - 2 * (1 - bottom.G) * (1 - top.G),
            bottom.B < 0.5 ? 2 * bottom.B * top.B : 1 - 2 * (1 - bottom.B) * (1 - top.B),
            bottom.A * top.A);
    }

    private static Color Darken(Color bottom, Color top)
    {
        return new Color(
            Math.Min(bottom.R, top.R),
            Math.Min(bottom.G, top.G),
            Math.Min(bottom.B, top.B),
            bottom.A * top.A);
    }

    private static Color Lighten(Color bottom, Color top)
    {
        return new Color(
            Math.Max(bottom.R, top.R),
            Math.Max(bottom.G, top.G),
            Math.Max(bottom.B, top.B),
            bottom.A * top.A);
    }

    private static Color Difference(Color bottom, Color top)
    {
        return new Color(
            Math.Abs(bottom.R - top.R),
            Math.Abs(bottom.G - top.G),
            Math.Abs(bottom.B - top.B),
            bottom.A * top.A);
    }

    private static Color Addition(Color bottom, Color top)
    {
        return new Color(
            Math.Min(bottom.R + top.R, 1.0f),
            Math.Min(bottom.G + top.G, 1.0f),
            Math.Min(bottom.B + top.B, 1.0f),
            bottom.A * top.A);
    }

    private static Color Subtract(Color bottom, Color top)
    {
        return new Color(
            Math.Max(bottom.R - top.R, 0),
            Math.Max(bottom.G - top.G, 0),
            Math.Max(bottom.B - top.B, 0),
            bottom.A * top.A);
    }

    private static Color Divide(Color bottom, Color top)
    {
        return new Color(
            top.R == 0 ? 1 : bottom.R / top.R,
            top.G == 0 ? 1 : bottom.G / top.G,
            top.B == 0 ? 1 : bottom.B / top.B,
            bottom.A * top.A);
    }

    private static Color Exclusion(Color bottom, Color top)
    {
        return new Color(
            bottom.R + top.R - 2 * bottom.R * top.R,
            bottom.G + top.G - 2 * bottom.G * top.G,
            bottom.B + top.B - 2 * bottom.B * top.B,
            bottom.A * top.A);
    }

    private static Color Erase(Color bottom, Color top)
    {
        return new Color(
            bottom.R,
            bottom.G,
            bottom.B,
            bottom.A * (1 - top.A)); // Soft erasing by reducing alpha
    }

    private static Color HardErase(Color bottom, Color top)
    {
        return new Color(
            bottom.R,
            bottom.G,
            bottom.B,
            bottom.A * (1 - top.A) * 0.5f); // Hard erasing by setting alpha to zero
    }

    private static Color AlphaBlend(Color bottom, Color top)
    {
        float alpha = top.A;
        return new Color(
            (top.R * alpha) + (bottom.R * (1 - alpha)),
            (top.G * alpha) + (bottom.G * (1 - alpha)),
            (top.B * alpha) + (bottom.B * (1 - alpha)),
            alpha + (bottom.A * (1 - alpha)));
    }
}