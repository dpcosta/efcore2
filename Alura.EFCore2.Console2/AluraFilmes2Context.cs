using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alura.EFCore2.Console2
{
    public partial class AluraFilmes2Context : DbContext
    {
        public virtual DbSet<Ator> Atores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AluraFilmes2;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ator>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK__AFL_TBL___F2ADC59ED85C6CD1");

                entity.ToTable("AFL_TBL_ATORES");

                entity.HasIndex(e => e.UltimoNome)
                    .HasName("IDX_AFL_TBL_ATORES_TX_ULTIMO_NOME");

                entity.Property(e => e.Id).HasColumnName("IN_ID_AUTOR");

                //entity.Property(e => e.UltimaAtualizacao)
                //    .HasColumnName("DT_ULTIMA_ATUALIZACAO")
                //    .HasColumnType("datetime")
                //    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.PrimeiroNome)
                    .IsRequired()
                    .HasColumnName("TX_PRIMEIRO_NOME")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.UltimoNome)
                    .IsRequired()
                    .HasColumnName("TX_ULTIMO_NOME")
                    .HasColumnType("varchar(45)");
            });
        }
    }
}