
namespace SaveUp.Interfaces
{
    public interface IAlertService
    {
        Task ShowAsync(string title, string message);

        Task<bool> ConfirmAsync(string title, string message);
    }
}