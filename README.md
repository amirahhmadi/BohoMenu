
# ☕ BohoMenu

**BohoMenu** is a digital menu management system designed for cafes and restaurants. It provides a modern public-facing menu and an admin dashboard that allows cafe owners to manage their menu without modifying code.

Built with **ASP.NET Core MVC**, **Entity Framework Core**, and **SQL Server**, BohoMenu focuses on clean code, maintainability, security, and practical business requirements.

## 📸 Screenshots

<table>
  <tr>
    <th>Public Menu</th>
    <th>Admin Dashboard</th>
  </tr>
 <tr>
  <td width="50%">
    <img src="https://github.com/user-attachments/assets/f6da9129-5cca-43f3-bd03-30a2fe9ac52f"
         alt="BohoMenu Public Menu" width="100%">
  </td>
  <td width="50%">
    <img src="https://github.com/user-attachments/assets/a9eee34c-7c6a-46bc-b9fd-858cb0186957"
         alt="BohoMenu Admin Dashboard" width="100%">
  </td>
</tr>
</table>

## ✨ Features

### Public Menu
- Display cafe name, tagline, and logo.
- Browse menu items by category.
- Search for menu items.
- Display item details, prices, and images.
- Hide unavailable items from the public menu.
- Responsive interface for mobile and desktop devices.

### Admin Dashboard
- Secure admin authentication.
- Cookie-based authentication and authorization.
- Change admin password.
- Manage menu categories (Create, Read, Update, Delete).
- Manage menu items (Create, Read, Update, Delete).
- Upload and manage menu item images.
- Manage cafe information:
  - Cafe name
  - Short tagline
  - Cafe logo

### Security
- Password hashing using ASP.NET Core PasswordHasher.
- Role-based authorization.
- Anti-forgery token validation.
- Secure authentication cookies.
- Security Stamp validation to invalidate existing sessions after password changes.
- Server-side validation for uploaded images.

## 🛠️ Technologies

- C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- LINQ
- Razor Views
- HTML5
- CSS3
- JavaScript
- Bootstrap

## 🏗️ Project Structure

The solution is organized into three main projects:

```text
BohoMenu/
│
├── BohoMenu/
│   ├── Areas/
│   │   └── Admin/
│   │       ├── Controllers/
│   │       └── Views/
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   └── Program.cs
│
├── BohoMenu.Core/
│   ├── DTOs/
│   ├── Services/
│   └── ...
│
├── BohoMenu.DataBase/
│   ├── Entities/
│   ├── Configurations/
│   ├── Migrations/
│   └── ApplicationDbContext.cs
│
└── README.md
```

## 🚀 Getting Started

### Prerequisites

Make sure you have the following installed:

- .NET SDK
- Visual Studio 2022 or later
- SQL Server
- SQL Server Management Studio (optional)

### 1. Clone the repository

```bash
git clone https://github.com/amirahhmadi/BohoMenu.git
```

### 2. Configure the database

Add your SQL Server connection string to the configuration of the main project.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BohoMenuDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Configure the admin account

Configure the initial admin credentials using ASP.NET Core User Secrets or another secure configuration provider.

Do not commit passwords, password hashes, or other secrets to source control.

### 4. Apply database migrations

Run the following command from the solution directory:

```bash
dotnet ef database update --project ./BohoMenu.DataBase/BohoMenu.DataBase.csproj --startup-project ./BohoMenu/BohoMenu.csproj
```

### 5. Run the application

```bash
dotnet run --project ./BohoMenu/BohoMenu.csproj
```

The application will be available at the local URL provided by ASP.NET Core.

## 🔒 Security Notes

- Never commit production credentials or secrets.
- Use HTTPS in production.
- Configure a secure database connection.
- Back up the database regularly.

## 👨‍💻 Developer

**Amir Ahmadi**

- GitHub: [@amirahhmadi](https://github.com/amirahhmadi)

---

<p align="center">
  Built with ❤️ using ASP.NET Core
</p>
