using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Alura.EFCore2.Curso.Aula5
{
    class MelhoresPraticasMapeamento
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

        private class AtorConfiguracao
        {
            public static void Configurar(EntityTypeBuilder<Ator> entityBuilder)
            {
                entityBuilder
                    .ToTable("actor")
                    .HasIndex(a => a.UltimoNome)
                    .HasName("idx_actor_last_name");

                entityBuilder
                    .HasAlternateKey(a => new { a.PrimeiroNome, a.UltimoNome })
                    .HasName("idx_actor_unique_first_last_name");

                entityBuilder
                    .Property(a => a.Id)
                    .HasColumnName("actor_id");

                entityBuilder
                    .Property(a => a.PrimeiroNome)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(45)");

                entityBuilder
                    .Property(a => a.UltimoNome)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(45)");

                entityBuilder
                    .Property<DateTime>("last_update")
                    .HasDefaultValueSql("getdate()");
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

        private class FilmeConfiguracao
        {
            public static void Configurar(EntityTypeBuilder<Filme> entityBuilder)
            {
                entityBuilder
                    .ToTable("film");

                entityBuilder
                    .Property(f => f.Id)
                    .HasColumnName("film_id");

                entityBuilder
                    .Property(f => f.Titulo)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)");

                entityBuilder
                    .Property(f => f.Descricao)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entityBuilder
                    .Property(f => f.AnoLancamento)
                    .HasColumnName("release_year")
                    .HasColumnType("varchar(4)");

                entityBuilder
                    .Property(f => f.Classificacao)
                    .HasColumnName("rating")
                    .HasColumnType("varchar(10)");

                entityBuilder
                    .Property(f => f.Duracao)
                    .HasColumnName("length")
                    .HasColumnType("smallint");

                entityBuilder
                    .Ignore(f => f.ClassificacaoIndicativa);
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
                AtorConfiguracao.Configurar(modelBuilder.Entity<Ator>());
                FilmeConfiguracao.Configurar(modelBuilder.Entity<Filme>());
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - vimos que existem 3 meios de fazer o EF mapear nossas classes as tabelas
        ///         - quais são as melhores práticas?
        ///             - use convenção sempre que possível >> pois é produtivo
        ///             - use anotações para validar suas classes
        ///             - use Fluent API para o resto
        ///             - isolar o código fluente de configuração em uma classe!
        ///         - vamos fazer isso para a classe Ator e Filme
        ///         - confirmar se o script vai continuar igual...
        ///         - confirmar se o SELECT para atores e filmes vão funcionar
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

                foreach (var filme in contexto.Filmes)
                {
                    System.Console.WriteLine(filme);
                }
            }
        }
    }
}
