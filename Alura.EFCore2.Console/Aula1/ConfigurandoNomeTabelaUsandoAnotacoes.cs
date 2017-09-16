using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alura.EFCore2.Curso.Aula1
{
    class ConfigurandoNomeTabelaUsandoAnotacoes
    {
        [Table("actor")]
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
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - configurar o nome da tabela usando a anotação Table
        ///         - executar e observar a SqlException: 
        ///             Invalid column name 'Id'.
        ///             Invalid column name 'PrimeiroNome'.
        ///             Invalid column name 'UltimoNome'.
        ///         - contudo, mostrar que o SQL gerado agora possui o nome da tabela certo!
        ///         - explicar que no processo de mapeamento, o segundo passo é Verificar Anotações
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
            }
        }
    }
}
