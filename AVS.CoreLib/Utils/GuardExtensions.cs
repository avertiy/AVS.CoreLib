using System;

namespace AVS.CoreLib.Utils
{
    public interface IHasValue
    {
        bool HasValue { get; }
    }

    public static class GuardExtensions
    {
        public static void EnsureHasValue(this IHasValue obj)
        {
            if (obj == null || !obj.HasValue)
                throw new ArgumentNullException(nameof(obj));
        }
    }
}