using Microsoft.EntityFrameworkCore;
using ThaiBevAssignment.Models;

namespace ThaiBevAssignment.AppContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
