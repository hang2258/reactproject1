using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using UrlShortener.Models;

namespace UrlShortener.Data
{
    public class UrlDbContext : DbContext
    {
        public DbSet<UrlEntity> Urls { get; set; }

        public UrlDbContext(DbContextOptions<UrlDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configuration if needed
        }
    }
}