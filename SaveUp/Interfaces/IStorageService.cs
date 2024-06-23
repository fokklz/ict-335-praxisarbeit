namespace SaveUp.Interfaces
{
    public interface IStorageService
    {
        public bool HasUser();
        (string, string) Get();
        public void Clear();
        Task InitializeAsync();
        Task SaveChangesAsync();
        void StoreUser(string username, string token, string refreshToken);
    }
}