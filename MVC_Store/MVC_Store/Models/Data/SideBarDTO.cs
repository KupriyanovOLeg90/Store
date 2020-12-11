using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Store.Models.Data
{
    [Table("tblSidebars")]
    public class SideBarDTO
    {
        [Key]
        public int Id { get; set; }

        public string Body { get; set; }
    }
}