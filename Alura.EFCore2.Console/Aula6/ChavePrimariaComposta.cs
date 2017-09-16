using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Alura.EFCore2.Curso.Aula6
{
    class ChavePrimariaComposta
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

        private class FilmeAtor
        {
            public Filme Filme { get; set; }
            public Ator Ator { get; set; }
        }

        private class FilmeAtorConfiguracao
        {
            public static void Configurar(EntityTypeBuilder<FilmeAtor> entityBuilder)
            {
                entityBuilder
                    .ToTable("film_actor");

                entityBuilder
                    .Property<int>("film_id");

                entityBuilder
                    .Property<int>("actor_id");

                entityBuilder
                    .HasKey("film_id", "actor_id")
                    .HasName("pk_film_actor");
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
                FilmeAtorConfiguracao.Configurar(modelBuilder.Entity<FilmeAtor>());
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - agora vamor relacionar atores a filmes
        ///         - sabemos que um ator pode estrelar vários filmes
        ///         - ...e um filme tem um elenco com vários atores
        ///         - relacionamento Muitos para Muitos
        ///         - no curso anterior vimos que o EF Core não suporta M para N sem uma classe de join
        ///         - vamos modelar isso em nossas classes
        ///         - criar classe FilmeAtor, com propriedades de navegação Filme e Ator
        ///         - e as chave primária? No banco ela é formada pela junção de actor_id e film_id
        ///         - criar classe de configuração com a definição da chave
        ///         - gerar o script e verificar se está ok
        ///         - observar que essa tabela não serve de nada, precisamos dos relacionamentos
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.StartLogSqlToConsole();

                var info = contexto.GetDBTableInfo(typeof(FilmeAtor));
                Console.WriteLine(info.NomeTabela);

            }
        }
    }
}
