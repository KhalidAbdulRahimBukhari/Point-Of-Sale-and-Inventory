import { Card, CardContent, Typography, Box, useTheme } from "@mui/material";
import {
  PieChart,
  Pie,
  Cell,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from "recharts";
import { salesByCategory } from "./MockData";

export default function SalesByCategoryChart() {
  const theme = useTheme();
  const total = salesByCategory.reduce((s, c) => s + c.value, 0);

  return (
    <Card
      elevation={0}
      sx={{ border: `1px solid ${theme.palette.divider}`, height: "100%" }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ mb: 1 }}>
          <Typography variant="h6" sx={{ fontWeight: 600, color: "text.primary" }}>
            Sales by Category
          </Typography>
          <Typography variant="body2" sx={{ color: "text.secondary" }}>
            Revenue distribution across product categories
          </Typography>
        </Box>

        <Box sx={{ width: "100%", height: 300 }}>
          <ResponsiveContainer>
            <PieChart>
              <Pie
                data={salesByCategory}
                cx="50%"
                cy="50%"
                innerRadius={65}
                outerRadius={100}
                paddingAngle={3}
                dataKey="value"
                stroke="none"
              >
                {salesByCategory.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip
                contentStyle={{
                  backgroundColor: theme.palette.background.paper,
                  border: `1px solid ${theme.palette.divider}`,
                  borderRadius: 8,
                  boxShadow: theme.shadows[3],
                }}
                formatter={(value: number) => [
                  `$${value.toLocaleString()} (${((value / total) * 100).toFixed(1)}%)`,
                  undefined,
                ]}
              />
              <Legend
                wrapperStyle={{ fontSize: 12, paddingTop: 4 }}
                formatter={(value) => (
                  <span style={{ color: theme.palette.text.primary, fontSize: 12 }}>{value}</span>
                )}
              />
            </PieChart>
          </ResponsiveContainer>
        </Box>
      </CardContent>
    </Card>
  );
}
