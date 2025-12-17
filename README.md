# Consultant Management API

A comprehensive REST API for managing consultants, roles, tasks, and work entries built with ASP.NET Core 8.0.

## Overview

This API provides a complete solution for managing consultant assignments and tracking their work hours over the festive period. It handles role-based salary calculations, task assignments, and ensures business rules like maximum daily work hours are enforced.

## Key Features

- **Consultant Management**: Create, read, update consultants with profile images
- **Role Management**: Define consultant roles (Level 1, Level 2) with customizable hourly rates
- **Task Management**: Create tasks and assign multiple consultants to each task
- **Work Entry Tracking**: Capture hours worked per consultant per task per day
- **Business Rules Enforcement**:
  - Maximum 12 hours per consultant per day
  - Rate at time of work is stored (historical accuracy)
  - Consultants must be assigned to tasks before logging hours
- **Payment Calculations**: View total amounts due to consultants over specific timeframes
- **JWT Authentication**: Secure API access with token-based authentication

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite (in-memory for quick setup)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer Token
- **API Documentation**: Swagger/OpenAPI
- **Password Hashing**: BCrypt

## Project Structure

```
ConsultantManagementApi/
├── Configuration/       # Configuration Classes (Strongly-Typed DI)
│   ├── JwtSettings.cs
│   ├── DatabaseSettings.cs
│   └── CorsSettings.cs
├── Controllers/         # API Controllers
│   ├── AuthController.cs
│   ├── ConsultantsController.cs
│   ├── RolesController.cs
│   ├── TasksController.cs
│   └── WorkEntriesController.cs
├── Middleware/          # Custom Middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Models/              # Domain Models
│   ├── Consultant.cs
│   ├── ConsultantRole.cs
│   ├── ConsultantTask.cs
│   ├── TaskAssignment.cs
│   ├── WorkEntry.cs
│   └── User.cs
├── DTOs/                # Data Transfer Objects
│   ├── AuthDtos.cs
│   ├── ConsultantDtos.cs
│   ├── RoleDtos.cs
│   ├── TaskDtos.cs
│   └── WorkEntryDtos.cs
├── Data/                # Database Context
│   └── AppDbContext.cs
├── Services/            # Business Services
│   ├── IJwtService.cs
│   └── JwtService.cs
├── Migrations/          # Entity Framework Migrations
└── Program.cs           # Application Configuration & DI Setup
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Any IDE (Visual Studio, VS Code, Rider)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ConsultantManagementApi
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

The API will start on `https://localhost:5001` (or `http://localhost:5000`)

### Configuration

The application uses strongly-typed dependency injection for configuration. All settings are defined in `appsettings.json` and can be overridden with environment variables.

#### Configuration Sections

**JWT Settings** (`appsettings.json`):
```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
  "Issuer": "ConsultantManagementApi",
  "Audience": "ConsultantManagementApiUsers",
  "ExpirationMinutes": 480
}
```

**Database Settings**:
```json
"DatabaseSettings": {
  "ConnectionString": "Data Source=consultantmanagement.db"
}
```

**CORS Settings**:
```json
"CorsSettings": {
  "AllowedOrigins": ["http://localhost:3000", "http://localhost:5000"]
}
```

#### Environment Variable Overrides

For production deployments, override settings using environment variables with double underscores:

```bash
# Override JWT Secret
export JwtSettings__SecretKey="your-production-secret-key"

# Override Database Connection
export DatabaseSettings__ConnectionString="Data Source=/data/production.db"

# Override CORS Origins
export CorsSettings__AllowedOrigins="https://yourdomain.com"
```

### Database Setup

The SQLite database is automatically created and seeded with initial data when the application starts:

- **Default User**: `admin` / `admin123`
- **Default Roles**:
  - Consultant Level 1: $50/hour
  - Consultant Level 2: $75/hour

### Middleware

The application includes two custom middleware components:

1. **ExceptionHandlingMiddleware**: Catches unhandled exceptions and returns standardized error responses
2. **RequestLoggingMiddleware**: Logs all HTTP requests with timing information and status codes

