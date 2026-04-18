// cashierApi.ts
import api from "../../api/api";
import type { SaleDraft } from "./types/sale";

export const cashierApi = {
  /* ---------- Add Sale ---------- */
  createSale: async (saleDraft: SaleDraft) => {
    const response = await api.post("/sales", saleDraft);
    return response.data; // { saleId: number, invoiceNumber: string }
  },
};