using CommonLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Contexts
{
    public class MaterialDBContext : DbContext
    {
        public DbSet<Material> Materials { get; set; }

        public MaterialDBContext(DbContextOptions<MaterialDBContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=MaterialDBConnection");
            }
        }
    }
}
