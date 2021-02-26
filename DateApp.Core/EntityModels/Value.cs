using System.ComponentModel.DataAnnotations.Schema;

namespace DateApp.Core.EntityModels
{
    public class Value
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
