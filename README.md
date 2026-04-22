# POS & Inventory Management System

## Overview
This project is a full-stack Point of Sale (POS) and Inventory Management System designed for a single-store retail environment. It provides a complete workflow for handling sales, managing inventory, issuing invoices, and analyzing business performance.

The system emphasizes **data integrity, transactional safety, and real-world retail logic**, ensuring that operations like sales, stock updates, and payments are handled reliably.

---

## Features

### Authentication & Authorization
- JWT-based authentication  
- Role-based access control:
  - Admin  
  - Cashier  
- Secure password hashing using ASP.NET Identity  

---

### POS (Point of Sale)
- Add products via search or barcode  
- Fast checkout workflow  
- Percentage-based discounts  
- Real-time total calculation  
- Server-side validation of totals  

---

### Sales Processing
- Full transactional sale handling  
- Automatic:
  - Tax calculation (15%)  
  - Discount application (capped at 15%)  
- Prevents:
  - Invalid totals  
  - Insufficient stock  
  - Underpayment  
- Generates invoice number per sale  

---

### Inventory Management
- Stock tracked per product variant  
- Automatic stock deduction after sales  
- Manual stock adjustment  
- Stock movement tracking (audit-friendly design)  

---

### Products Management
- Product + Variant architecture  
- Supports:
  - Barcode  
  - SKU  
  - Size / Color  
- Category-based organization  

---

### Invoice Management
- Generate invoice for each sale  
- Retrieve full invoice history  
- Includes:
  - Items  
  - Payment details  
  - Taxes and discounts  

---

### Returns & Refunds
- Process product returns  
- Update stock automatically  
- Maintain transaction consistency  

---

### Dashboard
- Visual analytics using charts and tables  
- Sales insights  
- Inventory overview  

---

## Technologies Used

### Backend
- ASP.NET Web API  
- SQL Server  
- T-SQL  
- Entity Framework Core  

### Frontend
- React  
- Vite  
- TypeScript  

---

## System Architecture
- RESTful API using ASP.NET  
- Relational database design (SQL Server)  
- Separation of concerns:
  - Controllers (API layer)  
  - Business logic (handled within controllers/services)  
  - Data access (EF Core)  
- Secure communication using JWT  

---

## Database Schema

> *(Insert your DB schema diagram image here)*

---

## API Endpoints

### Authentication

| Method | Endpoint            | Description                     |
|--------|--------------------|---------------------------------|
| POST   | `/api/auth/login`  | Authenticate user and return JWT token |

**Notes:**
- Includes role claims in JWT  
- Token expiration is configurable  

---

### Users (Admin Only)

| Method | Endpoint                          | Description                     |
|--------|----------------------------------|---------------------------------|
| GET    | `/api/users`                     | Get all users                   |
| GET    | `/api/users/{id}`                | Get user by ID                  |
| POST   | `/api/users`                     | Create new user                 |
| PUT    | `/api/users/{id}/activate`       | Activate user                   |
| PUT    | `/api/users/{id}/deactivate`     | Deactivate user                 |

**Details:**
- Users linked to full profile (Person entity)  
- Passwords hashed securely  
- Soft activation instead of deletion  

---

### Products & Inventory

| Method | Endpoint                                      | Description                         |
|--------|------------------------------------------------|-------------------------------------|
| GET    | `/api/products`                                | Get all product variants            |
| GET    | `/api/products/{variantId}`                    | Get product variant by ID           |
| POST   | `/api/products`                                | Create product + variant            |
| PUT    | `/api/products/{variantId}`                    | Update product & variant            |
| POST   | `/api/products/{variantId}/stock?quantity=x`   | Add stock manually                  |
| GET    | `/api/products/categories`                     | Get all categories                  |

**Details:**
- Variant-based inventory system  
- Supports barcode and SKU  
- Stock tracked per variant  

---

### Sales (POS Core)

| Method | Endpoint        | Description                          |
|--------|----------------|--------------------------------------|
| POST   | `/api/sales`   | Create a complete sale transaction   |

**Processing Logic:**
- Validates product existence and stock  
- Calculates totals on server  
- Applies:
  - Tax (15%)  
  - Discount (max 15%)  
- Prevents invalid transactions  
- Creates:
  - Sale record  
  - Sale items  
  - Payment record  
  - Stock movements  
- Uses database transaction (atomic operation)  

---

### Invoices

| Method | Endpoint          | Description                     |
|--------|------------------|---------------------------------|
| GET    | `/api/invoices`  | Get all invoices with items     |

**Details:**
- Uses database views for optimized reads  
- Returns fully aggregated invoice data  

---

## Installation

### Prerequisites
- .NET SDK  
- SQL Server  
- ASP.NET  

---

### Steps

1. Clone the repository  

2. Set up database:
   - Create a database in SQL Server  
   - Run provided SQL scripts  

3. Configure backend:
   - Update `appsettings.json` connection string  

4. Run backend:
   ```bash
   dotnet run

   ## Contact
If you have any question feel free to reach out to me through linkdin:

https://www.linkedin.com/in/khalid-abdul-rahim-451943254/

Thank you.
