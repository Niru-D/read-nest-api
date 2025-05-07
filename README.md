# ReadNest - Library Management System 📚

This is a **Library Management System** built using **ASP.NET Core Web API**. The system handles core library functions such as book management, user management, book borrowing, due date management and secure access to endpoints. It is designed based on a case study to simulate real-world requirements and standards.

## 🚀 Technologies Used

- **ASP.NET Core Web API** – Backend framework
- **Entity Framework Core (EF Core)** – ORM for database access
- **SQL Server** – Relational database
- **JWT Authentication** – For secure login and token-based access
- **Role-Based Access Control (RBAC)** – For managing permissions by user role
- **Swagger** – For interactive API documentation
- **Interceptors** – For handling cross-cutting concerns (like logging)
- **Extension Methods** – To keep the codebase clean and reusable
- **Exception & Error Handling** – Structured and user-friendly responses

## ✅ Features

- 📚 **Book & User Management**
- 🔐 **Secure API with JWT Tokens**
- 👥 **RBAC for Admin, Library Member and Public User roles**
- 🛠 **Proper Exception Handling and Error Responses**
- 💡 **Extension methods and Interceptors**
- 📄 **Interactive API Docs with Swagger UI**

## 📦 Getting Started

1. Clone the repository  
   ```bash
   git clone https://github.com/Niru-D/read-nest.git
   ```

2. Update the connection string in appsettings.json to point to your SQL Server instance.
   
4. Apply EF Core migrations
   ```bash
   dotnet ef database update
   ```
   
5. Run the application
   ```bash
   dotnet run
   ```
   
6. Access Swagger UI at:
   ```bash
   https://localhost:{port}/swagger
   ```
   
