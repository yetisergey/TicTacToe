namespace Storage.Models
{
    using System.Collections.Generic;

    public class Game
    {
        public int Id { get; set; }

        public bool State { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<GameMove> GameMoves { get; set; }
    }
}