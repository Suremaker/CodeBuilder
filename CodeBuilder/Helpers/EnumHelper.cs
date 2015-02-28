namespace CodeBuilder.Helpers
{
    public static class EnumHelper
    {
        public static bool IsSet<TEnum>(TEnum e, TEnum expected)
        {
            var evalue = (int)(object)e;
            var eexpected = (int)(object)expected;
            return (evalue & eexpected) != 0;
        }
    }
}
