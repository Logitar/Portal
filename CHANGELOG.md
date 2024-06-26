# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

Nothing yet.

## [4.1.2] - 2024-06-26

### Fixed

- Fixed messages sent in duplicate.

## [4.1.1] - 2024-06-11

### Fixed

- Updated Docker Compose file.
- Updated NPM packages & fixed security issues.
- Fixed Twilio sender replacement.

## [4.1.0] - 2024-03-28

### Added

- Implemented SMS messages.

### Changed

- Upgraded NuGet packages & refactored domain aggregates.

### Fixed

- Fixed the HttpContextExtensions.GetBaseUri method.

## [4.0.1] - 2024-03-09

### Fixed

- Fixed Authentication and Session issues.
- Fixed configuration string environment variables.

## [4.0.0] - 2024-03-08

### Added

- Implemented One-Time Password (OTP) management with [Identity](https://github.com/Logitar/Identity).
- Added Microsoft SQL Server.
- Added MongoDB support for logs.
- Implemented a MassTransit/RabbitMQ interface.
- Created a Worker service to clean the token blacklist.
- Added a Realm Selector in the Frontend.
- Added simple Bearer authentication.
- Sending messages from Mailgun.

### Changed

- Reimplemented logging.
- Reimplemented configuration management.
- Reimplemented account management.
- Reimplemented realm management.
- Reimplemented role management with Identity.
- Reimplemented API key management with Identity.
- Reimplemented user management with Identity.
- Reimplemented session management with Identity.
- Reimplemented token management with Identity.
- Reimplemented dictionary management.
- Reimplemented sender management.
- Reimplemented template management.
- Reimplemented message management.
- Reimplemented HTTP client.
- Optimized GraphQL queries.

### Fixed

- Fixed user realm refreshing after creation.

## [3.0.3] - 2023-10-16

### Added

- Completed NuGet package documentation.

### Fixed

- Fixed NPM audits.

## [3.0.2] - 2023-09-19

### Fixed

- Required FormSelect with no options.

## [3.0.1] - 2023-09-18

### Fixed

- CI/CD pipelines.
- CHANGELOG links.

## [3.0.0] - 2023-09-18

### Added

- Implemented role management.
- Documented software architecture.

### Changed

- Reimplemented logging.
- Reimplemented configuration management.
- Reimplemented HTTP client.
- Reimplemented realm management.
- Reimplemented user management.
- Reimplemented session management.
- Reimplemented API key management.
- Reimplemented token management.
- Reimplemented dictionary management.
- Reimplemented sender management.
- Reimplemented template management.
- Reimplemented message management.

## [2.1.0] - 2023-05-01

### Added

- Added a configuration interface and actors.
- Added phone country code & extension fields.

### Fixed

- Do not display environment tag in Production environment.
- Fixed session properties when refreshed.
- Increment session version when signing-out.

### Changed

- The initial user is now the actor in its creation and initialization of the configuration.
- Refactored database & caching initialization.
- Renamed alias to slug and remove accents in slugs.
- Updated NuGet packages and set DoNotUseFullAssemblyName to true.

## [2.0.0] - 2023-04-15

### Added

- Implemented Basic authentication.
- Added client endpoint tests.

### Fixed

- Remove password recovery sender/template from realm upon deletion.

### Changed

- Archived V1 solution.
- Reimplemented realm management.
- Reimplemented user management.
- Reimplemented configuration management.
- Reimplemented session management.
- Reimplemented token management.
- Reimplemented dictionary management.
- Reimplemented sender management.
- Reimplemented template management.
- Reimplemented message management.
- Reimplemented HTTP client.
- Reimplemented logging.
- Reimplemented caching.
- Reimplemented basic configuration.
- Updated repository information.
- Replaced AllowedUsernameCharacters by UsernameSettings and extented initial configuration.
- Updated NPM packages.
- Updated NuGet packages.
- Enhanced realm JWT secret.
- Merged ICurrentActor into IApplicationContext.
- Merged database migrations.

### Removed

- Removed API key management.

## [1.1.5] - 2022-10-27

- Final V1 release.

[unreleased]: https://github.com/Logitar/Portal/compare/v4.1.2...HEAD
[4.1.2]: https://github.com/Logitar/Portal/compare/v4.1.1...v4.1.2
[4.1.1]: https://github.com/Logitar/Portal/compare/v4.1.0...v4.1.1
[4.1.0]: https://github.com/Logitar/Portal/compare/v4.0.1...v4.1.0
[4.0.1]: https://github.com/Logitar/Portal/compare/v4.0.0...v4.0.1
[4.0.0]: https://github.com/Logitar/Portal/compare/v3.0.3...v4.0.0
[3.0.3]: https://github.com/Logitar/Portal/compare/v3.0.2...v3.0.3
[3.0.2]: https://github.com/Logitar/Portal/compare/v3.0.1...v3.0.2
[3.0.1]: https://github.com/Logitar/Portal/compare/v3.0.0...v3.0.1
[3.0.0]: https://github.com/Logitar/Portal/compare/v2.1.0...v3.0.0
[2.1.0]: https://github.com/Logitar/Portal/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/Logitar/Portal/compare/v1.1.5...v2.0.0
[1.1.5]: https://github.com/Logitar/Portal/releases/tag/v1.1.5
