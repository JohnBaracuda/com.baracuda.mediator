using Baracuda.Bedrock.Odin;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Bedrock.Statistics
{
    public class IntegerStat : StatAsset<ulong>
    {
        #region Fields

        [Foldout("Settings")]
        [Title("Stat")]
        [SerializeField] private Modification type;

        [ShowIf(nameof(type), Modification.Increment)]
        [SerializeField] [Min(1)] private ulong increment = 1;

        [ShowIf(nameof(type), Modification.Minimal)]
        [SerializeField] private ulong defaultMinimal = ulong.MaxValue;

        #endregion


        #region Overrides

        protected override ulong DefaultValue()
        {
            return type == Modification.Minimal ? defaultMinimal : 0;
        }

        public override Modification Type()
        {
            return type;
        }

        public override string ValueString => Value.ToString();

        #endregion


        #region Public

        public UpdateResult<ulong> IncrementStat()
        {
            Assert.IsTrue(type == Modification.Increment);
            var from = StatData.value;
            var to = from + increment;
            StatData.value = to;
            SetStatDirty();
            return new UpdateResult<ulong>(type, from, to, true);
        }

        public UpdateResult<ulong> IncrementStat(ulong value)
        {
            Assert.IsTrue(type == Modification.Increment);
            var from = StatData.value;
            var to = from + value;
            StatData.value = to;
            SetStatDirty();
            return new UpdateResult<ulong>(type, from, to, true);
        }

        public UpdateResult<ulong> UpdateStat(ulong value)
        {
            Assert.IsTrue(type == Modification.Increment);
            var from = StatData.value;
            StatData.value = value;
            SetStatDirty();
            return new UpdateResult<ulong>(type, from, value, true);
        }

        public UpdateResult<ulong> HighscoreStat(ulong value)
        {
            Assert.IsTrue(type == Modification.Highscore);
            var current = StatData.value;
            if (current < value)
            {
                StatData.value = value;
                SetStatDirty();
                return new UpdateResult<ulong>(type, current, value, true);
            }
            return new UpdateResult<ulong>(type, current, current, false);
        }

        public UpdateResult<ulong> MinimalStat(ulong value)
        {
            Assert.IsTrue(type == Modification.Minimal);
            var current = StatData.value;
            if (current > value)
            {
                StatData.value = value;
                SetStatDirty();
                return new UpdateResult<ulong>(type, current, value, true);
            }
            return new UpdateResult<ulong>(type, current, current, false);
        }

        #endregion


        #region Debug

#if UNITY_EDITOR
        [Button("Increment")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Increment)]
        public void ButtonIncrementStat()
        {
            IncrementStat();
        }

        [Button("Increment")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Increment)]
        public void ButtonIncrementStat(int value)
        {
            IncrementStat((ulong) value);
        }

        [Button("Update")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Update)]
        public void ButtonUpdateStat(int value)
        {
            UpdateStat((ulong) value);
        }

        [Button("Update Highscore")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Highscore)]
        public void ButtonHighscoreStat(int value)
        {
            HighscoreStat((ulong) value);
        }

        [Button("Update Minimal")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Minimal)]
        public void ButtonMinimalStat(int value)
        {
            MinimalStat((ulong) value);
        }
#endif

        #endregion
    }
}