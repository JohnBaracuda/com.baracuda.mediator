using Baracuda.Utilities.Helper;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator
{
    [CustomPropertyDrawer(typeof(Statement))]
    public class StatementPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("checkMethod");
            var conditionsProperty = property.FindPropertyRelative("conditions");

            GUIHelper.BeginBox();
            EditorGUILayout.LabelField(label);
            EditorGUILayout.PropertyField(typeProperty);
            GUIHelper.IncreaseIndent();
            EditorGUILayout.PropertyField(conditionsProperty);
            GUIHelper.DecreaseIndent();
            GUIHelper.EndBox();
        }
    }
}
