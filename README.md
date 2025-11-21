# Contract Monthly Claim System - README

##  Project Overview

The **Contract Monthly Claim System** is a comprehensive ASP.NET Core MVC web application designed to automate and streamline the claim submission and approval process for academic institutions. The system provides role-based access for lecturers, program coordinators, and academic managers with automated workflows, real-time calculations, and professional invoicing capabilities.

##  Features

### Core Functionality
- **User Authentication & Authorization** - Secure role-based access control
- **Claim Management** - Complete lifecycle from submission to approval
- **Document Handling** - File upload and management support
- **Real-time Calculations** - Automatic amount calculations
- **Dashboard Analytics** - Comprehensive statistics and reporting

### Automation Features
- **Smart Approval Workflow** - Automated routing based on claim amounts
- **Background Auto-Approval** - Scheduled processing of eligible claims
- **Automated Validation** - Business rule enforcement and duplicate detection
- **Invoice Generation** - Professional invoice creation upon approval

### User Roles
- **Lecturer** - Submit claims, view personal history, track status
- **Program Coordinator** - Approve claims (up to R1000), manage workflows
- **Academic Manager** - Full system access, bulk operations, reporting

##  Technology Stack

### Backend
- **ASP.NET Core 6.0 MVC** - Web framework
- **C#** - Programming language
- **Dependency Injection** - Service management
- **Session Management** - User state management
- **Background Services** - Automated processing

### Frontend
- **Bootstrap 5.1.3** - Responsive UI framework
- **jQuery** - Client-side interactions
- **Font Awesome** - Icons
- **JavaScript** - Real-time features
- **Razor Pages** - Server-side rendering

### Security
- **Authentication Cookies** - Secure login
- **Role-based Authorization** - Access control
- **Input Validation** - Data integrity
- **Anti-forgery Tokens** - CSRF protection

##  Project Structure

```
PROG6212_POE/
├── Controllers/
│   ├── AccountController.cs      # Authentication & login
│   ├── ClaimsController.cs       # Claim management
│   ├── HomeController.cs         # Dashboard & navigation
│   └── InvoicesController.cs     # Invoice management
├── Models/
│   ├── Entities/                 # Data models
│   │   ├── Claim.cs
│   │   ├── User.cs
│   │   ├── Document.cs
│   │   ├── Invoice.cs
│   │   └── ClaimWorkflow.cs
│   ├── ViewModels/               # View models
│   │   ├── LoginViewModel.cs
│   │   ├── ClaimViewModel.cs
│   │   └── DashboardViewModel.cs
│   └── Services/                 # Business logic
├── Services/
│   ├── IClaimService.cs          # Claim service interface
│   ├── ClaimService.cs           # Claim service implementation
│   ├── IFileService.cs           # File service interface
│   ├── FileService.cs            # File service implementation
│   └── ClaimAutomationService.cs # Background service
├── Views/                        # UI templates
│   ├── Account/
│   ├── Claims/
│   ├── Home/
│   └── Shared/
└── Program.cs                    # Application entry point
```

##  Installation & Setup

### Prerequisites
- .NET 6.0 SDK or later
- Visual Studio 2022 or VS Code
- Web browser with JavaScript enabled

### Steps
1. **Clone or download** the project files
2. **Open the solution** in Visual Studio
3. **Restore NuGet packages** if necessary
4. **Build the solution** (Ctrl+Shift+B)
5. **Run the application** (F5)

### Configuration
The application uses in-memory data storage for demonstration purposes. No database configuration is required.

##  User Credentials

### Demo Accounts
| Role | Username | Password | Access Level |
|------|----------|----------|-------------|
| Lecturer | `lecturer1` | `password` | Basic claim submission |
| Program Coordinator | `coordinator1` | `password` | Claim management & approval |
| Academic Manager | `manager1` | `password` | Full system access |

### Quick Login
Use the quick login buttons on the login page for instant access to any role.

##  Key Features Explained

### 1. Smart Approval Workflow
```csharp
// Automated routing based on claim amount
if (claim.TotalAmount <= 500) {
    // Auto-approve by Coordinator
} else if (claim.TotalAmount <= 1000) {
    // Coordinator approval required
} else {
    // Manager approval required
}
```

### 2. Background Auto-Approval
- Runs every 5 minutes
- Processes claims meeting criteria:
  - Amount ≤ R300
  - At least 24 hours old
  - Not marked "urgent"
  - Hours ≤ 40

### 3. Automated Validation
```csharp
public async Task<ValidationResult> ValidateClaimAsync(ClaimViewModel model, int lecturerId)
{
    // Business rules validation
    // Monthly limits, duplicates, weekend work, etc.
}
```

### 4. Invoice Generation
- Automatic generation upon claim approval
- Professional formatting with payment details
- Downloadable in text format
- Unique invoice numbering

##  Workflow Process

