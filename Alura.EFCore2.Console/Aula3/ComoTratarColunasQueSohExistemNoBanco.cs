using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alura.EFCore2.Curso.Aula3
{
    class ComoTratarColunasQueSohExistemNoBanco
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
        ///         - reconhecer uma coluna que existe no banco sem trazê-la para a classe
        ///         - só é possível via Fluent API
        ///         - através do método Property<DateTime>("last_update")
        ///         - dizer que o nome para essas colunas são Shadow Properties
        ///         - gerar o script novamente, e verificar que está ficando bem parecido!
        ///         - mas como recuperar o valor de uma shadow property?
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                var info = contexto.GetDBTableInfo(typeof(Ator));

                System.Console.WriteLine("Informações de Mapeamento...");
                System.Console.WriteLine($"Classe: {info.NomeClasse} >> Tabela: {info.NomeTabela}");
                foreach (var col in info.Columns)
                {
                    System.Console.WriteLine($"Propriedade: {col.NomePropriedade} >> Coluna: {col.NomeColuna} - tipo: {col.TipoColuna}, tamanho: {col.TamanhoColuna}, pode ser nulo? {col.EhNulo}");
                }
            }
        }
    }
}
