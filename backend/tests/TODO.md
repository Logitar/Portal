# TODO

## Logitar.Portal.Contracts

- Actor
- Aggregate

## Logitar.Portal.Domain

- ConfigurationAggregate
- ConfigurationId
- JwtSecretUnit / JwtSecretValidator
- ReadOnlyPasswordSettings / PasswordSettingsValidator
- ReadOnlyUniqueNameSettings / UniqueNameSettingsValidator
- RealmAggregate
- RealmId
- SlugValidator
- UniqueSlugUnit / UniqueSlugValidator

## Logitar.Portal.Application

- CustomAttributeContractValidator
- InitializeCachingCommandHandler
- InitializeConfigurationCommandHandler
- InitializeConfigurationValidator / SessionPayloadValidator / UserPayloadValidator
- InitialUserValidator
- PortalRoleSettingsResolver
- PortalUserSettingsResolver
- ReadConfigurationQueryHandler
- SettingsExtensions

## Logitar.Portal.Infrastructure

- CacheService

## Logitar.Portal.EntityFrameworkCore.Relational

- ActorService
- ConfigurationQuerier
- ConfigurationRepository
- Handlers\Realms
- Mapper
- RealmRepository

## Logitar.Portal

- CurrentUser
- HttpApplicationContext
- HttpContextExtensions
- InitializeConfigurationModel
- IsConfigurationInitialized
