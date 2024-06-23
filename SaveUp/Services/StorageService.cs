using Newtonsoft.Json;
using SaveUp.Interfaces;
using SaveUp.Models;
using System.Diagnostics;

namespace SaveUp.Services
{
    public class StorageService : IStorageService
    {
        /// <summary>
        /// The currently logged in user atm
        /// </summary>
        private StorageSettings _settings = new StorageSettings();

        /// <summary>
        /// Relay to the storage service
        /// </summary>
        public bool HasUser()
        {
            return _settings.IsSome();
        }

        /// <summary>
        /// Get the user from the storage
        /// Access Token, Refresh Token
        /// </summary>
        /// <returns></returns>
        public (string, string) Get()
        {
            return (_settings.AccessToken!, _settings.RefreshToken!);
        }

        /// <summary>
        /// Clear the stored data
        /// </summary>
        public void Clear()
        {
            _settings = new StorageSettings();
        }

        /// <summary>
        /// Loads the stored data from the secure storage
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await LoadStoredDataAsync();
        }


        /// <summary>
        /// Writes the stored data to the secure storage
        /// </summary>
        public async Task SaveChangesAsync()
        {
            try
            {
                await SecureStorage.SetAsync("saveup_user", JsonConvert.SerializeObject(_settings));
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Debug.WriteLine("Storage Service: At Save", ex.Message);
            }
        }

        /// <summary>
        /// Stores the given user credentials in the secure storage
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="token">The token of the user</param>
        /// <param name="refreshToken">The refresh token of the user</param>
        public void StoreUser(string username, string token, string refreshToken)
        {
            _settings = new StorageSettings
            {
                Username = username,
                AccessToken = token,
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Loads the stored data from the secure storage
        /// </summary>
        private async Task LoadStoredDataAsync()
        {
            try
            {
                var rawUser = await SecureStorage.GetAsync("saveup_user") ?? "{}";
                var user = JsonConvert.DeserializeObject<StorageSettings>(rawUser);
                // nothing to update
                if (user == null) return;
                _settings = user;
            }
            catch (Exception ex)
            {
                // Handle exceptions (should not happen?)
                Debug.WriteLine("Storage Service: At Load", ex.Message);
            }
        }
    }
}
