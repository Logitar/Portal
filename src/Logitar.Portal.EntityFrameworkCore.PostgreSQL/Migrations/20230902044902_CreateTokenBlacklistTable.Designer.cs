﻿// <auto-generated />
using System;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    [DbContext(typeof(PortalContext))]
    [Migration("20230902044902_CreateTokenBlacklistTable")]
    partial class CreateTokenBlacklistTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.ActorEntity", b =>
                {
                    b.Property<long>("ActorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ActorId"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("Id")
                        .HasMaxLength(255)
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("ActorId");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Type");

                    b.ToTable("Actors", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.ApiKeyEntity", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ApiKeyId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("AuthenticatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributesSerialized")
                        .HasColumnType("text")
                        .HasColumnName("CustomAttributes");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpiresOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Secret")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("ApiKeyId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("AuthenticatedOn");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("ExpiresOn");

                    b.HasIndex("TenantId");

                    b.HasIndex("Title");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("ApiKeys", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.ApiKeyRoleEntity", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("ApiKeyId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("ApiKeyRoles", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.BlacklistedTokenEntity", b =>
                {
                    b.Property<long>("BlacklistedTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("BlacklistedTokenId"));

                    b.Property<DateTime?>("ExpiresOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.HasKey("BlacklistedTokenId");

                    b.HasIndex("ExpiresOn");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("TokenBlacklist", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEntity", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("LogId"));

                    b.Property<string>("ActivityData")
                        .HasColumnType("text");

                    b.Property<string>("ActivityType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("ActorId")
                        .HasColumnType("uuid");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("CorrelationId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Destination")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("interval");

                    b.Property<DateTime?>("EndedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ErrorsSerialized")
                        .HasColumnType("text")
                        .HasColumnName("Errors");

                    b.Property<bool>("HasErrors")
                        .HasColumnType("boolean");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Method")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("OperationName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("OperationType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid?>("SessionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Source")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<DateTime>("StartedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("StatusCode")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LogId");

                    b.HasIndex("ActivityType");

                    b.HasIndex("ActorId");

                    b.HasIndex("CorrelationId");

                    b.HasIndex("Duration");

                    b.HasIndex("EndedOn");

                    b.HasIndex("HasErrors");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("IsCompleted");

                    b.HasIndex("Level");

                    b.HasIndex("Method");

                    b.HasIndex("OperationName");

                    b.HasIndex("OperationType");

                    b.HasIndex("SessionId");

                    b.HasIndex("StartedOn");

                    b.HasIndex("StatusCode");

                    b.HasIndex("UserId");

                    b.ToTable("Logs", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEventEntity", b =>
                {
                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.HasKey("LogId", "EventId");

                    b.ToTable("LogEvents", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RealmEntity", b =>
                {
                    b.Property<int>("RealmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RealmId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AllowedUniqueNameCharacters")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ClaimMappingsSerialized")
                        .HasColumnType("text")
                        .HasColumnName("ClaimMappings");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributesSerialized")
                        .HasColumnType("text")
                        .HasColumnName("CustomAttributes");

                    b.Property<string>("DefaultLocale")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordStrategy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("PasswordsRequireDigit")
                        .HasColumnType("boolean");

                    b.Property<bool>("PasswordsRequireLowercase")
                        .HasColumnType("boolean");

                    b.Property<bool>("PasswordsRequireNonAlphanumeric")
                        .HasColumnType("boolean");

                    b.Property<bool>("PasswordsRequireUppercase")
                        .HasColumnType("boolean");

                    b.Property<bool>("RequireConfirmedAccount")
                        .HasColumnType("boolean");

                    b.Property<bool>("RequireUniqueEmail")
                        .HasColumnType("boolean");

                    b.Property<int>("RequiredPasswordLength")
                        .HasColumnType("integer");

                    b.Property<int>("RequiredPasswordUniqueChars")
                        .HasColumnType("integer");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("UniqueSlug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueSlugNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RealmId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("UniqueSlug");

                    b.HasIndex("UniqueSlugNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.ToTable("Realms", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RoleEntity", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RoleId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributesSerialized")
                        .HasColumnType("text")
                        .HasColumnName("CustomAttributes");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RoleId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("TenantId");

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "UniqueNameNormalized")
                        .IsUnique();

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.SessionEntity", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SessionId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributesSerialized")
                        .HasColumnType("text")
                        .HasColumnName("CustomAttributes");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPersistent")
                        .HasColumnType("boolean");

                    b.Property<string>("Secret")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SignedOutBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("SignedOutOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SessionId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("IsActive");

                    b.HasIndex("IsPersistent");

                    b.HasIndex("SignedOutOn");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("UserId");

                    b.HasIndex("Version");

                    b.ToTable("Sessions", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserEntity", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("AddressCountry")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressFormatted")
                        .HasMaxLength(1536)
                        .HasColumnType("character varying(1536)");

                    b.Property<string>("AddressLocality")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressPostalCode")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressRegion")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressStreet")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressVerifiedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("AddressVerifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("AuthenticatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributesSerialized")
                        .HasColumnType("text")
                        .HasColumnName("CustomAttributes");

                    b.Property<string>("DisabledBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("DisabledOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailAddressNormalized")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailVerifiedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("EmailVerifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FullName")
                        .HasMaxLength(768)
                        .HasColumnType("character varying(768)");

                    b.Property<string>("Gender")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("HasPassword")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAddressVerified")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPhoneVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Locale")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Nickname")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordChangedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("PasswordChangedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PhoneCountryCode")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("PhoneE164Formatted")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("PhoneExtension")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("PhoneVerifiedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("PhoneVerifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Picture")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("Profile")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TimeZone")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueNameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<string>("Website")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.HasKey("UserId");

                    b.HasIndex("AddressFormatted");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("AuthenticatedOn");

                    b.HasIndex("Birthdate");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisabledOn");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("FirstName");

                    b.HasIndex("FullName");

                    b.HasIndex("Gender");

                    b.HasIndex("HasPassword");

                    b.HasIndex("IsConfirmed");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("LastName");

                    b.HasIndex("Locale");

                    b.HasIndex("MiddleName");

                    b.HasIndex("Nickname");

                    b.HasIndex("PasswordChangedOn");

                    b.HasIndex("PhoneE164Formatted");

                    b.HasIndex("TenantId");

                    b.HasIndex("TimeZone");

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "EmailAddressNormalized");

                    b.HasIndex("TenantId", "UniqueNameNormalized")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserIdentifierEntity", b =>
                {
                    b.Property<int>("UserIdentifierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserIdentifierId"));

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ValueNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("UserIdentifierId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.HasIndex("TenantId", "Key", "Value")
                        .IsUnique();

                    b.ToTable("UserIdentifiers", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserRoleEntity", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.ApiKeyRoleEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.ApiKeyEntity", null)
                        .WithMany()
                        .HasForeignKey("ApiKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.SessionEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserEntity", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserIdentifierEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserEntity", "User")
                        .WithMany("Identifiers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserRoleEntity", b =>
                {
                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.LogEntity", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.UserEntity", b =>
                {
                    b.Navigation("Identifiers");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
