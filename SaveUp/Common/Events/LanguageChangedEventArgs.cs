namespace SaveUp.Common.Events
{
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
