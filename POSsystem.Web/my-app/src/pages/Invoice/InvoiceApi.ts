// src/pages/Invoices/InvoicesApi.ts

import api from "../../api/api";

import type { Invoice } from "./InvoiceTypes";

export const InvoicesApi = {
  async getAll(): Promise<Invoice[]> {
    const response = await api.get<Invoice[]>("/invoices");
    return response.data;
  },
};