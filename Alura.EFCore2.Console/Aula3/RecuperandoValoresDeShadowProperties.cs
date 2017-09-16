using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Alura.EFCore2.Curso.Aula3
{
    class RecuperandoValoresDeShadowProperties
    {
        private class Ator
        {
            public int Id { get; set; }

            [Required]
            [Column("first_name", TypeName = "varchar(45)")]
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

                //modelBuilder.Entity<Ator>()
                //    .Property(a => a.PrimeiroNome)
                //    .HasColumnName("first_name");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.UltimoNome)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(45)")
                    .IsRequired();

                modelBuilder.Entity<Ator>()
                    .Property<DateTime>("last_update");

            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - recuperar o valor da shadow property last_update
        ///         - através do método EF.Property<DateTime>(a, "last_update") na query
        ///         
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                var atores = contexto.Atores
                    .OrderByDescending(a => EF.Property<DateTime>(a, "last_update"))
                    .Select(a => new { Nome = a.PrimeiroNome + " " + a.UltimoNome, UltimaAtualizacao = EF.Property<DateTime>(a, "last_update") });
                foreach (var ator in atores)
                {
                    System.Console.WriteLine($"{ator.Nome} - {ator.UltimaAtualizacao:dd/MM/yyyy}");
                }
            }
        }
    }
}
