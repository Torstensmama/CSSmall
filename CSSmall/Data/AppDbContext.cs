﻿using Microsoft.EntityFrameworkCore;
using CSSmall.Models;

namespace CSSmall.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ApiUsageLog> ApiUsageLogs { get; set; } // INLOGGNING

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.BookingID);  // Endast primärnyckeln krävs
        }
    }
}
