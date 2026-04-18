import { useState, useEffect, useMemo } from "react";
import {
  Box,
  Typography,
  Button,
  Paper,
  Grid,
  TextField,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  InputAdornment,
  Alert,
  CircularProgress,
} from "@mui/material";
import {
  Search as SearchIcon,
  Add as AddIcon,
  Edit as EditIcon,
} from "@mui/icons-material";
import ProductAndVariantDialog from "./ProductAndVariantDialog";
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

export default function StockIn() {
  // Only a single value for quantity
  const [quantity, setQuantity] = useState<number>(1);

  const [products, setProducts] = useState<Product[]>([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const [uiState, setUiState] = useState({
    loading: false,
    error: "",
    success: "",
    productDialogOpen: false,
    editMode: false,
  });

  const [productToEdit, setProductToEdit] = useState<Product | null>(null);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      const data = await stockApi.getProducts();
      setProducts(data);
    } catch {
      setUiState((prev) => ({ ...prev, error: "Failed to load products" }));
    }
  };

  const filteredProducts = useMemo(() => {
    if (!searchQuery.trim()) return products;
    const query = searchQuery.toLowerCase().trim();
    return products.filter(
      (product) =>
        product.name.toLowerCase().includes(query) ||
        product.productId.toString().includes(query) ||
        (product.barcode && product.barcode.includes(query)) ||
        (product.sku && product.sku.toLowerCase().includes(query)),
    );
  }, [searchQuery, products]);

  const handleStockIn = async () => {
    if (!selectedProduct) {
      setUiState((prev) => ({
        ...prev,
        error: "Please select a product from the table",
      }));
      return;
    }
    if (quantity <= 0) {
      setUiState((prev) => ({
        ...prev,
        error: "Quantity must be greater than 0",
      }));
      return;
    }

    setUiState((prev) => ({ ...prev, loading: true, error: "" }));

    try {
      await stockApi.stockIn(selectedProduct.variantId, quantity);

      await fetchProducts();

      setQuantity(1);
      setSelectedProduct(null);

      setUiState((prev) => ({
        ...prev,
        success: `Added ${quantity} units to ${selectedProduct.name}`,
      }));

      setTimeout(() => setUiState((prev) => ({ ...prev, success: "" })), 3000);
    } catch (err) {
      setUiState((prev) => ({
        ...prev,
        error: err instanceof Error ? err.message : "Failed to add stock",
      }));
    } finally {
      setUiState((prev) => ({ ...prev, loading: false }));
    }
  };

  const handleEditProduct = (product: Product) => {
    setProductToEdit(product);
    setUiState((prev) => ({
      ...prev,
      editMode: true,
      productDialogOpen: true,
    }));
  };

  const handleProductCreated = (newProduct?: Product) => {
    if (newProduct && uiState.editMode && productToEdit) {
      const updatedProducts = products.map((p) =>
        p.variantId === productToEdit.variantId ? newProduct : p,
      );
      setProducts(updatedProducts);
    } else if (!uiState.editMode && newProduct) {
      setProducts((prev) => [newProduct, ...prev]);
    }
    setUiState((prev) => ({
      ...prev,
      productDialogOpen: false,
      editMode: false,
    }));
    setProductToEdit(null);
  };

  const handleProductDialogClose = () => {
    setUiState((prev) => ({
      ...prev,
      productDialogOpen: false,
      editMode: false,
    }));
    setProductToEdit(null);
  };

  const getStatusColor = (stock: number): "success" | "error" | "default" =>
    stock > 0 ? "success" : "error";
  const getStatusText = (stock: number): string =>
    stock > 0 ? "In stock" : "Out of stock";

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Inventory Management
      </Typography>

      {uiState.error && (
        <Alert
          severity="error"
          sx={{ mb: 2 }}
          onClose={() => setUiState((prev) => ({ ...prev, error: "" }))}
        >
          {uiState.error}
        </Alert>
      )}
      {uiState.success && (
        <Alert
          severity="success"
          sx={{ mb: 2 }}
          onClose={() => setUiState((prev) => ({ ...prev, success: "" }))}
        >
          {uiState.success}
        </Alert>
      )}

      <Paper sx={{ p: 2, mb: 2 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid size={{ xs: 12, md: 8 }}>
            <TextField
              fullWidth
              label="Search by Product Name, ID, Barcode or SKU"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
              placeholder="Type to search products..."
            />
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>
            <Button
              variant="outlined"
              startIcon={<AddIcon />}
              onClick={() =>
                setUiState((prev) => ({ ...prev, productDialogOpen: true }))
              }
              fullWidth
            >
              Add New Product
            </Button>
          </Grid>
        </Grid>
      </Paper>

      <Typography variant="h6" gutterBottom sx={{ mb: 2 }}>
        Inventory ({filteredProducts.length} products)
      </Typography>
      <TableContainer component={Paper} sx={{ mb: 4 }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Product ID</TableCell>
              <TableCell>Product Name</TableCell>
              <TableCell>Category</TableCell>
              <TableCell>Size</TableCell>
              <TableCell align="center">Stock</TableCell>
              <TableCell align="center">Status</TableCell>
              <TableCell align="center">Selling Price</TableCell>
              <TableCell align="center">Edit</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredProducts.length === 0 ? (
              <TableRow>
                <TableCell colSpan={8} align="center">
                  {searchQuery ? "No products found" : "No products available"}
                </TableCell>
              </TableRow>
            ) : (
              filteredProducts.map((product) => (
                <TableRow
                  key={product.variantId}
                  sx={{
                    cursor: "pointer",
                    backgroundColor:
                      selectedProduct?.variantId === product.variantId
                        ? "action.selected"
                        : "inherit",
                    "&:hover": { backgroundColor: "action.hover" },
                  }}
                  onClick={() => setSelectedProduct(product)}
                >
                  <TableCell>{product.productId}</TableCell>
                  <TableCell>
                    <Typography fontWeight="medium">{product.name}</Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={product.category}
                      size="small"
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell>{product.size || "-"}</TableCell>
                  <TableCell align="center">
                    <Typography
                      variant="body1"
                      fontWeight="bold"
                      color={product.currentStock === 0 ? "error" : "primary"}
                    >
                      {product.currentStock}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">
                    <Chip
                      label={getStatusText(product.currentStock)}
                      size="small"
                      color={getStatusColor(product.currentStock)}
                      variant={product.currentStock > 0 ? "filled" : "outlined"}
                    />
                  </TableCell>
                  <TableCell align="center">
                    <Typography fontWeight="medium">
                      ${product.sellingPrice.toFixed(2)}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">
                    <IconButton
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleEditProduct(product);
                      }}
                      title="Edit Product"
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {selectedProduct && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            {selectedProduct.name}
          </Typography>
          <Grid container spacing={2} alignItems="flex-end">
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Current Stock
              </Typography>
              <Typography variant="h6" color="primary.main">
                {selectedProduct.currentStock} units
              </Typography>
            </Grid>

            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Quantity to Add"
                type="number"
                value={quantity}
                onChange={(e) => setQuantity(parseInt(e.target.value) || 1)}
                fullWidth
                InputProps={{ inputProps: { min: 1 } }}
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <Button
                variant="contained"
                onClick={handleStockIn}
                disabled={uiState.loading}
                fullWidth
                sx={{ height: "56px" }}
              >
                {uiState.loading ? <CircularProgress size={24} /> : "Add Stock"}
              </Button>
            </Grid>
          </Grid>
        </Paper>
      )}

      <ProductAndVariantDialog
        open={uiState.productDialogOpen}
        onClose={handleProductDialogClose}
        onSuccess={handleProductCreated}
        editMode={uiState.editMode}
        productData={productToEdit}
      />
    </Box>
  );
}
