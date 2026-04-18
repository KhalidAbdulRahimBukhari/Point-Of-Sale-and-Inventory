// src/pages/Invoices/useInvoice.ts

import { useEffect, useState } from "react";
import { InvoicesApi } from "./InvoiceApi";
import type { Invoice} from "./InvoiceTypes";

export function useListInvoices() {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [searchTerm, setSearchTerm] = useState("");

  const loadInvoices = async () => {
    try {
      setLoading(true);

      const data = await InvoicesApi.getAll();
      setInvoices(data);


    } catch {
      setError("Failed to load invoices");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadInvoices();
  }, []);

  const term = searchTerm.toLowerCase();

  const filteredInvoices = invoices.filter((i) =>
    i.invoiceNo.toLowerCase().includes(term),
  );

  return {
    invoices, // 🔥 full invoices for details / print
    filteredInvoices, // table uses this
    loading,
    error,
    searchTerm,
    setSearchTerm,
  };
}
