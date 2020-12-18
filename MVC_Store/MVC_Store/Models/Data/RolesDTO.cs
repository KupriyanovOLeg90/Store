using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Store.Models.Data
{
    [Table("tblRoles")]
    public class RolesDTO
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}