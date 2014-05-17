using System;
using System.Reflection.Emit;
using CodeBuilder.Context;

namespace CodeBuilder.Helpers
{
    public static class EmitHelper
    {
        private static readonly Type[] _bigIntTypes = new[] { typeof(uint), typeof(long), typeof(ulong) };
        public static void ConvertToNativeInt(IBuildContext ctx, Type fromType)
        {
            ctx.Generator.Emit(IsBiggerThanInt(fromType) ? OpCodes.Conv_Ovf_I : OpCodes.Conv_I);
        }

        public static bool IsBiggerThanInt(Type type)
        {
            return CollectionHelper.Contains(_bigIntTypes, type);
        }
    }
}