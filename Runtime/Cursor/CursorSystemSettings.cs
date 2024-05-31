using Baracuda.Bedrock.Odin;
using Baracuda.Bedrock.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    public class CursorSystemSettings : SettingsAsset
    {
        [Foldout("Cursor Assets")]
        [SerializeField] private CursorType startCursor;
        [SerializeField] [Required] private CursorSet startCursorSet;

        public CursorType StartCursor => startCursor;
        public CursorSet StartCursorSet => startCursorSet;
    }
}