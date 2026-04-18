import {
  Card,
  CardContent,
  Typography,
  Box,
  useTheme,
  LinearProgress,
  alpha,
} from "@mui/material";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import { lowStockItems } from "./MockData";

export default function LowStockAlerts() {
  const theme = useTheme();

  return (
    <Card
      elevation={0}
      sx={{ border: `1px solid ${theme.palette.divider}`, height: "100%" }}
    >
      <CardContent sx={{ p: 2.5, "&:last-child": { pb: 2.5 } }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
          <WarningAmberIcon sx={{ color: "warning.main", fontSize: 22 }} />
          <Box>
            <Typography variant="h6" sx={{ fontWeight: 600, color: "text.primary", lineHeight: 1.2 }}>
              Low Stock Alerts
            </Typography>
            <Typography variant="body2" sx={{ color: "text.secondary" }}>
              Items below reorder threshold
            </Typography>
          </Box>
        </Box>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          {lowStockItems.map((item) => {
            const percentage = (item.stock / item.threshold) * 100;
            const isCritical = percentage <= 25;

            return (
              <Box key={item.id}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "flex-start",
                    mb: 0.5,
                  }}
                >
                  <Box>
                    <Typography variant="body2" sx={{ fontWeight: 500, lineHeight: 1.3 }}>
                      {item.name}
                    </Typography>
                    <Typography variant="caption" sx={{ color: "text.secondary" }}>
                      {item.variant}
                    </Typography>
                  </Box>
                  <Typography
                    variant="body2"
                    sx={{
                      fontWeight: 700,
                      color: isCritical ? "error.main" : "warning.main",
                      whiteSpace: "nowrap",
                      ml: 1,
                    }}
                  >
                    {item.stock} / {item.threshold}
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={percentage}
                  sx={{
                    height: 6,
                    borderRadius: 3,
                    bgcolor: alpha(isCritical ? theme.palette.error.main : theme.palette.warning.main, 0.1),
                    "& .MuiLinearProgress-bar": {
                      borderRadius: 3,
                      bgcolor: isCritical ? "error.main" : "warning.main",
                    },
                  }}
                />
              </Box>
            );
          })}
        </Box>
      </CardContent>
    </Card>
  );
}
