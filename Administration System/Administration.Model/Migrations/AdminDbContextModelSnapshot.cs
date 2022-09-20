﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;

#nullable disable

namespace Model.Migrations
{
    [DbContext(typeof(AdminDbContext))]
    partial class AdminDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Model.Types.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

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

                    b.Property<int>("Marks")
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

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("Model.Types.Hint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CostInMarks")
                        .HasColumnType("int");

                    b.Property<string>("HtmlFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Hint");
                });

            modelBuilder.Entity("Model.Types.Invitation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("InviteeId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Sent")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("InviteeId");

                    b.HasIndex("SenderId");

                    b.ToTable("Invitation");
                });

            modelBuilder.Entity("Model.Types.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Model.Types.StudentGroup", b =>
                {
                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.HasKey("StudentId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("StudentGroups");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<byte[]>("DescriptionContent")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("DescriptionMime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Language")
                        .HasColumnType("int");

                    b.Property<int>("MaxMarks")
                        .HasColumnType("int");

                    b.Property<bool>("NextTaskClearsFunctions")
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

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Tests")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

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

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Model.Types.Activity", b =>
                {
                    b.HasOne("Model.Types.Assignment", "Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");
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
                        .WithMany("Groups")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Model.Types.Hint", b =>
                {
                    b.HasOne("Model.Types.Task", "Task")
                        .WithMany("Hints")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Model.Types.Invitation", b =>
                {
                    b.HasOne("Model.Types.User", "Invitee")
                        .WithMany()
                        .HasForeignKey("InviteeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Model.Types.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Invitee");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Model.Types.StudentGroup", b =>
                {
                    b.HasOne("Model.Types.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Model.Types.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.HasOne("Model.Types.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Types.Task", "NextTask")
                        .WithMany()
                        .HasForeignKey("NextTaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.Task", "PreviousTask")
                        .WithMany()
                        .HasForeignKey("PreviousTaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Author");

                    b.Navigation("NextTask");

                    b.Navigation("PreviousTask");
                });

            modelBuilder.Entity("Model.Types.User", b =>
                {
                    b.HasOne("Model.Types.Organisation", "Organisation")
                        .WithMany("Teachers")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Model.Types.Organisation", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("Teachers");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.Navigation("Hints");
                });
#pragma warning restore 612, 618
        }
    }
}
