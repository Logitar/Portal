import type { Actor } from "./actor";

export type Identifier = {
  id: string;
  key: string;
  value: string;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
  version: number;
};
