namespace ObjectCartographer.ExpressionBuilder
{
    internal static class OrderDefaults
    {
        public static int Default { get; } = 0;

        public static int DefaultMinusOne { get; } = -1;
        public static int DefaultPlusOne { get; } = 1;
        public static int DefaultPlusTwo { get; } = 2;
        public static int First { get; } = int.MinValue;
        public static int Last { get; } = int.MaxValue;

        public static int LastMinusOne { get; } = int.MaxValue - 1;
    }
}