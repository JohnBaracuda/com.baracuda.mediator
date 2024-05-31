using JetBrains.Annotations;

namespace Baracuda.Bedrock.Statistics
{
    public readonly ref struct UpdateResult<T>
    {
        [PublicAPI] public readonly T OldValue;
        [PublicAPI] public readonly T NewValue;
        [PublicAPI] public readonly bool Changed;
        [PublicAPI] public readonly Modification Modification;

        public UpdateResult(Modification modification, T oldValue, T newValue, bool changed)
        {
            Modification = modification;
            OldValue = oldValue;
            NewValue = newValue;
            Changed = changed;
        }

        public static implicit operator bool(UpdateResult<T> result)
        {
            return result.Changed;
        }
    }
}