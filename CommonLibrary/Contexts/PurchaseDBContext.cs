using CommonLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Contexts
{
    public class PurchaseDBContext : DbContext
    {
        public DbSet<Purchase> Purchases { get; set; }

        public PurchaseDBContext(DbContextOptions<PurchaseDBContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=PurchaseDBConnection");
            }
        }
    }
}
