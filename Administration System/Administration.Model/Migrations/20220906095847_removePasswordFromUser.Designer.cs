﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;

#nullable disable

namespace Model.Migrations
{
    [DbContext(typeof(AdminDbContext))]
    [Migration("20220906095847_removePasswordFromUser")]
    partial class removePasswordFromUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Model.Types.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("Model.Types.Assignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AssignedById")
                        .HasColumnType("int");

                    b.Property<int>("AssignedToId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DueBy")
                        .HasColumnType("datetime2");

                    b.Property<int>("MarksAwarded")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssignedById");

                    b.HasIndex("AssignedToId");

                    b.HasIndex("TaskId");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("Model.Types.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Model.Types.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AsRole")
                        .HasColumnType("int");

                    b.Property<int>("FromUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("IssuedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("ToJoinId")
                        .HasColumnType("int");

                    b.Property<string>("ToUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ValidForDays")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToJoinId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("Model.Types.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HintCosts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hints")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Language")
                        .HasColumnType("int");

                    b.Property<int?>("MaxMarks")
                        .HasColumnType("int");

                    b.Property<bool>("NextTaskDoesNotClearFunctions")
                        .HasColumnType("bit");

                    b.Property<int?>("NextTaskId")
                        .HasColumnType("int");

                    b.Property<bool>("PasteExpression")
                        .HasColumnType("bit");

                    b.Property<bool>("PasteFunctions")
                        .HasColumnType("bit");

                    b.Property<int?>("PreviousTaskId")
                        .HasColumnType("int");

                    b.Property<string>("ReadyMadeFunctions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tests")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NextTaskId");

                    b.HasIndex("PreviousTaskId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Model.Types.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<int?>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<int?>("PreferredLanguage")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Model.Types.Activity", b =>
                {
                    b.HasOne("Model.Types.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Model.Types.Assignment", b =>
                {
                    b.HasOne("Model.Types.User", "AssignedBy")
                        .WithMany()
                        .HasForeignKey("AssignedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Model.Types.User", "AssignedTo")
                        .WithMany()
                        .HasForeignKey("AssignedToId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Model.Types.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedBy");

                    b.Navigation("AssignedTo");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Model.Types.Group", b =>
                {
                    b.HasOne("Model.Types.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Model.Types.Invitation", b =>
                {
                    b.HasOne("Model.Types.User", "FromUser")
                        .WithMany()
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Model.Types.Organisation", "ToJoin")
                        .WithMany()
                        .HasForeignKey("ToJoinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromUser");

                    b.Navigation("ToJoin");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.HasOne("Model.Types.Task", "NextTask")
                        .WithMany()
                        .HasForeignKey("NextTaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.Task", "PreviousTask")
                        .WithMany()
                        .HasForeignKey("PreviousTaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("NextTask");

                    b.Navigation("PreviousTask");
                });

            modelBuilder.Entity("Model.Types.User", b =>
                {
                    b.HasOne("Model.Types.Group", null)
                        .WithMany("Students")
                        .HasForeignKey("GroupId");

                    b.HasOne("Model.Types.Organisation", "Organisation")
                        .WithMany("Teachers")
                        .HasForeignKey("OrganisationId");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Model.Types.Group", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("Model.Types.Organisation", b =>
                {
                    b.Navigation("Teachers");
                });
#pragma warning restore 612, 618
        }
    }
}
