using Baracuda.Utilities;
using Baracuda.Utilities.Helper;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator
{
    [CustomPropertyDrawer(typeof(ValueAsset<>), true)]
    public class ValueAssetPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null)
            {
                DrawDefaultInspector(property, label);
            }
            else
            {
                EditorGUILayout.PropertyField(property, label);
            }
        }

        private static void DrawDefaultInspector(SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                DrawRuntimeInspector(property, label);
            }
            else
            {
                DrawEditorInspector(property, label);
            }
        }


        //--------------------------------------------------------------------------------------------------------------

        private static void DrawEditorInspector(SerializedProperty property, GUIContent label)
        {
            var serializedObject = new SerializedObject(property.objectReferenceValue);
            var valueProperty = serializedObject.FindProperty("value");

            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(valueProperty, label);
            GUIHelper.BeginIndentOverride(0);
            EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.MaxWidth(100));
            GUIHelper.EndIndentOverride();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawRuntimeInspector(SerializedProperty property, GUIContent label)
        {
            var serializedObject = new SerializedObject(property.objectReferenceValue);

            var valueProperty = serializedObject.FindProperty("value");
            var cachedProperty = serializedObject.FindProperty("cached");

            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(valueProperty, label);
            GUI.enabled = false;
            GUIHelper.BeginIndentOverride(0);
            EditorGUILayout.PropertyField(cachedProperty, GUIContent.none, GUILayout.MaxWidth(100));
            GUIHelper.EndIndentOverride();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
