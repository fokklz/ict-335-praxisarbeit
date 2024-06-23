using Newtonsoft.Json;
using SaveUp.Tests.Util.Helper;
using System.Text;
using Moq;
using Microsoft.Extensions.Configuration;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Services;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.DTOs.Requests;

namespace SaveUp.Tests
{
    public class Environment
    {
        public Mock<IServiceProvider> ServiceProviderMock { get; private set; }
        public Mock<IConfiguration> ConfigurationMock { get; private set; }

        public Environment()
        {
            ConfigurationMock = new Mock<IConfiguration>();
            ConfigurationMock.Setup(x => x["API:BaseURL"]).Returns("http://localhost:9000/api/");

            new Localization();

            Setup();
        }

        private void Setup()
        {
            SettingsManager.PreferencesAPI = new FakePreferences();

            ServiceProviderMock = new Mock<IServiceProvider>();

            AddService(ConfigurationMock);;
            AddService<IStorageService>(new StorageService());
        }

        public Environment UseAuth()
        {
            Task.Run(async () =>
            {
                var result = new HttpClient().PostAsync("http://localhost:9000/api/users/login", new StringContent(JsonConvert.SerializeObject(new LoginRequest
                {
                    Email = "admin@admin.com",
                    Password = "admin",
                    RememberMe = true
                }), Encoding.UTF8, "application/json"));

                if (result.Result.IsSuccessStatusCode)
                {
                    var response = JsonConvert.DeserializeObject<LoginResponse>(await result.Result.Content.ReadAsStringAsync());
                    AuthManager.Login(response.Auth.Token, response.Auth.RefreshToken, response.Id);
                }
            }).GetAwaiter().GetResult();
            return this;
        }

        /// <summary>
        /// Add a service to the ServiceProviderMock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void AddService<T>(T instance)
            where T : class
        {
            ServiceProviderMock.Setup(x => x.GetService(typeof(T))).Returns(instance);
        }

        /// <summary>
        /// Overload for AddService that takes a Mock<T> instead of a T
        /// </summary>
        /// <typeparam name="T">The Target Type</typeparam>
        /// <param name="mock"></param>
        public void AddService<T>(Mock<T> mock)
            where T : class
        {
            AddService(mock.Object);
        }

        public void Dispose()
        {
            AuthManager.Logout();
            Setup();
        }
    }
}
