﻿// <auto-generated />
using System;
using Logitar.Portal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    [DbContext(typeof(PortalContext))]
    [Migration("20230203195502_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.ActorEntity", b =>
                {
                    b.Property<int>("ActorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ActorId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Picture")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ActorId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.ToTable("Actors");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.ActorTypeEntity", b =>
                {
                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Value");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ActorTypes");

                    b.HasData(
                        new
                        {
                            Value = 0,
                            Name = "System"
                        },
                        new
                        {
                            Value = 1,
                            Name = "User"
                        },
                        new
                        {
                            Value = 2,
                            Name = "ApiKey"
                        });
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.ApiKeyEntity", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ApiKeyId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpiresOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SecretHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("ApiKeyId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("ExpiresOn");

                    b.HasIndex("Title");

                    b.HasIndex("UpdatedOn");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.BlacklistedJwtEntity", b =>
                {
                    b.Property<long>("BlacklistedJwtId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("BlacklistedJwtId"));

                    b.Property<DateTime?>("ExpiresOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.HasKey("BlacklistedJwtId");

                    b.HasIndex("ExpiresOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("JwtBlacklist");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.DictionaryEntity", b =>
                {
                    b.Property<int>("DictionaryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DictionaryId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Entries")
                        .HasColumnType("jsonb");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("DictionaryId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("RealmId", "Locale")
                        .IsUnique();

                    b.ToTable("Dictionaries");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.EventEntity", b =>
                {
                    b.Property<long>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("EventId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("AggregateType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("OccurredOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("EventId");

                    b.HasIndex("Version");

                    b.HasIndex("AggregateType", "AggregateId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.ExternalProviderEntity", b =>
                {
                    b.Property<int>("ExternalProviderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ExternalProviderId"));

                    b.Property<string>("AddedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("AddedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("RealmId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("ExternalProviderId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.HasIndex("RealmId", "Key", "Value")
                        .IsUnique();

                    b.ToTable("ExternalProviders");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.ProviderTypeEntity", b =>
                {
                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Value");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("SenderProviderTypes");

                    b.HasData(
                        new
                        {
                            Value = 0,
                            Name = "SendGrid"
                        });
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.RealmEntity", b =>
                {
                    b.Property<int>("RealmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RealmId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("AliasNormalized")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DefaultLocale")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("GoogleClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("JwtSecret")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int?>("PasswordRecoverySenderId")
                        .HasColumnType("integer");

                    b.Property<int?>("PasswordRecoveryTemplateId")
                        .HasColumnType("integer");

                    b.Property<string>("PasswordSettings")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<bool>("RequireConfirmedAccount")
                        .HasColumnType("boolean");

                    b.Property<bool>("RequireUniqueEmail")
                        .HasColumnType("boolean");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("UsernameSettings")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RealmId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("Alias");

                    b.HasIndex("AliasNormalized")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("PasswordRecoverySenderId")
                        .IsUnique();

                    b.HasIndex("PasswordRecoveryTemplateId")
                        .IsUnique();

                    b.HasIndex("UpdatedOn");

                    b.ToTable("Realms");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.SenderEntity", b =>
                {
                    b.Property<int>("SenderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SenderId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<int>("Provider")
                        .HasColumnType("integer");

                    b.Property<int?>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("Settings")
                        .HasColumnType("jsonb");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SenderId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("IsDefault")
                        .HasFilter("\"IsDefault\" = true");

                    b.HasIndex("Provider");

                    b.HasIndex("RealmId");

                    b.HasIndex("UpdatedOn");

                    b.ToTable("Senders");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.SessionEntity", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SessionId"));

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPersistent")
                        .HasColumnType("boolean");

                    b.Property<string>("KeyHash")
                        .HasColumnType("text");

                    b.Property<string>("SignedOutBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("SignedOutOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SessionId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("IpAddress");

                    b.HasIndex("IsActive");

                    b.HasIndex("IsPersistent");

                    b.HasIndex("SignedOutOn");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.TemplateEntity", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TemplateId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("KeyNormalized")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("TemplateId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("Key");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("RealmId", "KeyNormalized")
                        .IsUnique();

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.UserEntity", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisabledBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("DisabledOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("EmailConfirmedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("EmailConfirmedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailNormalized")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("FullName")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<bool>("HasPassword")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAccountConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Locale")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime?>("PasswordChangedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumberConfirmedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("PhoneNumberConfirmedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PhoneNumberNormalized")
                        .HasColumnType("text");

                    b.Property<string>("Picture")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int?>("RealmId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("SignedInOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("UsernameNormalized")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("UserId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedOn");

                    b.HasIndex("Email");

                    b.HasIndex("EmailNormalized");

                    b.HasIndex("FirstName");

                    b.HasIndex("IsAccountConfirmed");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("LastName");

                    b.HasIndex("MiddleName");

                    b.HasIndex("PasswordChangedOn");

                    b.HasIndex("PhoneNumber");

                    b.HasIndex("SignedInOn");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Username");

                    b.HasIndex("RealmId", "UsernameNormalized")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.DictionaryEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.RealmEntity", "Realm")
                        .WithMany("Dictionaries")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.ExternalProviderEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.RealmEntity", "Realm")
                        .WithMany("ExternalProviders")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Logitar.Portal.Infrastructure.Entities.UserEntity", "User")
                        .WithMany("ExternalProviders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realm");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.RealmEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.SenderEntity", "PasswordRecoverySender")
                        .WithOne("UsedAsPasswordRecoverySenderInRealm")
                        .HasForeignKey("Logitar.Portal.Infrastructure.Entities.RealmEntity", "PasswordRecoverySenderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Logitar.Portal.Infrastructure.Entities.TemplateEntity", "PasswordRecoveryTemplate")
                        .WithOne("UsedAsPasswordRecoveryTemplateInRealm")
                        .HasForeignKey("Logitar.Portal.Infrastructure.Entities.RealmEntity", "PasswordRecoveryTemplateId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("PasswordRecoverySender");

                    b.Navigation("PasswordRecoveryTemplate");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.SenderEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.RealmEntity", "Realm")
                        .WithMany("Senders")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.SessionEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.UserEntity", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.TemplateEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.RealmEntity", "Realm")
                        .WithMany("Templates")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.UserEntity", b =>
                {
                    b.HasOne("Logitar.Portal.Infrastructure.Entities.RealmEntity", "Realm")
                        .WithMany("Users")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.RealmEntity", b =>
                {
                    b.Navigation("Dictionaries");

                    b.Navigation("ExternalProviders");

                    b.Navigation("Senders");

                    b.Navigation("Templates");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.SenderEntity", b =>
                {
                    b.Navigation("UsedAsPasswordRecoverySenderInRealm");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.TemplateEntity", b =>
                {
                    b.Navigation("UsedAsPasswordRecoveryTemplateInRealm");
                });

            modelBuilder.Entity("Logitar.Portal.Infrastructure.Entities.UserEntity", b =>
                {
                    b.Navigation("ExternalProviders");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}