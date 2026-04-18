import { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";
import { stockApi } from "./StockApi";

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
}

interface Category {
  categoryId: number;
  name: string;
}

interface ProductAndVariantDialogProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (updatedProduct?: Product) => void;
  editMode: boolean;
  productData?: Product | null;
}

interface FormData {
  name: string;
  description: string;
  brand: string;
  categoryId: string;
  categoryName?: string;
  imagePath: string;
  size: string;
  color: string;
  barcode: string;
  sku: string;
  costPrice: string;
  sellingPrice: string;
  CurrentStock: string;
}

export default function ProductAndVariantDialog({
  open,
  onClose,
  onSuccess,
  editMode,
  productData,
}: ProductAndVariantDialogProps) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [formData, setFormData] = useState<FormData>({
    name: "",
    description: "",
    brand: "",
    categoryId: "",
    categoryName: "",
    imagePath: "",
    size: "",
    color: "",
    barcode: "",
    sku: "",
    costPrice: "",
    sellingPrice: "",
    CurrentStock: "0",
  });

  const [ui, setUi] = useState({
    loading: false,
    error: "",
  });

  useEffect(() => {
    if (open) {
      fetchCategories();
      if (editMode && productData) {
        loadProductData();
      } else {
        resetForm();
      }
    }
  }, [editMode, productData, open]);

  const fetchCategories = async () => {
    try {
      const data = await stockApi.getCategories();
      setCategories(data);
    } catch {
      setUi((prev) => ({ ...prev, error: "Failed to load categories" }));
    }
  };

  const loadProductData = () => {
    if (!productData) return;

    const category = categories.find((c) => c.name === productData.category);

    setFormData({
      name: productData.name || "",
      description: productData.description || "",
      brand: productData.brand || "",
      categoryId: category?.categoryId.toString() || "",
      categoryName: productData.category || "",
      imagePath: productData.imagePath || "",
      size: productData.size || "",
      color: productData.color || "",
      barcode: productData.barcode || "",
      sku: productData.sku || "",
      costPrice: productData.costPrice?.toString() || "",
      sellingPrice: productData.sellingPrice?.toString() || "",
      CurrentStock: productData.currentStock?.toString() || "0",
    });
  };

  const handleSubmit = async () => {
    if (!formData.name.trim()) {
      setUi((prev) => ({ ...prev, error: "Product name is required" }));
      return;
    }

    if (!formData.barcode.trim()) {
      setUi((prev) => ({ ...prev, error: "Barcode is required" }));
      return;
    }

    if (!formData.sellingPrice || Number(formData.sellingPrice) <= 0) {
      setUi((prev) => ({ ...prev, error: "Valid selling price is required" }));
      return;
    }

    if (!formData.categoryId) {
      setUi((prev) => ({ ...prev, error: "Category is required" }));
      return;
    }

    setUi((prev) => ({ ...prev, loading: true, error: "" }));

    try {
      const basePayload = {
        name: formData.name.trim(),
        categoryId: Number(formData.categoryId),
        barcode: formData.barcode.trim(),
        sellingPrice: Number(formData.sellingPrice),
        currentStock: Number(formData.CurrentStock), // ADD THIS
        costPrice: formData.costPrice ? Number(formData.costPrice) : 0,

        // OPTIONALS — undefined, NOT null
        description: formData.description || undefined,
        brand: formData.brand || undefined,
        imagePath: formData.imagePath || undefined,
        sku: formData.sku || undefined,
        size: formData.size || undefined,
        color: formData.color || undefined,
      };

      let result;

      if (editMode && productData) {
        // UPDATE — no initialStock
        result = await stockApi.updateProduct(
          productData.variantId,
          basePayload,
        );
      } else {
        // CREATE — initialStock REQUIRED
        result = await stockApi.createProduct({
          ...basePayload,
          initialStock: Number(formData.CurrentStock) || 0,
        });
      }

      onSuccess(result);
      resetForm();
      onClose();
    } catch (err) {
      setUi((prev) => ({
        ...prev,
        error:
          err instanceof Error
            ? err.message
            : `Failed to ${editMode ? "update" : "create"} product`,
      }));
    } finally {
      setUi((prev) => ({ ...prev, loading: false }));
    }
  };

  const resetForm = () => {
    setFormData({
      name: "",
      description: "",
      brand: "",
      categoryId: "",
      categoryName: "",
      imagePath: "",
      size: "",
      color: "",
      barcode: "",
      sku: "",
      costPrice: "",
      sellingPrice: "",
      CurrentStock: "0",
    });
    setUi({
      loading: false,
      error: "",
    });
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const updateField = (field: keyof FormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>{editMode ? "Edit Product" : "New Product"}</DialogTitle>

      <DialogContent>
        {ui.error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {ui.error}
          </Alert>
        )}

        <Grid container spacing={2} sx={{ mt: 1 }}>
          <Grid size={{ xs: 12 }}>
            <TextField
              label="Product Name *"
              value={formData.name}
              onChange={(e) => updateField("name", e.target.value)}
              fullWidth
              autoFocus
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Brand"
              value={formData.brand}
              onChange={(e) => updateField("brand", e.target.value)}
              fullWidth
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <FormControl fullWidth>
              <InputLabel>Category *</InputLabel>
              <Select
                value={formData.categoryId}
                onChange={(e) => updateField("categoryId", e.target.value)}
                label="Category *"
              >
                <MenuItem value="">Select Category</MenuItem>
                {categories.map((category) => (
                  <MenuItem
                    key={category.categoryId}
                    value={category.categoryId}
                  >
                    {category.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid size={{ xs: 12 }}>
            <TextField
              label="Description"
              value={formData.description}
              onChange={(e) => updateField("description", e.target.value)}
              fullWidth
              multiline
              rows={2}
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <TextField
              label="Image Path (optional)"
              value={formData.imagePath}
              onChange={(e) => updateField("imagePath", e.target.value)}
              fullWidth
              placeholder="/images/products/..."
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <hr
              style={{
                border: "none",
                borderTop: "1px solid #e0e0e0",
                margin: "16px 0",
              }}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Size (optional)"
              value={formData.size}
              onChange={(e) => updateField("size", e.target.value)}
              fullWidth
              placeholder="S, M, L, XL..."
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Color (optional)"
              value={formData.color}
              onChange={(e) => updateField("color", e.target.value)}
              fullWidth
              placeholder="Red, Blue, Black..."
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Barcode *"
              value={formData.barcode}
              onChange={(e) => updateField("barcode", e.target.value)}
              fullWidth
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="SKU (optional)"
              value={formData.sku}
              onChange={(e) => updateField("sku", e.target.value)}
              fullWidth
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Cost Price (optional)"
              type="number"
              value={formData.costPrice}
              onChange={(e) => updateField("costPrice", e.target.value)}
              fullWidth
              InputProps={{
                inputProps: { min: 0, step: 0.01 },
              }}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Selling Price *"
              type="number"
              value={formData.sellingPrice}
              onChange={(e) => updateField("sellingPrice", e.target.value)}
              fullWidth
              InputProps={{
                inputProps: { min: 0.01, step: 0.01 },
              }}
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <TextField
              label="Initial Stock"
              type="number"
              value={formData.CurrentStock}
              onChange={(e) => updateField("CurrentStock", e.target.value)}
              fullWidth
              InputProps={{
                inputProps: { min: 0 },
              }}
            />
          </Grid>
        </Grid>
      </DialogContent>

      <DialogActions>
        <Button onClick={handleClose} disabled={ui.loading}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={ui.loading}
        >
          {ui.loading
            ? editMode
              ? "Updating..."
              : "Creating..."
            : editMode
              ? "Update Product"
              : "Create Product"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
