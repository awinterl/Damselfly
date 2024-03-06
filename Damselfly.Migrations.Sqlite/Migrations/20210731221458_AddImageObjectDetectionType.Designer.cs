﻿// <auto-generated />
using System;
using Damselfly.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Damselfly.Core.Migrations
{
    [DbContext(typeof(ImageContext))]
    [Migration("20210731221458_AddImageObjectDetectionType")]
    partial class AddImageObjectDetectionType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-preview.6.21352.1");

            modelBuilder.Entity("Damselfly.Core.Models.Basket", b =>
                {
                    b.Property<int>("BasketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("BasketId");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("Damselfly.Core.Models.BasketEntry", b =>
                {
                    b.Property<int>("BasketEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BasketId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT");

                    b.Property<int>("ImageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("BasketEntryId");

                    b.HasIndex("BasketId");

                    b.HasIndex("ImageId")
                        .IsUnique();

                    b.HasIndex("ImageId", "BasketId")
                        .IsUnique();

                    b.ToTable("BasketEntries");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Camera", b =>
                {
                    b.Property<int>("CameraId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Make")
                        .HasColumnType("TEXT");

                    b.Property<string>("Model")
                        .HasColumnType("TEXT");

                    b.Property<string>("Serial")
                        .HasColumnType("TEXT");

                    b.HasKey("CameraId");

                    b.ToTable("Cameras");
                });

            modelBuilder.Entity("Damselfly.Core.Models.CloudTransaction", b =>
                {
                    b.Property<int>("CloudTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransType")
                        .HasColumnType("INTEGER");

                    b.HasKey("CloudTransactionId");

                    b.HasIndex("Date", "TransType");

                    b.ToTable("CloudTransactions");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ConfigSetting", b =>
                {
                    b.Property<int>("ConfigSettingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("ConfigSettingId");

                    b.ToTable("ConfigSettings");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ExifOperation", b =>
                {
                    b.Property<int>("ExifOperationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ImageId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Operation")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ExifOperationId");

                    b.HasIndex("TimeStamp");

                    b.HasIndex("ImageId", "Text");

                    b.ToTable("KeywordOperations");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ExportConfig", b =>
                {
                    b.Property<int>("ExportConfigId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("KeepFolders")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WatermarkText")
                        .HasColumnType("TEXT");

                    b.HasKey("ExportConfigId");

                    b.ToTable("DownloadConfigs");
                });

            modelBuilder.Entity("Damselfly.Core.Models.FTSTag", b =>
                {
                    b.Property<int>("FTSTagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Keyword")
                        .HasColumnType("TEXT");

                    b.HasKey("FTSTagId");

                    b.ToTable("FTSTags");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Folder", b =>
                {
                    b.Property<int>("FolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("FolderScanDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("ParentFolderId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.HasKey("FolderId");

                    b.HasIndex("FolderScanDate");

                    b.HasIndex("Path");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ClassificationId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("ClassificationScore")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("FileCreationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FileLastModDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("FileSizeBytes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FolderId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SortDate")
                        .HasColumnType("TEXT");

                    b.HasKey("ImageId");

                    b.HasIndex("FileLastModDate");

                    b.HasIndex("FileName");

                    b.HasIndex("FolderId");

                    b.HasIndex("LastUpdated");

                    b.HasIndex("SortDate");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageClassification", b =>
                {
                    b.Property<int>("ClassificationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Label")
                        .HasColumnType("TEXT");

                    b.HasKey("ClassificationId");

                    b.HasIndex("Label")
                        .IsUnique();

                    b.ToTable("ImageClassifications");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageMetaData", b =>
                {
                    b.Property<int>("MetaDataId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AILastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("CameraId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Caption")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTaken")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Exposure")
                        .HasColumnType("TEXT");

                    b.Property<string>("FNum")
                        .HasColumnType("TEXT");

                    b.Property<bool>("FlashFired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<int>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ISO")
                        .HasColumnType("TEXT");

                    b.Property<int>("ImageId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("LensId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ThumbLastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<int>("Width")
                        .HasColumnType("INTEGER");

                    b.HasKey("MetaDataId");

                    b.HasIndex("AILastUpdated");

                    b.HasIndex("CameraId");

                    b.HasIndex("DateTaken");

                    b.HasIndex("Hash");

                    b.HasIndex("ImageId")
                        .IsUnique();

                    b.HasIndex("LensId");

                    b.HasIndex("ThumbLastUpdated");

                    b.ToTable("ImageMetaData");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageObject", b =>
                {
                    b.Property<int>("ImageObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ImageId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PersonId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecogntionSource")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RectHeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RectWidth")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RectX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RectY")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Score")
                        .HasColumnType("REAL");

                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("ImageObjectId");

                    b.HasIndex("ImageId");

                    b.HasIndex("PersonId");

                    b.HasIndex("TagId");

                    b.ToTable("ImageObjects");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageTag", b =>
                {
                    b.Property<int>("ImageId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ImageId", "TagId");

                    b.HasIndex("TagId");

                    b.HasIndex("ImageId", "TagId")
                        .IsUnique();

                    b.ToTable("ImageTags");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Lens", b =>
                {
                    b.Property<int>("LensId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Make")
                        .HasColumnType("TEXT");

                    b.Property<string>("Model")
                        .HasColumnType("TEXT");

                    b.Property<string>("Serial")
                        .HasColumnType("TEXT");

                    b.HasKey("LensId");

                    b.ToTable("Lenses");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Person", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AzurePersonId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.HasKey("PersonId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Favourite")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Keyword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TagType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("TagId");

                    b.HasIndex("Keyword")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Damselfly.Core.Models.BasketEntry", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Basket", "Basket")
                        .WithMany("BasketEntries")
                        .HasForeignKey("BasketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Damselfly.Core.Models.Image", "Image")
                        .WithOne("BasketEntry")
                        .HasForeignKey("Damselfly.Core.Models.BasketEntry", "ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Basket");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ExifOperation", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Image", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Folder", "Folder")
                        .WithMany("Images")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageClassification", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Image", null)
                        .WithOne("Classification")
                        .HasForeignKey("Damselfly.Core.Models.ImageClassification", "ClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageMetaData", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Camera", "Camera")
                        .WithMany()
                        .HasForeignKey("CameraId");

                    b.HasOne("Damselfly.Core.Models.Image", "Image")
                        .WithOne("MetaData")
                        .HasForeignKey("Damselfly.Core.Models.ImageMetaData", "ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Damselfly.Core.Models.Lens", "Lens")
                        .WithMany()
                        .HasForeignKey("LensId");

                    b.Navigation("Camera");

                    b.Navigation("Image");

                    b.Navigation("Lens");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageObject", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Image", "Image")
                        .WithMany("ImageObjects")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Damselfly.Core.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId");

                    b.HasOne("Damselfly.Core.Models.Tag", "Tag")
                        .WithMany("ImageObjects")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Person");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Damselfly.Core.Models.ImageTag", b =>
                {
                    b.HasOne("Damselfly.Core.Models.Image", "Image")
                        .WithMany("ImageTags")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Damselfly.Core.Models.Tag", "Tag")
                        .WithMany("ImageTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Basket", b =>
                {
                    b.Navigation("BasketEntries");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Folder", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Image", b =>
                {
                    b.Navigation("BasketEntry");

                    b.Navigation("Classification");

                    b.Navigation("ImageObjects");

                    b.Navigation("ImageTags");

                    b.Navigation("MetaData");
                });

            modelBuilder.Entity("Damselfly.Core.Models.Tag", b =>
                {
                    b.Navigation("ImageObjects");

                    b.Navigation("ImageTags");
                });
#pragma warning restore 612, 618
        }
    }
}
