import type { Actor } from "@/types/actor";
import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttributes";
import type { User } from "@/types/users";

export type Session = Aggregate & {
  id: string;
  isPersistent: boolean;
  refreshToken?: string;
  isActive: boolean;
  signedOutBy?: Actor;
  signedOutOn?: string;
  customAttributes: CustomAttribute[];
  user: User;
};
