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
import { recentSales } from "./MockData";

export default function RecentSalesTable() {
  const theme = useTheme();

  return (
    <Card
      elevation={0}
      sx={{ border: `1px solid ${theme.palette.divider}`, height: "100%" }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 600, color: "text.primary" }}>
            Recent Sales
          </Typography>
          <Typography variant="body2" sx={{ color: "text.secondary" }}>
            Latest transactions
          </Typography>
        </Box>

        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Invoice
                </TableCell>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Date
                </TableCell>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Cashier
                </TableCell>
                <TableCell align="center" sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Items
                </TableCell>
                <TableCell align="right" sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Total
                </TableCell>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Payment
                </TableCell>
                <TableCell sx={{ fontWeight: 600, color: "text.secondary", fontSize: 12 }}>
                  Status
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {recentSales.map((sale) => (
                <TableRow
                  key={sale.id}
                  sx={{ "&:last-child td": { border: 0 } }}
                >
                  <TableCell sx={{ fontWeight: 500, fontSize: 12, fontFamily: "monospace" }}>
                    {sale.id}
                  </TableCell>
                  <TableCell sx={{ fontSize: 12, color: "text.secondary" }}>
                    {sale.date}
                  </TableCell>
                  <TableCell sx={{ fontSize: 13 }}>{sale.cashier}</TableCell>
                  <TableCell align="center" sx={{ fontSize: 13 }}>
                    {sale.items}
                  </TableCell>
                  <TableCell align="right" sx={{ fontWeight: 600, fontSize: 13 }}>
                    ${sale.total.toFixed(2)}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={sale.payment}
                      size="small"
                      variant="outlined"
                      sx={{ fontSize: 11, height: 22 }}
                    />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={sale.status}
                      size="small"
                      color={sale.status === "COMPLETED" ? "success" : "error"}
                      variant="filled"
                      sx={{ fontSize: 11, height: 22, fontWeight: 600 }}
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
