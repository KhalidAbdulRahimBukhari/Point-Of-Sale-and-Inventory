// src/pages/Invoices/types.ts

export interface InvoiceItem {
  name: string;
  size?: string | null;
  price: number;
  qty: number;
}

export interface Invoice {
  invoiceNo: string;
  createdAt: string;
  cashier: string;

  items: InvoiceItem[];

  subtotal: number;
  tax: number;
  discount: number;
  total: number;

  amountPaid: number;
  change: number;
  paymentMethod: string;
}


export interface InvoiceRow {
  invoiceNo: string;
  cashier: string;
  paymentMethod: string;
  status: "Completed";
  createdAt: string; // formatted for display
}

