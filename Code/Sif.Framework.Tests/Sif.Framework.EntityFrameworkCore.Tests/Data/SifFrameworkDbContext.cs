using Microsoft.EntityFrameworkCore;
using Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;

namespace Sif.Framework.EntityFrameworkCore.Tests.Data
{
    public partial class SifFrameworkDbContext : DbContext
    {
        public SifFrameworkDbContext()
        {
        }

        public SifFrameworkDbContext(DbContextOptions<SifFrameworkDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationInfo> ApplicationInfos { get; set; } = null!;
        //public virtual DbSet<ApplicationRegister> ApplicationRegisters { get; set; } = null!;
        //public virtual DbSet<Environment> Environments { get; set; } = null!;
        //public virtual DbSet<EnvironmentInfrastructureService> EnvironmentInfrastructureServices { get; set; } = null!;
        //public virtual DbSet<EnvironmentProvisionedZone> EnvironmentProvisionedZones { get; set; } = null!;
        //public virtual DbSet<EnvironmentRegister> EnvironmentRegisters { get; set; } = null!;
        //public virtual DbSet<EnvironmentRegisterInfrastructureService> EnvironmentRegisterInfrastructureServices { get; set; } = null!;
        //public virtual DbSet<EnvironmentRegisterProvisionedZone> EnvironmentRegisterProvisionedZones { get; set; } = null!;
        public virtual DbSet<InfrastructureService> InfrastructureServices { get; set; } = null!;

        public virtual DbSet<ProductIdentity> ProductIdentities { get; set; } = null!;

        //public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<ProvisionedZone> ProvisionedZones { get; set; } = null!;

        public virtual DbSet<Right> Rights { get; set; } = null!;
        public virtual DbSet<Models.Infrastructure.Service> Services { get; set; } = null!;
        //public virtual DbSet<SifObjectBinding> SifObjectBindings { get; set; } = null!;
        //public virtual DbSet<Zone> Zones { get; set; } = null!;
        //public virtual DbSet<ZoneProperty> ZoneProperties { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationInfo>(entity =>
            {
                entity.ToTable("APPLICATION_INFO");

                entity.Property(e => e.Id).HasColumnName("APPLICATION_INFO_ID");

                entity.Property(e => e.ApplicationKey).HasColumnName("APPLICATION_KEY");

                entity.Property(e => e.DataModelNamespace).HasColumnName("DATA_MODEL_NAMESPACE");

                entity.Property(e => e.SupportedInfrastructureVersion).HasColumnName("SUPPORTED_INFRASTRUCTURE_VERSION");

                entity.Property(e => e.Transport).HasColumnName("TRANSPORT");

                entity.HasOne(b => b.AdapterProduct)
                    .WithOne()
                    .HasForeignKey<ProductIdentity>("ADAPTER_PRODUCT_ID")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.ApplicationProduct)
                    .WithOne()
                    .HasForeignKey<ProductIdentity>("APPLICATION_PRODUCT_ID")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //modelBuilder.Entity<ApplicationRegister>(entity =>
            //{
            //    entity.ToTable("APPLICATION_REGISTER");

            //    entity.Property(e => e.ApplicationRegisterId)
            //        .HasColumnType("integer")
            //        .HasColumnName("APPLICATION_REGISTER_ID");

            //    entity.Property(e => e.ApplicationKey).HasColumnName("APPLICATION_KEY");

            //    entity.Property(e => e.SharedSecret).HasColumnName("SHARED_SECRET");

            //    entity.HasMany(d => d.EnvironmentRegisters)
            //        .WithMany(p => p.ApplicationRegisters)
            //        .UsingEntity<Dictionary<string, object>>(
            //            "ApplicationEnvironmentRegister",
            //            l => l.HasOne<EnvironmentRegister>().WithMany().HasForeignKey("EnvironmentRegisterId").OnDelete(DeleteBehavior.ClientSetNull),
            //            r => r.HasOne<ApplicationRegister>().WithMany().HasForeignKey("ApplicationRegisterId").OnDelete(DeleteBehavior.ClientSetNull),
            //            j =>
            //            {
            //                j.HasKey("ApplicationRegisterId", "EnvironmentRegisterId");

            //                j.ToTable("APPLICATION_ENVIRONMENT_REGISTERS");

            //                j.IndexerProperty<long>("ApplicationRegisterId").HasColumnName("APPLICATION_REGISTER_ID");

            //                j.IndexerProperty<long>("EnvironmentRegisterId").HasColumnName("ENVIRONMENT_REGISTER_ID");
            //            });
            //});

            //modelBuilder.Entity<Environment>(entity =>
            //{
            //    entity.ToTable("ENVIRONMENT");

            //    entity.HasIndex(e => e.ApplicationInfoId, "IX_ENVIRONMENT_APPLICATION_INFO_ID")
            //        .IsUnique();

            //    entity.Property(e => e.EnvironmentId).HasColumnName("ENVIRONMENT_ID");

            //    entity.Property(e => e.ApplicationInfoId).HasColumnName("APPLICATION_INFO_ID");

            //    entity.Property(e => e.AuthenticationMethod).HasColumnName("AUTHENTICATION_METHOD");

            //    entity.Property(e => e.ConsumerName).HasColumnName("CONSUMER_NAME");

            //    entity.Property(e => e.InstanceId).HasColumnName("INSTANCE_ID");

            //    entity.Property(e => e.SessionToken).HasColumnName("SESSION_TOKEN");

            //    entity.Property(e => e.SolutionId).HasColumnName("SOLUTION_ID");

            //    entity.Property(e => e.Type).HasColumnName("TYPE");

            //    entity.Property(e => e.UserToken).HasColumnName("USER_TOKEN");

            //    entity.Property(e => e.ZoneId).HasColumnName("ZONE_ID");

            //    entity.HasOne(d => d.ApplicationInfo)
            //        .WithOne(p => p.Environment)
            //        .HasForeignKey<Environment>(d => d.ApplicationInfoId);

            //    entity.HasOne(d => d.Zone)
            //        .WithMany(p => p.Environments)
            //        .HasForeignKey(d => d.ZoneId);
            //});

            //modelBuilder.Entity<EnvironmentInfrastructureService>(entity =>
            //{
            //    entity.HasKey(e => new { e.EnvironmentId, e.Name });

            //    entity.ToTable("ENVIRONMENT_INFRASTRUCTURE_SERVICES");

            //    entity.Property(e => e.EnvironmentId).HasColumnName("ENVIRONMENT_ID");

            //    entity.Property(e => e.Name).HasColumnName("NAME");

            //    entity.Property(e => e.InfrastructureServiceId).HasColumnName("INFRASTRUCTURE_SERVICE_ID");

            //    entity.HasOne(d => d.Environment)
            //        .WithMany(p => p.EnvironmentInfrastructureServices)
            //        .HasForeignKey(d => d.EnvironmentId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);

            //    entity.HasOne(d => d.InfrastructureService)
            //        .WithMany(p => p.EnvironmentInfrastructureServices)
            //        .HasForeignKey(d => d.InfrastructureServiceId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);
            //});

            //modelBuilder.Entity<EnvironmentProvisionedZone>(entity =>
            //{
            //    entity.HasKey(e => new { e.EnvironmentId, e.SifId });

            //    entity.ToTable("ENVIRONMENT_PROVISIONED_ZONES");

            //    entity.Property(e => e.EnvironmentId).HasColumnName("ENVIRONMENT_ID");

            //    entity.Property(e => e.SifId).HasColumnName("SIF_ID");

            //    entity.Property(e => e.ProvisionedZoneId).HasColumnName("PROVISIONED_ZONE_ID");

            //    entity.HasOne(d => d.Environment)
            //        .WithMany(p => p.EnvironmentProvisionedZones)
            //        .HasForeignKey(d => d.EnvironmentId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);

            //    entity.HasOne(d => d.ProvisionedZone)
            //        .WithMany(p => p.EnvironmentProvisionedZones)
            //        .HasForeignKey(d => d.ProvisionedZoneId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);
            //});

            //modelBuilder.Entity<EnvironmentRegister>(entity =>
            //{
            //    entity.ToTable("ENVIRONMENT_REGISTER");

            //    entity.Property(e => e.EnvironmentRegisterId)
            //        .HasColumnType("integer")
            //        .HasColumnName("ENVIRONMENT_REGISTER_ID");

            //    entity.Property(e => e.ApplicationKey).HasColumnName("APPLICATION_KEY");

            //    entity.Property(e => e.InstanceId).HasColumnName("INSTANCE_ID");

            //    entity.Property(e => e.SolutionId).HasColumnName("SOLUTION_ID");

            //    entity.Property(e => e.UserToken).HasColumnName("USER_TOKEN");

            //    entity.Property(e => e.ZoneId).HasColumnName("ZONE_ID");

            //    entity.HasOne(d => d.Zone)
            //        .WithMany(p => p.EnvironmentRegisters)
            //        .HasForeignKey(d => d.ZoneId);
            //});

            //modelBuilder.Entity<EnvironmentRegisterInfrastructureService>(entity =>
            //{
            //    entity.HasKey(e => new { e.EnvironmentRegisterId, e.Name });

            //    entity.ToTable("ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES");

            //    entity.Property(e => e.EnvironmentRegisterId).HasColumnName("ENVIRONMENT_REGISTER_ID");

            //    entity.Property(e => e.Name).HasColumnName("NAME");

            //    entity.Property(e => e.InfrastructureServiceId).HasColumnName("INFRASTRUCTURE_SERVICE_ID");

            //    entity.HasOne(d => d.EnvironmentRegister)
            //        .WithMany(p => p.EnvironmentRegisterInfrastructureServices)
            //        .HasForeignKey(d => d.EnvironmentRegisterId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);

            //    entity.HasOne(d => d.InfrastructureService)
            //        .WithMany(p => p.EnvironmentRegisterInfrastructureServices)
            //        .HasForeignKey(d => d.InfrastructureServiceId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);
            //});

            //modelBuilder.Entity<EnvironmentRegisterProvisionedZone>(entity =>
            //{
            //    entity.HasKey(e => new { e.EnvironmentRegisterId, e.SifId });

            //    entity.ToTable("ENVIRONMENT_REGISTER_PROVISIONED_ZONES");

            //    entity.Property(e => e.EnvironmentRegisterId).HasColumnName("ENVIRONMENT_REGISTER_ID");

            //    entity.Property(e => e.SifId).HasColumnName("SIF_ID");

            //    entity.Property(e => e.ProvisionedZoneId).HasColumnName("PROVISIONED_ZONE_ID");

            //    entity.HasOne(d => d.EnvironmentRegister)
            //        .WithMany(p => p.EnvironmentRegisterProvisionedZones)
            //        .HasForeignKey(d => d.EnvironmentRegisterId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);

            //    entity.HasOne(d => d.ProvisionedZone)
            //        .WithMany(p => p.EnvironmentRegisterProvisionedZones)
            //        .HasForeignKey(d => d.ProvisionedZoneId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);
            //});

            modelBuilder.Entity<InfrastructureService>(entity =>
            {
                entity.ToTable("INFRASTRUCTURE_SERVICE");

                entity.Property(e => e.Id).HasColumnName("INFRASTRUCTURE_SERVICE_ID");

                entity.Property(e => e.Name).HasColumnName("NAME");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });

            modelBuilder.Entity<ProductIdentity>(entity =>
            {
                entity.ToTable("PRODUCT_IDENTITY");

                entity.Property(e => e.Id).HasColumnName("PRODUCT_IDENTITY_ID");

                entity.Property(e => e.IconUri).HasColumnName("ICON_URI");

                entity.Property(e => e.ProductName).HasColumnName("PRODUCT_NAME");

                entity.Property(e => e.ProductVersion).HasColumnName("PRODUCT_VERSION");

                entity.Property(e => e.VendorName).HasColumnName("VENDOR_NAME");
            });

            //modelBuilder.Entity<Property>(entity =>
            //{
            //    entity.ToTable("PROPERTY");

            //    entity.Property(e => e.PropertyId)
            //        .HasColumnType("integer")
            //        .HasColumnName("PROPERTY_ID");

            //    entity.Property(e => e.Name).HasColumnName("NAME");

            //    entity.Property(e => e.Value).HasColumnName("VALUE");
            //});

            modelBuilder.Entity<ProvisionedZone>(entity =>
            {
                entity.ToTable("PROVISIONED_ZONE");

                entity.Property(e => e.Id).HasColumnName("PROVISIONED_ZONE_ID");

                entity.Property(e => e.SifId).HasColumnName("SIF_ID");

                entity.HasMany(e => e.Services)
                    .WithOne()
                    .HasForeignKey("PROVISIONED_ZONE_ID")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Right>(entity =>
            {
                entity.ToTable("RIGHT");

                entity.Property(e => e.Id).HasColumnName("RIGHT_ID");

                entity.Property(e => e.Type).HasColumnName("TYPE");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });

            modelBuilder.Entity<Models.Infrastructure.Service>(entity =>
            {
                entity.ToTable("SERVICE");

                entity.Property(e => e.Id).HasColumnName("SERVICE_ID");

                entity.Property(e => e.ContextId).HasColumnName("CONTEXTID");

                entity.Property(e => e.Name).HasColumnName("NAME");

                entity.Property(e => e.Type).HasColumnName("TYPE");

                entity.HasMany(e => e.Rights)
                    .WithOne()
                    .HasForeignKey("SERVICE_ID")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //modelBuilder.Entity<SifObjectBinding>(entity =>
            //{
            //    entity.ToTable("SIF_OBJECT_BINDING");

            //    entity.Property(e => e.SifObjectBindingId)
            //        .HasColumnType("integer")
            //        .HasColumnName("SIF_OBJECT_BINDING_ID");

            //    entity.Property(e => e.OwnerId).HasColumnName("OWNER_ID");

            //    entity.Property(e => e.RefId).HasColumnName("REF_ID");
            //});

            //modelBuilder.Entity<Zone>(entity =>
            //{
            //    entity.ToTable("ZONE");

            //    entity.Property(e => e.ZoneId)
            //        .HasColumnType("integer")
            //        .HasColumnName("ZONE_ID");

            //    entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

            //    entity.Property(e => e.SifId).HasColumnName("SIF_ID");
            //});

            //modelBuilder.Entity<ZoneProperty>(entity =>
            //{
            //    entity.HasKey(e => new { e.ZoneId, e.Name });

            //    entity.ToTable("ZONE_PROPERTIES");

            //    entity.Property(e => e.ZoneId).HasColumnName("ZONE_ID");

            //    entity.Property(e => e.Name).HasColumnName("NAME");

            //    entity.Property(e => e.PropertyId).HasColumnName("PROPERTY_ID");

            //    entity.HasOne(d => d.Property)
            //        .WithMany(p => p.ZoneProperties)
            //        .HasForeignKey(d => d.PropertyId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);

            //    entity.HasOne(d => d.Zone)
            //        .WithMany(p => p.ZoneProperties)
            //        .HasForeignKey(d => d.ZoneId)
            //        .OnDelete(DeleteBehavior.ClientSetNull);
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}