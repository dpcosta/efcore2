﻿using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Alura.EFCore2.Curso.Aula6
{
    class MultiplosRelacionamentosEntreMesmasTabelas
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
            public Idioma IdiomaFalado { get; set; }
            public Idioma IdiomaOriginal { get; set; }

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

                entityBuilder
                    .Property<byte>("language_id")
                    .IsRequired();

                entityBuilder
                    .HasOne(f => f.IdiomaFalado)
                    .WithMany(i => i.FilmesFalados)
                    .HasForeignKey("language_id")
                    .HasConstraintName("fk_film_language");

                entityBuilder
                    .Property<byte?>("original_language_id");

                entityBuilder
                    .HasOne(f => f.IdiomaOriginal)
                    .WithMany(i => i.FilmesOriginais)
                    .HasForeignKey("original_language_id")
                    .HasConstraintName("fk_film_language_original");
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

        private class Idioma
        {
            public byte Id { get; set; }
            public string Nome { get; set; }

            //navegação
            public ICollection<Filme> FilmesFalados { get; set; }
            public ICollection<Filme> FilmesOriginais { get; set; }

            public Idioma()
            {
                FilmesFalados = new HashSet<Filme>();
                FilmesOriginais = new HashSet<Filme>();
            }

            public override string ToString()
            {
                return $"Idioma ({this.Id}) {this.Nome}";
            }
        }

        private class IdiomaConfiguracao
        {
            public static void Configurar(EntityTypeBuilder<Idioma> entityBuilder)
            {
                entityBuilder
                    .ToTable("language")
                    .HasKey(i => i.Id)
                    .HasName("pk_language");

                entityBuilder
                    .Property(i => i.Id)
                    .HasColumnName("language_id");

                entityBuilder
                    .Property(i => i.Nome)
                    .HasColumnName("name");
            }
        }

        private class AluraFilmesContexto : DbContext
        {
            public DbSet<Ator> Atores { get; set; }
            public DbSet<Filme> Filmes { get; set; }
            public DbSet<Idioma> Idiomas { get; set; }

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
                IdiomaConfiguracao.Configurar(modelBuilder.Entity<Idioma>());
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - banco de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - vamos mapear agora a relação Filme x Idioma
        ///         - repare que um filme tem dois idiomas: um original (que pode ser nulo), outro que indica o idioma da mídia em estoque (não nulo)
        ///         - neste caso, como fazer? neste caso não podemos contar com as convenções
        ///         - primeiro vamos criar a classe Idioma e configurar seu mapeamento
        ///         - fazer um SELECT pra testar
        ///         - erro no tipo language_id: mudar prop pra byte
        ///         - funfou
        ///         - agora mapear os 2 relacionamentos 1 x N - TESTAR SE VAI FUNFAR SEM CONFIGURAR
        ///         - 1o) relação IdiomaFalado; relembrar termos...
        ///             - Entidade Principal: Idioma
        ///             - Entidade Dependente: Filme
        ///             - Chave Estrangeira: shadow property Filme.language_id
        ///             - Chave Principal: Idioma.Id
        ///             - Navegação Coleção: Idioma.FilmesFalados
        ///             - Navegação Referência: Filme.IdiomaFalado
        ///             - criar relação 
        ///             - lembrar que essa relação é NOT NULL (script tem que acusar...)
        ///         - 2o) relação IdiomaOriginal; relembrar termos:
        ///             - Entidade Principal: Idioma
        ///             - Entidade Dependente: Filme
        ///             - Chave Estrangeira: shadow property Filme.original_language_id
        ///             - Chave Principal: Idioma.Id
        ///             - Navegação Coleção: Idioma.FilmesOriginais
        ///             - Navegação Referência: Filme.IdiomaOriginal
        ///             - criar relação 
        ///             - lembrar que essa relação pode ser NULL (script tem que acusar...)
        ///             - ou seja, colocar Property<byte?> na shadow property
        ///          - fazer um SELECT em filmes com idioma falado == "French"
        ///          - necessário rodar um script de carga
        /// </summary>
        static void Main()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.StartLogSqlToConsole();

                foreach (var idioma in contexto.Idiomas)
                {
                    Console.WriteLine(idioma);
                }

                var idiomaPesquisado = "French";
                Console.WriteLine($"\nFilmes cujo idioma falado é {idiomaPesquisado}:");
                var filmesNoIdiomaPesquisado = contexto.Filmes
                    .Include(f => f.IdiomaFalado)
                    .Include(f => f.IdiomaOriginal)
                    .Where(f => f.IdiomaFalado.Nome == idiomaPesquisado);

                foreach (var filme in filmesNoIdiomaPesquisado)
                {
                    Console.WriteLine(filme);
                }

            }
        }
    }
}
