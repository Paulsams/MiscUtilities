using System.Collections.Generic;
using UnityEngine;

namespace Paulsams.MicsUtil
{
    public static class PhysicsMyUtilities
    {
        public static void IgnoreCollision(IList<Collider> collidersFirst, IList<Collider> collidersSecond, bool state)
        {
            for (int i = 0; i < collidersFirst.Count; ++i)
            {
                for (int j = 0; j < collidersSecond.Count; ++j)
                {
                    Physics.IgnoreCollision(collidersFirst[i], collidersSecond[j], state);
                }
            }
        }
    }
}