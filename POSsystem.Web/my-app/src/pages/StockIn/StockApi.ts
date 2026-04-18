// StockApi.ts
import api from "../../api/api";

/* =======================
   Types
======================= */

export interface ProductPayload {
  name: string;
  categoryId: number;
  barcode: string;
  sellingPrice: number;
  currentStock: number; // ADD THIS
  brand?: string;
  description?: string;
  sku?: string;
  costPrice?: number;
  size?: string;
  color?: string;
  imagePath?: string;
}


export interface ProductCreatePayload extends ProductPayload {
  initialStock: number;
}

/* =======================
   API
======================= */

export const stockApi = {
  /* ---------- Products ---------- */

  getProducts: async () => {
    const res = await api.get("/products");
    return res.data;
  },

  createProduct: async (payload: ProductCreatePayload) => {
    const res = await api.post("/products", payload);
    return res.data;
  },

  updateProduct: async (variantId: number, payload: ProductPayload) => {
    const res = await api.put(`/products/${variantId}`, payload);
    return res.data;
  },

  /* ---------- Categories ---------- */

  getCategories: async () => {
    const res = await api.get("/products/categories");
    return res.data;
  },

  /* ---------- Stock ---------- */

  stockIn: async (variantId: number, quantity: number) => {
    const res = await api.post(`/products/${variantId}/stock`, null, {
      params: { quantity },
    });
    return res.data;
  },
};
