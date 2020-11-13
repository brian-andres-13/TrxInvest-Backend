using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trxinvest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace trxinvest.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PlansContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<PlansContext>>()))
            {
                if (!context.Database.EnsureCreated())
                    context.Database.Migrate();
                if (context.Plans.Any())
                {
                    return;   // DB has been seeded
                }
                context.Plans.AddRange(
                   new Plans
                   {
                       Name = "Diamond Plan",
                       DailyIncome = 3.7F,
                       Days = 0
                   },
                   new Plans
                   {
                       Name = "Platinum Plan",
                       DailyIncome = 4.7F,
                       Days = 45
                   },
                   new Plans
                   {
                       Name = "Gold Plan",
                       DailyIncome = 5.7F,
                       Days = 25
                   },
                   new Plans
                   {
                       Name = "Bronze Plan",
                       DailyIncome = 6.7F,
                       Days = 18
                   }
                );
                context.SaveChanges();
            }
        }
    }
}
