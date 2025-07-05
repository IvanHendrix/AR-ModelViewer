namespace Services.Input
{
    public static class InputProviderResolver
    {
        public static IInputProvider InputProvider;

        public static void Init()
        {
#if UNITY_EDITOR
            InputProvider = new MouseInputProvider();
#else
        InputProvider = new TouchInputProvider();
#endif
        }
    }
}