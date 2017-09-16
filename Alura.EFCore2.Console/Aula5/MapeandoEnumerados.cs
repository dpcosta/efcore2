using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace Alura.EFCore2.Curso.Aula5
{
    class MapeandoEnumerados
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

        private enum ClassificacaoIndicativa
        {
            ///(G) - Livre, 
            ///(PG) - Menores10, 
            ///(PG-13) - Menores13, 
            ///(R) - Restrito, 
            ///(NC-17)- Maiores18
            ///
            Livre,
            Menores10,
            Menores13,
            Restrito,
            Maiores18
        }

        private class Filme
        {
            public int Id { get; set; }
            [Required]
            public string Titulo { get; set; }
            public string Descricao { get; set; }
            public short Duracao { get; set; }
            public string AnoLancamento { get; set; }
            public string Classificacao { get; private set; }
            public ClassificacaoIndicativa ClassificacaoIndicativa
            {
                get
                {
                    if (Classificacao == "G") return ClassificacaoIndicativa.Livre;
                    if (Classificacao == "PG") return ClassificacaoIndicativa.Menores10;
                    if (Classificacao == "PG-13") return ClassificacaoIndicativa.Menores13;
                    if (Classificacao == "R") return ClassificacaoIndicativa.Restrito;
                    if (Classificacao == "NC-17") return ClassificacaoIndicativa.Maiores18;
                    return ClassificacaoIndicativa.Livre;
                }
                set
                {
                    switch (value)
                    {
                        case ClassificacaoIndicativa.Livre:
                            Classificacao = "G";
                            break;
                        case ClassificacaoIndicativa.Menores10:
                            Classificacao = "PG";
                            break;
                        case ClassificacaoIndicativa.Menores13:
                            Classificacao = "PG-13";
                            break;
                        case ClassificacaoIndicativa.Restrito:
                            Classificacao = "R";
                            break;
                        case ClassificacaoIndicativa.Maiores18:
                            Classificacao = "NC-17";
                            break;
                        default:
                            Classificacao = "G";
                            break;
                    }
                }
            }

            public override string ToString()
            {
                return $"Filme ({this.Id}): {this.Titulo} [{this.AnoLancamento}] - {this.ClassificacaoIndicativa}";
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

                modelBuilder.Entity<Filme>()
                    .Ignore(f => f.ClassificacaoIndicativa);
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - observar os valores do campo Rating. 
        ///         - no banco existe uma restrição CHECK que valida os seguintes valores:
        ///             ADD CONSTRAINT [CHECK_special_rating] CHECK ([rating]='NC-17' OR [rating]='R' OR [rating]='PG-13' OR [rating]='PG' OR [rating]='G');
        ///             que significam (G) - Livre, (PG) - Menores10, (PG-13) - Menores13, (R) - Restrito, (NC-17)- Maiores18
        ///         - como podemos mapear no lado das classes? Enums!
        ///         - e qual a convenção para Enumerados no Entity?
        ///         - olhar o script para descobrir que vai mapear para um int
        ///         - criamos o enum, e criamos uma prop desse tipo
        ///         - mas dps q fazemos  a mudança pra enum, dá erro de conversão!
        ///         - como resolver?
        ///         - duas propriedades: uma do tipo string com private set
        ///         - e outra do tipo enum, mas com getter e setter explícito para converter a string em enum e vice versa
        ///         - além disso, informar ao EF que queremos ignorar a prop enum
        ///         - testar o SELECT novamente
        ///         - 
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

                //var f = new Filme();
                //f.ClassificacaoIndicativa = ClassificacaoIndicativa.Livre;
                //System.Console.WriteLine(f.Classificacao);

            }
        }
    }

}
