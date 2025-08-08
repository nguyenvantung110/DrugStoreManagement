export interface Customer {
  id: string;
  customerCode: string;
  fullName: string;
  dateOfBirth?: Date;
  gender: CustomerGender;
  phone: string;
  email?: string;
  address?: CustomerAddress;
  identityCard?: string;
  insuranceCard?: string;
  allergyHistory?: string[];
  medicalHistory?: MedicalRecord[];
  loyaltyPoints: number;
  totalSpent: number;
  status: CustomerStatus;
  createdAt: Date;
  updatedAt: Date;
  lastVisit?: Date;
}

export interface CustomerAddress {
  street: string;
  ward: string;
  district: string;
  city: string;
  zipCode?: string;
}

export interface MedicalRecord {
  id: string;
  date: Date;
  diagnosis: string;
  medications: string[];
  doctor: string;
  notes?: string;
}

export type CustomerGender = 'male' | 'female' | 'other';
export type CustomerStatus = 'active' | 'inactive' | 'blocked';

export interface CreateCustomerRequest {
  fullName: string;
  dateOfBirth?: Date;
  gender: CustomerGender;
  phone: string;
  email?: string;
  address?: CustomerAddress;
  identityCard?: string;
  insuranceCard?: string;
  allergyHistory?: string[];
}

export interface UpdateCustomerRequest {
  fullName?: string;
  dateOfBirth?: Date;
  gender?: CustomerGender;
  phone?: string;
  email?: string;
  address?: CustomerAddress;
  identityCard?: string;
  insuranceCard?: string;
  allergyHistory?: string[];
  status?: CustomerStatus;
}

export interface CustomerListFilter {
  search?: string;
  gender?: CustomerGender;
  status?: CustomerStatus;
  ageRange?: AgeRange;
  city?: string;
  page: number;
  limit: number;
}

export interface AgeRange {
  min?: number;
  max?: number;
}

export interface TableHeader {
  title: string;
  key: string;
  width?: string;
  align?: 'start' | 'center' | 'end';
  sortable?: boolean;
}

export interface PurchaseHistory {
  id: string;
  customerId: string;
  transactionId: string;
  date: Date;
  totalAmount: number;
  itemCount: number;
  paymentMethod: string;
  items: PurchaseItem[];
  notes?: string;
}

export interface PurchaseItem {
  drugId: string;
  drugName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  batchNumber?: string;
}