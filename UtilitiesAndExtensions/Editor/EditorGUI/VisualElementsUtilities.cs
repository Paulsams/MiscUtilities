using System.Reflection;
using UnityEngine.UIElements;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities associated with class: <see cref="T:UnityEngine.UIElements.VisualElement"/>.
    /// </summary>
    public static class VisualElementsUtilities
    {
        /// <summary>
        /// Creates and returns a container to draw something at the level of the foldout itself, and also returns a checkmark.
        /// </summary>
        /// <param name="foldout"> Foldout element on which modifications are made. </param>
        /// <param name="containerOnSameRowWithToggle"> Container is in line with the foldout. </param>
        /// <param name="checkmark"> Checkmark in foldout. </param>
        public static void SetAlignedLabelFromFoldout(Foldout foldout, out VisualElement containerOnSameRowWithToggle, out VisualElement checkmark)
        {
            var toggle = foldout.Q<Toggle>();
            containerOnSameRowWithToggle = new VisualElement();
            containerOnSameRowWithToggle.style.flexGrow = 1f;
            toggle.Add(containerOnSameRowWithToggle);
            toggle.AddToClassList(BaseField<Label>.alignedFieldUssClassName);
            checkmark = toggle.Q<VisualElement>("unity-checkmark");
            
            var labelToggle = toggle.Q<Label>();
            
            var tempToggle = toggle;
            var tempCheckmark = checkmark;
            var tempContainerOnSameRowWithToggle = containerOnSameRowWithToggle;
            void OnGeometryChangedEvent(GeometryChangedEvent callback)
            {
                tempToggle[0].style.marginRight = -tempCheckmark.worldBound.width;
                tempToggle.Query(className: "unity-text-element").ForEach((element) =>
                {
                    if (element == labelToggle)
                        return;
                    // Because of "unity-foldout__toggle .unity-text-element"
                    element.style.marginLeft = 2;
                });
                tempContainerOnSameRowWithToggle.style.marginLeft = -3;
                tempContainerOnSameRowWithToggle.style.marginTop = -1;
                tempContainerOnSameRowWithToggle.style.marginBottom = -1;
                labelToggle.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            }
            
            typeof(BaseField<bool>).GetProperty("labelElement",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(toggle, labelToggle);
            labelToggle.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            toggle[0].style.flexGrow = 0f;
        }
    }
}
