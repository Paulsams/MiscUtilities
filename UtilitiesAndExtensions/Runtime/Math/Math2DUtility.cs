using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities related to 2D mathematics. Currently all methods involve 2D angles.
    /// </summary>
    public static class Math2DUtility
    {
        /// <summary>
        /// Allows you to get an angle based on the direction in the trigonometric system.
        /// </summary>
        /// <param name="direction"> Normalized direction required. </param>
        /// <returns> Angle in [0; 360) degrees. </returns>
        public static float GetAngleFromVector(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0f)
                angle += 360f;
            return angle;
        }

        /// <summary>
        /// Allows you to get directions based on angle in the trigonometric system.
        /// </summary>
        /// <param name="angle"> Angle in degrees. </param>
        /// <returns> Normalized direction. </returns>
        public static Vector2 GetVectorFromAngle(float angle)
        {
            float angleRadian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
        }

        /// <summary>
        /// Clamp angle in the range [-180; 180).
        /// </summary>
        /// <param name="angle"> Any angle in degrees. </param>
        /// <returns> Аngle in the range [-180; 180). </returns>
        public static float ClampAngle(float angle) => (angle + 360f) % 360f - 180f;

        /// <summary>
        /// Finds the nearest angle relative to the step.
        /// </summary>
        /// <param name="angle"> Angle in degrees. </param>
        /// <param name="stepRotation"> Step rotation in [0; 360). </param>
        /// <returns></returns>
        public static float NearestAngle(float angle, float stepRotation) =>
            Mathf.Round(angle / stepRotation) * stepRotation % 360f;

        /// <summary>
        /// Finds the nearest angle relative to the array of angles [-180; 180].
        /// </summary>
        /// <param name="angle"> Angle in [0; 360] degrees. </param>
        /// <param name="endAngles"> Arrays nearested angles in [-180; 180] degrees. </param>
        /// <returns> Angle in [0; 360] degrees. </returns>
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