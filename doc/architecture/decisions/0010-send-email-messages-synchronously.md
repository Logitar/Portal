# 10. Send email messages synchronously

Date: 2023-09-18

## Status

Accepted

## Context

We do not want to introduce the concept of asynchronicity in our application at this stage.

## Decision

We will be sending email messages synchronously, on the same thread that an user placed a request.

## Consequences

Users may wait longer when they send a message, and a timeout may occur when sending a message to
multiple recipients. Development is simpler this way.
