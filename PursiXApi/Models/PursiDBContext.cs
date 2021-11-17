using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PursiXApi.Models
{
    public partial class PursiDBContext : DbContext
    {
        public PursiDBContext()
        {
        }

        public PursiDBContext(DbContextOptions<PursiDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EventParticipations> EventParticipations { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventParticipations>(entity =>
            {
                entity.HasKey(e => e.ParticipationId);

                entity.Property(e => e.ParticipationId).HasColumnName("ParticipationID");

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.LoginId).HasColumnName("LoginID");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventParticipations)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventParticipations_Events");

                entity.HasOne(d => d.Login)
                    .WithMany(p => p.EventParticipations)
                    .HasForeignKey(d => d.LoginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventParticipations_Login");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.EventDateTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.Property(e => e.LoginId).HasColumnName("LoginID");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.Property(e => e.UserInfoId).HasColumnName("UserInfoID");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.LoginId).HasColumnName("LoginID");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.PostalCode).HasMaxLength(5);

                entity.HasOne(d => d.Login)
                    .WithMany(p => p.UserInfo)
                    .HasForeignKey(d => d.LoginId)
                    .HasConstraintName("FK_UserInfo_Login");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
