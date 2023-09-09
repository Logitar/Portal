# 7. Developing an embedded user interface

Date: 2023-09-09

## Status

Accepted

## Context

To minimize the deployment burden, we may not use [Jamstack](https://jamstack.org/). We need an easy
way to deploy an user interface.

## Decision

We will be developing an embedded Vue.js 3 Web Single-Page Application, available from the `/app`
route.

## Consequences

REST endpoints will be available under the `/api` route, and the application will be available under
the `/app` route. We must ensure that route not under these are handled properly.
