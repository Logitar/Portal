# 8. Superuser administrator users

Date: 2023-09-09

## Status

Accepted

## Context

Permissions can be infinitely granular and very complex to implement and configure.

## Decision

Until we have an incentive or a real need to configure permissions for administrator users, users of
the Portal (not in a realm) will be considered as superusers, and may be able to do everything in
the Portal, without any restriction.

## Consequences

It won't be possible to configure permissions per group or per administrator user in the Portal, so
all administrator users will be granted full privilege.
