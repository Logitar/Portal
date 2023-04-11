﻿// <auto-generated />
using System;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Migrations
{
    [DbContext(typeof(PortalContext))]
    [Migration("20230411155552_CreateTemplateTable")]
    partial class CreateTemplateTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.BlacklistedTokenEntity", b =>
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

                    b.ToTable("TokenBlacklist");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.DictionaryEntity", b =>
                {
                    b.Property<int>("DictionaryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DictionaryId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Entries")
                        .HasColumnType("jsonb");

                    b.Property<int>("EntryCount")
                        .HasColumnType("integer");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("DictionaryId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("EntryCount");

                    b.HasIndex("Locale");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("RealmId", "Locale")
                        .IsUnique();

                    b.ToTable("Dictionaries");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.ExternalIdentifierEntity", b =>
                {
                    b.Property<int>("ExternalIdentifierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ExternalIdentifierId"));

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
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

                    b.HasKey("ExternalIdentifierId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UserId");

                    b.HasIndex("RealmId", "Key", "ValueNormalized")
                        .IsUnique();

                    b.ToTable("ExternalIdentifiers");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", b =>
                {
                    b.Property<int>("RealmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RealmId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ClaimMappings")
                        .HasColumnType("jsonb");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("jsonb");

                    b.Property<string>("DefaultLocale")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordSettings")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<bool>("RequireConfirmedAccount")
                        .HasColumnType("boolean");

                    b.Property<bool>("RequireUniqueEmail")
                        .HasColumnType("boolean");

                    b.Property<string>("Secret")
                        .IsRequired()
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
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(65535)
                        .HasColumnType("character varying(65535)");

                    b.Property<string>("UsernameSettings")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("RealmId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("UniqueName");

                    b.HasIndex("UniqueNameNormalized")
                        .IsUnique();

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UpdatedOn");

                    b.ToTable("Realms");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.SenderEntity", b =>
                {
                    b.Property<int>("SenderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SenderId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("Settings")
                        .HasColumnType("jsonb");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SenderId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("Provider");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("RealmId", "IsDefault")
                        .IsUnique()
                        .HasFilter("\"IsDefault\" = true");

                    b.ToTable("Senders");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.SessionEntity", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SessionId"));

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("jsonb");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPersistent")
                        .HasColumnType("boolean");

                    b.Property<string>("Key")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SignedOutBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("SignedOutById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("SignedOutOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("SessionId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("IsActive");

                    b.HasIndex("IsPersistent");

                    b.HasIndex("SignedOutById");

                    b.HasIndex("SignedOutOn");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.TemplateEntity", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TemplateId"));

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Contents")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("RealmId")
                        .HasColumnType("integer");

                    b.Property<string>("Subject")
                        .IsRequired()
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
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("TemplateId");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisplayName");

                    b.HasIndex("UniqueName");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("RealmId", "UniqueNameNormalized")
                        .IsUnique();

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.UserEntity", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("AddressCountry")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressFormatted")
                        .HasMaxLength(65535)
                        .HasColumnType("character varying(65535)");

                    b.Property<string>("AddressLine1")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressLine2")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressLocality")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressPostalCode")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressRegion")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AddressVerifiedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("AddressVerifiedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("AddressVerifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("jsonb");

                    b.Property<string>("DisabledBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("DisabledById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DisabledOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailAddressNormalized")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailVerifiedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("EmailVerifiedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("EmailVerifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FullName")
                        .HasMaxLength(65535)
                        .HasColumnType("character varying(65535)");

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
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

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
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("PasswordChangedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("PasswordChangedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PhoneCountryCode")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneE164Formatted")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneExtension")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneVerifiedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("PhoneVerifiedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("PhoneVerifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Picture")
                        .HasMaxLength(65535)
                        .HasColumnType("character varying(65535)");

                    b.Property<string>("Profile")
                        .HasMaxLength(65535)
                        .HasColumnType("character varying(65535)");

                    b.Property<int>("RealmId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("SignedInOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TimeZone")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("jsonb");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UsernameNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.Property<string>("Website")
                        .HasMaxLength(65535)
                        .HasColumnType("character varying(65535)");

                    b.HasKey("UserId");

                    b.HasIndex("AddressFormatted");

                    b.HasIndex("AddressVerifiedById");

                    b.HasIndex("AggregateId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("DisabledById");

                    b.HasIndex("DisabledOn");

                    b.HasIndex("EmailAddress");

                    b.HasIndex("EmailVerifiedById");

                    b.HasIndex("FirstName");

                    b.HasIndex("FullName");

                    b.HasIndex("IsConfirmed");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("LastName");

                    b.HasIndex("MiddleName");

                    b.HasIndex("Nickname");

                    b.HasIndex("PasswordChangedById");

                    b.HasIndex("PasswordChangedOn");

                    b.HasIndex("PhoneE164Formatted");

                    b.HasIndex("PhoneVerifiedById");

                    b.HasIndex("SignedInOn");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Username");

                    b.HasIndex("RealmId", "EmailAddressNormalized");

                    b.HasIndex("RealmId", "UsernameNormalized")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.DictionaryEntity", b =>
                {
                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", "Realm")
                        .WithMany("Dictionaries")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.ExternalIdentifierEntity", b =>
                {
                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", "Realm")
                        .WithMany("ExternalIdentifiers")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.UserEntity", "User")
                        .WithMany("ExternalIdentifiers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realm");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.SenderEntity", b =>
                {
                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", "Realm")
                        .WithMany("Senders")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.SessionEntity", b =>
                {
                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.UserEntity", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.TemplateEntity", b =>
                {
                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", "Realm")
                        .WithMany("Templates")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.UserEntity", b =>
                {
                    b.HasOne("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", "Realm")
                        .WithMany("Users")
                        .HasForeignKey("RealmId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Realm");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.RealmEntity", b =>
                {
                    b.Navigation("Dictionaries");

                    b.Navigation("ExternalIdentifiers");

                    b.Navigation("Senders");

                    b.Navigation("Templates");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities.UserEntity", b =>
                {
                    b.Navigation("ExternalIdentifiers");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
