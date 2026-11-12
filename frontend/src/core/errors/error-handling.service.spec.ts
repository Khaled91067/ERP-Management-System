import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpErrorResponse, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { ErrorHandlingService } from './error-handling.service';
import { FormValidationService } from './form-validation.service';
import { errorInterceptor } from '@core/interceptors/error.interceptor';
import { TranslationService } from '@core/i18n/translation.service';
import { NotificationService } from '@core/services/notification.service';
import { withErrorHandlingOptions } from '@core/http/http-context.tokens';
import { NormalizedApiError } from '@core/models/api-error.model';

describe('Centralized Error Handling & Form Validation Architecture', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let errorService: ErrorHandlingService;
  let formValidation: FormValidationService;
  let i18n: TranslationService;
  let notificationSpy: { error: ReturnType<typeof vi.fn>; warning: ReturnType<typeof vi.fn>; success: ReturnType<typeof vi.fn>; info: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    localStorage.clear();
    notificationSpy = {
      error: vi.fn(),
      warning: vi.fn(),
      success: vi.fn(),
      info: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        { provide: NotificationService, useValue: notificationSpy }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    errorService = TestBed.inject(ErrorHandlingService);
    formValidation = TestBed.inject(FormValidationService);
    i18n = TestBed.inject(TranslationService);
    errorService.resetNotificationDeduplication();
  });

  afterEach(() => {
    httpMock.verify();
    formValidation.clearActiveForm();
  });

  it('normalizes RFC 7807 ProblemDetails and ValidationProblemDetails responses accurately', () => {
    const validationErr = new HttpErrorResponse({
      status: 400,
      url: '/api/v1/customers',
      error: {
        title: 'Validation Error',
        status: 400,
        detail: 'One or more validation errors occurred.',
        instance: '/api/v1/customers',
        traceId: '00-abc123-def456-00',
        errors: {
          Email: ["'Email' is not a valid email address."],
          Name: ["'Name' must not be empty."]
        }
      }
    });

    const normalized = errorService.normalizeHttpError(validationErr);
    expect(normalized.status).toBe(400);
    expect(normalized.category).toBe('validation');
    expect(normalized.correlationId).toBe('00-abc123-def456-00');
    expect(normalized.fieldErrors['Email']).toEqual(["'Email' is not a valid email address."]);
    expect(normalized.fieldErrors['Name']).toEqual(["'Name' must not be empty."]);
  });

  it('maps 400, 401, 403, 404, 409, 422, 429, 500, 502/503/504, and Network Error in both English and Arabic', () => {
    const cases: Array<{
      status: number;
      url?: string;
      body: unknown;
      expectedCategory: NormalizedApiError['category'];
      expectedKey: string;
      expectedEnContains: string;
      expectedArContains: string;
    }> = [
      {
        status: 400,
        body: 'ID mismatch between route and body.',
        expectedCategory: 'badRequest',
        expectedKey: 'errors.idMismatch',
        expectedEnContains: 'identifier in the request does not match',
        expectedArContains: 'لا يتطابق'
      },
      {
        status: 400,
        body: { title: 'Bad Request', status: 400, detail: 'Insufficient stock for product Widget.' },
        expectedCategory: 'badRequest',
        expectedKey: 'errors.insufficientStock',
        expectedEnContains: 'Insufficient stock',
        expectedArContains: 'المخزون غير كافية'
      },
      {
        status: 401,
        url: '/api/v1/auth/login',
        body: { title: 'Unauthorized', status: 401, detail: 'Invalid email or password.' },
        expectedCategory: 'unauthorized',
        expectedKey: 'errors.invalidCredentials',
        expectedEnContains: 'Invalid email or password',
        expectedArContains: 'البريد الإلكتروني أو كلمة المرور غير صحيحة'
      },
      {
        status: 403,
        body: '',
        expectedCategory: 'forbidden',
        expectedKey: 'errors.forbidden',
        expectedEnContains: 'do not have permission',
        expectedArContains: 'ليست لديك صلاحية'
      },
      {
        status: 404,
        body: { type: 'https://tools.ietf.org/html/rfc9110#section-15.5.5', title: 'Not Found', status: 404, traceId: 'tr-404' },
        expectedCategory: 'notFound',
        expectedKey: 'errors.notFound',
        expectedEnContains: 'not found',
        expectedArContains: 'لم يتم العثور'
      },
      {
        status: 409,
        body: { title: 'Conflict', status: 409, detail: 'A product with this SKU already exists.' },
        expectedCategory: 'conflict',
        expectedKey: 'errors.duplicateSku',
        expectedEnContains: 'SKU already exists',
        expectedArContains: 'رمز SKU'
      },
      {
        status: 409,
        body: { title: 'Concurrency Conflict', status: 409, detail: 'The record was modified by another user.' },
        expectedCategory: 'conflict',
        expectedKey: 'errors.concurrencyConflict',
        expectedEnContains: 'modified by another user',
        expectedArContains: 'مستخدم آخر'
      },
      {
        status: 422,
        body: { title: 'Unprocessable Entity', status: 422, errors: { Quantity: ["'Quantity' must be greater than '0'."] } },
        expectedCategory: 'validation',
        expectedKey: 'errors.validation',
        expectedEnContains: 'highlighted validation errors',
        expectedArContains: 'أخطاء التحقق'
      },
      {
        status: 429,
        body: '',
        expectedCategory: 'rateLimit',
        expectedKey: 'errors.rateLimit',
        expectedEnContains: 'Too many requests',
        expectedArContains: 'طلبات كثيرة'
      },
      {
        status: 500,
        body: { title: 'Server Error', status: 500, detail: 'An unexpected error occurred. Please try again later.' },
        expectedCategory: 'serverError',
        expectedKey: 'errors.serverError',
        expectedEnContains: 'unexpected server error',
        expectedArContains: 'خطأ غير متوقع في الخادم'
      },
      {
        status: 503,
        body: '<html>503 Service Unavailable</html>',
        expectedCategory: 'serviceUnavailable',
        expectedKey: 'errors.serviceUnavailable',
        expectedEnContains: 'temporarily unavailable',
        expectedArContains: 'غير متاحة مؤقتاً'
      },
      {
        status: 0,
        body: new ProgressEvent('error'),
        expectedCategory: 'network',
        expectedKey: 'errors.networkError',
        expectedEnContains: 'Unable to connect',
        expectedArContains: 'تعذر الاتصال'
      }
    ];

    for (const testCase of cases) {
      i18n.setLanguage('en');
      const errEn = new HttpErrorResponse({
        status: testCase.status,
        url: testCase.url ?? '/api/v1/orders',
        error: testCase.body
      });
      const normEn = errorService.normalizeHttpError(errEn);
      expect(normEn.category).toBe(testCase.expectedCategory);
      expect(normEn.translationKey).toBe(testCase.expectedKey);
      expect(normEn.userMessage).toContain(testCase.expectedEnContains);

      i18n.setLanguage('ar');
      const normAr = errorService.normalizeHttpError(errEn);
      expect(normAr.userMessage).toContain(testCase.expectedArContains);
    }
  });

  it('never exposes raw stack traces, SQL errors, or internal exception details to users', () => {
    const sensitiveError = new HttpErrorResponse({
      status: 400,
      url: '/api/v1/orders',
      error: {
        title: 'Bad Request',
        status: 400,
        detail: 'System.Data.SqlClient.SqlException: Violation of PRIMARY KEY constraint PK_Orders\n   at ERP.Infrastructure.Repositories.OrderRepository.AddAsync() in C:\\src\\OrderRepository.cs:line 42'
      }
    });

    const normalized = errorService.normalizeHttpError(sensitiveError);
    expect(normalized.userMessage).not.toContain('SqlException');
    expect(normalized.userMessage).not.toContain('PRIMARY KEY');
    expect(normalized.userMessage).not.toContain('OrderRepository.cs');
    expect(normalized.userMessage).toBe(i18n.translate('errors.badRequest'));
  });

  it('maps top-level, aliased, and nested FormArray validation errors to form controls, preserves validators, and suppresses global toast', () => {
    const fb = TestBed.inject(FormBuilder);
    const form = fb.group({
      supplierId: [1, Validators.required],
      expectedDeliveryDate: [new Date(), Validators.required],
      lines: fb.array([
        fb.group({
          productId: [10, Validators.required],
          quantity: [1, [Validators.required, Validators.min(1)]],
          unitCost: [5, [Validators.required, Validators.min(0)]]
        })
      ])
    });

    formValidation.prepareFormForSubmission(form);

    http.post('/api/v1/purchase-orders', {}).subscribe({
      error: () => {}
    });

    const req = httpMock.expectOne('/api/v1/purchase-orders');
    req.flush(
      {
        title: 'Validation Error',
        status: 400,
        detail: 'One or more validation errors occurred.',
        errors: {
          ExpectedDelivery: ["'Expected Delivery' must be in the future."],
          'Lines[0].Quantity': ["'Quantity' must be greater than '0'."]
        }
      },
      { status: 400, statusText: 'Bad Request' }
    );

    const deliveryControl = form.get('expectedDeliveryDate')!;
    const lineQtyControl = (form.get('lines') as FormArray).at(0).get('quantity')!;

    expect(deliveryControl.hasError('serverError')).toBe(true);
    expect(deliveryControl.getError('serverError')).toBe('Date must be in the future');
    expect(lineQtyControl.hasError('serverError')).toBe(true);
    expect(lineQtyControl.getError('serverError')).toBe('Value must be greater than zero');

    // Global toast should be suppressed because all field errors mapped to form controls
    expect(notificationSpy.error).not.toHaveBeenCalled();

    // Editing the control clears serverError while preserving Angular validators
    lineQtyControl.setValue(0);
    expect(lineQtyControl.hasError('serverError')).toBe(false);
    expect(lineQtyControl.hasError('min')).toBe(true);
  });

  it('prevents duplicate global error notifications within the deduplication window and respects SKIP_GLOBAL_ERROR_NOTIFICATION', () => {
    // 1. Duplicate 500 errors
    http.get('/api/v1/orders').subscribe({ error: () => {} });
    http.get('/api/v1/orders').subscribe({ error: () => {} });

    const reqs = httpMock.match('/api/v1/orders');
    expect(reqs.length).toBe(2);
    reqs[0].flush({ title: 'Server Error', status: 500 }, { status: 500, statusText: 'Server Error' });
    reqs[1].flush({ title: 'Server Error', status: 500 }, { status: 500, statusText: 'Server Error' });

    expect(notificationSpy.error).toHaveBeenCalledTimes(1);

    // 2. Opt-out via withErrorHandlingOptions({ skipGlobalNotification: true })
    notificationSpy.error.mockClear();
    http.get('/api/v1/dashboard', {
      context: withErrorHandlingOptions({ skipGlobalNotification: true })
    }).subscribe({ error: () => {} });

    const silentReq = httpMock.expectOne('/api/v1/dashboard');
    silentReq.flush({ title: 'Not Found', status: 404 }, { status: 404, statusText: 'Not Found' });

    expect(notificationSpy.error).not.toHaveBeenCalled();
  });
});
