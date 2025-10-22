# Contract Monthly Claim System

## 📋 Project Overview

The **Contract Monthly Claim System** is a comprehensive web application designed for academic institutions to manage monthly contract claims. The system facilitates seamless claim submission by lecturers and provides an efficient approval workflow for program coordinators and academic managers.

## 🚀 Features

### Core Functionality
- **🔐 Role-Based Access Control** - Three user roles with distinct permissions
- **📝 Claim Submission** - Intuitive form for lecturers to submit monthly claims
- **✅ Claim Management** - Approval workflow for coordinators and managers
- **📎 Document Upload** - Secure file attachment system for supporting documents
- **📊 Status Tracking** - Real-time claim status updates and transparency
- **📱 Responsive Design** - Mobile-friendly interface using Bootstrap 5

### User Roles & Permissions

| Role | Permissions |
|------|-------------|
| **Lecturer** | Submit claims, View own claims, Upload documents, Track status |
| **Program Coordinator** | View all claims, Approve/Reject claims, Manage workflow |
| **Academic Manager** | View all claims, Approve/Reject claims, Oversee process |

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core MVC 6.0
- **Language**: C#
- **Architecture**: MVC Pattern with Service Layer

### Frontend
- **UI Framework**: Bootstrap 5.1.3
- **Icons**: Font Awesome 6.0
- **JavaScript**: jQuery 3.6.0
- **Styling**: Custom CSS3

### Testing
- **Testing Framework**: xUnit
- **Mocking Framework**: Moq
- **Test Runner**: .NET Core Test SDK

## 📁 Project Structure

```
PROG6212 POE/
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   └── ClaimsController.cs
├── Models/              # Data Models and ViewModels
│   ├── Entities/        # Domain Entities
│   │   ├── User.cs
│   │   ├── Claim.cs
│   │   └── Document.cs
│   ├── ViewModels/      # View Models
│   │   ├── LoginViewModel.cs
│   │   ├── ClaimViewModel.cs
│   │   └── DashboardViewModel.cs
├── Services/            # Business Logic Layer
│   ├── IClaimService.cs
│   ├── ClaimService.cs
│   └── FileService.cs
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
│       ├── Details.cshtml
│       └── Manage.cshtml
├── Tests/               # Unit Tests
│   ├── ClaimServiceTests.cs
│   ├── FileServiceTests.cs
│   └── ClaimsControllerTests.cs
├── wwwroot/             # Static Files
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── uploads/
├── Program.cs           # Application Entry Point
└── README.md
```

## 🚀 Quick Start

### Prerequisites
- .NET 6.0 SDK or later
- Visual Studio 2022 / VS Code / Any .NET IDE
- Web browser with JavaScript enabled

### Installation & Running

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd "PROG6212 POE"
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Access the Application**
   - Open browser and navigate to: `https://localhost:7000` or `http://localhost:5000`
   - The application starts directly on the Dashboard (no login required for demo)

### Demo Credentials (if login enabled)

| Role | Username | Password | Access |
|------|----------|----------|---------|
| Lecturer | `lecturer1` | `password` | Submit & view claims |
| Program Coordinator | `coordinator1` | `password` | Approve claims |
| Academic Manager | `manager1` | `password` | Approve claims |

## 💡 Usage Guide

### For Lecturers
1. **Submit a Claim**
   - Navigate to "Submit Claim"
   - Fill in claim details (title, hours, rate, date)
   - Upload supporting documents (optional)
   - Submit with one click

2. **Track Claims**
   - View "My Claims" to see status
   - Monitor approval progress
   - Download submitted documents

### For Coordinators/Managers
1. **Review Claims**
   - Access "Manage Claims" dashboard
   - View all pending claims with full details
   - See claim amounts and supporting information

2. **Approve/Reject**
   - Use one-click approve/reject buttons
   - Real-time status updates
   - Transparent audit trail

## 🧪 Testing

### Running Unit Tests
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test project
dotnet test PROG6212\ POE.Tests.csproj
```

### Test Coverage
- **Service Layer**: Claim creation, status updates, file handling
- **Business Logic**: Calculations, validations, workflows
- **Controllers**: HTTP endpoints, session management
- **File Handling**: Upload validation, type restrictions

## ⚙️ Configuration

### File Upload Settings
- **Max File Size**: 5MB
- **Allowed Formats**: PDF, DOCX, XLSX, JPG, PNG
- **Storage**: In-memory (for demo purposes)

### Session Configuration
- **Timeout**: 30 minutes
- **Storage**: In-memory session
- **Auto-login**: Enabled for demo mode

## 🔧 Development

### Adding New Features
1. Create appropriate ViewModel in `Models/ViewModels/`
2. Implement service logic in `Services/`
3. Add controller actions in relevant controller
4. Create/update views in `Views/`
5. Write unit tests in `Tests/`

### Data Storage
- **Current**: In-memory storage (volatile)
- **Production Ready**: Easy to switch to Entity Framework with SQL Server

### Key Design Patterns
- **MVC Architecture** - Separation of concerns
- **Service Layer** - Business logic abstraction
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling

## 🐛 Troubleshooting

### Common Issues

1. **Claim Submission Fails**
   - Check browser console for JavaScript errors
   - Verify all required fields are filled
   - Ensure file size and type comply with limits

2. **Styling Not Loading**
   - Clear browser cache (Ctrl+F5)
   - Check if Bootstrap CDN is accessible
   - Verify static files are enabled in Program.cs

3. **Session Issues**
   - Application restarts clear in-memory data
   - Use browser refresh to reinitialize demo data

### Debug Mode
Add debug information to any view:
```html
<div class="alert alert-info">
    <strong>Debug Info:</strong><br>
    UserId: @Context.Session.GetInt32("UserId")<br>
    UserRole: @Context.Session.GetInt32("UserRole")<br>
    UserName: @Context.Session.GetString("UserName")
</div>
```

## 📈 Future Enhancements

### Planned Features
- [ ] Database persistence with SQL Server
- [ ] Email notifications for status changes
- [ ] Advanced reporting and analytics
- [ ] Bulk claim operations
- [ ] Audit logging system
- [ ] Multi-language support
- [ ] API for third-party integrations

### Technical Improvements
- [ ] Implement proper authentication
- [ ] Add password hashing
- [ ] Database migrations
- [ ] Caching layer
- [ ] Performance optimization

## 🤝 Contributing

### Development Process
1. Fork the repository
2. Create a feature branch
3. Implement changes with tests
4. Submit pull request

### Code Standards
- Follow C# coding conventions
- Include XML documentation
- Write comprehensive unit tests
- Update README for new features

## 📄 License

This project is developed as part of **PROG6212 POE (Professional Practice Project)** for academic purposes.

## 🆘 Support

For technical support or questions:
1. Check the troubleshooting section above
2. Review the code documentation
3. Contact the development team

## 🎯 Academic Context

This project demonstrates:
- **MVC Architecture** implementation
- **Role-Based Access Control** systems
- **File Upload** and validation
- **Unit Testing** methodologies
- **Software Development** best practices

---

**PROG6212 POE - Contract Monthly Claim System**  
*Academic Year 2025 - Professional Practice Project*