using System;

namespace Animation
{
    public static class AnimationLib
    {
        public static float SwayMovementOffset(double delta, float range, float speed)
        {
            // Implement sway movement calculation
            return (float)(Math.Sin(delta * speed) * 0.5 + 0.5) * range;
        }

        public static float BounceMovementOffset(double delta, float range, float speed)
        {
            // Implement bounce movement calculation
            return (float)(Math.Abs(Math.Sin(delta * speed)) * range);
        }

        public static float LinearMovementOffset(double delta, float range, float speed)
        {
            // Implement linear movement calculation
            return (float)((delta * speed) % range);
        }

        public static float LerpXScale(double delta, float current, float target, float speed)
        {
            // Implement linear interpolation for X scale
            if (Math.Abs(target - current) < 0.01f)
                return target;
            return current + (target - current) * speed * (float)delta;
        }

        public static float LerpYScale(double delta, float current, float target, float speed)
        {
            // Implement linear interpolation for Y scale
            if (Math.Abs(target - current) < 0.01f)
                return target;
            return current + (target - current) * speed * (float)delta;
        }

        public static float LerpAlpha(double delta, float current, float target, float speed)
        {
            // Implement linear interpolation for alpha
            if (Math.Abs(target - current) < 0.01f)
                return target;
            return current + (target - current) * speed * (float)delta;
        }

        public static float LerpRotation(double delta, float current, float target, float speed)
        {
            // Implement linear interpolation for rotation
            if (Math.Abs(target - current) < 0.01f)
                return target;
            return current + (target - current) * speed * (float)delta;
        }

        public static float LerpPositionX(double delta, float current, float target, float speed)
        {
            // Implement linear interpolation for position X
            if (Math.Abs(target - current) < 0.01f)
                return target;
            return current + (target - current) * speed * (float)delta;
        }

        public static float LerpPositionY(double delta, float current, float target, float speed)
        {
            // Implement linear interpolation for position Y
            if (Math.Abs(target - current) < 0.01f)
                return target;
            return current + (target - current) * speed * (float)delta;
        }
    }
}