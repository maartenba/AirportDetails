﻿// <auto-generated />
using System;
using AirportDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AirportDetails.Migrations
{
    [DbContext(typeof(Database))]
    [Migration("20231115090030_AirlinesCallsignCanBeNull")]
    partial class AirlinesCallsignCanBeNull
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("AirportDetails.Airline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Callsign")
                        .HasColumnType("TEXT");

                    b.Property<int?>("CountryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Iata")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedIata")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("Iata");

                    b.HasIndex("NormalizedIata");

                    b.ToTable("Airlines");
                });

            modelBuilder.Entity("AirportDetails.Airport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int?>("CountryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Iata")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(10,5)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(10,5)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedCity")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedIata")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Timezone")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("City");

                    b.HasIndex("CountryId");

                    b.HasIndex("Iata");

                    b.HasIndex("NormalizedCity");

                    b.HasIndex("NormalizedIata");

                    b.ToTable("Airports");
                });

            modelBuilder.Entity("AirportDetails.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IsoCode")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedIsoCode")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IsoCode");

                    b.HasIndex("NormalizedIsoCode");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("AirportDetails.Airline", b =>
                {
                    b.HasOne("AirportDetails.Country", "Country")
                        .WithMany("Airlines")
                        .HasForeignKey("CountryId");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("AirportDetails.Airport", b =>
                {
                    b.HasOne("AirportDetails.Country", "Country")
                        .WithMany("Airports")
                        .HasForeignKey("CountryId");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("AirportDetails.Country", b =>
                {
                    b.Navigation("Airlines");

                    b.Navigation("Airports");
                });
#pragma warning restore 612, 618
        }
    }
}
