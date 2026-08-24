import { User } from './user.model';

export interface AuthResponse {
  accessToken?: string;
  token?: string;
  refreshToken?: string;
  user?: User;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
}

export interface LoginCredentials {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
