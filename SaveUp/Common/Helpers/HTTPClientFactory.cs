namespace SaveUp.Common.Helpers
{
    /// <summary>
    /// Factory class for creating HTTPClient instances that ignore SSL certificate errors.
    ///  - This should be changed in production to use a valid certificate/validation.
    /// </summary>
    public static class HTTPClientFactory
    {
        public static HttpClient Create()
        {
            var client = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                }
            });
            return client;
        }
    }
}
