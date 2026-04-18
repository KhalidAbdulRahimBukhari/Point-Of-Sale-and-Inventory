// types/sale.ts

export type SaleItemDraft = {
  productId: number;
  variantId: number;
  name: string;
  size?: string;
  color?: string;
  unitPrice: number;
  qty: number;
  taxAmount: number;
  total: number;
};

export type SaleTotals = {
  subTotal: number;
  taxPercentage: number;
  taxAmount: number;
  discountPercentage: number;
  discountAmount: number;
  grandTotal: number;
};

export type PaymentDraft = {
  method: "cash" | "card" | "transfer";
  amountPaid: number;
  change: number;
  notes?: string;
};

export type SaleDraft = {
  shopId: number;
  currencyCode: "USD";
  cashier: {
    userId: number;
    username: string;
  };
  items: SaleItemDraft[];
  totals: SaleTotals;
  payment?: PaymentDraft;
  invoiceNumber?: string;
  createdAt: string;
};




