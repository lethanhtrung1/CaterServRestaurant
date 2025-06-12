# ASP.NET Core Web API Project

## 📌 Overview

This project is a **Restaurant Management System** built with **ASP.NET Core Web API**. It provides backend services for managing restaurant operations, allowing customers to:

-   Book tables
-   Place food orders
-   Make payments
-   Track order status

The system also includes features for admins/staff to manage:

-   Menu items
-   Table availability
-   Customer orders
-   Payment status

## ⚙️ Technologies Used

-   ASP.NET Core 8
-   Entity Framework Core
-   SQL Server
-   AutoMapper
-   Swagger
-   JWT Authentication
-   Clean Architecture (or Layered Architecture / DDD)

## 📁 Project Structure

```plaintext
├── Domain/
├── Application/
├── Infrastructure/
├── WebApi/
└── README.md
```

🚀 Getting Started

1. Clone the repository:
   clone project -> cd repo

2. Configure the connection string

3. Apply migrations and create the database:
   dotnet ef database update

4. Run the application:
   dotnet run --project WebApi
