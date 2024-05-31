using Baracuda.Tools;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Bedrock.Statistics
{
    public class FloatStat : StatAsset<double>
    {
        #region Fields

        [Foldout("Settings")]
        [Title("Stat")]
        [SerializeField] private Modification type;

        [ShowIf(nameof(type), Modification.Increment)]
        [SerializeField] private double increment = 1;
        [ShowIf(nameof(type), Modification.Increment)]
        [SerializeField] private double minIncrement = .1f;

        [ShowIf(nameof(type), Modification.Minimal)]
        [SerializeField] private double defaultMinimal = double.MaxValue;

        #endregion


        #region Overrides

        protected override double DefaultValue()
        {
            return type == Modification.Minimal ? defaultMinimal : 0;
        }

        public override Modification Type()
        {
            return type;
        }

        public override string ValueString => Value.ToString("0.00");

        #endregion


        #region Public

        [PublicAPI]
        public UpdateResult<double> IncrementStat()
        {
            Assert.IsTrue(type == Modification.Increment);
            var from = StatData.value;
            var to = from + increment;
            StatData.value = to;
            SetStatDirty();
            return new UpdateResult<double>(type, from, to, true);
        }

        [PublicAPI]
        public UpdateResult<double> IncrementStat(double value)
        {
            Assert.IsTrue(type == Modification.Increment);
            if (minIncrement > value)
            {
                return new UpdateResult<double>(type, StatData.value, StatData.value, false);
            }
            var from = StatData.value;
            var to = from + value;
            StatData.value = to;
            SetStatDirty();
            return new UpdateResult<double>(type, from, to, true);
        }

        [PublicAPI]
        public UpdateResult<double> UpdateStat(double value)
        {
            Assert.IsTrue(type == Modification.Increment);
            var from = StatData.value;
            StatData.value = value;
            SetStatDirty();
            return new UpdateResult<double>(type, from, value, true);
        }

        [PublicAPI]
        public UpdateResult<double> HighscoreStat(double value)
        {
            Assert.IsTrue(type == Modification.Highscore);
            var current = StatData.value;
            if (current < value)
            {
                StatData.value = value;
                SetStatDirty();
                return new UpdateResult<double>(type, current, value, true);
            }
            return new UpdateResult<double>(type, current, current, false);
        }

        [PublicAPI]
        public UpdateResult<double> MinimalStat(double value)
        {
            Assert.IsTrue(type == Modification.Minimal);
            var current = StatData.value;
            if (current > value)
            {
                StatData.value = value;
                SetStatDirty();
                return new UpdateResult<double>(type, current, value, true);
            }
            return new UpdateResult<double>(type, current, current, false);
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
        public void ButtonIncrementStat(double value)
        {
            IncrementStat(value);
        }

        [Button("Update")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Update)]
        public void ButtonUpdateStat(double value)
        {
            UpdateStat(value);
        }

        [Button("Update Highscore")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Highscore)]
        public void ButtonHighscoreStat(double value)
        {
            HighscoreStat(value);
        }

        [Button("Update Minimal")]
        [Foldout("Debug")]
        [ShowIf(nameof(type), Modification.Minimal)]
        public void ButtonMinimalStat(double value)
        {
            MinimalStat(value);
        }
#endif

        #endregion
    }
}