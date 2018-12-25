namespace Storage.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class GameMove
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }

        public int Abscissa { get; set; }

        public int Ordinate { get; set; }

        public bool State { get; set; }
    }
}