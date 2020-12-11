using MVC_Store.Models.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVC_Store.Models.ViewModels.Pages
{
    public class SidebarVM
    { 
        public SidebarVM() { }

        public SidebarVM(SideBarDTO row)
        {
            Id = row.Id;
            Body = row.Body;
        }        
        
        
        public int Id { get; set; }

        [Required]
        [AllowHtml]
        public string Body { get; set; }
    }
}