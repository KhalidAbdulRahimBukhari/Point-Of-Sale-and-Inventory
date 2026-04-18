import { Card, CardContent, Typography, Box, useTheme, LinearProgress, alpha } from "@mui/material";
import { paymentMethods } from "./MockData";

export default function PaymentMethodsChart() {
  const theme = useTheme();
  const maxAmount = Math.max(...paymentMethods.map((p) => p.amount));
  const totalAmount = paymentMethods.reduce((s, p) => s + p.amount, 0);

  return (
    <Card
      elevation={0}
      sx={{ border: `1px solid ${theme.palette.divider}`, height: "100%" }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 600, color: "text.primary" }}>
            Payment Methods
          </Typography>
          <Typography variant="body2" sx={{ color: "text.secondary" }}>
            Breakdown of payment types this month
          </Typography>
        </Box>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 2.5 }}>
          {paymentMethods.map((pm) => (
            <Box key={pm.method}>
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                  mb: 0.75,
                }}
              >
                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                  <Box
                    sx={{
                      width: 10,
                      height: 10,
                      borderRadius: "50%",
                      bgcolor: pm.color,
                      flexShrink: 0,
                    }}
                  />
                  <Typography variant="body2" sx={{ fontWeight: 500 }}>
                    {pm.method}
                  </Typography>
                </Box>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
                  <Typography variant="caption" sx={{ color: "text.secondary" }}>
                    {pm.count} txn
                  </Typography>
                  <Typography variant="body2" sx={{ fontWeight: 600, minWidth: 70, textAlign: "right" }}>
                    ${pm.amount.toLocaleString()}
                  </Typography>
                </Box>
              </Box>
              <LinearProgress
                variant="determinate"
                value={(pm.amount / maxAmount) * 100}
                sx={{
                  height: 8,
                  borderRadius: 4,
                  bgcolor: alpha(pm.color, 0.1),
                  "& .MuiLinearProgress-bar": {
                    borderRadius: 4,
                    bgcolor: pm.color,
                  },
                }}
              />
              <Typography
                variant="caption"
                sx={{ color: "text.secondary", mt: 0.25, display: "block" }}
              >
                {((pm.amount / totalAmount) * 100).toFixed(1)}% of total
              </Typography>
            </Box>
          ))}
        </Box>
      </CardContent>
    </Card>
  );
}
