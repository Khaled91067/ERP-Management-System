export interface UserDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  roleId: number;
  roleName: string;
  role?: string;
}

export interface ChangeRoleDto {
  roleId: number;
}