## API Documentation

Once the application is running, access the Swagger UI at:
- `https://localhost:5001/swagger` (HTTPS)
- `http://localhost:5000/swagger` (HTTP)

### Authentication

All endpoints except `/api/auth/login` require a valid JWT token.

#### 1. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin"
}
```

#### 2. Using the Token

Include the token in the Authorization header for all subsequent requests:
```
Authorization: Bearer <your-token>
```

In Swagger UI:
1. Click the "Authorize" button at the top
2. Enter: `Bearer <your-token>`
3. Click "Authorize"

## API Endpoints

### Roles

- `GET /api/roles` - Get all consultant roles
- `GET /api/roles/{id}` - Get role by ID
- `POST /api/roles` - Create new role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role

**Example: Create Role**
```json
POST /api/roles
{
  "name": "Senior Consultant",
  "ratePerHour": 100.00
}
```

### Consultants

- `GET /api/consultants` - Get all consultants
- `GET /api/consultants/{id}` - Get consultant by ID
- `POST /api/consultants` - Create new consultant
- `PUT /api/consultants/{id}` - Update consultant
- `POST /api/consultants/{id}/profile-image` - Upload profile image
- `DELETE /api/consultants/{id}` - Delete consultant

**Example: Create Consultant**
```json
POST /api/consultants
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "consultantRoleId": 1
}
```

**Example: Upload Profile Image**
```http
POST /api/consultants/1/profile-image
Content-Type: multipart/form-data

file: [binary image data]
```

### Tasks

- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create new task
- `POST /api/tasks/{taskId}/assign` - Assign consultant to task
- `DELETE /api/tasks/{taskId}/unassign/{consultantId}` - Unassign consultant
- `DELETE /api/tasks/{id}` - Delete task

**Example: Create Task**
```json
POST /api/tasks
{
  "name": "Holiday Support Coverage",
  "description": "Provide customer support during holiday season",
  "durationHours": 40
}
```

**Example: Assign Consultant**
```json
POST /api/tasks/1/assign
{
  "consultantId": 1
}
```

### Work Entries

- `GET /api/workentries` - Get all work entries
- `GET /api/workentries/{id}` - Get work entry by ID
- `POST /api/workentries` - Create work entry
- `GET /api/workentries/consultant/{id}/summary?startDate=2024-12-01&endDate=2024-12-31` - Get payment summary
- `DELETE /api/workentries/{id}` - Delete work entry

**Example: Create Work Entry**
```json
POST /api/workentries
{
  "consultantId": 1,
  "taskId": 1,
  "workDate": "2024-12-17",
  "hoursWorked": 8.5
}
```

**Example: Get Payment Summary**
```http
GET /api/workentries/consultant/1/summary?startDate=2024-12-01&endDate=2024-12-31
```

**Response:**
```json
{
  "consultantId": 1,
  "consultantName": "John Doe",
  "startDate": "2024-12-01",
  "endDate": "2024-12-31",
  "totalHours": 85.5,
  "totalAmount": 4275.00,
  "workEntries": [...]
}
```

## Business Rules Implemented

1. **Role-Based Rates**: Each consultant has a role with an hourly rate
2. **Historical Rate Preservation**: When work is logged, the current rate is stored with the work entry
3. **Rate Change Independence**: Changing a role's rate doesn't affect previously logged hours
4. **Task Assignment Requirement**: Consultants must be assigned to a task before logging work hours
5. **Daily Hour Limit**: Maximum 12 hours per consultant per day (enforced when creating work entries)
6. **Multiple Task Assignments**: A consultant can be assigned to multiple tasks
7. **Multiple Consultants Per Task**: A task can have multiple consultants assigned

## Data Models

### Consultant
- Personal information (name, email)
- Profile image
- Associated role
- Work entries and task assignments

### ConsultantRole
- Role name (Level 1, Level 2, etc.)
- Hourly rate
- Can be updated without affecting historical data

### ConsultantTask
- Task details (name, description, duration)
- Multiple consultant assignments
- Work entries

### WorkEntry
- Links consultant to task
- Work date and hours worked
- **RatePerHourAtTimeOfWork**: Captures rate when work was logged
- Used for payment calculations

## Testing the API

### Quick Test Flow

1. **Login**
   ```bash
   curl -X POST https://localhost:5001/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123"}'
   ```

2. **Create a Consultant**
   ```bash
   curl -X POST https://localhost:5001/api/consultants \
     -H "Authorization: Bearer <token>" \
     -H "Content-Type: application/json" \
     -d '{"firstName":"Jane","lastName":"Smith","email":"jane@example.com","consultantRoleId":1}'
   ```

3. **Create a Task**
   ```bash
   curl -X POST https://localhost:5001/api/tasks \
     -H "Authorization: Bearer <token>" \
     -H "Content-Type: application/json" \
     -d '{"name":"Holiday Support","description":"Customer support","durationHours":40}'
   ```

4. **Assign Consultant to Task**
   ```bash
   curl -X POST https://localhost:5001/api/tasks/1/assign \
     -H "Authorization: Bearer <token>" \
     -H "Content-Type: application/json" \
     -d '{"consultantId":1}'
   ```

5. **Log Work Hours**
   ```bash
   curl -X POST https://localhost:5001/api/workentries \
     -H "Authorization: Bearer <token>" \
     -H "Content-Type: application/json" \
     -d '{"consultantId":1,"taskId":1,"workDate":"2024-12-17","hoursWorked":8}'
   ```

6. **Get Payment Summary**
   ```bash
   curl "https://localhost:5001/api/workentries/consultant/1/summary?startDate=2024-12-01&endDate=2024-12-31" \
     -H "Authorization: Bearer <token>"
   ```

## Error Handling

The API returns appropriate HTTP status codes:

- `200 OK` - Successful GET requests
- `201 Created` - Successful POST requests
- `204 No Content` - Successful PUT/DELETE requests
- `400 Bad Request` - Validation errors or business rule violations
- `401 Unauthorized` - Missing or invalid authentication token
- `404 Not Found` - Resource not found

Example error response:
```json
{
  "message": "Cannot exceed 12 hours per day. Already worked 8 hours on this day."
}
```

## Configuration

### JWT Settings

Edit `appsettings.json` to configure JWT authentication:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ConsultantManagementApi",
    "Audience": "ConsultantManagementApiUsers"
  }
}
```

