/*
 * Copyright 2022 Systemic Pty Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.EntityFrameworkCore;
using Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;

namespace Sif.Framework.EntityFrameworkCore.Tests.Data;

public class SifFrameworkDbContext : DbContext
{
    public SifFrameworkDbContext(DbContextOptions<SifFrameworkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationInfo> ApplicationInfos { get; set; } = null!;
    public virtual DbSet<ApplicationRegister> ApplicationRegisters { get; set; } = null!;
    public virtual DbSet<Environment> Environments { get; set; } = null!;
    public virtual DbSet<EnvironmentRegister> EnvironmentRegisters { get; set; } = null!;
    public virtual DbSet<InfrastructureService> InfrastructureServices { get; set; } = null!;
    public virtual DbSet<ProductIdentity> ProductIdentities { get; set; } = null!;
    public virtual DbSet<Property> Properties { get; set; } = null!;
    public virtual DbSet<ProvisionedZone> ProvisionedZones { get; set; } = null!;
    public virtual DbSet<Right> Rights { get; set; } = null!;
    public virtual DbSet<Models.Infrastructure.Service> Services { get; set; } = null!;
    public virtual DbSet<Zone> Zones { get; set; } = null!;

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

            entity.HasOne(d => d.AdapterProduct)
                .WithOne()
                .HasForeignKey<ProductIdentity>("ADAPTER_PRODUCT_ID")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.ApplicationProduct)
                .WithOne()
                .HasForeignKey<ProductIdentity>("APPLICATION_PRODUCT_ID")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApplicationRegister>(entity =>
        {
            entity.ToTable("APPLICATION_REGISTER");

            entity.Property(e => e.Id).HasColumnName("APPLICATION_REGISTER_ID");

            entity.Property(e => e.ApplicationKey).HasColumnName("APPLICATION_KEY");

            entity.Property(e => e.SharedSecret).HasColumnName("SHARED_SECRET");

            entity.HasMany(d => d.EnvironmentRegisters)
                .WithOne()
                .HasForeignKey("APPLICATION_REGISTER_ID")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Environment>(entity =>
        {
            entity.ToTable("ENVIRONMENT");

            entity.Property(e => e.Id).HasColumnName("ENVIRONMENT_ID");

            entity.Property(e => e.AuthenticationMethod).HasColumnName("AUTHENTICATION_METHOD");

            entity.Property(e => e.ConsumerName).HasColumnName("CONSUMER_NAME");

            entity.Property(e => e.InstanceId).HasColumnName("INSTANCE_ID");

            entity.Property(e => e.SessionToken).HasColumnName("SESSION_TOKEN");

            entity.Property(e => e.SolutionId).HasColumnName("SOLUTION_ID");

            entity.Property(e => e.Type).HasColumnName("TYPE");

            entity.Property(e => e.UserToken).HasColumnName("USER_TOKEN");

            entity.HasOne(d => d.ApplicationInfo)
                .WithOne()
                .HasForeignKey<ApplicationInfo>("ENVIRONMENT_ID")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.DefaultZone)
                .WithOne()
                .HasForeignKey<Zone>("ENVIRONMENT_ID")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.InfrastructureServices)
                .WithOne()
                .HasForeignKey("ENVIRONMENT_ID")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.ProvisionedZones)
                .WithOne()
                .HasForeignKey("ENVIRONMENT_ID")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EnvironmentRegister>(entity =>
        {
            entity.ToTable("ENVIRONMENT_REGISTER");

            entity.Property(e => e.Id).HasColumnName("ENVIRONMENT_REGISTER_ID");

            entity.Property(e => e.ApplicationKey).HasColumnName("APPLICATION_KEY");

            entity.Property(e => e.InstanceId).HasColumnName("INSTANCE_ID");

            entity.Property(e => e.SolutionId).HasColumnName("SOLUTION_ID");

            entity.Property(e => e.UserToken).HasColumnName("USER_TOKEN");

            entity.HasOne(d => d.DefaultZone)
                .WithOne()
                .HasForeignKey<Zone>("ENVIRONMENT_REGISTER_ID")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.InfrastructureServices)
                .WithOne()
                .HasForeignKey("ENVIRONMENT_REGISTER_ID")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.ProvisionedZones)
                .WithOne()
                .HasForeignKey("ENVIRONMENT_REGISTER_ID")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InfrastructureService>(entity =>
        {
            entity.ToTable("INFRASTRUCTURE_SERVICE");

            entity.Property(e => e.Id).HasColumnName("INFRASTRUCTURE_SERVICE_ID");

            entity.Property(e => e.Name).HasColumnName("NAME").HasConversion<string>();

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

        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("PROPERTY");

            entity.Property(e => e.Id).HasColumnName("PROPERTY_ID");

            entity.Property(e => e.Name).HasColumnName("NAME");

            entity.Property(e => e.Value).HasColumnName("VALUE");
        });

        modelBuilder.Entity<ProvisionedZone>(entity =>
        {
            entity.ToTable("PROVISIONED_ZONE");

            entity.Property(e => e.Id).HasColumnName("PROVISIONED_ZONE_ID");

            entity.Property(e => e.SifId).HasColumnName("SIF_ID");

            entity.HasMany(d => d.Services)
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

            entity.HasMany(d => d.Rights)
                .WithOne()
                .HasForeignKey("SERVICE_ID")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.ToTable("ZONE");

            entity.Property(e => e.Id).HasColumnName("ZONE_ID");

            entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

            entity.Property(e => e.SifId).HasColumnName("SIF_ID");

            entity.HasMany(d => d.Properties)
                .WithOne()
                .HasForeignKey("ZONE_ID")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}