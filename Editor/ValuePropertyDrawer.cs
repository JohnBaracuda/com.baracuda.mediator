using Baracuda.Utilities;
using Baracuda.Utilities.Helper;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator
{
    [CustomPropertyDrawer(typeof(Variable<>), true)]
    [CustomPropertyDrawer(typeof(VariableRO<>), true)]
    public class ValuePropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent modeLabel = new GUIContent("Pass By", "Set the value directly or set a reference to a value object.");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var byReferenceProperty = property.FindPropertyRelative("byReference");
            var referenceProperty = property.FindPropertyRelative("reference");
            var valueProperty = property.FindPropertyRelative("value");

            EditorGUILayout.PropertyField(byReferenceProperty.boolValue ? referenceProperty : valueProperty, label);

            var index = byReferenceProperty.boolValue ? 1 : 0;
            index = EditorGUILayout.Popup(modeLabel, index, new[] {"Value", "Reference"});
            byReferenceProperty.boolValue = index == 1;

            GUIHelper.Space(1);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
