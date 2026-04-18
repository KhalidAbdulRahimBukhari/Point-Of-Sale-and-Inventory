import { Box, Typography } from "@mui/material";
import KpiCards from "./KpiCards";
import RevenueChart from "./RevenueChart";
import SalesByCategoryChart from "./SalesByCatagoryCahrt";
import HourlySalesChart from "./HourlySalesChart";
import PaymentMethodsChart from "./PaymentMethodsChart";
import ExpensesChart from "./ExpensesChar";
import StockMovementsChart from "./StockMovementChart";
import TopProductsTable from "./TopProductsTable";
import RecentSalesTable from "./RecentSalesTable";
import LowStockAlerts from "./LowStockAlerts";

export default function Dashboard() {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* Page Header */}
      <Box>
        <Typography variant="h4" sx={{ fontWeight: 700, color: "text.primary" }}>
          Dashboard
        </Typography>
        <Typography variant="body1" sx={{ color: "text.secondary", mt: 0.5 }}>
          Welcome back. {"Here's what's happening with your store today."}
        </Typography>
      </Box>

      {/* KPI Summary Cards */}
      <KpiCards />

      {/* Revenue Chart + Sales by Category */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: { xs: "1fr", lg: "2fr 1fr" },
          gap: 3,
        }}
      >
        <RevenueChart />
        <SalesByCategoryChart />
      </Box>

      {/* Hourly Sales + Payment Methods */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: { xs: "1fr", lg: "3fr 2fr" },
          gap: 3,
        }}
      >
        <HourlySalesChart />
        <PaymentMethodsChart />
      </Box>

      {/* Stock Movements + Expenses */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" },
          gap: 3,
        }}
      >
        <StockMovementsChart />
        <ExpensesChart />
      </Box>

      {/* Top Products + Low Stock Alerts */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: { xs: "1fr", lg: "2fr 1fr" },
          gap: 3,
        }}
      >
        <TopProductsTable />
        <LowStockAlerts />
      </Box>

      {/* Recent Sales Table */}
      <RecentSalesTable />
    </Box>
  );
}