### Database

The application uses SQLite by default. To switch to SQL Server:

1. Update `Program.cs`:
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

2. Add connection string to `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ConsultantManagement;Trusted_Connection=True;"
     }
   }
   ```

3. Install SQL Server package:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   ```

## Security Considerations

- JWT tokens expire after 8 hours
- Passwords are hashed using BCrypt
- All endpoints except login require authentication
- CORS is enabled for development (configure appropriately for production)
- File uploads are restricted to image types only
- Input validation on all endpoints

## Production Deployment

Before deploying to production:

1. Change JWT secret key to a strong, random value
2. Configure CORS to allow only your frontend domain
3. Use a production database (SQL Server, PostgreSQL)
4. Enable HTTPS only
5. Set up proper logging and monitoring
6. Configure file upload size limits
7. Implement rate limiting
8. Add comprehensive error logging

## Troubleshooting

### Database Issues
If you encounter database issues, delete the `consultantmanagement.db` file and restart the application.

### Port Already in Use
If port 5001 is already in use, edit `Properties/launchSettings.json` to use different ports.

### Authentication Issues
Ensure you're including the full token with "Bearer " prefix in the Authorization header.

## Future Enhancements

- Email notifications for task assignments
- Reporting dashboard
- Export functionality (CSV, Excel)
- Audit logging for all changes
- Role-based access control (Admin, Manager, User)
- Approval workflow for work entries
- Integration with payroll systems

## Support

For issues, questions, or contributions, please contact the development team.

## License

This project is developed as part of a technical assessment.

---

**Built with ASP.NET Core 8.0**
