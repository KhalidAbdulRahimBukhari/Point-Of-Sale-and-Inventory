import {
  Card,
  CardContent,
  Typography,
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  useTheme,
} from "@mui/material";
import { topProducts } from "./MockData";

const categoryColors: Record<string, "primary" | "success" | "warning" | "secondary" | "error"> = {
  Electronics: "primary",
  Clothing: "success",
  Accessories: "warning",
  Footwear: "secondary",
  "Home & Living": "error",
};

export default function TopProductsTable() {
  const theme = useTheme();

  return (
    <Card
      elevation={0}
      sx={{ border: `1px solid ${theme.palette.divider}`, height: "100%" }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 600, color: "text.primary" }}>
            Top Selling Products
          </Typography>
          <Typography variant="body2" sx={{ color: "text.secondary" }}>
            Best performing products this month
          </Typography>
        </Box>

        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Product
                </TableCell>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Category
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Sold
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Revenue
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Stock
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {topProducts.map((product) => (
                <TableRow
                  key={product.id}
                  sx={{ "&:last-child td": { border: 0 } }}
                >
                  <TableCell sx={{ fontWeight: 500, fontSize: 13 }}>
                    {product.name}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={product.category}
                      size="small"
                      color={categoryColors[product.category] || "default"}
                      variant="outlined"
                      sx={{ fontSize: 11, height: 24 }}
                    />
                  </TableCell>
                  <TableCell align="right" sx={{ fontWeight: 500, fontSize: 13 }}>
                    {product.sold}
                  </TableCell>
                  <TableCell align="right" sx={{ fontWeight: 600, fontSize: 13 }}>
                    ${product.revenue.toLocaleString()}
                  </TableCell>
                  <TableCell align="right">
                    <Chip
                      label={product.stock}
                      size="small"
                      color={product.stock <= 10 ? "error" : product.stock <= 20 ? "warning" : "default"}
                      variant={product.stock <= 10 ? "filled" : "outlined"}
                      sx={{ fontSize: 11, height: 22, minWidth: 36 }}
                    />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </CardContent>
    </Card>
  );
}
