using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class NereyeDBContext : DbContext
    {
        public NereyeDBContext(DbContextOptions<NereyeDBContext> options)
            : base(options) { }

        public DbSet<Restaurant> Restaurants { get; set; }
    }
}
