using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class MathUtility
    {
        public static float GetAngleFromVector(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0f)
            {
                angle += 360f;
            }

            return angle;
        }

        public static Vector2 GetVectorFromAngle(float angle)
        {
            float angleRadian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
        }

        public static float ClampAngle(float angle) => (angle + 360f) % 360f - 180f;

        public static float NearestAngle(float angle, float stepRotation) =>
            Mathf.Round(angle / stepRotation) * stepRotation % 360f;

        public static float NearestAngle(float angle, float[] endAngles)
        {
            angle -= 180f;

            float minAngle = 360f;
            float minEndAngle = 0f;

            for (int i = 0; i < endAngles.Length; ++i)
            {
                float substractionAngles = Mathf.Abs(angle - endAngles[i]);
                if (substractionAngles < minAngle)
                {
                    minAngle = substractionAngles;
                    minEndAngle = endAngles[i];
                }
            }

            minEndAngle += 180f;

            return minEndAngle;
        }
    }
}