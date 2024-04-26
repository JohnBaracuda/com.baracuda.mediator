using Baracuda.Mediator.Settings;
using Baracuda.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Mediator.Cursor
{
    public class CursorSystemSettings : SettingsAsset
    {
        [Foldout("Cursor Assets")]
        [SerializeField] private CursorType startCursor;
        [SerializeField] [Required] private CursorSet startCursorSet;

        [Foldout("Mediator")]
        [SerializeField] [Required] private HideCursorLocks cursorHide;
        [SerializeField] [Required] private ConfineCursorLocks cursorConfine;
        [SerializeField] [Required] private LockCursorLocks cursorLock;

        public CursorType StartCursor => startCursor;
        public CursorSet StartCursorSet => startCursorSet;
        public HideCursorLocks CursorHide => cursorHide;
        public ConfineCursorLocks CursorConfine => cursorConfine;
        public LockCursorLocks CursorLock => cursorLock;
    }
}