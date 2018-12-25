namespace Storage
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class TicTacContext : DbContext, ITicTacContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameMove> GameMoves { get; set; }

        public async Task<int> SaveAsync()
        {
            return await this.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=TicTacDataBase;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Game>()
                .HasMany(u => u.Users);

            modelBuilder.Entity<Game>()
                .HasMany(u => u.GameMoves);

            modelBuilder.Entity<GameMove>()
                .HasIndex(u => new
                {
                    u.GameId,
                    u.Abscissa,
                    u.Ordinate
                }).IsUnique();

            modelBuilder.Entity<GameMove>()
                .HasIndex(u => new
                {
                    u.GameId,
                    u.Abscissa,
                    u.Ordinate,
                    u.State
                }).IsUnique();
        }
    }
}
