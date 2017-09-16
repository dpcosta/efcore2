using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.EFCore2.Curso.Extensions
{
    public class DBColumnInfo
    {
        public string NomePropriedade { get; set; }
        public string NomeColuna { get; set; }
        public string TipoColuna { get; set; }
        public int? TamanhoColuna { get; set; }
        public bool EhNulo { get; set; }
    }

    public class DBTableInfo
    {
        public string NomeClasse { get; set; }
        public string NomeTabela { get; set; }
        public ICollection<DBColumnInfo> Columns { get; set; }

        public DBTableInfo()
        {
            Columns = new HashSet<DBColumnInfo>();
        }
    }

    public static class DBTableInfoExtensions
    {
        public static DBTableInfo GetDBTableInfo(this DbContext contexto, Type tipo)
        {
            var tbInfo = new DBTableInfo();
            var dbServices = contexto.GetService<IDbContextServices>();
            var relationalDbServices = dbServices.DatabaseProviderServices as IRelationalDatabaseProviderServices;
            var annotationProvider = relationalDbServices.AnnotationProvider;

            var entityType = contexto.Model.FindEntityType(tipo);

            var tableMap = annotationProvider.For(entityType);

            tbInfo.NomeClasse = tipo.Name;
            tbInfo.NomeTabela = tableMap.TableName;

            var typeMapper = relationalDbServices.TypeMapper;
            var columns = from property in entityType.GetProperties()
                          let columnMap = annotationProvider.For(property)
                          let columnTypeMap = typeMapper.FindMapping(property)
                          select new DBColumnInfo
                          {
                              NomePropriedade = property.Name,
                              NomeColuna = columnMap.ColumnName,
                              TipoColuna = columnTypeMap.StoreType,
                              TamanhoColuna = columnTypeMap.Size,
                              EhNulo = property.IsNullable
                          };
            foreach (var column in columns)
            {
                tbInfo.Columns.Add(column);
            }

            return tbInfo;
        }
    }
}
