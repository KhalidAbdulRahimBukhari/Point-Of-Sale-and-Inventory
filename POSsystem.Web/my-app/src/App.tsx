import { ThemeProvider, createTheme } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import { Routes, Route, Navigate } from "react-router-dom";

import DashboardLayout from "./layout/DashboardLayout";

// Pages
import LoginPage from "./pages/auth/Login";
import Dashboard from "./pages/Dashbaord/Dashboard";
import Users from "./pages/Users/Users";
import Cashier from "./pages/Cahier/Cashier";
import Returns from "./pages/Returns/Returns";
import StockInPage from "./pages/StockIn/StockInPage";

const theme = createTheme({
  palette: {
    mode: "dark",
    primary: { main: "#84acd3" },
    secondary: { main: "#9546a3" },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />

      <Routes>
        {/* Public */}
        <Route path="/login" element={<LoginPage />} />

        {/* Dashboard Layout */}
        <Route element={<DashboardLayout />}>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/users" element={<Users />} />
          <Route path="/cashier" element={<Cashier />} />
          <Route path="/returns" element={<Returns />} />
          <Route path="/StockIn" element={<StockInPage />} />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </ThemeProvider>
  );
}

export default App;
