
namespace Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IGameService
    {
        Task<int> AddGame(int user1Id, int userId2);
        Task<bool> MakeMoveAsync(int userId, int abscissa, int ordinate);
    }
}
