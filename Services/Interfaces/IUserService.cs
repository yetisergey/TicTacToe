namespace Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<string> GetUserName(int userId);
        Task<int> AddUser(string name);
        Task<int> GetCurrentGameId(int userId);
        Task SetCurrentGameId(int userId, int? gameId);
        Task<bool> IsUserMove(int userId);
    }
}
