using Baracuda.Utilities;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator
{

    [CustomPropertyDrawer(typeof(ValueRW<>), true)]
    [CustomPropertyDrawer(typeof(ValueRO<>), true)]
    public class ValuePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        ///   <para>Override this method to specify how tall the GUI for this field is in pixels.</para>
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>
        ///   <para>The height in pixels.</para>
        /// </returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        /// <summary>
        ///   <para>Override this method to make your own IMGUI based GUI for the property.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var byReferenceProperty = property.FindPropertyRelative("byReference");
            var referenceProperty = property.FindPropertyRelative("reference");
            var valueProperty = property.FindPropertyRelative("value");

            var index = byReferenceProperty.boolValue ? 1 : 0;
            index = EditorGUILayout.Popup(index, new[] {"Value", "Reference"});
            byReferenceProperty.boolValue = index == 1;
            EditorGUILayout.PropertyField(byReferenceProperty.boolValue ? referenceProperty : valueProperty, label);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
