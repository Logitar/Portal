# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added a configuration interface.

### Changed

- The initial user is now the actor in its creation and initialization of the configuration.
- Refactored database & caching initialization.

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

[unreleased]: https://github.com/Utar94/Portal/compare/v2.0.0...HEAD
[2.0.0]: https://github.com/Utar94/Portal/compare/v1.1.5...v2.0.0
[1.1.5]: https://github.com/Utar94/Portal/releases/tag/v1.1.5
