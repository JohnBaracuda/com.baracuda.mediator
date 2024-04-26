using Baracuda.Mediator.Callbacks;
using Baracuda.Mediator.Registry;
using Baracuda.Utilities;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Mediator.Installer
{
    /// <summary>
    ///     Base class for assets that are used to install and setup runtime game systems.
    /// </summary>
    public abstract class Installer : ScriptableAsset, IComparable<Installer>
    {
        [SerializeField] private int priority;

        /// <summary>
        ///     Higher priorities are executed first.
        /// </summary>
        public virtual int Priority => priority;

        /// <summary>
        ///     Called on the installer during the installation process.
        /// </summary>
        public abstract UniTask InstallAsync();

        /// <summary>
        ///     Called on the installer after all installations have completed.
        /// </summary>
        public virtual void OnPostProcessInstallation()
        {
        }

        /// <summary>
        ///     Creates a divider GameObject and sets its sibling index to 0
        /// </summary>
        [Conditional("DEBUG")]
        protected void CreateDividerGameObject()
        {
            var divider = new GameObject(new string('-', 25));
            divider.DontDestroyOnLoad();
            divider.transform.SetSiblingIndex(0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AssetRegistry.RegisterInstaller(this);
        }

        public int CompareTo(Installer other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }
            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            if (Priority < other.Priority)
            {
                return 1;
            }
            return Priority > other.Priority ? -1 : 0;
        }
    }
}