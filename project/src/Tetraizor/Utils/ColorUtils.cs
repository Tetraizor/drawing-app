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