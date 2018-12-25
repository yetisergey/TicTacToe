namespace Services.Services
{
    using System;
    using Storage;
    using Interfaces;
    using System.Linq;
    using Storage.Models;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class GameService : IGameService
    {
        private ITicTacContext _ticTacContext;

        private const int WIDTH_QUAD = 20;

        public GameService(ITicTacContext ticTacContext)
        {
            _ticTacContext = ticTacContext;
        }

        public async Task<int> AddGame(int user1Id, int user2Id)
        {
            var users = await _ticTacContext.Users
                .Where(u => (new int[] { user1Id, user2Id }).Contains(u.Id))
                .ToListAsync();

            users.First().State = true;
            users.Last().State = false;

            var game = await _ticTacContext.Games.AddAsync(new Game()
            {
                Users = users,
                State = true
            });

            await _ticTacContext.SaveAsync();

            return game.Property(u => u.Id).CurrentValue;
        }

        //TODO: sql
        public async Task<bool> MakeMoveAsync(int userId, int abscissa, int ordinate)
        {
            var user = await _ticTacContext.Users
                .Include(u => u.Game)
                .Include(u => u.Game.GameMoves)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var game = user.Game;

            var coordinateX = GetCoordinate(abscissa);
            var coordinateY = GetCoordinate(ordinate);

            if (!(game.GameMoves.Any(m => m.Abscissa == coordinateX &&
                          (m.Ordinate - WIDTH_QUAD == coordinateY || m.Ordinate + WIDTH_QUAD == coordinateY)) ||
                  game.GameMoves.Any(m => m.Abscissa == coordinateX - WIDTH_QUAD &&
                          (m.Ordinate - WIDTH_QUAD == coordinateY || m.Ordinate + WIDTH_QUAD == coordinateY || m.Ordinate == coordinateY)) ||
                  game.GameMoves.Any(m => m.Abscissa == coordinateX + WIDTH_QUAD &&
                          (m.Ordinate - WIDTH_QUAD == coordinateY || m.Ordinate + WIDTH_QUAD == coordinateY || m.Ordinate == coordinateY))))
            {
                if (!(game.GameMoves.Count == 0 && coordinateX == 0 && coordinateY == 0))
                {
                    throw new Exception("Incorrect Move");
                }
            }

            using (var transaction = _ticTacContext.Database.BeginTransaction())
            {
                try
                {
                    await _ticTacContext.GameMoves.AddAsync(new GameMove()
                    {
                        Abscissa = coordinateX,
                        Ordinate = coordinateY,
                        GameId = game.Id,
                        State = user.State
                    });

                    game.State = !game.State;

                    await _ticTacContext.SaveAsync();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return user.State;
        }

        private int GetCoordinate(int num)
        {
            while (num % WIDTH_QUAD != 0)
                num--;
            return num;
        }
    }
}