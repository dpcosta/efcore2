using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alura.EFCore2.Curso.Aula4
{
    class DefinindoIndices
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
                    .ToTable("actor")
                    .HasIndex(a => a.UltimoNome)
                    .HasName("idx_actor_last_name");

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
        ///         - mostrar que a coluna last_name tem um índice definido
        ///         - maiores detalhes em https://docs.microsoft.com/en-us/ef/core/modeling/indexes
        ///         - explicar que por convenção um índice é criado para cada chave estrangeira
        ///         - como criar o índice para last_name?
        ///         - só pode ser definida via Fluent API
        ///         - através do método HasIndex(a => a.UltimoNome)
        ///         - para mudar o nome do índice usamos HasName("")
        ///         - gerar o script para verificar
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                
            }
        }
    }
}
