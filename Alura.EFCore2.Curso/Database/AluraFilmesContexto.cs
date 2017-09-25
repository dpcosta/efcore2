using Microsoft.EntityFrameworkCore;
using Alura.EFCore2.Curso.Negocio;

namespace Alura.EFCore2.Curso.Database
{
    public class AluraFilmesContexto : DbContext
    {
        public DbSet<Ator> Atores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AluraFilmes;Trusted_Connection=true;");
            }
        }
    }
}
