using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Attribute so that you can see value in the inspector, but not change it.
    /// But the order is important for him if he has some kind of custom rendering and it may not always work.
    /// </summary>
    public class ReadonlyFieldAttribute : PropertyAttribute
    {
    }
}