using System.ComponentModel.DataAnnotations.Schema;

namespace Alura.EFCore2.Console2
{
    [Table("AFL_TBL_ATORES", Schema = "dbo")]
    public partial class Ator
    {
        public int Id { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
    }
}
