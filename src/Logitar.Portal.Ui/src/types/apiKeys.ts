import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { Modification } from "@/types/modifications";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { RoleModification } from "@/types/roles";
import type { SearchPayload, SortOption } from "@/types/search";

export type ApiKey = Aggregate & {
  id: string;
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
  realm?: string;
  displayName: string;
  description?: string;
  expiresOn?: Date;
  customAttributes?: CustomAttribute[];
  roles?: Role[];
};

export type SearchApiKeysPayload = SearchPayload & {
  realm?: string;
  status?: ApiKeyStatus;
  sort?: ApiKeySortOption[];
};

export type UpdateApiKeyPayload = {
  displayName?: string;
  description?: Modification<string>;
  expiresOn?: Date;
  customAttributes?: CustomAttributeModification[];
  roles?: RoleModification[];
};
