using System;
using Windows.UI;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Win2DApp
{
    public struct RenderOption : IEquatable<RenderOption>
    {
        public RenderOption(float strokeWidth, Color color)
        {
            StrokeWidth = strokeWidth;
            Color = color;
        }

        public static RenderOption Default { get; } = new RenderOption(1f, Colors.Black);

        public float StrokeWidth { get; }

        public Color Color { get; }

        public override bool Equals(object obj)
        {
            if ((null == obj) || !(obj is RenderOption))
            {
                return false;
            }
            RenderOption value = (RenderOption)obj;
            return Equals(this, value);
        }

        public bool Equals(RenderOption value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return StrokeWidth.GetHashCode() ^
                   Color.GetHashCode();
        }

        public static bool operator ==(RenderOption left, RenderOption right)
        {
            return left.StrokeWidth == right.StrokeWidth &&
                   left.Color == right.Color;
        }

        public static bool operator !=(RenderOption left, RenderOption right)
        {
            return !(left == right);
        }

        public static bool Equals(RenderOption renderOption1, RenderOption renderOption2)
        {
            return renderOption1.StrokeWidth.Equals(renderOption2.StrokeWidth) &&
                   renderOption1.Color.Equals(renderOption2.Color);
        }
    }
}
