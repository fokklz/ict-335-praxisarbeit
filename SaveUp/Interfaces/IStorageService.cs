namespace SaveUp.Interfaces
{
    public interface IStorageService
    {
        bool HasUser { get; }

        (string, string) Get();
        public void Clear();
        Task InitializeAsync();
        Task SaveChangesAsync();
        void StoreUser(string username, string token, string refreshToken);
    }
}