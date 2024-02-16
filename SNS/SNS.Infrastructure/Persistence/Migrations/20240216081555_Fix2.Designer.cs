﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SNS.Infrastructure.Persistence;

#nullable disable

namespace SNS.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SNSDbContext))]
    [Migration("20240216081555_Fix2")]
    partial class Fix2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SNS.Domain.AlbumEntity.Album", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("_authorId")
                        .HasColumnType("bigint")
                        .HasColumnName("author_id");

                    b.HasKey("Id")
                        .HasName("pk_albums");

                    b.HasIndex("_authorId")
                        .HasDatabaseName("ix_albums_author_id");

                    b.ToTable("albums", (string)null);
                });

            modelBuilder.Entity("SNS.Domain.AlbumEntity.Subscriber", b =>
                {
                    b.Property<long>("SubscriberId")
                        .HasColumnType("bigint")
                        .HasColumnName("subscriber_id");

                    b.Property<long>("AlbumId")
                        .HasColumnType("bigint")
                        .HasColumnName("album_id");

                    b.HasKey("SubscriberId")
                        .HasName("pk_subscribers");

                    b.HasIndex("AlbumId")
                        .HasDatabaseName("ix_subscribers_album_id");

                    b.ToTable("subscribers", (string)null);
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Favourite", b =>
                {
                    b.Property<long>("ImageId")
                        .HasColumnType("bigint")
                        .HasColumnName("image_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("favouriter_id");

                    b.HasKey("ImageId", "UserId")
                        .HasName("pk_favourites");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_favourites_user_id");

                    b.ToTable("favourites", (string)null);
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Image", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("_albumId")
                        .HasColumnType("bigint")
                        .HasColumnName("album_id");

                    b.Property<long>("_authorId")
                        .HasColumnType("bigint")
                        .HasColumnName("author_id");

                    b.HasKey("Id")
                        .HasName("pk_images");

                    b.HasIndex("_albumId")
                        .HasDatabaseName("ix_images__album_id");

                    b.HasIndex("_authorId")
                        .HasDatabaseName("ix_images_author_id");

                    b.ToTable("images", (string)null);
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Like", b =>
                {
                    b.Property<long>("ImageId")
                        .HasColumnType("bigint")
                        .HasColumnName("image_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("liker_id");

                    b.HasKey("ImageId", "UserId")
                        .HasName("pk_likes");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_likes_user_id");

                    b.ToTable("likes", (string)null);
                });

            modelBuilder.Entity("SNS.Domain.UserEntity.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("_avatar")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("avatar");

                    b.Property<string>("_biography")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("biography");

                    b.Property<string>("_header")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("header");

                    b.Property<string>("_nickname")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nickname");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("UserUser", b =>
                {
                    b.Property<long>("follower")
                        .HasColumnType("bigint")
                        .HasColumnName("follower");

                    b.Property<long>("following")
                        .HasColumnType("bigint")
                        .HasColumnName("following");

                    b.HasKey("follower", "following")
                        .HasName("pk_followers");

                    b.HasIndex("following")
                        .HasDatabaseName("ix_followers_following");

                    b.ToTable("followers", (string)null);
                });

            modelBuilder.Entity("SNS.Domain.AlbumEntity.Album", b =>
                {
                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("_authorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_albums_users_author_id");
                });

            modelBuilder.Entity("SNS.Domain.AlbumEntity.Subscriber", b =>
                {
                    b.HasOne("SNS.Domain.AlbumEntity.Album", null)
                        .WithMany("_subscribers")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_subscribers_albums_album_id");

                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_subscribers_users_subscriber_id");
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Favourite", b =>
                {
                    b.HasOne("SNS.Domain.ImageAggregate.ImageEntity.Image", null)
                        .WithMany("_favouritedBy")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_favourites_images_image_id");

                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_favourites_users_favouriter_id");
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Image", b =>
                {
                    b.HasOne("SNS.Domain.AlbumEntity.Album", null)
                        .WithMany()
                        .HasForeignKey("_albumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_images_albums__album_id");

                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("_authorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_images_users_author_id");

                    b.OwnsMany("SNS.Domain.ImageAggregate.CommentEntity.Comment", "_comments", b1 =>
                        {
                            b1.Property<long>("Id")
                                .HasColumnType("bigint")
                                .HasColumnName("id");

                            b1.Property<long?>("_authorId")
                                .HasColumnType("bigint")
                                .HasColumnName("_author_id");

                            b1.Property<DateTime>("_commentAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("comment_at");

                            b1.Property<long>("_commenter")
                                .HasColumnType("bigint")
                                .HasColumnName("commenter");

                            b1.Property<string>("content")
                                .HasColumnType("text")
                                .HasColumnName("content");

                            b1.Property<long>("image_id")
                                .HasColumnType("bigint")
                                .HasColumnName("image_id");

                            b1.HasKey("Id")
                                .HasName("pk_comments");

                            b1.HasIndex("_authorId")
                                .HasDatabaseName("ix_comments__author_id");

                            b1.HasIndex("image_id")
                                .HasDatabaseName("ix_comments_image_id");

                            b1.ToTable("comments", (string)null);

                            b1.HasOne("SNS.Domain.UserEntity.User", null)
                                .WithMany()
                                .HasForeignKey("_authorId")
                                .HasConstraintName("fk_comments_users__author_id");

                            b1.WithOwner()
                                .HasForeignKey("image_id")
                                .HasConstraintName("fk_comments_images_image_id");
                        });

                    b.Navigation("_comments");
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Like", b =>
                {
                    b.HasOne("SNS.Domain.ImageAggregate.ImageEntity.Image", null)
                        .WithMany("_likedBy")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_likes_images_image_id");

                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_likes_users_liker_id");
                });

            modelBuilder.Entity("UserUser", b =>
                {
                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("follower")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_followers_users_follower");

                    b.HasOne("SNS.Domain.UserEntity.User", null)
                        .WithMany()
                        .HasForeignKey("following")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_followers_users_following");
                });

            modelBuilder.Entity("SNS.Domain.AlbumEntity.Album", b =>
                {
                    b.Navigation("_subscribers");
                });

            modelBuilder.Entity("SNS.Domain.ImageAggregate.ImageEntity.Image", b =>
                {
                    b.Navigation("_favouritedBy");

                    b.Navigation("_likedBy");
                });
#pragma warning restore 612, 618
        }
    }
}
