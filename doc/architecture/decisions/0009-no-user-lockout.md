# 9. No user lockout

Date: 2023-09-09

## Status

Accepted

## Context

User lockout can be a complex feature, and implementing it may require a lot of analysis and changes
in the application.

## Decision

Until we have an incentive or a real need to develop user lockout, we will not be implementing this
feature.

## Consequences

Administrator users (not in a realm) may try to sign-in infinitely with any username/password
combination. The system won't prevent them to do so. If developers want their application to support
user lockout, they will need to develop it into their application.
