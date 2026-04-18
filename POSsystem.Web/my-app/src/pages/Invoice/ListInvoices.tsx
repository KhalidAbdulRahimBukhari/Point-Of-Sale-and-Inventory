import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Alert,
  CircularProgress,
  Chip,
  IconButton,
} from "@mui/material";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";
import { Search } from "@mui/icons-material";
import { useListInvoices } from "./useListInvoices";
import ShowInvoice from "../Cahier/showInvoice";
import { useState } from "react";
import type { Invoice } from "./InvoiceTypes";

export default function Invoices() {
  const {
    invoices,
    filteredInvoices, // solve this problem !! how to filter ?? should we filter here ?
    loading,
    error,
    searchTerm,
    setSearchTerm,
  } = useListInvoices();

  const [selectedInvoice, setSelectedInvoice] = useState<Invoice | null>(null);
  const [open, setOpen] = useState(false);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box p={3}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" mb={3}>
        <Box>
          <Typography variant="h4" fontWeight="bold">
            Invoices
          </Typography>
          <Typography color="text.secondary">
            {invoices.length} invoices
          </Typography>
        </Box>
      </Box>

      {/* Search */}
      <TextField
        fullWidth
        placeholder="Search by invoice number..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        InputProps={{ startAdornment: <Search /> }}
        sx={{ mb: 3 }}
      />

      {/* Error */}
      {error && <Alert severity="error">{error}</Alert>}

      {/* Table */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Invoice #</TableCell>
              <TableCell>Cashier</TableCell>
              <TableCell>Payment Method</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Date</TableCell>
              <TableCell /> {/* actions */}
            </TableRow>
          </TableHead>

          <TableBody>
            {filteredInvoices.map((invoice) => (
              <TableRow key={invoice.invoiceNo}>
                <TableCell>{invoice.invoiceNo}</TableCell>
                <TableCell>{invoice.cashier}</TableCell>
                <TableCell>{invoice.paymentMethod || "N/A"}</TableCell>
                <TableCell>
                  <Chip label="Completed" size="small" color="success" />
                </TableCell>
                <TableCell>
                  {new Date(invoice.createdAt).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => {
                      setSelectedInvoice(invoice);
                      setOpen(true);
                    }}
                  >
                    <ReceiptLongIcon fontSize="small" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Empty state */}
      {filteredInvoices.length === 0 && (
        <Box textAlign="center" py={6}>
          <Typography color="text.secondary">No invoices found</Typography>
        </Box>
      )}

      <ShowInvoice
        open={open}
        onClose={() => {
          setOpen(false);
          setSelectedInvoice(null);
        }}
        invoice={selectedInvoice}
      />
    </Box>
  );
}
