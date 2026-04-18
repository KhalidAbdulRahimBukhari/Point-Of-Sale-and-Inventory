"use client";

import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Box,
  TextField,
  MenuItem,
  Button,
  Divider,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { useState } from "react";
import ShowInvoice from "./showInvoice";
import type { PaymentDraft, SaleDraft } from "./types/sale";
import type { Invoice } from "../Invoice/InvoiceTypes";
import { cashierApi } from "./Cahier.Api";

type PaymentUIState = {
  method: "cash" | "card" | "transfer";
  tenderedAmount: number;
  notes?: string;
};

type PaymentDialogProps = {
  open: boolean;
  onClose: () => void;
  saleDraft: SaleDraft | null;
  onPaymentComplete: (payment: PaymentDraft) => void;
  onSaleComplete: (invoiceNumber: string) => void;
};

export default function PaymentDialog({
  open: paymentOpen,
  onClose,
  saleDraft,
  onPaymentComplete,
  onSaleComplete,
}: PaymentDialogProps) {
  const [invoiceOpen, setInvoiceOpen] = useState(false);
  const [payment, setPayment] = useState<PaymentUIState>({
    method: "cash",
    tenderedAmount: 0,
    notes: "",
  });

  if (!saleDraft) return null;
  const amountToPay = Math.round(saleDraft.totals.grandTotal * 100) / 100;
  const change =
    Math.round(Math.max(0, payment.tenderedAmount - amountToPay) * 100) / 100;

  const handlePay = async () => {
    if (payment.tenderedAmount < amountToPay) return;

    try {
      // Prepare payment data
      const paymentData: PaymentDraft = {
        method: payment.method,
        amountPaid: payment.tenderedAmount,
        change: change,
        notes: payment.notes,
      };

      // Create the complete sale draft with payment
      const saleData: SaleDraft = {
        ...saleDraft,
        payment: paymentData,
        createdAt: new Date().toISOString(),
      };

      // Call API to create sale
      const response = await cashierApi.createSale(saleData);

      console.log("Sale created:", response); // { saleId: X, invoiceNumber: "INV-..." }

      // Update parent state with invoice number
      onSaleComplete(response.invoiceNumber);

      // Update parent state with payment
      onPaymentComplete(paymentData);

      // Open invoice
      setInvoiceOpen(true);
    } catch (err) {
      console.error("Payment failed:", err);
      alert("Failed to process payment. Please try again.");
    }
  };

  function saleDraftToInvoice(sale: SaleDraft): Invoice {
    return {
      invoiceNo: sale.invoiceNumber ?? "TEMP",
      createdAt: sale.createdAt,
      cashier: sale.cashier.username,

      items: sale.items.map((i) => ({
        name: i.name,
        size: i.size,
        price: i.unitPrice,
        qty: i.qty,
      })),

      subtotal: sale.totals.subTotal,
      tax: sale.totals.taxAmount,
      discount: sale.totals.discountAmount,
      total: sale.totals.grandTotal,

      amountPaid: sale.payment?.amountPaid ?? 0,
      change: sale.payment?.change ?? 0,
      paymentMethod: sale.payment?.method ?? "",
    };
  }

  return (
    <>
      <Dialog open={paymentOpen} onClose={onClose} maxWidth="xs" fullWidth>
        <DialogTitle
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          Payment
          <IconButton onClick={onClose} size="small">
            <CloseIcon />
          </IconButton>
        </DialogTitle>

        <DialogContent>
          <Box
            sx={{
              display: "flex",
              flexDirection: "column",
              gap: 2.5,
              mt: 1,
            }}
          >
            <TextField
              select
              label="Payment Method"
              value={payment.method}
              size="small"
              fullWidth
              onChange={(e) =>
                setPayment((p) => ({
                  ...p,
                  method: e.target.value as "cash" | "card" | "transfer",
                }))
              }
            >
              <MenuItem value="cash">Cash</MenuItem>
              <MenuItem value="card">Card</MenuItem>
              <MenuItem value="transfer">Bank Transfer</MenuItem>
            </TextField>

            <TextField
              label="Amount to Pay"
              size="small"
              value={amountToPay}
              InputProps={{ readOnly: true }}
              fullWidth
            />

            <TextField
              label="Tendered Amount"
              size="small"
              type="number"
              fullWidth
              value={payment.tenderedAmount}
              onChange={(e) =>
                setPayment((p) => ({
                  ...p,
                  tenderedAmount: Number(e.target.value),
                }))
              }
            />

            <TextField
              label="Change"
              size="small"
              value={change}
              InputProps={{ readOnly: true }}
              fullWidth
            />

            <Divider />

            <TextField
              label="Notes (optional)"
              size="small"
              fullWidth
              multiline
              minRows={2}
              placeholder="Add any notes for this transaction..."
              value={payment.notes}
              onChange={(e) =>
                setPayment((p) => ({ ...p, notes: e.target.value }))
              }
            />
          </Box>
        </DialogContent>

        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button variant="outlined" onClick={onClose}>
            Cancel
          </Button>
          <Button variant="contained" onClick={handlePay}>
            Pay
          </Button>
        </DialogActions>
      </Dialog>

      <ShowInvoice
        open={invoiceOpen}
        onClose={() => setInvoiceOpen(false)}
        invoice={saleDraftToInvoice(saleDraft)}
      />
    </>
  );
}
