using Microsoft.EntityFrameworkCore;
using Alura.EFCore2.Curso.Extensions;

namespace Alura.EFCore2.Curso.Aula2
{
    class OModeloDeDados
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
        ///         - explicar que o Entity usa uma estrutura pra guardar o mapeamento chamada Modelo de Dados
        ///         - essa estrutura fica exposta na propriedade DbContext.Model
        ///         - mostrar informações sobre o mapeamento da classe no console através do método de extensão GetDBTableInfo
        ///         - finalizar a aula dizendo que todo o processo de mapeamento basicamente é para criar a estrutura Model
        ///         - mostrar diagrama CLASSES -- MODEL - BANCO DE DADOS
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
                    System.Console.WriteLine($"Propriedade: {col.NomePropriedade} >> Coluna: {col.NomeColuna}");
                }
            }
        }
    }
}
