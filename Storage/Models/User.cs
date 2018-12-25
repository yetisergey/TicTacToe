namespace Storage.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        public int Id { get; set; }

        [MaxLength(200), Required]
        public string Name { get; set; }

        public bool State { get; set; }

        public int? GameId { get; set; }

        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
    }
}