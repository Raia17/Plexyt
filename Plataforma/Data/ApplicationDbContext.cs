using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Plataforma.Extensions;
using Plataforma.Models.AutoLog;
using Plataforma.Models.Contracts;
using Plataforma.Models.Identity;
using Plataforma.Models.Media;
using Plataforma.Models.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BstHelpers;
using Plataforma.Data.Interceptors;
using User = Plataforma.Models.Identity.User;

namespace Plataforma.Data;

public sealed class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int, IdentityUserClaim, IdentityUserRole<int>,
    IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> {

    #region PrivateProperties
    private bool _enabledTracking = true;
    private readonly IServiceProvider _serviceProvider;
    #endregion

    #region Structure
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Error> Errors { get; set; }
    #endregion

    #region Media
    public DbSet<FileGroup> FileGroups { get; set; }
    public DbSet<File> Files { get; set; }
    #endregion

    #region Identity
    public override DbSet<User> Users { get; set; }
    #endregion


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider) : base(options) {
        _serviceProvider = serviceProvider;
        ChangeTracker.StateChanged += OnEntityStateChanged;
        ChangeTracker.Tracked += OnEntityTracked;
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        // Override the database collation
        builder.UseCollation("Latin1_General_CI_AI_WS");

        // Errors set user to null when user is deleted
        builder.Entity<Error>().HasOne(ci => ci.User).WithMany().OnDelete(DeleteBehavior.SetNull);

        // Delete files when deleting file group
        builder.Entity<FileGroup>()
            .HasMany(f => f.Files)
            .WithOne(f => f.FileGroup)
            .HasForeignKey(uc => uc.FileGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        #region Identity
        // Override identity configuration and table names
        builder.Entity<User>(b => {
            b.ToTable("IdentityUsers");
            b.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId).IsRequired();
        });
        builder.Entity<IdentityUserClaim>(b => b.ToTable("IdentityUserClaims"));
        builder.Entity<IdentityUserLogin<int>>(b => b.ToTable("IdentityUserLogins"));
        builder.Entity<IdentityUserRole<int>>(b => b.ToTable("IdentityUserRoles"));
        builder.Entity<IdentityRole<int>>(b => b.ToTable("IdentityRoles"));


        builder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("IdentityRoleClaims"));
        builder.Entity<IdentityUserToken<int>>(b => b.ToTable("IdentityUserTokens"));
        #endregion

        #region Autolog
        var entities = builder.Model.GetEntityTypes()
            .Where(e => typeof(IHasAutoLog).IsAssignableFrom(e.ClrType)).ToList();
        // Cascade AutoLog User property
        foreach (var relationship in entities.SelectMany(e => e.GetForeignKeys()))
            if (relationship.Properties.Any(a => a.Name == nameof(AutoLogUpdate.UserId))) relationship.DeleteBehavior = DeleteBehavior.SetNull;
        // Prevent auto include auto log
        foreach (var entityType in entities)
            builder.Entity(entityType.ClrType, typeBuilder => {
                typeBuilder.Navigation(nameof(IHasAutoLog.AutoLogUpdates)).AutoInclude(false);
                var tableName = entityType.ClrType.GetSqlTableName(this);
                typeBuilder
                    .OwnsMany(typeof(AutoLogUpdate), nameof(IHasAutoLog.AutoLogUpdates))
                    .ToTable($"{tableName}_AutoLogUpdates");
            });
        #endregion

        SeedData(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.AddInterceptors(new WithNoLockInterceptor());
        optionsBuilder.AddInterceptors(new CollateInterceptor());
    }

    #region OverrideSaveChanges
    public override int SaveChanges(bool acceptAllChangesOnSuccess) {
        return SaveChangesAsync(acceptAllChangesOnSuccess).Result;
    }
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new()) {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        var ret = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ret;
    }


    #endregion

    #region Tracking
    public void DisableTracking() {
        _enabledTracking = false;
        ChangeTracker.StateChanged -= OnEntityStateChanged;
        ChangeTracker.Tracked -= OnEntityTracked;
    }

    private void OnEntityStateChanged(object sender, EntityStateChangedEventArgs e) {
        if (!_enabledTracking) return;
        if (e.NewState != EntityState.Modified) return;
        ProcessChangedEntity(e.Entry);
    }
    private void OnEntityTracked(object sender, EntityTrackedEventArgs e) {
        if (!_enabledTracking) return;
        if (e.FromQuery || e.Entry.State != EntityState.Added) return;
        ProcessChangedEntity(e.Entry);
    }

    private void ProcessChangedEntity(EntityEntry entry) {
        switch (entry.Entity) {
            case IHasAutoLog:
                // Update AutoLog of entity that was changed and implements the interface IHasAutoLog
                var user = _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.User;
                UpdateAutoLog(entry, user);
                break;
        }
    }
    #endregion

    #region AutoLog
    public void UpdateAutoLog(EntityEntry entry, ClaimsPrincipal user) {
        if (entry.Entity is not IHasAutoLog entity) return;
        if (entry.State == EntityState.Added) {
            entity.AutoLogCreate = new AutoLogCreate { CreationTime = DateTime.Now, UserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty) };
            return;
        } else {
            //entry.
        }
        if (entry.State != EntityState.Modified) return;
        entity.AutoLogCreate ??= new AutoLogCreate();
        entity.AutoLogUpdates ??= new List<AutoLogUpdate>();

        // Deletes old items
        var list = entity.AutoLogUpdates?.Where(item => item.Time <= DateTime.Now - AutoLogUpdate.MaxTimeSpan).ToList();
        foreach (var item in list) this.Remove(item);

        CreateAutoLogItem(entry, user);
    }

    private static void CreateAutoLogItem(EntityEntry entry, ClaimsPrincipal user) {
        var entity = (IHasAutoLog)entry.Entity;
        var userId = int.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var name = user?.FindFirst(ClaimTypes.Name)?.Value ?? "Aplicação";

        var autoLog = new AutoLogUpdate {
            Time = DateTime.Now,
            UserId = userId != 0 ? userId : null,
            UserName = name
        };
        entity.AutoLogUpdates.Add(autoLog);
    }
    #endregion

    #region Seed Data

    private static void SeedData(ModelBuilder builder) {
        #region DefaultAdministrator

        builder.Entity<User>().HasData(new User {
            Id = -1,
            UserName = "info@bomsite.com",
            NormalizedUserName = "INFO@BOMSITE.COM",
            Email = "info@bomsite.com",
            NormalizedEmail = "INFO@BOMSITE.COM",
            Name = "Bomsite",
            PasswordHash = "AQAAAAEAACcQAAAAEMGIVnbRp0Jwh2TNZwR2SRnuXxDZ6n+BfbsXhIvWUuBxBa+7nFzYPD++BzjvdKw/Lw==", //Bst2003
            SecurityStamp = "V3A7AYPV5U2QVNZ4H324NRXI42PDZRSW",
            ConcurrencyStamp = "ad8f7307-051b-4939-b137-1e685af77e31",
            EmailConfirmed = true,
            Master = true,
            Active = true,
            CreationTime = new DateTime(1900, 1, 1)
        });
        #endregion
    }
    #endregion

}