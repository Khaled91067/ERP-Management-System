import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpErrorResponse, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Subject, throwError, of } from 'rxjs';
import { authInterceptor, resetAuthInterceptorState } from './auth.interceptor';
import { errorInterceptor } from './error.interceptor';
import { AuthService } from '@core/auth/auth.service';
import { NotificationService } from '@core/services/notification.service';

describe('authInterceptor & errorInterceptor Integration', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let authServiceMock: {
    token: ReturnType<typeof vi.fn>;
    refreshToken: ReturnType<typeof vi.fn>;
    refreshAccessToken: ReturnType<typeof vi.fn>;
    logout: ReturnType<typeof vi.fn>;
  };
  let notificationMock: {
    error: ReturnType<typeof vi.fn>;
    warning: ReturnType<typeof vi.fn>;
    success: ReturnType<typeof vi.fn>;
    info: ReturnType<typeof vi.fn>;
  };

  beforeEach(() => {
    resetAuthInterceptorState();
    let currentToken: string | null = 'expired-jwt-token';

    authServiceMock = {
      token: vi.fn(() => currentToken),
      refreshToken: vi.fn(() => 'valid-refresh-token'),
      refreshAccessToken: vi.fn(() => {
        currentToken = 'fresh-jwt-token';
        return of({ accessToken: 'fresh-jwt-token', refreshToken: 'new-refresh-token' });
      }),
      logout: vi.fn()
    };

    notificationMock = {
      error: vi.fn(),
      warning: vi.fn(),
      success: vi.fn(),
      info: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor, authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: authServiceMock },
        { provide: NotificationService, useValue: notificationMock }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    resetAuthInterceptorState();
  });

  it('refreshes the access token on 401 and retries the original request silently without showing an error toast', () => {
    let responseData: unknown;

    http.get('/api/v1/orders').subscribe(res => {
      responseData = res;
    });

    // First attempt fails with 401
    const firstReq = httpMock.expectOne('/api/v1/orders');
    expect(firstReq.request.headers.get('Authorization')).toBe('Bearer expired-jwt-token');
    firstReq.flush('', { status: 401, statusText: 'Unauthorized' });

    expect(authServiceMock.refreshAccessToken).toHaveBeenCalledTimes(1);

    // Retried request uses the new token
    const retryReq = httpMock.expectOne('/api/v1/orders');
    expect(retryReq.request.headers.get('Authorization')).toBe('Bearer fresh-jwt-token');
    retryReq.flush({ items: [{ id: 1 }], total: 1 });

    expect(responseData).toEqual({ items: [{ id: 1 }], total: 1 });
    expect(notificationMock.error).not.toHaveBeenCalled();
  });

  it('queues concurrent 401 requests so only one refresh-token request is made and all requests retry', () => {
    const refreshSubject = new Subject<{ accessToken: string; refreshToken: string }>();
    let token = 'expired-jwt-token';
    authServiceMock.token.mockImplementation(() => token);
    authServiceMock.refreshAccessToken.mockImplementation(() => refreshSubject.asObservable());

    let resA: unknown;
    let resB: unknown;

    http.get('/api/v1/orders').subscribe(r => (resA = r));
    http.get('/api/v1/customers').subscribe(r => (resB = r));

    const reqA = httpMock.expectOne('/api/v1/orders');
    const reqB = httpMock.expectOne('/api/v1/customers');

    reqA.flush('', { status: 401, statusText: 'Unauthorized' });
    reqB.flush('', { status: 401, statusText: 'Unauthorized' });

    // Only one refresh call should be triggered for both concurrent 401s
    expect(authServiceMock.refreshAccessToken).toHaveBeenCalledTimes(1);

    token = 'new-shared-token';
    refreshSubject.next({ accessToken: 'new-shared-token', refreshToken: 'rt-2' });
    refreshSubject.complete();

    const retryA = httpMock.expectOne('/api/v1/orders');
    const retryB = httpMock.expectOne('/api/v1/customers');
    expect(retryA.request.headers.get('Authorization')).toBe('Bearer new-shared-token');
    expect(retryB.request.headers.get('Authorization')).toBe('Bearer new-shared-token');

    retryA.flush({ ok: 'orders' });
    retryB.flush({ ok: 'customers' });

    expect(resA).toEqual({ ok: 'orders' });
    expect(resB).toEqual({ ok: 'customers' });
  });

  it('logs out and does not loop infinitely when refresh-token fails', () => {
    authServiceMock.refreshAccessToken.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 401, statusText: 'Unauthorized', url: '/api/v1/auth/refresh' }))
    );

    let caughtError: HttpErrorResponse | undefined;
    http.get('/api/v1/orders').subscribe({
      error: (err) => (caughtError = err)
    });

    const req = httpMock.expectOne('/api/v1/orders');
    req.flush('', { status: 401, statusText: 'Unauthorized' });

    expect(authServiceMock.refreshAccessToken).toHaveBeenCalledTimes(1);
    expect(authServiceMock.logout).toHaveBeenCalledTimes(1);
    expect(caughtError?.status).toBe(401);
  });

  it('never triggers token refresh or logout on 403 Forbidden', () => {
    let caughtError: HttpErrorResponse | undefined;
    http.delete('/api/v1/roles/5').subscribe({
      error: (err) => (caughtError = err)
    });

    const req = httpMock.expectOne('/api/v1/roles/5');
    req.flush('', { status: 403, statusText: 'Forbidden' });

    expect(authServiceMock.refreshAccessToken).not.toHaveBeenCalled();
    expect(authServiceMock.logout).not.toHaveBeenCalled();
    expect(caughtError?.status).toBe(403);
    expect(notificationMock.error).toHaveBeenCalledTimes(1);
  });
});
