using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Alura.EFCore2.Curso.Aula1
{
    class GerandoSqlExceptionDevidoAusenciaDeMapeamento
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
        }

        /// <summary>
        ///     Pré-requisitos:
        ///         - bando de dados AluraFilmes criado e populado
        ///         - EF Core instalado no projeto
        ///         - classe LogSQLExtensions criada para logar o SQL
        ///     Objetivos:
        ///         - vídeo anterior: preparando o ambiente
        ///         - executar e observar a SqlException 'invalid object name "Atores"'
        ///         - mostrar o SQL gerado
        ///         - explicar o processo de mapeamento, cujo primeiro passo é Usar Convenções
        ///         - próximo vídeo: configurando o nome da tabela usando anotações
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
