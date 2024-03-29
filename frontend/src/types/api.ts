export type ApiError = {
  data?: unknown;
  status: number;
};

export type ApiResult<T> = {
  data: T;
  status: number;
};

export type Error = {
  code: string;
  message: string;
  data: ErrorData[];
};

export type ErrorData = {
  key: string;
  value?: string;
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

export type Severity = "Error" | "Info" | "Warning";

export type ValidationError = Error & {
  errors: ValidationFailure[];
};

export type ValidationFailure = {
  propertyName?: string;
  errorMessage?: string;
  attemptedValue?: unknown;
  customState?: unknown;
  severity?: Severity;
  errorCode?: string;
  formattedMessagePlaceholderValues?: Map<string, unknown>;
};
