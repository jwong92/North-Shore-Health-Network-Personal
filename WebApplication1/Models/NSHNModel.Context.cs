﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NSHNContext : DbContext
    {
        public NSHNContext()
            : base("name=NSHNContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<donation> donations { get; set; }
        public DbSet<north_shore_accounts> north_shore_accounts { get; set; }
        public DbSet<payment_information> payment_information { get; set; }
        public DbSet<province> provinces { get; set; }
        public DbSet<role> roles { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<image> images { get; set; }
        public DbSet<news> news { get; set; }
    }
}