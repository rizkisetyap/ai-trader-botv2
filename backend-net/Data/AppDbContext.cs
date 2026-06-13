using backend_net.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_net.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<AnalisaRequestModel> AnalisaRequests { get; set; }
    }
}