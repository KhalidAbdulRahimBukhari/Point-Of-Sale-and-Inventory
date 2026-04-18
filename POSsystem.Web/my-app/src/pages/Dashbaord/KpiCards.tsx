import { Box, Card, CardContent, Typography, alpha, useTheme } from "@mui/material";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import AssignmentReturnIcon from "@mui/icons-material/AssignmentReturn";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import InventoryIcon from "@mui/icons-material/Inventory";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import { kpiSummary } from "./MockData";

interface KpiCardProps {
  title: string;
  value: string;
  subtitle: string;
  icon: React.ReactNode;
  color: string;
  trend?: string;
  trendUp?: boolean;
}

function KpiCard({ title, value, subtitle, icon, color, trend, trendUp }: KpiCardProps) {
  const theme = useTheme();

  return (
    <Card
      elevation={0}
      sx={{
        border: `1px solid ${theme.palette.divider}`,
        height: "100%",
        transition: "box-shadow 0.2s, transform 0.2s",
        "&:hover": {
          boxShadow: theme.shadows[4],
          transform: "translateY(-2px)",
        },
      }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
          <Box sx={{ flex: 1 }}>
            <Typography
              variant="body2"
              sx={{ color: "text.secondary", fontWeight: 500, mb: 0.5 }}
            >
              {title}
            </Typography>
            <Typography
              variant="h5"
              sx={{ fontWeight: 700, color: "text.primary", lineHeight: 1.2, mb: 0.5 }}
            >
              {value}
            </Typography>
            <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
              {trend && (
                <Typography
                  variant="caption"
                  sx={{
                    fontWeight: 600,
                    color: trendUp ? "success.main" : "error.main",
                  }}
                >
                  {trend}
                </Typography>
              )}
              <Typography variant="caption" sx={{ color: "text.secondary" }}>
                {subtitle}
              </Typography>
            </Box>
          </Box>
          <Box
            sx={{
              width: 48,
              height: 48,
              borderRadius: 2,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              bgcolor: alpha(color, 0.1),
              color: color,
              flexShrink: 0,
            }}
          >
            {icon}
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
}

export default function KpiCards() {
  const cards: KpiCardProps[] = [
    {
      title: "Today's Revenue",
      value: `$${kpiSummary.todayRevenue.toLocaleString()}`,
      subtitle: "vs yesterday",
      icon: <AttachMoneyIcon />,
      color: "#1976d2",
      trend: "+12.5%",
      trendUp: true,
    },
    {
      title: "Today's Transactions",
      value: kpiSummary.todayTransactions.toString(),
      subtitle: "vs yesterday",
      icon: <ReceiptLongIcon />,
      color: "#2e7d32",
      trend: "+8.2%",
      trendUp: true,
    },
    {
      title: "Avg. Order Value",
      value: `$${kpiSummary.todayAvgOrder.toFixed(2)}`,
      subtitle: "this month",
      icon: <ShoppingCartIcon />,
      color: "#ed6c02",
      trend: "+3.1%",
      trendUp: true,
    },
    {
      title: "Monthly Revenue",
      value: `$${kpiSummary.monthRevenue.toLocaleString()}`,
      subtitle: "Feb 2026",
      icon: <TrendingUpIcon />,
      color: "#0288d1",
      trend: "+5.4%",
      trendUp: true,
    },
    {
      title: "Monthly Expenses",
      value: `$${kpiSummary.monthExpenses.toLocaleString()}`,
      subtitle: "Feb 2026",
      icon: <TrendingUpIcon />,
      color: "#d32f2f",
      trend: "-2.3%",
      trendUp: false,
    },
    {
      title: "Returns",
      value: kpiSummary.monthReturns.toString(),
      subtitle: "this month",
      icon: <AssignmentReturnIcon />,
      color: "#d32f2f",
      trend: "-15%",
      trendUp: true,
    },
    {
      title: "Low Stock Alerts",
      value: kpiSummary.lowStockCount.toString(),
      subtitle: "items below threshold",
      icon: <WarningAmberIcon />,
      color: "#ed6c02",
    },
    {
      title: "Total Products",
      value: kpiSummary.totalProducts.toString(),
      subtitle: `${kpiSummary.totalVariants} variants`,
      icon: <InventoryIcon />,
      color: "#7b1fa2",
    },
  ];

  return (
    <Box
      sx={{
        display: "grid",
        gridTemplateColumns: {
          xs: "1fr",
          sm: "repeat(2, 1fr)",
          md: "repeat(4, 1fr)",
        },
        gap: 2.5,
      }}
    >
      {cards.map((card) => (
        <KpiCard key={card.title} {...card} />
      ))}
    </Box>
  );
}
