using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LocalSportsLeague.Models
{
    public partial class Team112DBContext : DbContext
    {
        public Team112DBContext()
        {
        }

        public Team112DBContext(DbContextOptions<Team112DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Coach> Coach { get; set; }
        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<Official> Official { get; set; }
        public virtual DbSet<Season> Season { get; set; }
        public virtual DbSet<SeasonSport> SeasonSport { get; set; }
        public virtual DbSet<SeasonTeam> SeasonTeam { get; set; }
        public virtual DbSet<Sport> Sport { get; set; }
        public virtual DbSet<Team> Team { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=buscissql1601\\cisweb;Database=Team112DB;User ID=graphics;Password=stands;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Coach>(entity =>
            {
                entity.ToTable("coach");

                entity.Property(e => e.Coachid).HasColumnName("coachid");

                entity.Property(e => e.Fname)
                    .IsRequired()
                    .HasColumnName("fname")
                    .HasMaxLength(30);

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("lname")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("game");

                entity.HasIndex(e => e.Datetime)
                    .HasName("IX_game")
                    .IsUnique();

                entity.Property(e => e.Gameid).HasColumnName("gameid");

                entity.Property(e => e.Ascore).HasColumnName("ascore");

                entity.Property(e => e.Ateamid).HasColumnName("ateamid");

                entity.Property(e => e.Datetime)
                    .HasColumnName("datetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Hscore).HasColumnName("hscore");

                entity.Property(e => e.Hteamid).HasColumnName("hteamid");

                entity.Property(e => e.Officialid).HasColumnName("officialid");

                entity.Property(e => e.Ot).HasColumnName("ot");

                entity.Property(e => e.Seasonid).HasColumnName("seasonid");

                entity.Property(e => e.Sportid).HasColumnName("sportid");

                entity.Property(e => e.Winner).HasColumnName("winner");

                entity.HasOne(d => d.Ateam)
                    .WithMany(p => p.GameAteam)
                    .HasForeignKey(d => d.Ateamid)
                    .HasConstraintName("FK_ateam");

                entity.HasOne(d => d.Hteam)
                    .WithMany(p => p.GameHteam)
                    .HasForeignKey(d => d.Hteamid)
                    .HasConstraintName("FK_hteam");

                entity.HasOne(d => d.Official)
                    .WithMany(p => p.Game)
                    .HasForeignKey(d => d.Officialid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_game_official");

                entity.HasOne(d => d.Season)
                    .WithMany(p => p.Game)
                    .HasForeignKey(d => d.Seasonid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_game_season");

                entity.HasOne(d => d.WinnerNavigation)
                    .WithMany(p => p.GameWinnerNavigation)
                    .HasForeignKey(d => d.Winner)
                    .HasConstraintName("FK_winner");
            });

            modelBuilder.Entity<Official>(entity =>
            {
                entity.ToTable("official");

                entity.Property(e => e.Officialid).HasColumnName("officialid");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Fname)
                    .IsRequired()
                    .HasColumnName("fname")
                    .HasMaxLength(30);

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("lname")
                    .HasMaxLength(30);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.ToTable("season");

                entity.Property(e => e.Seasonid).HasColumnName("seasonid");

                entity.Property(e => e.Edate)
                    .HasColumnName("edate")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sdate)
                    .HasColumnName("sdate")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<SeasonSport>(entity =>
            {
                entity.ToTable("season_sport");

                entity.Property(e => e.SeasonSportId).HasColumnName("season_sport_id");

                entity.Property(e => e.Seasonid).HasColumnName("seasonid");

                entity.Property(e => e.Sportid).HasColumnName("sportid");

                entity.HasOne(d => d.Season)
                    .WithMany(p => p.SeasonSport)
                    .HasForeignKey(d => d.Seasonid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__season_sp__seaso__73BA3083");

                entity.HasOne(d => d.Sport)
                    .WithMany(p => p.SeasonSport)
                    .HasForeignKey(d => d.Sportid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__season_sp__sport__74AE54BC");
            });

            modelBuilder.Entity<SeasonTeam>(entity =>
            {
                entity.ToTable("season_team");

                entity.Property(e => e.SeasonTeamId)
                    .HasColumnName("season_team_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Seasonid).HasColumnName("seasonid");

                entity.Property(e => e.Teamid).HasColumnName("teamid");

                entity.HasOne(d => d.Season)
                    .WithMany(p => p.SeasonTeam)
                    .HasForeignKey(d => d.Seasonid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__season_te__seaso__6FE99F9F");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.SeasonTeam)
                    .HasForeignKey(d => d.Teamid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__season_te__teami__70DDC3D8");
            });

            modelBuilder.Entity<Sport>(entity =>
            {
                entity.ToTable("sport");

                entity.Property(e => e.Sportid).HasColumnName("sportid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team");

                entity.Property(e => e.Teamid).HasColumnName("teamid");

                entity.Property(e => e.Coachid).HasColumnName("coachid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sportid).HasColumnName("sportid");

                entity.HasOne(d => d.Coach)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.Coachid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_team_coach");

                entity.HasOne(d => d.Sport)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.Sportid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_team_sport");
            });
        }
    }
}
