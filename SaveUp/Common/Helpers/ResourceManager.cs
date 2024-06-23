using System.Globalization;
using System.Reflection;
using SystemTextJsonPatch;

namespace SaveUp.Common.Helpers
{
    /// <summary>
    /// Should be used to interact with ressources
    /// </summary>
    public class ResourceManager

    {
        /// <summary>
        /// Register Syncfusion license
        /// </summary>
        public static void RegisterSyncfusionLicense()
        {
            var assembly = Assembly.GetExecutingAssembly();
            // a license can be obtained from https://www.syncfusion.com/products/communitylicense
            var resourceName = "SkiServiceApp.SyncfusionLicense.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string licenseKey = reader.ReadToEnd();
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }
        }

        /// <summary>
        /// Get a language resource by key
        /// </summary>
        /// <param name="key">The key to extract</param>
        /// <param name="culture">The target Language</param>
        /// <returns>The defined translation for the UI</returns>
        public static string GetLanguageResource(string key, CultureInfo culture)
        {
            try
            {
                return Resources.Strings.Strings.ResourceManager.GetString(key, culture) ?? $"!{key}";
            }
            catch (Exception)
            {
                return $"!{key}";
            }
        }

    }
}
