// ── Mock data for the POS & Inventory Dashboard ──
// Based on DB schema: Sales, Products, Expenses, Stock, Payments, Categories

// Monthly revenue for the past 12 months
export const monthlyRevenue = [
  { month: "Mar", revenue: 18200, cost: 11400, profit: 6800 },
  { month: "Apr", revenue: 21500, cost: 13100, profit: 8400 },
  { month: "May", revenue: 19800, cost: 12200, profit: 7600 },
  { month: "Jun", revenue: 24100, cost: 14500, profit: 9600 },
  { month: "Jul", revenue: 22700, cost: 13800, profit: 8900 },
  { month: "Aug", revenue: 26300, cost: 15200, profit: 11100 },
  { month: "Sep", revenue: 23900, cost: 14900, profit: 9000 },
  { month: "Oct", revenue: 28400, cost: 16100, profit: 12300 },
  { month: "Nov", revenue: 31200, cost: 17800, profit: 13400 },
  { month: "Dec", revenue: 35800, cost: 19600, profit: 16200 },
  { month: "Jan", revenue: 27600, cost: 16400, profit: 11200 },
  { month: "Feb", revenue: 29100, cost: 17200, profit: 11900 },
];

// Daily sales for the current month (last 28 days)
export const dailySales = Array.from({ length: 28 }, (_, i) => ({
  day: `${i + 1}`,
  sales: Math.floor(Math.random() * 800 + 400),
  transactions: Math.floor(Math.random() * 30 + 10),
}));

// Sales by category (pie chart)
export const salesByCategory = [
  { name: "Electronics", value: 34500, color: "#1976d2" },
  { name: "Clothing", value: 22100, color: "#2e7d32" },
  { name: "Accessories", value: 15800, color: "#ed6c02" },
  { name: "Footwear", value: 11200, color: "#9c27b0" },
  { name: "Home & Living", value: 8700, color: "#d32f2f" },
];

// Payment methods breakdown
export const paymentMethods = [
  { method: "Cash", amount: 42300, count: 312, color: "#1976d2" },
  { method: "Card", amount: 31200, count: 198, color: "#2e7d32" },
  { method: "Mobile Pay", amount: 12800, count: 87, color: "#ed6c02" },
  { method: "Bank Transfer", amount: 5900, count: 23, color: "#9c27b0" },
];

// Expense categories for the current month
export const expensesByCategory = [
  { category: "Rent", amount: 4500 },
  { category: "Salaries", amount: 8200 },
  { category: "Utilities", amount: 1200 },
  { category: "Supplies", amount: 2100 },
  { category: "Marketing", amount: 1800 },
  { category: "Maintenance", amount: 900 },
];

// Top selling products
export const topProducts = [
  {
    id: 1,
    name: "Wireless Earbuds Pro",
    category: "Electronics",
    sold: 142,
    revenue: 8520,
    stock: 38,
  },
  {
    id: 2,
    name: "Classic Denim Jacket",
    category: "Clothing",
    sold: 98,
    revenue: 5880,
    stock: 24,
  },
  {
    id: 3,
    name: "Running Shoes X1",
    category: "Footwear",
    sold: 87,
    revenue: 6960,
    stock: 15,
  },
  {
    id: 4,
    name: "Smart Watch Band",
    category: "Accessories",
    sold: 76,
    revenue: 2280,
    stock: 52,
  },
  {
    id: 5,
    name: "Cotton Crew T-Shirt",
    category: "Clothing",
    sold: 203,
    revenue: 4060,
    stock: 8,
  },
  {
    id: 6,
    name: "Bluetooth Speaker",
    category: "Electronics",
    sold: 65,
    revenue: 4550,
    stock: 31,
  },
  {
    id: 7,
    name: "Leather Wallet",
    category: "Accessories",
    sold: 112,
    revenue: 3360,
    stock: 44,
  },
  {
    id: 8,
    name: "Yoga Mat Premium",
    category: "Home & Living",
    sold: 54,
    revenue: 2160,
    stock: 19,
  },
];

