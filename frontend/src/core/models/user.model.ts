export type UserRole =
  | 'Administrator'
  | 'Sales'
  | 'Purchasing'
  | 'Warehouse'
  | 'Finance'
  | 'HR';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string;
  role: UserRole;
  permissions: string[];
  isActive: boolean;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
}
