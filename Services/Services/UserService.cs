namespace Services.Services
{
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Storage;
    using Storage.Models;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private ITicTacContext _ticTacContext;
        private IGameService _gameService;
        public UserService(ITicTacContext ticTacContext,
                           IGameService gameService)
        {
            _ticTacContext = ticTacContext;
            _gameService = gameService;
        }


        public async Task<int> AddUser(string name)
        {
            var oldUser = await _ticTacContext.Users.FirstOrDefaultAsync(u => u.Name == name);

            if (oldUser == null)
            {
                var user = await _ticTacContext.Users.AddAsync(new User()
                {
                    Name = name
                });
                await _ticTacContext.SaveAsync();
                return user.Property(u => u.Id).CurrentValue;
            }

            return oldUser.Id;
        }

        public async Task<int> GetCurrentGameId(int userId)
        {
            var user = await _ticTacContext.Users.FindAsync(userId);
            return user.GameId.Value;
        }

        public async Task SetCurrentGameId(int userId, int? gameId)
        {
            var user = await _ticTacContext.Users.FindAsync(userId);
            user.GameId = gameId;
            await _ticTacContext.SaveAsync();
        }

        public async Task<bool> IsUserMove(int userId)
        {
            var user = await _ticTacContext.Users.Include(u => u.Game)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user.State == user.Game.State;
        }

        public async Task<string> GetUserName(int userId)
        {
            var user = await _ticTacContext.Users.FindAsync(userId);
            return user.Name;
        }
    }
}