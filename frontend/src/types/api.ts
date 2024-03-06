export type ApiError = {
  data?: unknown;
  status: number;
};

export type ApiResult<T> = {
  data: T;
  status: number;
};

export type ErrorDetail = {
  errorCode: string;
  errorMessage: string;
};

export type GraphQLError = {
  extensions?: GraphQLErrorExtensions;
  locations?: GraphQLLocation[];
  message?: string;
};

export type GraphQLErrorExtensions = {
  code?: string;
  codes?: string[];
  details?: string;
};

export type GraphQLLocation = {
  column?: number;
  line?: number;
};

export type GraphQLRequest<T> = {
  query: string;
  variables?: T;
};

export type GraphQLResponse<T> = {
  data?: T;
  errors?: GraphQLError[];
};
