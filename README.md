# Investment Tracker System

A production-quality investment management system built with ASP.NET Core 9.0 MVC following Clean Architecture principles.

## 🎯 Overview

Investment Tracker is a comprehensive system for managing investment cycles, participants, payments, and generating detailed financial reports. Built with modern technologies and best practices for maintainability and scalability.

## ✨ Features

### Core Functionality
- **Participant Management** - Complete CRUD operations with search and validation
- **Investment Cycles** - Track investments with automatic calculations for interest and profits
- **Payment Tracking** - Record and manage payments with automatic schedule updates
- **Payment Schedules** - Auto-generated based on investment terms
- **Settings Management** - Configurable system parameters

### Dashboard (Step 7)
- 8 Key Performance Indicator Cards
- 4 Interactive Chart.js Visualizations:
  - Monthly Profit Trend (Line Chart)
  - Investment Cycles by Status (Pie Chart)
  - Payment Methods Distribution (Doughnut Chart)
  - Top Participants (Horizontal Bar Chart)
- Real-time data tables for overdue, due soon, and today's payments
- Recent activity monitoring

### Reports (Step 8)
Three comprehensive report types, each available in Excel and PDF formats:

1. **Participant Statement**
   - Detailed individual participant information
   - All investment cycles and payments
   - Summary totals and statistics

2. **Investment Summary**
   - Portfolio-wide overview
   - Status breakdown and analytics
   - Detailed cycle information

3. **Monthly Cash Flow**
   - Payment activity by month
   - Interest vs Principal breakdown
   - Payment method distribution

## 🏗️ Architecture

Built using **Clean Architecture (Onion Architecture)** pattern:

```
├── InvestmentTracker.Core          # Domain Layer
│   ├── Entities/                   # Domain entities
│   ├── Enums/                      # Domain enumerations
│   └── Interfaces/                 # Repository contracts
│
├── InvestmentTracker.Application   # Application Layer
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Service contracts
│   └── Mappings/                   # Object mappings
│
├── InvestmentTracker.Infrastructure # Infrastructure Layer
│   ├── Data/                       # DbContext & Configurations
│   ├── Repositories/               # Repository implementations
│   └── Services/                   # Service implementations
│
└── InvestmentTracker.Web           # Presentation Layer
    ├── Controllers/                # MVC Controllers
    ├── Views/                      # Razor Views
    ├── wwwroot/                    # Static files
    └── Models/ViewModels/          # View Models
```

## 🛠️ Technology Stack

### Backend
- **.NET 9.0** - Latest framework
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server LocalDB** - Database

### Frontend
- **AdminLTE 3.2** - Admin dashboard template
- **Bootstrap 4.6** - Responsive framework
- **jQuery 3.7.0** - JavaScript library
- **DataTables 1.13.6** - Advanced tables
- **Chart.js 4.4.0** - Interactive charts
- **Font Awesome 6.4.0** - Icons

### Reports
- **ClosedXML 0.105.0** - Excel generation
- **QuestPDF 2026.6.1** - PDF generation

## 📊 Database Schema

### Entities (7)
- `ApplicationUser` - System users with roles
- `Participant` - Investment participants
- `InvestmentCycle` - Investment contracts
- `PaymentSchedule` - Expected payment dates
- `Payment` - Actual payment records
- `Setting` - System configuration
- `BaseEntity` - Abstract base with audit fields

### Features
- Soft delete support
- Automatic audit trail (Created/Modified tracking)
- Optimized indexes for performance
- Referential integrity with cascading rules

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server LocalDB (or SQL Server)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Sam-Ezzat/investment-tracker.git
   cd investment-tracker
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (if needed)
   Edit `appsettings.json` in `InvestmentTracker.Web` project

4. **Apply database migrations**
   ```bash
   cd src/InvestmentTracker.Web
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   Navigate to `https://localhost:5001` (or the port shown in console)

### Default Login
- **Username:** admin
- **Password:** Admin@123

## 📝 Development Principles

- **Clean Code** - Readable and maintainable
- **SOLID Principles** - Proper abstraction and separation
- **Async/Await** - All operations are asynchronous
- **Dependency Injection** - Proper IoC container usage
- **Repository Pattern** - Data access abstraction
- **Service Layer** - Business logic separation
- **Thin Controllers** - Controllers only orchestrate

## 🎨 UI Features

- **Responsive Design** - Works on all devices
- **RTL Support** - Arabic language ready (toggle button included)
- **Professional Theme** - AdminLTE for modern look
- **Interactive Tables** - Sorting, searching, pagination
- **Form Validation** - Client and server-side
- **Toast Notifications** - User feedback with TempData
- **Currency Formatting** - Automatic with settings

## 📈 Reports Features

### Excel Reports
- Professional formatting with ClosedXML
- Auto-fit columns
- Color-coded headers
- Currency and percentage formatting
- Summary totals

### PDF Reports
- Professional layouts with QuestPDF
- Page headers and footers
- Responsive tables
- Proper typography
- Page numbering

## 🔒 Security Considerations

- Password hashing for users
- SQL injection prevention (EF Core parameterization)
- XSS prevention (Razor encoding)
- HTTPS enforcement
- Input validation (client and server)

## 🧪 Testing

Unit tests structure included in:
```
tests/InvestmentTracker.Tests/
├── Unit/Services/
└── Integration/
```

## 📦 Database Seed Data

Default seeded data includes:
- 1 Admin user (username: admin, password: Admin@123)
- 8 System settings (Company name, currency, interest rate defaults, etc.)

## 🌍 Internationalization

- RTL (Right-to-Left) support built-in
- Toggle button in navigation
- LocalStorage persistence for user preference
- CSS-based RTL transformation

## 🔄 Future Enhancements

Planned features:
- Authentication & Authorization (Identity integration)
- Email notifications
- Logging with Serilog
- Advanced reporting
- API endpoints (RESTful)
- Mobile app integration

## 📄 License

This project is for educational and commercial use.

## 👤 Author

**Sam Ezzat**
- GitHub: [@Sam-Ezzat](https://github.com/Sam-Ezzat)

## 🙏 Acknowledgments

- AdminLTE for the beautiful admin template
- Microsoft for the excellent .NET ecosystem
- Open source community for amazing libraries

---

**Built with ❤️ using Clean Architecture and ASP.NET Core**
