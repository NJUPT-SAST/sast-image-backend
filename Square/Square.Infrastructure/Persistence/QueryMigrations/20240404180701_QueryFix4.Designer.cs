﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Square.Infrastructure.Persistence;

#nullable disable

namespace Square.Infrastructure.Persistence.QueryMigrations
{
    [DbContext(typeof(SquareQueryDbContext))]
    [Migration("20240404180701_QueryFix4")]
    partial class QueryFix4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("query")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Square.Application.CategoryServices.CategoryModel", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_categories_name");

                    b.ToTable("categories", "query");
                });

            modelBuilder.Entity("Square.Application.ColumnServices.Models.ColumnModel", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint")
                        .HasColumnName("author_id");

                    b.Property<DateTime>("PublishedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("published_at");

                    b.Property<long>("TopicId")
                        .HasColumnType("bigint")
                        .HasColumnName("topic_id");

                    b.ComplexProperty<Dictionary<string, object>>("Text", "Square.Application.ColumnServices.Models.ColumnModel.Text#ColumnText", b1 =>
                        {
                            b1.Property<string>("Value")
                                .HasColumnType("text")
                                .HasColumnName("text");
                        });

                    b.HasKey("Id")
                        .HasName("pk_columns");

                    b.HasIndex("TopicId")
                        .HasDatabaseName("ix_columns_topic_id");

                    b.ToTable("columns", "query");
                });

            modelBuilder.Entity("Square.Application.TopicServices.TopicModel", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("AuthorId")
                        .HasColumnType("bigint")
                        .HasColumnName("author_id");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime>("PublishedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("published_at");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_topics");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_topics_category_id");

                    b.ToTable("topics", "query");
                });

            modelBuilder.Entity("Square.Application.ColumnServices.Models.ColumnModel", b =>
                {
                    b.HasOne("Square.Application.TopicServices.TopicModel", null)
                        .WithMany("Columns")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_columns_topics_topic_id");

                    b.OwnsMany("Square.Domain.ColumnAggregate.ColumnEntity.ColumnLike", "Likes", b1 =>
                        {
                            b1.Property<long>("ColumnId")
                                .HasColumnType("bigint")
                                .HasColumnName("column_id");

                            b1.Property<long>("UserId")
                                .HasColumnType("bigint")
                                .HasColumnName("user_id");

                            b1.HasKey("ColumnId", "UserId")
                                .HasName("pk_column_likes");

                            b1.ToTable("column_likes", "query");

                            b1.WithOwner()
                                .HasForeignKey("ColumnId")
                                .HasConstraintName("fk_column_likes_columns_column_id");
                        });

                    b.OwnsMany("Square.Application.ColumnServices.Models.ColumnImage", "Images", b1 =>
                        {
                            b1.Property<int>("id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("id"));

                            b1.Property<string>("ThumbnailUrl")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("thumbnail_url");

                            b1.Property<string>("Url")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("url");

                            b1.Property<long>("column_id")
                                .HasColumnType("bigint")
                                .HasColumnName("column_id");

                            b1.HasKey("id")
                                .HasName("pk_column_images");

                            b1.HasIndex("column_id")
                                .HasDatabaseName("ix_column_images_column_id");

                            b1.ToTable("column_images", "query");

                            b1.WithOwner()
                                .HasForeignKey("column_id")
                                .HasConstraintName("fk_column_images_columns_column_id");
                        });

                    b.Navigation("Images");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Square.Application.TopicServices.TopicModel", b =>
                {
                    b.HasOne("Square.Application.CategoryServices.CategoryModel", null)
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_topics_categories_category_id");

                    b.OwnsMany("Square.Domain.TopicAggregate.TopicEntity.TopicSubscribe", "Subscribes", b1 =>
                        {
                            b1.Property<long>("UserId")
                                .HasColumnType("bigint")
                                .HasColumnName("user_id");

                            b1.Property<long>("TopicId")
                                .HasColumnType("bigint")
                                .HasColumnName("topic_id");

                            b1.HasKey("UserId", "TopicId")
                                .HasName("pk_topic_subscribes");

                            b1.HasIndex("TopicId")
                                .HasDatabaseName("ix_topic_subscribes_topic_id");

                            b1.ToTable("topic_subscribes", "query");

                            b1.WithOwner()
                                .HasForeignKey("TopicId")
                                .HasConstraintName("fk_topic_subscribes_topics_topic_id");
                        });

                    b.Navigation("Subscribes");
                });

            modelBuilder.Entity("Square.Application.TopicServices.TopicModel", b =>
                {
                    b.Navigation("Columns");
                });
#pragma warning restore 612, 618
        }
    }
}
