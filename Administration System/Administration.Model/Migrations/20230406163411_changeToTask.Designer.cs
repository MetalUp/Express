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
    [Migration("20230406163411_changeToTask")]
    partial class changeToTask
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<int>("GroupsId")
                        .HasColumnType("int");

                    b.Property<int>("StudentsId")
                        .HasColumnType("int");

                    b.HasKey("GroupsId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("StudentGroups", (string)null);
                });

            modelBuilder.Entity("HintTask", b =>
                {
                    b.Property<int>("HintsId")
                        .HasColumnType("int");

                    b.Property<int>("TasksId")
                        .HasColumnType("int");

                    b.HasKey("HintsId", "TasksId");

                    b.HasIndex("TasksId");

                    b.ToTable("HintTask");
                });

            modelBuilder.Entity("Model.Types.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ActivityType")
                        .HasColumnType("int");

                    b.Property<int?>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<string>("CodeSubmitted")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HintUsed")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssignedById")
                        .HasColumnType("int");

                    b.Property<int>("AssignedToId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DueBy")
                        .HasColumnType("datetime2");

                    b.Property<int>("Marks")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TeacherNotes")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssignedById");

                    b.HasIndex("AssignedToId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("Model.Types.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("ContentType")
                        .HasColumnType("int");

                    b.Property<string>("LanguageId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UniqueRef")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("LanguageId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Model.Types.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("FileId")
                        .HasColumnType("int");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("Hint");
                });

            modelBuilder.Entity("Model.Types.Language", b =>
                {
                    b.Property<string>("LanguageID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CSSstyle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompileArguments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompilerLanguageId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("HelpersFileId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RegExRulesFileId")
                        .HasColumnType("int");

                    b.Property<int?>("WrapperFileId")
                        .HasColumnType("int");

                    b.HasKey("LanguageID");

                    b.HasIndex("HelpersFileId");

                    b.HasIndex("RegExRulesFileId");

                    b.HasIndex("WrapperFileId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Model.Types.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Model.Types.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int?>("CommonHiddenCodeFileId")
                        .HasColumnType("int");

                    b.Property<int?>("CommonTestsFileId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LanguageId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("RegExRulesFileId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("WrapperFileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommonHiddenCodeFileId");

                    b.HasIndex("CommonTestsFileId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("RegExRulesFileId");

                    b.HasIndex("WrapperFileId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("DescriptionFileId")
                        .HasColumnType("int");

                    b.Property<int?>("HiddenCodeFileId")
                        .HasColumnType("int");

                    b.Property<bool>("NextTaskClearsFunctions")
                        .HasColumnType("bit");

                    b.Property<int?>("NextTaskId")
                        .HasColumnType("int");

                    b.Property<int?>("Number")
                        .HasColumnType("int")
                        .HasColumnName("Number");

                    b.Property<int?>("PreviousTaskId")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("RegExRulesFileId")
                        .HasColumnType("int");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TestsFileId")
                        .HasColumnType("int");

                    b.Property<bool>("TestsRunOnClient")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("WrapperFileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DescriptionFileId");

                    b.HasIndex("HiddenCodeFileId");

                    b.HasIndex("NextTaskId");

                    b.HasIndex("PreviousTaskId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RegExRulesFileId");

                    b.HasIndex("TestsFileId");

                    b.HasIndex("WrapperFileId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Model.Types.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InvitationCode")
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

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("Model.Types.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Types.User", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HintTask", b =>
                {
                    b.HasOne("Model.Types.Hint", null)
                        .WithMany()
                        .HasForeignKey("HintsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Types.Task", null)
                        .WithMany()
                        .HasForeignKey("TasksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

                    b.HasOne("Model.Types.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedBy");

                    b.Navigation("AssignedTo");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Model.Types.File", b =>
                {
                    b.HasOne("Model.Types.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Types.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Author");

                    b.Navigation("Language");
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
                    b.HasOne("Model.Types.File", "File")
                        .WithMany()
                        .HasForeignKey("FileId");

                    b.Navigation("File");
                });

            modelBuilder.Entity("Model.Types.Language", b =>
                {
                    b.HasOne("Model.Types.File", "HelpersFile")
                        .WithMany()
                        .HasForeignKey("HelpersFileId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.File", "RegExRulesFile")
                        .WithMany()
                        .HasForeignKey("RegExRulesFileId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.File", "WrapperFile")
                        .WithMany()
                        .HasForeignKey("WrapperFileId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("HelpersFile");

                    b.Navigation("RegExRulesFile");

                    b.Navigation("WrapperFile");
                });

            modelBuilder.Entity("Model.Types.Project", b =>
                {
                    b.HasOne("Model.Types.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Types.File", "CommonHiddenCodeFile")
                        .WithMany()
                        .HasForeignKey("CommonHiddenCodeFileId");

                    b.HasOne("Model.Types.File", "CommonTestsFile")
                        .WithMany()
                        .HasForeignKey("CommonTestsFileId");

                    b.HasOne("Model.Types.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.File", "RegExRulesFile")
                        .WithMany()
                        .HasForeignKey("RegExRulesFileId");

                    b.HasOne("Model.Types.File", "WrapperFile")
                        .WithMany()
                        .HasForeignKey("WrapperFileId");

                    b.Navigation("Author");

                    b.Navigation("CommonHiddenCodeFile");

                    b.Navigation("CommonTestsFile");

                    b.Navigation("Language");

                    b.Navigation("RegExRulesFile");

                    b.Navigation("WrapperFile");
                });

            modelBuilder.Entity("Model.Types.Task", b =>
                {
                    b.HasOne("Model.Types.File", "DescriptionFile")
                        .WithMany()
                        .HasForeignKey("DescriptionFileId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.File", "HiddenCodeFile")
                        .WithMany()
                        .HasForeignKey("HiddenCodeFileId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.Task", "NextTask")
                        .WithMany()
                        .HasForeignKey("NextTaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.Task", "PreviousTask")
                        .WithMany()
                        .HasForeignKey("PreviousTaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.Project", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId");

                    b.HasOne("Model.Types.File", "RegExRulesFile")
                        .WithMany()
                        .HasForeignKey("RegExRulesFileId");

                    b.HasOne("Model.Types.File", "TestsFile")
                        .WithMany()
                        .HasForeignKey("TestsFileId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Model.Types.File", "WrapperFile")
                        .WithMany()
                        .HasForeignKey("WrapperFileId");

                    b.Navigation("DescriptionFile");

                    b.Navigation("HiddenCodeFile");

                    b.Navigation("NextTask");

                    b.Navigation("PreviousTask");

                    b.Navigation("Project");

                    b.Navigation("RegExRulesFile");

                    b.Navigation("TestsFile");

                    b.Navigation("WrapperFile");
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

            modelBuilder.Entity("Model.Types.Project", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
