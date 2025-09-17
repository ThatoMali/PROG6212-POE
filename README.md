# Contract Monthly Claim System - PROG6212 POE

##  Project Overview

This is a **non-functional prototype** of a Contract Monthly Claim System developed as part of the PROG6212 Portfolio of Evidence (POE). The system is designed to allow lecturers to submit monthly claims, with programme coordinators and academic managers able to verify and approve these claims through a transparent workflow.

##  System Features

- **Role-based Access Control**: Three user roles with different permissions
- **Claim Submission**: Lecturers can submit claims with supporting documents
- **Approval Workflow**: Multi-level approval process for claims
- **Status Tracking**: Transparent tracking of claim status throughout the process
- **Document Management**: Support for uploading and managing supporting documents

##  User Roles

1. **Lecturer**: Can submit claims and track their status
2. **Program Coordinator**: Can review and approve/reject claims
3. **Academic Manager**: Final approval authority for claims

##  Technology Stack

- **Framework**: ASP.NET Core MVC
- **Frontend**: HTML5, CSS3, Bootstrap 5, JavaScript
- **Backend**: C#, .NET 6
- **Database**: SQL Server (conceptual design only - non-functional prototype)
- **Architecture**: MVC Pattern

##  Project Structure

```
PROG6212 POE/
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   └── ClaimsController.cs
├── Models/              # Data Models and ViewModels
│   ├── UserRole.cs
│   ├── LoginViewModel.cs
│   ├── ClaimViewModel.cs
│   └── DashboardViewModel.cs
├── Views/               # Razor Views
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Navigation.cshtml
│   │   └── _ViewImports.cshtml
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Dashboard.cshtml
│   ├── Account/
│   │   └── Login.cshtml
│   └── Claims/
│       ├── Index.cshtml
│       ├── Submit.cshtml
│       └── Details.cshtml
├── wwwroot/             # Static Files
│   ├── css/
│   │   └── site.css
│   └── js/
│       └── site.js
├── Program.cs           # Application entry point
└── PROG6212 POE.csproj  # Project configuration
```

##  Getting Started

### Prerequisites

- .NET 6 SDK or later
- Visual Studio 2022 or Visual Studio Code
- SQL Server (for future development)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "PROG6212 POE"
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open browser and navigate to `https://localhost:7000` or `http://localhost:5000`

## 🔧 Important Notes

###  Non-Functional Prototype
This is a **visual prototype only** with the following limitations:
- No database connectivity
- No authentication/authorization implementation
- No business logic implementation
- Sample hardcoded data for demonstration purposes
- Forms submit but don't actually process data

###  Login Credentials
Since this is a non-functional prototype, you can select any role from the dropdown:
- **Username**: Any text
- **Password**: Any text
- **Role**: Select from dropdown (Lecturer/Program Coordinator/Academic Manager)

###  Sample Data
The application uses hardcoded sample data including:
- 4 sample claims with different statuses
- Dashboard statistics (12 total claims, 3 pending, 7 approved)
- User role-based UI changes

##  Key Files

### Models
- **UserRole.cs**: Enum defining user roles
- **LoginViewModel.cs**: Login form data structure
- **ClaimViewModel.cs**: Claim data structure with validation
- **DashboardViewModel.cs**: Dashboard data structure

### Controllers
- **AccountController.cs**: Handles login/logout
- **HomeController.cs**: Handles dashboard and home pages
- **ClaimsController.cs**: Handles claim operations

### Views
- **Login.cshtml**: Login page with role selection
- **Dashboard.cshtml**: Role-specific dashboard
- **Submit.cshtml**: Claim submission form
- **Index.cshtml**: Claims listing page

##  UML Class Diagram

The system includes a comprehensive UML class diagram showing:
- **Inheritance relationships** between User and role-specific classes
- **Composition relationships** between User and Claim
- **Association relationships** between Claim, Document, Approval, and Notification
- **Database schema representation** with foreign key relationships

##  UI Features

- **Responsive Design**: Bootstrap 5 based responsive layout
- **Role-based UI**: Different navigation options based on user role
- **Form Validation**: Client-side and server-side validation
- **Status Indicators**: Color-coded badges for claim status
- **File Upload**: Support document upload interface

##  Workflow

1. **Login** → Select role from dropdown
2. **Dashboard** → View statistics and recent claims
3. **Submit Claim** → (Lecturers only) Fill claim form with document upload
4. **My Claims** → View all claims with status tracking
5. **View Details** → Examine individual claim details

##  Future Enhancements

For a functional implementation, the following would be needed:
- Database integration with Entity Framework
- Authentication and authorization system
- File upload handling and storage
- Email notifications
- Approval workflow engine
- Reporting and analytics
- Admin panel for user management

##  License

This project is created for educational purposes as part of the PROG6212 POE requirements.

##  Developer

- **Student**: Moneri MALI
- **Student ID**: ST10447949
- **Course**: PROG6212
- **Institution**: IIE MSA

---

**Note**: This is a prototype submission for assessment purposes. The system is not production-ready and requires additional development for real-world use.
