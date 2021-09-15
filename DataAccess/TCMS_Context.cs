using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class TCMS_Context : IdentityDbContext
    {
        public const string ConnectString = "Data Source=.; Initial Catalog=TCMS_Database;Integrated Security=True;MultipleActiveResultSets=True";
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(ConnectString);
        //    optionsBuilder.UseLoggerFactory(GetLoggerFactory());       // bật logger
        //}

        //private ILoggerFactory GetLoggerFactory()
        //{
        //    IServiceCollection serviceCollection = new ServiceCollection();
        //    serviceCollection.AddLogging(builder =>
        //            builder.AddConsole()
        //                   .AddFilter(DbLoggerCategory.Database.Command.Name,
        //                            LogLevel.Information));
        //    return serviceCollection.BuildServiceProvider()
        //            .GetService<ILoggerFactory>();
        //}
        public TCMS_Context(DbContextOptions<TCMS_Context> options) : base(options)
        {
            //Database.SetInitializer(new DbInitializer());
        }
        public virtual DbSet<AssignmentDetail> AssignmentDetail { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<MaintenanceDetail> MaintenanceDetail { get; set; }
        public virtual DbSet<MaintenanceInfo> MaintenanceInfo { get; set; }
        public virtual DbSet<OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<ShippingAssignment> ShippingAssignment { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName[6..]);
                }
            }
            builder.Entity<Company>()
                .Property(e => e.Name)
                .IsUnicode(false);

            builder.Entity<Company>()
                .Property(e => e.Address)
                .IsUnicode(false);

            builder.Entity<Company>()
                .Property(e => e.City)
                .IsUnicode(false);

            builder.Entity<Company>()
                .Property(e => e.State)
                .IsUnicode(false);

            builder.Entity<Company>()
                .Property(e => e.ContactPerson)
                .IsUnicode(false);

            //builder.Entity<Company>()
            //    .HasMany(e => e.OrderInfo)
            //    .WithOptional(e => e.Company)
            //    .HasForeignKey(e => e.DestinationID);

            //builder.Entity<Company>()
            //    .HasMany(e => e.OrderInfo1)
            //    .WithOptional(e => e.Company1)
            //    .HasForeignKey(e => e.SourceID);

            builder.Entity<Employee>()
                .Property(e => e.ID)
                .IsFixedLength();

            builder.Entity<Employee>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.Position)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.Address)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.City)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.State)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.HomePhoneNum)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.CellPhone)
                .IsUnicode(false);

            builder.Entity<Employee>()
                .Property(e => e.Email)
                .IsUnicode(false);

            builder.Entity<MaintenanceDetail>()
                .Property(e => e.Service)
                .IsUnicode(false);

            builder.Entity<MaintenanceDetail>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            builder.Entity<MaintenanceInfo>()
                .Property(e => e.EmployeeID)
                .IsFixedLength();

            builder.Entity<MaintenanceInfo>()
                .Property(e => e.VehicleID)
                .IsFixedLength();

            builder.Entity<MaintenanceInfo>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            builder.Entity<OrderInfo>()
                .Property(e => e.SourceAddress)
                .IsUnicode(false);

            builder.Entity<OrderInfo>()
                .Property(e => e.DestinationAddresss)
                .IsUnicode(false);

            builder.Entity<OrderInfo>()
                .Property(e => e.DocName)
                .IsUnicode(false);

            builder.Entity<OrderInfo>()
                .Property(e => e.DocType)
                .IsUnicode(false);

            builder.Entity<ShippingAssignment>()
                .Property(e => e.VehicleID)
                .IsFixedLength();

            builder.Entity<ShippingAssignment>()
                .Property(e => e.EmployeeID)
                .IsFixedLength();

            //builder.Entity<ShippingAssignment>()
            //    .HasMany(e => e.AssignmentDetail)
            //    .WithOptional(e => e.ShippingAssignment)
            //    .HasForeignKey(e => e.ShippingAssignmentlID);

            builder.Entity<Vehicle>()
                .Property(e => e.ID)
                .IsFixedLength();

            builder.Entity<Vehicle>()
                .Property(e => e.Brand)
                .IsUnicode(false);

            builder.Entity<Vehicle>()
                .Property(e => e.Model)
                .IsUnicode(false);

            builder.Entity<Vehicle>()
                .Property(e => e.Type)
                .IsUnicode(false);

            builder.Entity<Vehicle>()
                .Property(e => e.Parts)
                .IsUnicode(false);

        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Company>()
        //        .Property(e => e.Name)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Company>()
        //        .Property(e => e.Address)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Company>()
        //        .Property(e => e.City)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Company>()
        //        .Property(e => e.State)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Company>()
        //        .Property(e => e.ContactPerson)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Company>()
        //        .HasMany(e => e.OrderInfo)
        //        .WithOptional(e => e.Company)
        //        .HasForeignKey(e => e.DestinationID);

        //    modelBuilder.Entity<Company>()
        //        .HasMany(e => e.OrderInfo1)
        //        .WithOptional(e => e.Company1)
        //        .HasForeignKey(e => e.SourceID);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.ID)
        //        .IsFixedLength();

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.FirstName)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.MiddleName)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.LastName)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.Position)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.Address)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.City)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.State)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.HomePhoneNum)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.CellPhone)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.Email)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<MaintenanceDetail>()
        //        .Property(e => e.Service)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<MaintenanceDetail>()
        //        .Property(e => e.Notes)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<MaintenanceInfo>()
        //        .Property(e => e.EmployeeID)
        //        .IsFixedLength();

        //    modelBuilder.Entity<MaintenanceInfo>()
        //        .Property(e => e.VehicleID)
        //        .IsFixedLength();

        //    modelBuilder.Entity<MaintenanceInfo>()
        //        .Property(e => e.Notes)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<OrderInfo>()
        //        .Property(e => e.SourceAddress)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<OrderInfo>()
        //        .Property(e => e.DestinationAddresss)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<OrderInfo>()
        //        .Property(e => e.DocName)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<OrderInfo>()
        //        .Property(e => e.DocType)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<ShippingAssignment>()
        //        .Property(e => e.VehicleID)
        //        .IsFixedLength();

        //    modelBuilder.Entity<ShippingAssignment>()
        //        .Property(e => e.EmployeeID)
        //        .IsFixedLength();

        //    modelBuilder.Entity<ShippingAssignment>()
        //        .HasMany(e => e.AssignmentDetail)
        //        .WithOptional(e => e.ShippingAssignment)
        //        .HasForeignKey(e => e.ShippingAssignmentlID);

        //    modelBuilder.Entity<Vehicle>()
        //        .Property(e => e.ID)
        //        .IsFixedLength();

        //    modelBuilder.Entity<Vehicle>()
        //        .Property(e => e.Brand)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Vehicle>()
        //        .Property(e => e.Model)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Vehicle>()
        //        .Property(e => e.Type)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Vehicle>()
        //        .Property(e => e.Parts)
        //        .IsUnicode(false);
        //}
    }
}
