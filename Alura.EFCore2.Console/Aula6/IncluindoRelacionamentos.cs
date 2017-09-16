using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Alura.EFCore2.Curso.Aula6
{
    class IncluindoRelacionamentos
    {
        private class Ator
        {
            public int Id { get; set; }

            [Required]
            public string PrimeiroNome { get; set; }

            [Required]
            public string UltimoNome { get; set; }

            //navegação
            public ICollection<FilmeAtor> Filmografia { get; set; }

            public Ator()
            {
                Filmografia = new HashSet<FilmeAtor>();
            }

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

            //navegação
            public ICollection<FilmeAtor> Elenco { get; set; }

            public Filme()
            {
                Elenco = new HashSet<FilmeAtor>();
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

                entityBuilder
                    .HasOne(fa => fa.Filme)
                    .WithMany(f => f.Elenco)
                    .HasForeignKey("film_id");

                entityBuilder
                    .HasOne(fa => fa.Ator)
                    .WithMany(a => a.Filmografia)
                    .HasForeignKey("actor_id");
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
        ///         - observar que essa tabela não serve de nada, precisamos dos relacionamentos
        ///         - queremos fazer contexto.Filmes.Include(f => f.Elenco).ThenInclude(e => e.Ator);
        ///         - introduzir os termos do EF para relações (PPT!):
        ///             - entidade Dependente (ex. FilmeAtor)
        ///             - entidade Principal (ex. Filme/Ator)
        ///             - Chave Estrangeira (ex. FilmeAtor.FilmeId/FilmeAtor.AtorId)
        ///             - Chave Principal (ex. Filme.Id/Ator.Id)
        ///             - Propriedade de Navegação:
        ///                 - Propriedade de Navegação do tipo Coleção (ex. Filme.Elenco/Ator.Filmografia)
        ///                 - Propriedade de Navegação do tipo Referência (ex. FilmeAtor.Filme/FilmeAtor.Ator)
        ///                 - Propriedade de Navegação Inversa (ex. Filme.Elenco = FilmeAtor.Filme / Ator.Filmografia = FilmeAtor.Ator)
        ///             - criar as propriedades de navegação de coleção nas entidades principais Filme e Ator
        ///             - testar o SELECT, dará o sgte erro:
        ///             Exceção Sem Tratamento: System.Data.SqlClient.SqlException:
        ///                 Invalid column name 'AtorId'.
        ///                 Invalid column name 'FilmeId'.
        ///             - mostrar as convenções do EF, e observar que do jeito que fizemos
        ///                 o EF está procurando pelas colunas FilmeId e AtorId
        ///             - repare no SELECT gerado pelo join
        ///             SELECT [f0].[film_id], [f0].[actor_id], [f0].[AtorId], [f0].[FilmeId], [a].[actor_id], [a].[first_name], [a].[last_name], [a].[last_update]
        ///             FROM[film_actor] AS[f0]
        ///             INNER JOIN(
        ///                 SELECT DISTINCT TOP(1) [f].[film_id]
        ///                 FROM[film] AS[f]
        ///                 ORDER BY[f].[film_id]
        ///             ) AS[f1] ON[f0].[FilmeId] = [f1].[film_id]
        ///             LEFT JOIN[actor] AS[a] ON[f0].[AtorId] = [a].[actor_id]
        ///             ORDER BY[f1].[film_id]
        ///             - configurar as chaves estrangeiras:
        ///                 - entityBuilder.HasOne(fa => fa.Filme).WithMany(f => f.Elenco).HasForeignKey("film_id");
        ///                 - entityBuilder.HasOne(fa => fa.Ator).WithMany(a => a.Filmografia).HasForeignKey("actor_id");
        ///             - testar o SELECT; vai funfar!
        ///             - testar o script DDL
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.StartLogSqlToConsole();

                var filme = contexto.Filmes
                    .Include(f => f.Elenco)
                    .ThenInclude(e => e.Ator)
                    .First();

                Console.WriteLine($"Mostrando o elenco de {filme.Titulo}:");
                foreach (var item in filme.Elenco)
                {
                    Console.WriteLine(item.Ator);
                }

            }
        }
    }
}
