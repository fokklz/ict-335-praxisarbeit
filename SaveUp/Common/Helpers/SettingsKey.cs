namespace SaveUp.Common.Helpers
{
    /// <summary>
    /// Helper class to get the settings keys (maybe enum would be better)
    /// </summary>
    public static class SettingsKey
    {
        public static string Language { get; private set; } = "Language";

        public static string Theme { get; private set; } = "Theme";

        public static string Currency { get; private set; } = "Currency";

        public static string TimeSpan { get; private set; } = "TimeSpan";

        public static string DeptMode { get; private set; } = "DeptMode";
    }
}
