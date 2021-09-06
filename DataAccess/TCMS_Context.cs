using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace DataAccess
{
    public class TCMS_Context : DbContext
    {
        public TCMS_Context() : base("name=TCMS_Context")
        {
            Database.SetInitializer(new DbInitializer());
        }
        public virtual DbSet<AssignmentDetail> AssignmentDetail { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<MaintenanceDetail> MaintenanceDetail { get; set; }
        public virtual DbSet<MaintenanceInfo> MaintenanceInfo { get; set; }
        public virtual DbSet<OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<ShippingAssignment> ShippingAssignment { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .Property(e => e.ContactPerson)
                .IsUnicode(false);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.OrderInfo)
                .WithOptional(e => e.Company)
                .HasForeignKey(e => e.DestinationID);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.OrderInfo1)
                .WithOptional(e => e.Company1)
                .HasForeignKey(e => e.SourceID);

            modelBuilder.Entity<Employee>()
                .Property(e => e.ID)
                .IsFixedLength();

            modelBuilder.Entity<Employee>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Position)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.HomePhoneNum)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.CellPhone)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<MaintenanceDetail>()
                .Property(e => e.Service)
                .IsUnicode(false);

            modelBuilder.Entity<MaintenanceDetail>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<MaintenanceInfo>()
                .Property(e => e.EmployeeID)
                .IsFixedLength();

            modelBuilder.Entity<MaintenanceInfo>()
                .Property(e => e.VehicleID)
                .IsFixedLength();

            modelBuilder.Entity<MaintenanceInfo>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<OrderInfo>()
                .Property(e => e.SourceAddress)
                .IsUnicode(false);

            modelBuilder.Entity<OrderInfo>()
                .Property(e => e.DestinationAddresss)
                .IsUnicode(false);

            modelBuilder.Entity<OrderInfo>()
                .Property(e => e.DocName)
                .IsUnicode(false);

            modelBuilder.Entity<OrderInfo>()
                .Property(e => e.DocType)
                .IsUnicode(false);

            modelBuilder.Entity<ShippingAssignment>()
                .Property(e => e.VehicleID)
                .IsFixedLength();

            modelBuilder.Entity<ShippingAssignment>()
                .Property(e => e.EmployeeID)
                .IsFixedLength();

            modelBuilder.Entity<ShippingAssignment>()
                .HasMany(e => e.AssignmentDetail)
                .WithOptional(e => e.ShippingAssignment)
                .HasForeignKey(e => e.ShippingAssignmentlID);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.ID)
                .IsFixedLength();

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.Brand)
                .IsUnicode(false);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.Model)
                .IsUnicode(false);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.Parts)
                .IsUnicode(false);
        }
    }
}
