using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Alura.EFCore2.Curso.Aula5
{
    class MapeandoFilmes
    {
        private class Ator
        {
            public int Id { get; set; }

            [Required]
            public string PrimeiroNome { get; set; }

            [Required]
            public string UltimoNome { get; set; }

            public override string ToString()
            {
                return $"Ator ({this.Id}): {this.PrimeiroNome} {this.UltimoNome}";
            }
        }

        private class Filme
        {
            public int Id { get; set; }
            [Required]
            public string Titulo { get; set; }
            public string Descricao { get; set; }
            public short Duracao { get; set; }
            public string Classificacao { get; set; }
            public string AnoLancamento { get; set; }

            public override string ToString()
            {
                return $"Filme ({this.Id}): {this.Titulo} [{this.AnoLancamento}]";
            }
        }

        private class AluraFilmesContexto : DbContext
        {
            public DbSet<Ator> Atores { get; set; }
            public DbSet<Filme> Filmes { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AluraFilmes;Trusted_connection=true;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

                modelBuilder.Entity<Ator>()
                    .ToTable("actor")
                    .HasIndex(a => a.UltimoNome)
                    .HasName("idx_actor_last_name");

                modelBuilder.Entity<Ator>()
                    .HasAlternateKey(a => new { a.PrimeiroNome, a.UltimoNome })
                    .HasName("idx_actor_unique_first_last_name");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.Id)
                    .HasColumnName("actor_id");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.PrimeiroNome)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(45)");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.UltimoNome)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(45)");

                modelBuilder.Entity<Ator>()
                    .Property<DateTime>("last_update")
                    .HasDefaultValueSql("getdate()");
                
                modelBuilder.Entity<Filme>()
                    .ToTable("film");

                modelBuilder.Entity<Filme>()
                    .Property(f => f.Id)
                    .HasColumnName("film_id");

                modelBuilder.Entity<Filme>()
                    .Property(f => f.Titulo)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)");

                modelBuilder.Entity<Filme>()
                    .Property(f => f.Descricao)
                    .HasColumnName("description")
                    .HasColumnType("text");

                modelBuilder.Entity<Filme>()
                    .Property(f => f.AnoLancamento)
                    .HasColumnName("release_year")
                    .HasColumnType("varchar(4)");

                modelBuilder.Entity<Filme>()
                    .Property(f => f.Classificacao)
                    .HasColumnName("rating")
                    .HasColumnType("varchar(10)");

                modelBuilder.Entity<Filme>()
                    .Property(f => f.Duracao)
                    .HasColumnName("length")
                    .HasColumnType("smallint");
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - começar novo requisito: mapear filmes
        ///         -  usar boas práticas
        ///         - criar classe de configuração
        ///         - testar o SELECT
        ///         - erro no smallint de Duracao >> colocar propriedade como short
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.StartLogSqlToConsole();

                foreach (var filme in contexto.Filmes)
                {
                    System.Console.WriteLine(filme);
                }
            }
        }
    }
}
