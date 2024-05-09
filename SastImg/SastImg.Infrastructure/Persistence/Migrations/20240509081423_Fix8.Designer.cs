﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SastImg.Infrastructure.Persistence;

#nullable disable

namespace SastImg.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SastImgDbContext))]
    [Migration("20240509081423_Fix8")]
    partial class Fix8
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SastImg.Domain.AlbumAggregate.AlbumEntity.Album", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<int>("_accessibility")
                        .HasColumnType("integer")
                        .HasColumnName("accessibility");

                    b.Property<long>("_authorId")
                        .HasColumnType("bigint")
                        .HasColumnName("author_id");

                    b.Property<long>("_categoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<long[]>("_collaborators")
                        .IsRequired()
                        .HasColumnType("bigint[]")
                        .HasColumnName("collaborators");

                    b.Property<DateTime>("_createdAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("_description")
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

                    b.Property<DateTime>("_updatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_albums");

                    b.HasIndex("_categoryId")
                        .HasDatabaseName("ix_albums_category_id");

                    b.ToTable("albums", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain.CategoryEntity.Category", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("_description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("_name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.HasIndex("_name")
                        .IsUnique()
                        .HasDatabaseName("ix_categories__name");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain.TagEntity.Tag", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("_name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tags");

                    b.HasIndex("_name")
                        .IsUnique()
                        .HasDatabaseName("ix_tags__name");

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("SastImg.Domain.AlbumAggregate.AlbumEntity.Album", b =>
                {
                    b.HasOne("SastImg.Domain.CategoryEntity.Category", null)
                        .WithMany()
                        .HasForeignKey("_categoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_albums_categories_category_id");

                    b.OwnsOne("SastImg.Domain.AlbumAggregate.AlbumEntity.Cover", "_cover", b1 =>
                        {
                            b1.Property<long>("AlbumId")
                                .HasColumnType("bigint")
                                .HasColumnName("id");

                            b1.Property<bool>("IsLatestImage")
                                .HasColumnType("boolean")
                                .HasColumnName("cover_is_latest_image");

                            b1.Property<string>("Url")
                                .HasColumnType("text")
                                .HasColumnName("cover_url");

                            b1.HasKey("AlbumId");

                            b1.ToTable("albums");

                            b1.WithOwner()
                                .HasForeignKey("AlbumId")
                                .HasConstraintName("fk_albums_albums_id");
                        });

                    b.OwnsMany("SastImg.Domain.AlbumAggregate.ImageEntity.Image", "_images", b1 =>
                        {
                            b1.Property<long>("Id")
                                .HasColumnType("bigint")
                                .HasColumnName("id");

                            b1.Property<string>("_description")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("description");

                            b1.Property<bool>("_isRemoved")
                                .HasColumnType("boolean")
                                .HasColumnName("is_removed");

                            b1.Property<long[]>("_tags")
                                .IsRequired()
                                .HasColumnType("bigint[]")
                                .HasColumnName("tags");

                            b1.Property<string>("_title")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("title");

                            b1.Property<DateTime>("_uploadtedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("uploaded_at");

                            b1.Property<long>("album_id")
                                .HasColumnType("bigint")
                                .HasColumnName("album_id");

                            b1.HasKey("Id")
                                .HasName("pk_images");

                            b1.HasIndex("album_id")
                                .HasDatabaseName("ix_images_album_id");

                            b1.ToTable("images", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("album_id")
                                .HasConstraintName("fk_images_albums_album_id");

                            b1.OwnsOne("SastImg.Domain.AlbumAggregate.ImageEntity.ImageUrl", "_url", b2 =>
                                {
                                    b2.Property<long>("ImageId")
                                        .HasColumnType("bigint")
                                        .HasColumnName("id");

                                    b2.Property<string>("Original")
                                        .IsRequired()
                                        .HasColumnType("text")
                                        .HasColumnName("url");

                                    b2.Property<string>("Thumbnail")
                                        .IsRequired()
                                        .HasColumnType("text")
                                        .HasColumnName("thumbnail_url");

                                    b2.HasKey("ImageId");

                                    b2.ToTable("images");

                                    b2.WithOwner()
                                        .HasForeignKey("ImageId")
                                        .HasConstraintName("fk_images_images_id");
                                });

                            b1.Navigation("_url");
                        });

                    b.Navigation("_cover");

                    b.Navigation("_images");
                });
#pragma warning restore 612, 618
        }
    }
}
