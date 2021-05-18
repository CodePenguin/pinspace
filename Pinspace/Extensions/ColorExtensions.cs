using System;
using System.Drawing;

namespace Pinspace.Extensions
{
    public static class ColorExtensions
    {
        public static double GetPerceivedBrightness(this Color color)
        {
            // http://alienryderflex.com/hsp.html
            return Math.Sqrt(
                0.299 * color.R * color.R +
                0.587 * color.G * color.G +
                0.114 * color.B * color.B);
        }

        public static bool IsLight(this Color color)
        {
            const double threshold = 146.8;
            return color.GetPerceivedBrightness() > threshold;
        }

        public static Color TextColor(this Color color)
        {
            return color.IsLight() ? Color.Black : Color.White;
        }
    }
}
