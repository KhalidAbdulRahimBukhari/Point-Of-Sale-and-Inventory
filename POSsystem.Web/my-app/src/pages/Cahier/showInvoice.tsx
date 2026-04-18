'use client';

import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Box,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Divider,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import PrintIcon from "@mui/icons-material/Print";
import DownloadIcon from "@mui/icons-material/Download";
import type { Invoice } from "../Invoice/InvoiceTypes";


type InvoiceProps = {
  open: boolean;
  onClose: () => void;
  invoice: Invoice | null;
};


export default function ShowInvoice({ open, onClose, invoice }: InvoiceProps) {

  if (!invoice) return null;


const invoiceData = {
  invoiceNo: invoice.invoiceNo,
  date: new Date(invoice.createdAt).toLocaleDateString(),
  time: new Date(invoice.createdAt).toLocaleTimeString(),
  cashier: invoice.cashier,

  items: invoice.items.map((item, index) => ({
    id: index, // UI-only key, NOT business data
    name: item.name,
    size: item.size ?? null,
    price: item.price,
    qty: item.qty,
  })),

  subtotal: invoice.subtotal,
  tax: invoice.tax,
  discount: invoice.discount,
  total: invoice.total,

  amountPaid: invoice.amountPaid,
  change: invoice.change,
  paymentMethod: invoice.paymentMethod,
};

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="sm"
      fullWidth
      PaperProps={{
        sx: {
          maxHeight: "90vh",
          display: "flex",
          flexDirection: "column",
        },
      }}
    >
      <DialogTitle
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          borderBottom: 1,
          borderColor: "divider",
        }}
      >
        <Typography variant="h6" fontWeight="bold">
          Invoice
        </Typography>
        <IconButton onClick={onClose} size="small" edge="end">
          <CloseIcon />
        </IconButton>
      </DialogTitle>

      <DialogContent
        sx={{
          flex: 1,
          overflow: "auto",
          p: 3,
        }}
      >
        {/* Invoice Header */}
        <Box sx={{ textAlign: "center", mb: 3 }}>
          <Typography variant="h5" fontWeight="bold" gutterBottom>
            Urban Wear Germany
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Haupbahnhof Street, Dresden, Germany
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Tel: +49 115 189 457
          </Typography>
        </Box>

        <Divider sx={{ mb: 3 }} />

        {/* Invoice Info */}
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr",
            gap: 1,
            mb: 3,
          }}
        >
          <Typography variant="body2">
            <strong>Invoice No:</strong> {invoiceData.invoiceNo}
          </Typography>
          <Typography variant="body2" textAlign="right">
            <strong>Date:</strong> {invoiceData.date}
          </Typography>
          <Typography variant="body2">
            <strong>Cashier:</strong> {invoiceData.cashier}
          </Typography>
          <Typography variant="body2" textAlign="right">
            <strong>Time:</strong> {invoiceData.time}
          </Typography>
        </Box>

        {/* Items Table */}
        <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
          <Table size="small">
            <TableHead>
              <TableRow sx={{ bgcolor: "grey.100" }}>
                <TableCell sx={{ fontWeight: "bold" }}>Item</TableCell>
                <TableCell align="center" sx={{ fontWeight: "bold" }}>
                  Qty
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: "bold" }}>
                  Price
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: "bold" }}>
                  Total
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {invoiceData.items.map((item) => (
                <TableRow key={item.id}>
                  <TableCell>
                    <Typography variant="body2">{item.name}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {item.size}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">{item.qty}</TableCell>
                  <TableCell align="right">${item.price.toFixed(2)}</TableCell>
                  <TableCell align="right">
                    ${(item.price * item.qty).toFixed(2)}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        {/* Totals */}
        <Box sx={{ display: "flex", flexDirection: "column", gap: 1, mb: 3 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="body2">Subtotal:</Typography>
            <Typography variant="body2">
              ${invoiceData.subtotal.toFixed(2)}
            </Typography>
          </Box>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="body2">Tax:</Typography>
            <Typography variant="body2">
              ${invoiceData.tax.toFixed(2)}
            </Typography>
          </Box>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="body2">Discount:</Typography>
            <Typography variant="body2">
              -${invoiceData.discount.toFixed(2)}
            </Typography>
          </Box>
          <Divider />
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="h6" fontWeight="bold">
              Total:
            </Typography>
            <Typography variant="h6" fontWeight="bold">
              ${invoiceData.total.toFixed(2)}
            </Typography>
          </Box>
        </Box>

        <Divider sx={{ mb: 2 }} />

        {/* Payment Info */}
        <Box sx={{ display: "flex", flexDirection: "column", gap: 0.5, mb: 3 }}>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="body2">Payment Method:</Typography>
            <Typography variant="body2">{invoiceData.paymentMethod}</Typography>
          </Box>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="body2">Amount Paid:</Typography>
            <Typography variant="body2">
              ${invoiceData.amountPaid.toFixed(2)}
            </Typography>
          </Box>
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Typography variant="body2" fontWeight="bold">
              Change:
            </Typography>
            <Typography variant="body2" fontWeight="bold">
              ${invoiceData.change.toFixed(2)}
            </Typography>
          </Box>
        </Box>

        {/* Footer */}
        <Box sx={{ textAlign: "center", mt: 3 }}>
          <Typography variant="body2" color="text.secondary">
            Thank you for your purchase!
          </Typography>
          <Typography variant="caption" color="text.secondary">
            Please keep this receipt for your records.
          </Typography>
        </Box>
      </DialogContent>

      <DialogActions
        sx={{
          px: 3,
          pb: 2.5,
          pt: 1.5,
          borderTop: 1,
          borderColor: "divider",
          gap: 1,
        }}
      >
        <Button variant="outlined" onClick={onClose}>
          Close
        </Button>
        <Button variant="outlined" startIcon={<DownloadIcon />}>
          Download
        </Button>
        <Button variant="contained" startIcon={<PrintIcon />}>
          Print
        </Button>
      </DialogActions>
    </Dialog>
  );
}
