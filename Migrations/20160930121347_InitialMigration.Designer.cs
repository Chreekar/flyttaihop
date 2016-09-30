using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Flyttaihop.Framework.Database;

namespace flyttaihop.Migrations
{
    [DbContext(typeof(CriteriaContext))]
    [Migration("20160930121347_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("Flyttaihop.Framework.Models.Criteria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Criterias");
                });

            modelBuilder.Entity("Flyttaihop.Framework.Models.Duration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CriteriaId");

                    b.Property<int>("Minutes");

                    b.Property<string>("Target");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CriteriaId");

                    b.ToTable("Durations");
                });

            modelBuilder.Entity("Flyttaihop.Framework.Models.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CriteriaId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("CriteriaId");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("Flyttaihop.Framework.Models.Duration", b =>
                {
                    b.HasOne("Flyttaihop.Framework.Models.Criteria")
                        .WithMany("DurationCriterias")
                        .HasForeignKey("CriteriaId");
                });

            modelBuilder.Entity("Flyttaihop.Framework.Models.Keyword", b =>
                {
                    b.HasOne("Flyttaihop.Framework.Models.Criteria")
                        .WithMany("Keywords")
                        .HasForeignKey("CriteriaId");
                });
        }
    }
}
