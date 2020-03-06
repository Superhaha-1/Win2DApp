using System;

namespace Win2DApp
{
    public static class Extensions
    {
        public static float ToAngle(this float angle)
        {
            if(angle > 360)
            {
                return (angle - 360).ToAngle();
            }
            if(angle < 0)
            {
                return (angle + 360).ToAngle();
            }
            return angle;
        }

        public static float AngleToRadians(this float angle)
        {
            return angle * MathF.PI / 180f;
        }
    }
}
