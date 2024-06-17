using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveUp.Common.Helpers
{
    /// <summary>
    /// Helper class to get the settings keys (maybe enum would be better)
    /// </summary>
    public static class SettingsKey
    {
        public static string Language { get; private set; } = "Language";

        public static string Theme { get; private set; } = "Theme";

        public static string CancelInListView { get; private set; } = "CancelInListView";

        public static string AlwaysSaveLogin { get; private set; } = "AlwaysSaveLogin";
    }
}
