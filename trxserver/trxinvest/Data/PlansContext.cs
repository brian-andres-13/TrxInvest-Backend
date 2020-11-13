using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using trxinvest.Models;

namespace trxinvest.Data
{
    public class PlansContext : DbContext
    {
        public PlansContext(DbContextOptions<PlansContext> options)
            : base(options)
        {
        }

        public DbSet<Plans> Plans { get; set; }
    }
}
