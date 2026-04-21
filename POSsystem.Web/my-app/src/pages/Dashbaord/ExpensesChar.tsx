import { Card, CardContent, Typography, Box, useTheme } from "@mui/material";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import { expensesByCategory } from "./MockData";

export default function ExpensesChart() {
  const theme = useTheme();

  return (
    <Card
      elevation={0}
      sx={{ border: `1px solid ${theme.palette.divider}`, height: "100%" }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 600, color: "text.primary" }}>
            Expenses by Category
          </Typography>
          <Typography variant="body2" sx={{ color: "text.secondary" }}>
            Monthly expense breakdown
          </Typography>
        </Box>

        <Box sx={{ width: "100%", height: 300 }}>
          <ResponsiveContainer>
            <BarChart
              data={expensesByCategory}
              layout="vertical"
              margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
            >
              <CartesianGrid strokeDasharray="3 3" stroke={theme.palette.divider} horizontal={false} />
              <XAxis
                type="number"
                tick={{ fill: theme.palette.text.secondary, fontSize: 12 }}
                axisLine={false}
                tickLine={false}
                tickFormatter={(v) => `$${(v / 1000).toFixed(0)}k`}
              />
              <YAxis
                type="category"
                dataKey="category"
                tick={{ fill: theme.palette.text.secondary, fontSize: 12 }}
                axisLine={false}
                tickLine={false}
                width={80}
              />
              <Tooltip
                contentStyle={{
                  backgroundColor: theme.palette.background.paper,
                  border: `1px solid ${theme.palette.divider}`,
                  borderRadius: 8,
                  boxShadow: theme.shadows[3],
                }}
                formatter={(value: number | undefined) => [`$${value?.toLocaleString() ?? "N/A"}`, "Amount"]}
              />
              <Bar
                dataKey="amount"
                fill="#ed6c02"
                radius={[0, 4, 4, 0]}
                maxBarSize={24}
              />
            </BarChart>
          </ResponsiveContainer>
        </Box>
      </CardContent>
    </Card>
  );
}
