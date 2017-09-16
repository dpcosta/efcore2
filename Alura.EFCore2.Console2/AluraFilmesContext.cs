using Microsoft.EntityFrameworkCore;

namespace Alura.EFCore2.Console2
{
    public class AluraFilmesContext : DbContext
    {
        public DbSet<Ator> Atores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AluraFilmes2;Trusted_Connection=True;");
        }

    }
}
