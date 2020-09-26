﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Misc.DC.Storage;

namespace Misc.DC.Storage.Migrations
{
    [DbContext(typeof(DcDbContext))]
    [Migration("20200923150006_alterProcess")]
    partial class alterProcess
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Misc.DC.Models.ProcessInfo", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("comName")
                        .HasColumnType("text");

                    b.Property<int>("processId")
                        .HasColumnType("int");

                    b.Property<string>("processName")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("processInfos");
                });

            modelBuilder.Entity("Misc.DC.Models.TempAndHumid", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("addDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<decimal>("value")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("id");

                    b.ToTable("tempAndHumids");
                });
#pragma warning restore 612, 618
        }
    }
}