// Low stock alerts
export const lowStockItems = [
  {
    id: 1,
    name: "Cotton Crew T-Shirt",
    variant: "White / M",
    stock: 8,
    threshold: 20,
  },
  {
    id: 2,
    name: "Running Shoes X1",
    variant: "Black / 42",
    stock: 5,
    threshold: 15,
  },
  {
    id: 3,
    name: "Wireless Charger Pad",
    variant: "Standard",
    stock: 3,
    threshold: 10,
  },
  {
    id: 4,
    name: "Canvas Backpack",
    variant: "Navy",
    stock: 2,
    threshold: 10,
  },
  {
    id: 5,
    name: "USB-C Cable 2m",
    variant: "White",
    stock: 4,
    threshold: 25,
  },
];

// Recent sales
export const recentSales = [
  {
    id: "INV-2026-0487",
    date: "2026-02-13 14:32",
    customer: "Walk-in",
    cashier: "Ahmed",
    items: 3,
    total: 245.0,
    payment: "Cash",
    status: "COMPLETED",
  },
  {
    id: "INV-2026-0486",
    date: "2026-02-13 13:18",
    customer: "Walk-in",
    cashier: "Sara",
    items: 1,
    total: 89.99,
    payment: "Card",
    status: "COMPLETED",
  },
  {
    id: "INV-2026-0485",
    date: "2026-02-13 12:05",
    customer: "Walk-in",
    cashier: "Ahmed",
    items: 5,
    total: 412.5,
    payment: "Cash",
    status: "COMPLETED",
  },
  {
    id: "INV-2026-0484",
    date: "2026-02-13 11:47",
    customer: "Walk-in",
    cashier: "Omar",
    items: 2,
    total: 178.0,
    payment: "Mobile Pay",
    status: "COMPLETED",
  },
  {
    id: "INV-2026-0483",
    date: "2026-02-13 10:22",
    customer: "Walk-in",
    cashier: "Sara",
    items: 4,
    total: 335.0,
    payment: "Card",
    status: "COMPLETED",
  },
  {
    id: "INV-2026-0482",
    date: "2026-02-12 17:55",
    customer: "Walk-in",
    cashier: "Ahmed",
    items: 1,
    total: 59.99,
    payment: "Cash",
    status: "REFUNDED",
  },
];

// Hourly sales distribution (for today)
export const hourlySales = [
  { hour: "9AM", sales: 320, transactions: 8 },
  { hour: "10AM", sales: 580, transactions: 14 },
  { hour: "11AM", sales: 720, transactions: 18 },
  { hour: "12PM", sales: 890, transactions: 22 },
  { hour: "1PM", sales: 1020, transactions: 25 },
  { hour: "2PM", sales: 760, transactions: 19 },
  { hour: "3PM", sales: 640, transactions: 16 },
  { hour: "4PM", sales: 830, transactions: 21 },
  { hour: "5PM", sales: 950, transactions: 24 },
  { hour: "6PM", sales: 1100, transactions: 28 },
  { hour: "7PM", sales: 870, transactions: 22 },
  { hour: "8PM", sales: 540, transactions: 13 },
];

// Stock movement data (last 7 days)
export const stockMovements = [
  { day: "Mon", stockIn: 120, stockOut: 85, adjustments: -5 },
  { day: "Tue", stockIn: 45, stockOut: 92, adjustments: 0 },
  { day: "Wed", stockIn: 200, stockOut: 78, adjustments: -12 },
  { day: "Thu", stockIn: 0, stockOut: 105, adjustments: 3 },
  { day: "Fri", stockIn: 80, stockOut: 130, adjustments: -8 },
  { day: "Sat", stockIn: 30, stockOut: 145, adjustments: 0 },
  { day: "Sun", stockIn: 0, stockOut: 62, adjustments: -2 },
];

// KPI summary
export const kpiSummary = {
  todayRevenue: 4850.49,
  todayTransactions: 47,
  todayAvgOrder: 103.2,
  todayReturns: 2,
  monthRevenue: 29100,
  monthTransactions: 620,
  monthReturns: 18,
  monthExpenses: 18700,
  totalProducts: 342,
  totalVariants: 1248,
  lowStockCount: 5,
  activeUsers: 6,
};
