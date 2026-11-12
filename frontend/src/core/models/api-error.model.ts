export type ApiErrorCategory =
  | 'validation'
  | 'badRequest'
  | 'unauthorized'
  | 'forbidden'
  | 'notFound'
  | 'conflict'
  | 'unprocessable'
  | 'rateLimit'
  | 'serverError'
  | 'serviceUnavailable'
  | 'network';

export interface ProblemDetailsPayload {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  errors?: Record<string, string[] | string> | null;
  traceId?: string | null;
  correlationId?: string | null;
  errorCode?: string | null;
  code?: string | null;
  [key: string]: unknown;
}

export interface NormalizedApiError {
  status: number;
  category: ApiErrorCategory;
  type: string | null;
  title: string | null;
  detail: string | null;
  translationKey: string;
  translationParams?: Record<string, string | number>;
  userMessage: string;
  fieldErrors: Record<string, string[]>;
  correlationId: string | null;
  timestamp: string;
  handledByForm?: boolean;
}
