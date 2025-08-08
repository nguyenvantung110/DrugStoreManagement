export interface DrugItem {
  seq: number;
  name: string;
  quantity: number;
  unit: string;
  dosage: string;
  unitPrice: number;
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