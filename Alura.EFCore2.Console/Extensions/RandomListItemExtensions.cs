using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.EFCore2.Curso.Extensions
{
    public static class RandomListItemExtensions
    {
        public static T RandomItem<T>(this IList<T> lista)
        {
            if (lista == null) throw new ArgumentException();
            int index = new Random().Next(0, lista.Count);
            return lista[index];
        }
    }
}
