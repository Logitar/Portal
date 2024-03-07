import { beforeEach, describe, it, expect } from "vitest";
import { setActivePinia, createPinia } from "pinia";

import { useAccountStore } from "../account";
import type { Actor } from "@/types/actor";
import type { Session } from "@/types/sessions";
import type { User } from "@/types/users";

const actor: Actor = {
  id: "9e140e72-0c38-4402-8097-76c0302a50be",
  type: "User",
  isDeleted: false,
  displayName: "portal",
};
const now: string = new Date().toISOString();
const user: User = {
  id: actor.id,
  version: 1,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  uniqueName: "portal",
  hasPassword: false,
  isDisabled: false,
  isConfirmed: false,
  customAttributes: [],
  customIdentifiers: [],
  roles: [],
};
const session: Session = {
  id: "6c9eddce-48ea-4608-b475-d1b9c712060d",
  version: 1,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  isPersistent: false,
  isActive: true,
  customAttributes: [],
  user,
};

describe("accountStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it.concurrent("should be initially empty", () => {
    const account = useAccountStore();
    expect(account.authenticated).toBeUndefined();
    expect(account.currentSession).toBeUndefined();
  });

  it.concurrent("should set the current user", () => {
    const account = useAccountStore();
    account.setUser(user);
    expect(user.id).toBe(account.authenticated?.id);
  });

  it.concurrent("should sign-in an authenticated user", () => {
    const account = useAccountStore();
    account.signIn(session);
    expect(user.id).toBe(account.authenticated?.id);
    expect(session.id).toBe(account.currentSession?.id);
  });

  it.concurrent("should sign-out the authenticated user", () => {
    const account = useAccountStore();
    account.signIn(session);
    account.signOut();
    expect(account.authenticated).toBeUndefined();
    expect(account.currentSession).toBeUndefined();
  });
});
