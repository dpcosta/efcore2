using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Alura.EFCore2.Curso.Aula2
{
    class OutraManeiraDeConfigurar
    {
        private class Ator
        {
            public int Id { get; set; }
            public string PrimeiroNome { get; set; }
            public string UltimoNome { get; set; }

            public override string ToString()
            {
                return $"Ator ({this.Id}): {this.PrimeiroNome} {this.UltimoNome}";
            }
        }

        private class AluraFilmesContexto : DbContext
        {
            public DbSet<Ator> Atores { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AluraFilmes;Trusted_connection=true;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Ator>()
                    .ToTable("actor");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.Id)
                    .HasColumnName("actor_id");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.PrimeiroNome)
                    .HasColumnName("first_name");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.UltimoNome)
                    .HasColumnName("last_name");
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - configurar os nomes de tabelas e colunas usando Fluent API no método OnModelCreating
        ///         - executar e observar que o programa vai funcionar exatamente como antes!
        ///         - explicar que no processo de mapeamento, o terceiro passo é Verificar o método OnModelCreating
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.StartLogSqlToConsole();
                foreach (var ator in contexto.Atores)
                {
                    System.Console.WriteLine(ator);
                }
            }
        }
    }
}
