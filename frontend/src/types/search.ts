export type SearchOperator = "And" | "Or";

export type SearchPayload = {
  search?: TextSearch;
  idIn?: string[];
  sort?: SortOption[];
  skip?: number;
  limit?: number;
};

export type SearchResults<T> = {
  results: T[];
  total: number;
};

export type SearchTerm = {
  value: string;
};

export type SortOption = {
  field: string;
  isDescending: boolean;
};

export type TextSearch = {
  terms: SearchTerm[];
  operator: SearchOperator;
};
