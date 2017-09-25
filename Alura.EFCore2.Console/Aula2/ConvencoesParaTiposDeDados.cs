using Microsoft.EntityFrameworkCore;
using Alura.EFCore2.Curso.Extensions;

namespace Alura.EFCore2.Curso.Aula2
{
    class ConvencoesParaTiposDeDados
    {
        private class Ator
        {
            public int Id { get; set; }
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

                modelBuilder.Entity<Ator>()
                    .Property(a => a.PrimeiroNome)
                    .HasColumnName("first_name");

                modelBuilder.Entity<Ator>()
                    .Property(a => a.UltimoNome)
                    .HasColumnName("last_name");
            }
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - primeiro vídeo da aula 2
        ///         - mostrar que o mapeamento de colunas ainda está equivocado:
        ///             - tipo está definido para NVARCHAR(MAX) (convenção para mapear string)
        ///             - tamanho está definido para 4000 (convenção para tamanho de string)
        ///             - estão definidas como NULAS, mas no banco são NÃO NULAS (convenção para nulidade: pelo tipo da CLR, se esse tipo permitir null, então vai mapear para NULL)
        ///         - mas pq se preocupar? está funcionando!
        ///         - mas se precisássemos gerar um script DDL a partir deste modelo, o novo 
        ///             banco gerado estaria diferente do banco original!
        ///         - vamos ver esse problema? instalar o pacote Tools, 
        ///         - rodar Add-Migration...
        ///             Add-Migration Teste -Context Alura.EFCore2.Curso.Aula2.ConvencoesParaTiposDeDados+AluraFilmesContexto
        ///         - ... e depois Script-Migration
        ///         - olhar o script gerado para observar os problemas...
        ///         - mencionar também tamanho das colunas e possível problema de performance
        ///         - como resolver? configurando!
        ///         - próximo vídeo: configurando tipo de dados
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
