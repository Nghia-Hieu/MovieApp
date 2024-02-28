﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MovieManagementEntities : DbContext
    {
        public MovieManagementEntities()
            : base("name=MovieManagementEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Certification> Certifications { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<GoldenHour> GoldenHours { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<ShowTime> ShowTimes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
    }
}
