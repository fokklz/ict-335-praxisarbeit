namespace SaveUp.Common.Events
{
    /// <summary>
    /// Event arguments for the language changed event
    /// </summary>
    public class LanguageChangedEventArgs : EventArgs
    {
        public string Language { get; set; }

        public string LanguageCode { get; set; }

        public LanguageChangedEventArgs(string language, string languageCode)
        {
            Language = language;
            LanguageCode = languageCode;
        }
    }
}
