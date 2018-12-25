namespace TicTacToe.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TicTacToe.Models;

    [Authorize]
    public class GameHub : Hub
    {
        private IGameService _gameService { get; set; }

        private IUserService _userService { get; set; }


        public GameHub(IGameService gameService,
                       IUserService userService)
        {
            _gameService = gameService;
            _userService = userService;
        }

        public static Dictionary<int, string> userQueue = new Dictionary<int, string>();

        public async override Task OnConnectedAsync()
        {
            var userId = int.Parse(Context.User.Identity.Name);
            if (!userQueue.ContainsKey(userId))
            {
                userQueue.Add(userId, Context.ConnectionId);
            }

            if (userQueue.Count % 2 == 0 && userQueue.Count > 0)
            {
                await StartGame(userQueue.First(), userQueue.Last());
            }

            await base.OnConnectedAsync();
        }

        public async Task StartGame(KeyValuePair<int, string> user1, KeyValuePair<int, string> user2)
        {
            var gameId = await _gameService.AddGame(user1.Key, user2.Key);

            await Clients.Client(user1.Value).SendAsync("StartGame");
            await Clients.Client(user2.Value).SendAsync("StartGame");

            await Groups.AddToGroupAsync(user1.Value, gameId.ToString());
            await Groups.AddToGroupAsync(user2.Value, gameId.ToString());

            userQueue.Remove(user1.Key);
            userQueue.Remove(user2.Key);

            await _userService.SetCurrentGameId(user1.Key, gameId);
            await _userService.SetCurrentGameId(user2.Key, gameId);
        }

        public async Task Move(int abscissa, int ordinate)
        {
            var userId = int.Parse(Context.User.Identity.Name);
            if (await _userService.IsUserMove(userId))
            {
                var currentGameId = await _userService.GetCurrentGameId(userId);
                var previousMove = await _gameService.MakeMoveAsync(userId, abscissa, ordinate);
                await Clients.Group(currentGameId.ToString())
                    .SendAsync("GetMove", new Field()
                    {
                        Abscissa = abscissa,
                        Ordinate = ordinate,
                        State = previousMove
                    });
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (int.TryParse(Context.User.Identity.Name, out var userId))
            {
                userQueue.Remove(userId);
                var gameId = await _userService.GetCurrentGameId(userId);
                await _userService.SetCurrentGameId(userId, null);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString()).Wait();
            }
        }
    }
}