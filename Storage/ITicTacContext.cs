namespace Storage
{
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    public interface ITicTacContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Game> Games { get; set; }
        DbSet<GameMove> GameMoves { get; set; }
        DatabaseFacade Database { get; }

        Task<int> SaveAsync();
    }
}