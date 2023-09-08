import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { Modification } from "@/types/modifications";
import type { Role } from "@/types/roles";
import type { RoleModification } from "@/types/roles/index";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateApiKeyPayload = {
  realm?: string;
  displayName: string;
  description?: string;
  expiresOn?: Date;
  customAttributes?: CustomAttribute[];
  roles?: Role[];
};

export type ApiKeySort = "AuthenticatedOn" | "DisplayName" | "ExpiresOn" | "UpdatedOn";

export type ApiKeySortOption = SortOption & {
  field: ApiKeySort;
};

export type ApiKeyStatus = {
  isExpired: boolean;
  moment?: Date;
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
