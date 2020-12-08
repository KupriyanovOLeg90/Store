using System.Data.Entity;

namespace MVC_Store.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }

        public DbSet<SideBarDTO> SideBars { get; set; }

    }
}