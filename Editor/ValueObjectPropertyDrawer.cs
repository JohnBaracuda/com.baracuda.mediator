using Baracuda.EditorUtilities.Helper;
using Baracuda.Mediator.ValueObjects;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator.Editor
{
    [CustomPropertyDrawer(typeof(ValueObjectAssetT<>), true)]
    public class ValueObjectPropertyDrawer : PropertyDrawer
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
                DrawNullInspector(property, label);
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
            EditorGUILayout.BeginHorizontal();

            var serializedObject = new SerializedObject(property.objectReferenceValue);

            var valueProperty = serializedObject.FindProperty("value");

            serializedObject.Update();
            EditorGUILayout.PropertyField(valueProperty, label);
            GUIHelper.BeginIndentOverride(0);
            EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.MaxWidth(100));
            GUIHelper.EndIndentOverride();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawRuntimeInspector(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            var serializedObject = new SerializedObject(property.objectReferenceValue);

            var valueProperty = serializedObject.FindProperty("value");
            var cachedProperty = serializedObject.FindProperty("cached");

            serializedObject.Update();
            EditorGUILayout.PropertyField(valueProperty, label);
            GUI.enabled = false;
            GUIHelper.BeginIndentOverride(0);
            EditorGUILayout.PropertyField(cachedProperty, GUIContent.none, GUILayout.MaxWidth(100));
            GUIHelper.EndIndentOverride();
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();
        }

        //--------------------------------------------------------------------------------------------------------------

        private static void DrawNullInspector(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property, label);
            if (GUILayout.Button("Create", GUILayout.MaxWidth(80)))
            {
                var type = property.GetUnderlyingType();
                var scriptableObject = EditorHelper.CreateScriptableObjectAsset(type);
                property.objectReferenceValue = scriptableObject;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
