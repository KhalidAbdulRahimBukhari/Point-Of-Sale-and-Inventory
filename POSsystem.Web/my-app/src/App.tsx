import { Routes, Route, Navigate } from "react-router-dom";
import DashboardLayout from "./layout/DashboardLayout";
// Pages
import LoginPage from "./pages/auth/Login";
import Dashboard from "./pages/Dashbaord/Dashboard";
import Users from "./pages/Users/Users";
import Cashier from "./pages/Cahier/Cashier";
import Returns from "./pages/Returns/Returns";
import StockInPage from "./pages/StockIn/StockInPage";
import Invoices from "./pages/Invoice/ListInvoices";


function App() {
  return (

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
          <Route path="/Invoices" element={<Invoices />} />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>

  );
}

export default App;
