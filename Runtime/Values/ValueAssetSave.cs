using Baracuda.Mediator.Callbacks;
using Baracuda.Mediator.Events;
using Baracuda.Serialization;
using Baracuda.Tools;
using Baracuda.Utilities.Types;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Baracuda.Mediator.Values
{
    public abstract class ValueAssetSave<TValue> : ValueAssetRW<TValue>, IValueAsset<TValue>,
        IPersistentDataAsset<TValue>
    {
        [Line]
        [LabelText("Default Value")]
        [SerializeField] private TValue defaultPersistentValue;
        [SerializeField] private RuntimeGUID guid;
        [Tooltip("The level to store the data on. Either profile specific or shared between profiles")]
        [SerializeField] private StorageLevel storageLevel = StorageLevel.Profile;

        [NonSerialized] private readonly Broadcast<TValue> _changedEvent = new();
        [NonSerialized] private StoreOptions _storeOptions;

        [ShowInInspector]
        public override TValue Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public override void SetValue(TValue value)
        {
            if (EqualityComparer<TValue>.Default.Equals(value, GetValue()))
            {
                return;
            }
            Profile.SaveFile(Key, value, _storeOptions);
            _changedEvent.Raise(value);
        }

        public override TValue GetValue()
        {
            return Profile.LoadFile<TValue>(Key, _storeOptions);
        }

        /// <summary>
        ///     Called when the value was changed.
        /// </summary>
        public override event Action<TValue> Changed
        {
            add => _changedEvent.Add(value);
            remove => _changedEvent.Remove(value);
        }


        #region Initialization

        protected override void OnEnable()
        {
            base.OnEnable();
            _storeOptions = new StoreOptions(name);
            if (FileSystem.IsInitialized)
            {
                LoadPersistentData();
            }
            else
            {
                FileSystem.InitializationCompleted -= LoadPersistentData;
                FileSystem.InitializationCompleted += LoadPersistentData;
            }
        }

        [CallbackOnInitialization]
        private void Initialize()
        {
            LoadPersistentData();
        }

        [CallbackOnApplicationQuit]
        private void Shutdown()
        {
        }

        #endregion


        #region Persistent Data

        private ISaveProfile Profile => storageLevel switch
        {
            StorageLevel.Profile => FileSystem.Profile,
            StorageLevel.SharedProfile => FileSystem.SharedProfile,
            var _ => throw new ArgumentOutOfRangeException()
        };

        private string Key => guid.ToString();

        [Line]
        [Button]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 8)]
        private void OpenInFileSystem()
        {
            var dataPath = Application.persistentDataPath;
            var systemPath = FileSystem.RootFolder;
            var profilePath = Profile.Info.FolderName;
            var folderPath = Path.Combine(dataPath, systemPath, profilePath);
            Application.OpenURL(folderPath);
        }

        [Button("Load")]
        [ButtonGroup("Persistent")]
        public void LoadPersistentData()
        {
            if (Profile.HasFile(Key) is false)
            {
                SetValue(defaultPersistentValue);
            }
            else
            {
                var value = Profile.LoadFile<TValue>(Key);
                SetValue(value);
            }
        }

        [Button("Reset")]
        [ButtonGroup("Persistent")]
        public void ResetPersistentData()
        {
            Value = defaultPersistentValue;
        }

        #endregion
    }
}