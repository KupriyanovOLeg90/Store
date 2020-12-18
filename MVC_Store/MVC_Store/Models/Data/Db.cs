using System.Data.Entity;

namespace MVC_Store.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }

        public DbSet<SideBarDTO> SideBars { get; set; }

        public DbSet<CategoryDTO> Categories { get; set; }

        public DbSet<ProductDTO> Products { get; set; }

        public DbSet<UserDTO> Users { get; set; }

        public DbSet<RolesDTO> Roles { get; set; }

        public DbSet<UserRoleDTO> UserRoles { get; set; }
    }
}