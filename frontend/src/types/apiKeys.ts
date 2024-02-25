import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./customAttributes";
import type { Realm } from "./realms";
import type { Role } from "./roles";
import type { SearchPayload, SortOption } from "./search";

export type ApiKey = Aggregate & {
  xApiKey?: string;
  displayName: string;
  description?: string;
  expiresOn?: string;
  authenticatedOn?: string;
  customAttributes: CustomAttribute[];
  roles: Role[];
  realm?: Realm;
};

export type ApiKeySort = "AuthenticatedOn" | "DisplayName" | "ExpiresOn" | "UpdatedOn";

export type ApiKeySortOption = SortOption & {
  field: ApiKeySort;
};

export type ApiKeyStatus = {
  isExpired: boolean;
  moment?: Date;
};

export type CreateApiKeyPayload = {
  displayName: string;
  description?: string;
  expiresOn?: Date;
  customAttributes: CustomAttribute[];
  roles: string[];
};

export type SearchApiKeysPayload = SearchPayload & {
  realm?: string;
  status?: ApiKeyStatus;
  sort?: ApiKeySortOption[];
};

export type ReplaceApiKeyPayload = {
  displayName: string;
  description?: string;
  expiresOn?: Date;
  customAttributes: CustomAttribute[];
  roles: string[];
};
