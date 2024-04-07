using Baracuda.Mediation.Editor.Generation;
using Baracuda.Utilities.Editor.Inspector;
using System.Collections.Generic;
using UnityEngine;
using GUIUtility = Baracuda.Utilities.Editor.Helper.GUIUtility;

namespace Baracuda.Mediation.Editor
{
    public class MediatorSettingsProvider : UnityEditor.SettingsProvider
    {
        private FoldoutHandler Foldout { get; } = new(nameof(MediatorSettingsProvider));

        public MediatorSettingsProvider(string path, UnityEditor.SettingsScope scopes,
            IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        private UnityEditor.Editor _settingsEditor;

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            var serializedObject = new UnityEditor.SerializedObject(MediatorEditorSettings.instance);

            _settingsEditor ??= UnityEditor.Editor.CreateEditor(serializedObject.targetObject);

            serializedObject.Update();
            _settingsEditor.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();

            MediatorEditorSettings.instance.SaveSettings();

            if (GUI.changed)
            {
                Foldout.SaveState();
            }

            GUIUtility.Space();
            GUIUtility.DrawLine();
            GUIUtility.Space();

            if (GUILayout.Button("Generate Mediator"))
            {
                MediatorTypeGeneration.GenerateMediatorClasses();
            }

            GUIUtility.Space();
        }

        [UnityEditor.SettingsProviderAttribute]
        public static UnityEditor.SettingsProvider CreateSettingsProvider()
        {
            return new MediatorSettingsProvider("Project/Baracuda/Mediator", UnityEditor.SettingsScope.Project);
        }

        [UnityEditor.MenuItem("Tools/Mediator/Settings", priority = 5000)]
        public static void MenuItem()
        {
            UnityEditor.SettingsService.OpenProjectSettings("Project/Baracuda/Mediator");
        }
    }
}