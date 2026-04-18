"use client";

import { useState, useEffect, useMemo } from "react";
import { Autocomplete } from "@mui/material";
import {
  Box,
  TextField,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Typography,
  InputAdornment,
  Divider,
  Card,
  CardContent,
  Button,
} from "@mui/material";
import { Search, Trash2 } from "lucide-react";
import PaymentDialog from "./PaymentDialog";
import { stockApi } from "../StockIn/StockApi";
import type { PaymentDraft, SaleDraft, SaleItemDraft } from "./types/sale";

interface Product {
  productId: number;
  variantId: number;
  name: string;
  category: string;
  currentStock: number;
  sellingPrice: number;
  barcode?: string;
  sku?: string;
  brand?: string;
  description?: string;
  size?: string;
  color?: string;
  imagePath?: string;
  costPrice?: number;
  Qty: number;
}

interface SaleTotals {
  SubTotal: number;
  TaxPercentage: number;
  TaxAmount: number;
  DiscountPercentage: number;
  DiscountAmount: number;
  GrandTotal: number;
}

const TAX_PERCENTAGE = 15;
const MAX_DISCOUNT = 15;

export default function POSSystem() {
  const [paymentOpen, setPaymentOpen] = useState(false);

  const [products, setProducts] = useState<Product[]>([]);
  const [searchQuery, setSearchQuery] = useState("");

  const [CartItems, setCartItems] = useState<Product[]>([]);

  const [DiscountPercentage, setDiscountPercentage] = useState(0);

  const [saleDraft, setSaleDraft] = useState<SaleDraft | null>(null);

  const buildSaleDraft = (): SaleDraft => {
    const userId = localStorage.getItem("userId");
    const username = localStorage.getItem("username");

    const items: SaleItemDraft[] = CartItems.map((item) => ({
      productId: item.productId,
      variantId: item.variantId,
      name: item.name,
      size: item.size,
      color: item.color,
      unitPrice: item.sellingPrice,
      qty: item.Qty,
      taxAmount: (item.sellingPrice * item.Qty * TAX_PERCENTAGE) / 100,
      total: item.sellingPrice * item.Qty,
    }));

    return {
      shopId: 1,
      currencyCode: "USD",
      cashier: {
        userId: parseInt(userId ?? "0"),
        username: username ?? "Unknown",
      },
      items,
      totals: {
        subTotal: Summary.SubTotal,
        taxPercentage: Summary.TaxPercentage,
        taxAmount: Summary.TaxAmount,
        discountPercentage: Summary.DiscountPercentage,
        discountAmount: Summary.DiscountAmount,
        grandTotal: Summary.GrandTotal,
      },
      createdAt: new Date().toISOString(),
    };
  };

  const handlePaymentComplete = (payment: PaymentDraft) => {
    setSaleDraft((prev) => {
      if (!prev) return prev;
      return {
        ...prev,
        payment,
      };
    });

    setPaymentOpen(false);
  };
  const handleSaleComplete = (invoiceNumber: string) => {
    setSaleDraft((prev) => {
      if (!prev) return prev;
      return {
        ...prev,
        invoiceNumber,
      };
    });
  };

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const data = await stockApi.getProducts();
        setProducts(data);
      } catch (err) {
        console.error("Error fetching products:", err);
      }
    };

    fetchProducts();
  }, []);

  const filteredProducts = useMemo(() => {
    const query = searchQuery.toLowerCase().trim();

    return products
      .map((product) => {
        const inCartQty =
          CartItems.find((p) => p.productId === product.productId)?.Qty || 0;
        const available = product.currentStock - inCartQty;
        return { ...product, availableStock: available };
      })
      .filter((product) => product.availableStock > 0) // hide sold-out
      .filter((product) => {
        if (!query) return true;
        return (
          product.name.toLowerCase().includes(query) ||
          product.productId.toString().includes(query) ||
          (product.barcode && product.barcode.includes(query)) ||
          (product.sku && product.sku.toLowerCase().includes(query))
        );
      });
  }, [products, searchQuery, CartItems]);

  const handleDeleteClick = (productId: number) => {
    setCartItems((prev) => prev.filter((p) => p.productId !== productId));
  };

  const calculateSummary = (
    items: Product[],
    discountPercentage: number,
  ): SaleTotals => {
    const SubTotal = items.reduce(
      (sum, item) => sum + item.sellingPrice * item.Qty,
      0,
    );

    const TaxAmount = (SubTotal * TAX_PERCENTAGE) / 100;
    const DiscountAmount =
      (SubTotal * Math.min(discountPercentage, MAX_DISCOUNT)) / 100;

    return {
      SubTotal,
      TaxPercentage: TAX_PERCENTAGE,
      TaxAmount,
      DiscountPercentage: Math.min(discountPercentage, MAX_DISCOUNT),
      DiscountAmount,
      GrandTotal: SubTotal + TaxAmount - DiscountAmount,
    };
  };

  const Summary = calculateSummary(CartItems, DiscountPercentage);

  const handleCancel = () => {
    setCartItems([]);
    setDiscountPercentage(0);
  };

  const handleDiscountChange = (value: number) => {
    if (value < 0) return;
    setDiscountPercentage(Math.min(value, MAX_DISCOUNT));
  };

  return (
    <>
      <Box
        sx={{
          display: "flex",
          gap: 2,
          flex: 1,
          minHeight: 0,
          width: "100%",
        }}
      >
        {/* LEFT */}
        <Box
          sx={{
            flex: 1,
            display: "flex",
            flexDirection: "column",
            minWidth: 0,
            minHeight: 0,
          }}
        >
          <Autocomplete
            fullWidth
            size="small"
            options={filteredProducts}
            getOptionLabel={(option) =>
              ` Id: ${option.productId} | Name: ${option.name} | Size: ${option.size} | remaining: ${option.availableStock} `
            }
            value={null} // start empty
            onChange={(_, selected) => {
              if (!selected) return;

              setCartItems((prev) => {
                const existing = prev.find(
                  (p) => p.productId === selected.productId,
                );
                const availableStock =
                  selected.currentStock - (existing?.Qty || 0);

                if (existing) {
                  if (existing.Qty >= selected.currentStock) return prev; // cannot exceed stock
                  return prev.map((p) =>
                    p.productId === selected.productId
                      ? { ...p, Qty: p.Qty + 1 }
                      : p,
                  );
                }

                if (availableStock <= 0) return prev; // can't add if none left
                return [...prev, { ...selected, Qty: 1 }];
              });

              setSearchQuery(""); // clear input
            }}
            inputValue={searchQuery}
            onInputChange={(_, newValue) => setSearchQuery(newValue)}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Search by Product Name, ID, Barcode or SKU"
                placeholder="Type to search products..."
                InputProps={{
                  ...params.InputProps,
                  startAdornment: (
                    <InputAdornment position="start">
                      <Search size={18} />
                    </InputAdornment>
                  ),
                }}
                sx={{ mb: 2, flexShrink: 0 }}
              />
            )}
          />

          <Paper
            sx={{
              flex: 1,
              display: "flex",
              flexDirection: "column",
              minHeight: 0,
              overflow: "hidden",
            }}
          >
            <TableContainer sx={{ flex: 1, overflow: "auto" }}>
              <Table stickyHeader size="small">
                <TableHead>
                  <TableRow>
                    {[
                      "Product ID",
                      "Name",
                      "Size",
                      "Unit Price",
                      "Qty",
                      "Total",
                      "",
                    ].map((h) => (
                      <TableCell
                        key={h}
                        align={
                          h.includes("Price") || h === "Total"
                            ? "right"
                            : h === "" || h === "Qty"
                              ? "center"
                              : "left"
                        }
                        sx={{
                          bgcolor: "primary.main",
                          color: "primary.contrastText",
                          fontWeight: "bold",
                        }}
                      >
                        {h}
                      </TableCell>
                    ))}
                  </TableRow>
                </TableHead>

                <TableBody>
                  {CartItems.map((p) => (
                    <TableRow key={p.productId} hover>
                      <TableCell>{p.productId}</TableCell>
                      <TableCell>{p.name}</TableCell>
                      <TableCell>{p.size}</TableCell>
                      <TableCell align="right">
                        ${p.sellingPrice.toFixed(2)}
                      </TableCell>
                      <TableCell align="center">
                        <Typography sx={{ textAlign: "center" }}>
                          {p.Qty}
                        </Typography>
                      </TableCell>
                      <TableCell align="right">
                        ${(p.sellingPrice * p.Qty).toFixed(2)}
                      </TableCell>
                      <TableCell align="center">
                        <IconButton
                          color="error"
                          size="small"
                          onClick={() => handleDeleteClick(p.productId)}
                        >
                          <Trash2 size={16} />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>

          <Box sx={{ display: "flex", gap: 2, mt: 2 }}>
            <Button
              fullWidth
              variant="outlined"
              color="error"
              size="large"
              onClick={handleCancel}
            >
              Cancel
            </Button>
            <Button
              fullWidth
              variant="contained"
              color="success"
              size="large"
              disabled={CartItems.length === 0}
              onClick={() => {
                setSaleDraft(buildSaleDraft());
                setPaymentOpen(true);
              }}
            >
              Proceed
            </Button>
          </Box>
        </Box>

        {/* RIGHT */}
        <Card sx={{ width: 340, display: "flex", flexDirection: "column" }}>
          <CardContent
            sx={{ display: "flex", flexDirection: "column", gap: 2 }}
          >
            <Typography variant="h6" fontWeight="bold">
              Sale Totals
            </Typography>

            <TextField
              label="Subtotal"
              value={`$${Summary.SubTotal.toFixed(2)}`}
              size="small"
              InputProps={{ readOnly: true }}
              fullWidth
            />

            <TextField
              label="Tax (%)"
              value={Summary.TaxPercentage}
              size="small"
              InputProps={{ readOnly: true }}
              fullWidth
            />

            <TextField
              label="Tax Amount"
              value={`$${Summary.TaxAmount.toFixed(2)}`}
              size="small"
              InputProps={{ readOnly: true }}
              fullWidth
            />

            <TextField
              label="Discount (%)"
              type="number"
              size="small"
              value={DiscountPercentage}
              onChange={(e) => handleDiscountChange(Number(e.target.value))}
              inputProps={{ min: 0, max: MAX_DISCOUNT }}
              fullWidth
            />

            <TextField
              label="Discount Amount"
              value={`$${Summary.DiscountAmount.toFixed(2)}`}
              size="small"
              InputProps={{ readOnly: true }}
              fullWidth
            />

            <Divider />

            <TextField
              label="Grand Total"
              value={`$${Summary.GrandTotal.toFixed(2)}`}
              InputProps={{
                readOnly: true,
                sx: { fontWeight: "bold", fontSize: "1.25rem" },
              }}
              fullWidth
            />
          </CardContent>
        </Card>
      </Box>

      <PaymentDialog
        key={saleDraft?.createdAt}
        open={paymentOpen}
        onClose={() => setPaymentOpen(false)}
        saleDraft={saleDraft}
        onPaymentComplete={handlePaymentComplete}
        onSaleComplete={handleSaleComplete} // ← Add this
      />
    </>
  );
}
