using Alura.EFCore2.Curso.Database;
using Alura.EFCore2.Curso.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Alura.EFCore2.Curso
{
    class Program
    {
        static void Main(string[] args)
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
