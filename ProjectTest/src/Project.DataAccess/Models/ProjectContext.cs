using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Project.DataAccess.Models;

public partial class ProjectContext : IdentityDbContext<Users>
{

    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BarangRequest> BarangRequests { get; set; }

    public virtual DbSet<MasterBarang> MasterBarangs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BarangRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BarangRe__3214EC07CC76B084");

            entity.Property(e => e.NamaBarang)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NamaDivisi)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('Pending')");

            entity.HasOne(d => d.KodeBarangNavigation).WithMany(p => p.BarangRequests)
                .HasForeignKey(d => d.KodeBarang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BarangReq__KodeB__534D60F1");
        });

        modelBuilder.Entity<MasterBarang>(entity =>
        {
            entity.HasKey(e => e.KodeBarang).HasName("PK__MasterBa__BFE6592B3B32566B");

            entity.ToTable("MasterBarang");

            entity.Property(e => e.Harga)
                .HasDefaultValueSql("((0))")
                .HasColumnType("money");
            entity.Property(e => e.NamaBarang)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NamaSupplier)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StokBarang).HasDefaultValueSql("((0))");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
