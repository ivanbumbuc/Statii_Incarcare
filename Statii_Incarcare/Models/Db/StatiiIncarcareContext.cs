using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Statii_Incarcare.Models.Db
{
    public partial class StatiiIncarcareContext : DbContext
    {

        public StatiiIncarcareContext(DbContextOptions<StatiiIncarcareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Prize> Prizes { get; set; } = null!;
        public virtual DbSet<Rezervari> Rezervaris { get; set; } = null!;
        public virtual DbSet<Statii> Statiis { get; set; } = null!;
        public virtual DbSet<Tip> Tips { get; set; } = null!;
        public virtual DbSet<Utilizatori> Utilizatoris { get; set; } = null!;

        public virtual DbSet<Administratorii> Administratorii { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            //   optionsBuilder.UseSqlServer("Server=KOKI\\SQLEXPRESS;Database=StatiiIncarcare;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prize>(entity =>
            {
                entity.HasKey(e => e.PrizaId);

                entity.ToTable("Prize");

                entity.HasOne(d => d.Statie)
                    .WithMany(p => p.Prizes)
                    .HasForeignKey(d => d.StatieId)
                    .HasConstraintName("FK_Prize_Statie");

                entity.HasOne(d => d.Tip)
                    .WithMany(p => p.Prizes)
                    .HasForeignKey(d => d.TipId)
                    .HasConstraintName("FK_Prize_Tip");
            });

            modelBuilder.Entity<Rezervari>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK_Booking");

                entity.ToTable("Rezervari");

                entity.Property(e => e.BookingId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.NrMasina)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Priza)
                    .WithMany(p => p.Rezervaris)
                    .HasForeignKey(d => d.PrizaId)
                    .HasConstraintName("FK_Rezervari_Prize");

                entity.HasOne(d => d.Utilizator)
                    .WithMany(p => p.Rezervaris)
                    .HasForeignKey(d => d.UtilizatorId)
                    .HasConstraintName("FK_Rezervari_Utilizatori");
            });

            modelBuilder.Entity<Statii>(entity =>
            {
                entity.HasKey(e => e.StatieId);

                entity.ToTable("Statii");

                entity.Property(e => e.Adresa).HasMaxLength(100);

                entity.Property(e => e.Nume).HasMaxLength(100);

                entity.Property(e => e.Oras)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tip>(entity =>
            {
                entity.ToTable("Tip");

                entity.Property(e => e.Nume).HasMaxLength(100);
            });

            modelBuilder.Entity<Utilizatori>(entity =>
            {
                entity.HasKey(e => e.UtilizatorId)
                    .HasName("PK__Utilizat__73EB10B4686D4353");

                entity.ToTable("Utilizatori");

                entity.Property(e => e.UtilizatorId).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Nume).HasMaxLength(100);

                entity.Property(e => e.Parola).HasMaxLength(100);
            });
            modelBuilder.Entity<Administratorii>(entity =>
            { entity.Property(e => e.UtilizatorId).ValueGeneratedNever(); }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
