using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alura.EFCore2.Curso.Aula1
{
    class ConfigurandoNomeColunaUsandoAnotacoes
    {
        [Table("actor")]
        private class Ator
        {
            [Column("actor_id")]
            public int Id { get; set; }

            [Column("first_name")]
            public string PrimeiroNome { get; set; }

            [Column("last_name")]
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
        ///         - configurar os nomes da colunas usando a anotação Column
        ///         - executar e observar que o programa vai funcionar!
        ///         - mostrar que o SQL gerado agora possui o nome da tabela e coluna certo!
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
