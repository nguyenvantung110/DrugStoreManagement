export interface DrugItem {
  seq: number;
  productId: string;
  productName: string;
  quantity: number;
  unitOfMeasure: string;
  dosage: string;
  pricePerUnit: number;
  manufacturer?: string;
  subTotal: number;
  id?: string; // ID thuốc từ database
  barcode?: string; // Mã vạch
  expiryDate?: Date; // Hạn sử dụng
  batchNumber?: string; // Số lô
}

export interface SalesTransaction {
  id?: string;
  drugItems: DrugItem[];
  grandTotal: number;
  notes?: string;
  createdAt: Date;
  customerId?: string;
  staffId: string;
  paymentMethod: 'cash' | 'card' | 'transfer';
  status: 'pending' | 'completed' | 'cancelled';
}

export interface TableHeader {
  title: string;
  key: string;
  width?: string;
  align?: 'start' | 'center' | 'end';
  sortable?: boolean;
}

export interface OrderCreateDto {
  customer: CustomerForOrderCreate;
  userId: string;
  paymentMethod: string;
  products: ProductForOrder[];
  grandTotal: number;
  notes?: string;
}

export interface CustomerForOrderCreate {
  customerName: string;
  phoneNumber: string;
}

export interface ProductForOrder {
  productId: string;
  dosage: string;
  subTotal: number;
  unitPrice?: number;
  quantity?: number;
}