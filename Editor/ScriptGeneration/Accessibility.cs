using System;

namespace Baracuda.Bedrock.Editor.ScriptGeneration
{
    public enum AccessibilityModifiers
    {
        Public = 1,
        Private = 2,
        Protected = 3,
        Internal = 4,
        ProtectedInternal = 5,
        PrivateProtected = 6
    }

    public static class AccessibilityModifiersExtensions
    {
        public static string AsString(this AccessibilityModifiers accessibilityModifier)
        {
            switch (accessibilityModifier)
            {
                case AccessibilityModifiers.Public:
                    return "public";

                case AccessibilityModifiers.Private:
                    return "private";

                case AccessibilityModifiers.Protected:
                    return "protected";

                case AccessibilityModifiers.Internal:
                    return "internal";

                case AccessibilityModifiers.ProtectedInternal:
                    return "protected internal";

                case AccessibilityModifiers.PrivateProtected:
                    return "private protected";

                default:
                    throw new ArgumentOutOfRangeException(nameof(accessibilityModifier), accessibilityModifier, null);
            }
        }

        public static string AsStringSanitizeForType(this AccessibilityModifiers accessibilityModifier)
        {
            if (accessibilityModifier == AccessibilityModifiers.Public)
            {
                return "public";
            }
            return "internal";
        }
    }
}