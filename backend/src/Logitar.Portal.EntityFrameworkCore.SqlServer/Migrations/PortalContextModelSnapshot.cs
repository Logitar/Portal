﻿// <auto-generated />
using System;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(PortalContext))]
    partial class PortalContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.DictionaryEntity", b =>
                {
                    b.Property<int>("DictionaryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DictionaryId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Entries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntryCount")
                        .HasColumnType("int");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("LocaleNormalized")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("DictionaryId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("EntityId");

                    b.HasIndex("EntryCount");

                    b.HasIndex("Locale");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "EntityId")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.HasIndex("TenantId", "LocaleNormalized")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.ToTable("Dictionaries", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEntity", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("LogId"));

                    b.Property<string>("ActivityData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActivityType")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ActorId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ApiKeyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CorrelationId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Destination")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("time");

                    b.Property<DateTime?>("EndedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasErrors")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Method")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OperationName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OperationType")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid?>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Source")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<DateTime>("StartedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StatusCode")
                        .HasColumnType("int");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LogId");

                    b.HasIndex("ActivityType");

                    b.HasIndex("ActorId");

                    b.HasIndex("ApiKeyId");

                    b.HasIndex("CorrelationId");

                    b.HasIndex("Duration");

                    b.HasIndex("EndedOn");

                    b.HasIndex("HasErrors");

                    b.HasIndex("IsCompleted");

                    b.HasIndex("Level");

                    b.HasIndex("OperationName");

                    b.HasIndex("OperationType");

                    b.HasIndex("SessionId");

                    b.HasIndex("StartedOn");

                    b.HasIndex("StatusCode");

                    b.HasIndex("TenantId");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Logs", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEventEntity", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.HasKey("EventId");

                    b.HasIndex("LogId");

                    b.ToTable("LogEvents", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogExceptionEntity", b =>
                {
                    b.Property<long>("LogExceptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("LogExceptionId"));

                    b.Property<string>("Data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HResult")
                        .HasColumnType("int");

                    b.Property<string>("HelpLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetSite")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("LogExceptionId");

                    b.HasIndex("LogId");

                    b.HasIndex("Type");

                    b.ToTable("LogExceptions", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.MessageEntity", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageId"));

                    b.Property<string>("BodyText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BodyType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IgnoreUserLocale")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDemo")
                        .HasColumnType("bit");

                    b.Property<string>("Locale")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<int>("RecipientCount")
                        .HasColumnType("int");

                    b.Property<string>("ResultData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SenderAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("SenderDisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.Property<bool>("SenderIsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("SenderPhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("SenderProvider")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TemplateDisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("TemplateId")
                        .HasColumnType("int");

                    b.Property<string>("TemplateUniqueKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Variables")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("MessageId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("EntityId");

                    b.HasIndex("IsDemo");

                    b.HasIndex("RecipientCount");

                    b.HasIndex("SenderId");

                    b.HasIndex("Status");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("Subject");

                    b.HasIndex("TemplateId");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "EntityId")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.ToTable("Messages", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RealmEntity", b =>
                {
                    b.Property<int>("RealmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RealmId"));

                    b.Property<string>("AllowedUniqueNameCharacters")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DefaultLocale")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PasswordHashingStrategy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("PasswordsRequireDigit")
                        .HasColumnType("bit");

                    b.Property<bool>("PasswordsRequireLowercase")
                        .HasColumnType("bit");

                    b.Property<bool>("PasswordsRequireNonAlphanumeric")
                        .HasColumnType("bit");

                    b.Property<bool>("PasswordsRequireUppercase")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireUniqueEmail")
                        .HasColumnType("bit");

                    b.Property<int>("RequiredPasswordLength")
                        .HasColumnType("int");

                    b.Property<int>("RequiredPasswordUniqueChars")
                        .HasColumnType("int");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueSlug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueSlugNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RealmId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UniqueSlug");

                    b.HasIndex("UniqueSlugNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Realms", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RecipientEntity", b =>
                {
                    b.Property<int>("RecipientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecipientId"));

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("UserEmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserFullName")
                        .HasMaxLength(767)
                        .HasColumnType("nvarchar(767)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserPicture")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("UserUniqueName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("RecipientId");

                    b.HasIndex("Address");

                    b.HasIndex("DisplayName");

                    b.HasIndex("MessageId");

                    b.HasIndex("PhoneNumber");

                    b.HasIndex("Type");

                    b.HasIndex("UserId");

                    b.ToTable("Recipients", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.SenderEntity", b =>
                {
                    b.Property<int>("SenderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SenderId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Settings")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SenderId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("EntityId");

                    b.HasIndex("PhoneNumber");

                    b.HasIndex("Provider");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "EntityId")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.HasIndex("TenantId", "IsDefault");

                    b.ToTable("Senders", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.TemplateEntity", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TemplateId"));

                    b.Property<string>("ContentText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UniqueKeyNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("TemplateId");

                    b.HasIndex("ContentType");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EntityId");

                    b.HasIndex("StreamId")
                        .IsUnique();

                    b.HasIndex("Subject");

                    b.HasIndex("UniqueKey");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "EntityId")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.HasIndex("TenantId", "UniqueKeyNormalized")
                        .IsUnique()
                        .HasFilter("[TenantId] IS NOT NULL");

                    b.ToTable("Templates", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEventEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEntity", "Log")
                        .WithMany("Events")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Log");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogExceptionEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEntity", "Log")
                        .WithMany("Exceptions")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Log");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.MessageEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.SenderEntity", "Sender")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.TemplateEntity", "Template")
                        .WithMany("Messages")
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Sender");

                    b.Navigation("Template");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RecipientEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.MessageEntity", "Message")
                        .WithMany("Recipients")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEntity", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Exceptions");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.MessageEntity", b =>
                {
                    b.Navigation("Recipients");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.SenderEntity", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.TemplateEntity", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
