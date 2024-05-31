using Baracuda.Tools;
using Baracuda.Utilities.Editor.Helper;
using System;
using UnityEngine;
using GUIUtility = Baracuda.Utilities.Editor.Helper.GUIUtility;

namespace Baracuda.Bedrock.Editor.Tools
{
    [UnityEditor.CustomPropertyDrawer(typeof(FlagsToggleAttribute))]
    public class FlagsToggleDrawer : UnityEditor.PropertyDrawer
    {
        private Type _underLying;

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return -2f;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != UnityEditor.SerializedPropertyType.Enum)
            {
                GUIUtility.DrawMessageBox("Property must be an enum!");
                return;
            }

            _underLying ??= property.GetUnderlyingType() ?? throw new Exception();
            property.enumValueFlag = GUIUtility.DrawFlagsEnumAsToggle(property.enumValueFlag, _underLying, true);
        }
    }
}