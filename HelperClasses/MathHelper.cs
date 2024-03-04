using UnityEngine;

namespace _Project.Scripts.HelperClasses
{
    public static class MathHelper
    {
        public static float TAU => 6.283185307179586f; 
        public static Vector3 AngleToDirection(float angle) => new (Mathf.Cos(angle), Mathf.Sin(angle), 0);
        public static float DirectionToAngle(Vector3 direction) => Mathf.Atan2(direction.y, direction.x);
        public static float CalculateValueOverPer(this float value, float percentage) => (value / 100) * percentage;
        public static float CalculatePercentage(this float value, float total) => (value / total) * 100;
        public static float Remap(this float value, float from1, float to1, float from2, float to2) => 
            (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        public static double Pow (double a, double b) {
            double result = 1;
            for (int i = 0; i < b; i++) {
                result *= a;
            }
            return result;
        }
    }
}
