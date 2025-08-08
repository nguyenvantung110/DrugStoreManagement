export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  phone?: string;
  avatar?: string;
  role: UserRole;
  status: UserStatus;
  createdAt: Date;
  updatedAt: Date;
  lastLogin?: Date;
}

export interface UserRole {
  id: string;
  name: string;
  displayName: string;
  permissions: Permission[];
}

export interface Permission {
  id: string;
  name: string;
  displayName: string;
  module: string;
}

export type UserStatus = 'active' | 'inactive' | 'suspended';

export interface CreateUserRequest {
  username: string;
  email: string;
  fullName: string;
  phone?: string;
  password: string;
  roleId: string;
}

export interface UpdateUserRequest {
  email?: string;
  fullName?: string;
  phone?: string;
  roleId?: string;
  status?: UserStatus;
}

export interface UserListFilter {
  search?: string;
  role?: string;
  status?: UserStatus;
  page: number;
  limit: number;
}

export interface TableHeader {
  title: string;
  key: string;
  width?: string;
  align?: 'start' | 'center' | 'end';
  sortable?: boolean;
}