### Claim Lifecycle
1. **Submission** → Lecturer submits claim with details and documents
2. **Validation** → System validates against business rules
3. **Routing** → Automated routing based on amount
4. **Approval** → Coordinator/Manager reviews and approves
5. **Invoicing** → System generates professional invoice
6. **Completion** → Claim marked as processed

### Status Transitions
```
Pending → Approved/Rejected
Pending → Pending Manager Review → Approved/Rejected
```

##  Dashboard Features

### Statistical Overview
- Total claims count
- Pending approval count
- Approved claims count
- Monthly and all-time totals
- Average processing time
- Approval rate percentage

### Role-Specific Views
- **Lecturers**: Personal claim statistics
- **Coordinators**: High-priority claims and pending reviews
- **Managers**: Comprehensive system overview and reporting

##  Testing the Application

### Test Scenarios

#### 1. Basic Claim Submission
```
1. Login as Lecturer (lecturer1/password)
2. Navigate to "Submit Claim"
3. Fill in claim details:
   - Title: "Research Materials"
   - Hours: 10
   - Rate: R150
   - Date: Current date
4. Submit and verify success message
```

#### 2. Automated Approval Testing
```
1. Submit claim with R250 total
2. Wait 5 minutes (background service)
3. Check claim status - should be auto-approved
4. Verify invoice generation
```

#### 3. Coordinator Workflow
```
1. Login as Coordinator (coordinator1/password)
2. Go to "Manage Claims"
3. Review pending claims
4. Approve/Reject with notes
5. Verify status updates
```

#### 4. Manager Operations
```
1. Login as Manager (manager1/password)
2. Access all system features
3. Generate reports
4. Process bulk approvals
5. View system statistics
```

## Code Architecture

### Service Layer Pattern
```csharp
public interface IClaimService
{
    Task<List<Claim>> GetAllClaimsAsync();
    Task<Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId);
    // ... other methods
}

public class ClaimService : IClaimService
{
    // Implementation with business logic
}
```

### Dependency Injection
```csharp
// Program.cs
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddHostedService<ClaimAutomationService>();
```

### Model-View-ViewModel (MVVM)
- **Models**: Data entities and business objects
- **ViewModels**: UI-specific data representations
- **Views**: Razor templates for presentation

## Security Features

### Authentication & Authorization
```csharp
[Authorize(Roles = "ProgramCoordinator,AcademicManager")]
public async Task<IActionResult> Manage()
{
    // Controller actions protected by role authorization
}
```

### Input Validation
```csharp
[Required(ErrorMessage = "Claim title is required")]
[StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
public string Title { get; set; }
```

### Session Management
```csharp
HttpContext.Session.SetInt32("UserId", user.Id);
HttpContext.Session.SetInt32("UserRole", (int)user.Role);
```

## Business Rules

### Validation Rules
- **Monthly Limit**: R10,000 per lecturer
- **Maximum Hours**: 100 hours per claim
- **Rate Range**: R10 - R500 per hour
- **Future Dates**: Claims cannot have future dates
- **Duplicate Prevention**: Similar claims within 7 days blocked

### Approval Thresholds
- **Auto-approval**: ≤ R300 (by Coordinator)
- **Coordinator Approval**: ≤ R1000
- **Manager Approval**: > R1000

## Troubleshooting

### Common Issues

#### 1. Login Problems
- **Issue**: Cannot login as coordinator/manager
- **Solution**: Use quick login buttons or verify credentials

#### 2. Null Reference Exceptions
- **Issue**: Dashboard shows errors
- **Solution**: Collections are now properly initialized

#### 3. File Upload Issues
- **Issue**: Document upload fails
- **Solution**: Check file size (max 5MB) and type (PDF, DOCX, etc.)

#### 4. Auto-approval Not Working
- **Issue**: Claims not auto-approved
- **Solution**: Wait 5 minutes for background service or check criteria

### Debugging Tips
1. Check browser console for JavaScript errors
2. Verify session values in browser developer tools
3. Review application logs for server-side errors
4. Test with different user roles

##  Future Enhancements

### Planned Features
- **Email Notifications** - Automated status updates
- **Database Persistence** - SQL Server integration
- **Advanced Reporting** - Custom report builder
- **Mobile Application** - Native mobile support
- **API Endpoints** - REST API for integrations
- **Advanced Analytics** - Predictive insights and trends

### Technical Improvements
- **Caching** - Performance optimization
- **Unit Testing** - Comprehensive test coverage
- **Docker Support** - Containerization
- **CI/CD Pipeline** - Automated deployment

##  Support & Contact

### Getting Help
- Review this README for common solutions
- Check the code comments for implementation details
- Test with provided demo credentials

### Development Team
- **Course**: PROG6212
- **Institution**: IIE MSA
- **Academic Year**: 2025

##  License

This project is developed for academic purposes as part of the PROG6212 Portfolio of Evidence submission.

---
*Last Updated: November 2025*