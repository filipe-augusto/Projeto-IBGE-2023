using Microsoft.EntityFrameworkCore;
using ProjetoIBGE.Models;

namespace ProjetoIBGE.Data
{
    public class IBGEDataContext : DbContext
    {
        public IBGEDataContext(DbContextOptions<IBGEDataContext> options) : base(options)
        { }
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<User> User { get; set; }

    }
}

