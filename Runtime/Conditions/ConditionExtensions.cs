using System.Runtime.CompilerServices;

namespace Baracuda.Mediator
{
    public static class ConditionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All(this ConditionAsset[] conditions)
        {
            for (var i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].Check())
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool None(this ConditionAsset[] conditions)
        {
            for (var i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Check())
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any(this ConditionAsset[] conditions)
        {
            for (var i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Check())
                {
                    return true;
                }
            }

            return false;
        }
    }
}