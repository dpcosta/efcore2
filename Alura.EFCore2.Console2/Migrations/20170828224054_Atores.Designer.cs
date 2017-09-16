using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Alura.EFCore2.Console2;

namespace Alura.EFCore2.Console2.Migrations
{
    [DbContext(typeof(AluraFilmesContext))]
    [Migration("20170828224054_Atores")]
    partial class Atores
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Alura.EFCore2.Console2.Ator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PrimeiroNome");

                    b.Property<string>("UltimoNome");

                    b.HasKey("Id");

                    b.ToTable("AFL_TBL_ATORES","dbo");
                });
        }
    }
}
