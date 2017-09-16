using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Alura.EFCore2.Curso.Aula4
{
    class DefinindoValoresPadraoParaColunas
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
                    .Property<DateTime>("last_update")
                    .HasDefaultValueSql("getdate()");

            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - mostrar que a coluna last_update tem um valor padrão para data/hora atual quando inserida
        ///         - maiores detalhes em https://docs.microsoft.com/en-us/ef/core/modeling/relational/default-values
        ///         - explicar as convenções
        ///         - como mapear essa estratégia para a coluna last_update?
        ///         - só pode ser definida via Fluent API
        ///         - através do método HasDefaultValueSql("getdate()")
        ///         
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.StartLogSqlToConsole();

                var atorQualquer = contexto.Atores.First();

                MostraDataAtualizacaoAtor(contexto, atorQualquer);

                atorQualquer.UltimoNome = "STALLONE";
                contexto.SaveChanges();

                MostraDataAtualizacaoAtor(contexto, atorQualquer);
            }
        }

        private static void MostraDataAtualizacaoAtor(DbContext contexto, Ator ator)
        {
            var dataAtualizacao = contexto.Entry(ator)
                 .Property<DateTime>("last_update")
                 .CurrentValue;

            System.Console.WriteLine($"Data de atualização do ator {ator.PrimeiroNome} antes da atualização: {dataAtualizacao:dd/MM/yyyy}");
        }
    }
}
