using EduMedia.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EduMedia.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ContentItem> ContentItems { get; set; }
        

    }
}
