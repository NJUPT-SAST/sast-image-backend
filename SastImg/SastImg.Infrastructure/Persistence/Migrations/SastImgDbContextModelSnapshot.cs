﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SastImg.Infrastructure.Persistence;

#nullable disable

namespace SastImg.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SastImgDbContext))]
    partial class SastImgDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SastImg.Domain.Album.Album", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Accessibility")
                        .HasColumnType("integer")
                        .HasColumnName("accessibility");

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint")
                        .HasColumnName("author_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("_isRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<string>("_title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_albums");

                    b.ToTable("albums", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain.Album.Images.Image", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("AlbumId")
                        .HasColumnType("bigint")
                        .HasColumnName("album_id");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("_isNsfw")
                        .HasColumnType("boolean")
                        .HasColumnName("is_hidden");

                    b.Property<bool>("_isRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<string>("_title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uri");

                    b.Property<int>("ViewCount")
                        .HasColumnType("integer")
                        .HasColumnName("view_count");

                    b.HasKey("Id")
                        .HasName("pk_images");

                    b.HasIndex("AlbumId")
                        .HasDatabaseName("ix_images_album_id");

                    b.ToTable("images", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain.Categories.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain._tags.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tags");

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain.Album.Album", b =>
                {
                    b.OwnsOne("SastImg.Domain.Album.Cover", "Cover", b1 =>
                        {
                            b1.Property<long>("AlbumId")
                                .HasColumnType("bigint")
                                .HasColumnName("id");

                            b1.Property<long>("ImageId")
                                .HasColumnType("bigint")
                                .HasColumnName("cover_image_id");

                            b1.Property<bool>("IsLatestImage")
                                .HasColumnType("boolean")
                                .HasColumnName("cover_is_latest_image");

                            b1.Property<string>("Url")
                                .HasColumnType("text")
                                .HasColumnName("cover_uri");

                            b1.HasKey("AlbumId");

                            b1.ToTable("albums");

                            b1.WithOwner()
                                .HasForeignKey("AlbumId")
                                .HasConstraintName("fk_albums_albums_id");
                        });

                    b.Navigation("Cover")
                        .IsRequired();
                });

            modelBuilder.Entity("SastImg.Domain.Album.Images.Image", b =>
                {
                    b.HasOne("SastImg.Domain.Album.Album", null)
                        .WithMany("Images")
                        .HasForeignKey("AlbumId")
                        .HasConstraintName("fk_images_albums_album_id");
                });

            modelBuilder.Entity("SastImg.Domain.Album.Album", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
