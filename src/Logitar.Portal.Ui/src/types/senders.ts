import type { Aggregate } from "@/types/aggregate";
import type { Modification } from "@/types/modifications";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateSenderPayload = {
  realm?: string;
  emailAddress: string;
  displayName?: string;
  description?: string;
  provider: ProviderType;
  settings: ProviderSetting[];
};

export type ProviderSetting = {
  key: string;
  value: string;
};

export type ProviderSettingModification = {
  key: string;
  value?: string;
};

export type ProviderType = "SendGrid";

export type SearchSendersPayload = SearchPayload & {
  realm?: string;
  provider?: ProviderType;
  sort?: SenderSortOption[];
};

export type Sender = Aggregate & {
  id: string;
  isDefault: boolean;
  emailAddress: string;
  displayName?: string;
  description?: string;
  provider: ProviderType;
  settings: ProviderSetting[];
  realm?: Realm;
};

export type SenderSort = "DisplayName" | "EmailAddress" | "UpdatedOn";

export type SenderSortOption = SortOption & {
  field: SenderSort;
};

export type UpdateSenderPayload = {
  emailAddress?: string;
  displayName?: Modification<string>;
  description?: Modification<string>;
  settings?: ProviderSettingModification[];
};
