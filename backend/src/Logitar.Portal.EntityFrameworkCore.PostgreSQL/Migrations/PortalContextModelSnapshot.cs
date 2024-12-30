﻿// <auto-generated />
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    [DbContext(typeof(PortalContext))]
    partial class PortalContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.DictionaryEntity", b =>
                {
                    b.Property<int>("DictionaryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DictionaryId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Entries")
                        .HasColumnType("text");

                    b.Property<int>("EntryCount")
                        .HasColumnType("integer");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("LocaleNormalized")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .IsUnique();

                    b.HasIndex("TenantId", "LocaleNormalized")
                        .IsUnique();

                    b.ToTable("Dictionaries", (string)null);
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

                    b.Property<string>("ActorId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("text");

                    b.Property<string>("ApiKeyId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

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

                    b.Property<bool>("HasErrors")
                        .HasColumnType("boolean");

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

                    b.Property<string>("SessionId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Source")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<DateTime>("StartedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("StatusCode")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

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
                    b.Property<string>("EventId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

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

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("LogExceptionId"));

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<int>("HResult")
                        .HasColumnType("integer");

                    b.Property<string>("HelpLink")
                        .HasColumnType("text");

                    b.Property<long>("LogId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("StackTrace")
                        .HasColumnType("text");

                    b.Property<string>("TargetSite")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("LogExceptionId");

                    b.HasIndex("LogId");

                    b.HasIndex("Type");

                    b.ToTable("LogExceptions", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.MessageEntity", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MessageId"));

                    b.Property<string>("BodyText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BodyType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IgnoreUserLocale")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDemo")
                        .HasColumnType("boolean");

                    b.Property<string>("Locale")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<int>("RecipientCount")
                        .HasColumnType("integer");

                    b.Property<string>("ResultData")
                        .HasColumnType("text");

                    b.Property<string>("SenderAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("SenderDisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("SenderId")
                        .HasColumnType("integer");

                    b.Property<bool>("SenderIsDefault")
                        .HasColumnType("boolean");

                    b.Property<string>("SenderPhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("SenderProvider")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TemplateDisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("TemplateId")
                        .HasColumnType("integer");

                    b.Property<string>("TemplateUniqueKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Variables")
                        .HasColumnType("text");

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

                    b.HasIndex("TenantId");

                    b.HasIndex("UpdatedBy");

                    b.HasIndex("UpdatedOn");

                    b.HasIndex("Version");

                    b.HasIndex("TenantId", "EntityId")
                        .IsUnique();

                    b.ToTable("Messages", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.RealmEntity", b =>
                {
                    b.Property<int>("RealmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RealmId"));

                    b.Property<string>("AllowedUniqueNameCharacters")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomAttributes")
                        .HasColumnType("text");

                    b.Property<string>("DefaultLocale")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordHashingStrategy")
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

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueSlug")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueSlugNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RecipientId"));

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("UserEmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UserFullName")
                        .HasMaxLength(767)
                        .HasColumnType("character varying(767)");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("UserPicture")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("UserUniqueName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SenderId"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Settings")
                        .HasColumnType("text");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .IsUnique();

                    b.HasIndex("TenantId", "IsDefault");

                    b.ToTable("Senders", (string)null);
                });

            modelBuilder.Entity("Logitar.Portal.EntityFrameworkCore.Relational.Entities.TemplateEntity", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TemplateId"));

                    b.Property<string>("ContentText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UniqueKeyNormalized")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

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
                        .IsUnique();

                    b.HasIndex("TenantId", "UniqueKeyNormalized")
                        .IsUnique();

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