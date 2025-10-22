USE Payroll_DB;
GO

-- =========================
-- 1. Employees Table
-- =========================
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    MiddleInitial NVARCHAR(5) NULL,
    Department NVARCHAR(50),
    JobTitle NVARCHAR(50),
    BaseSalary DECIMAL(10,2) NOT NULL CHECK (BaseSalary >= 0),
    HireDate DATE NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =========================
-- 2. Users Table (AppUser model)
-- =========================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NULL,                              -- optional link to Employees
    UserName NVARCHAR(50) NOT NULL UNIQUE,           -- maps to AppUser.UserName
    PasswordHash VARBINARY(MAX) NOT NULL,            -- byte[] in C#
    Role NVARCHAR(20) NOT NULL 
        CHECK (Role IN ('Admin', 'HR', 'Finance', 'Employee')),
    IsActive BIT DEFAULT 1,                           -- active flag
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);

-- =========================
-- 3. Attendance Table
-- =========================
CREATE TABLE Attendance (
    AttendanceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    WorkDate DATE NOT NULL,
    HoursWorked DECIMAL(5,2) CHECK (HoursWorked >= 0),
    Status TINYINT DEFAULT 1, -- Present=1, Absent=0, Leave=2
    Notes NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);

-- =========================
-- 4. Payroll Table
-- =========================
CREATE TABLE Payroll (
    PayrollId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    PayPeriodStart DATE NOT NULL,
    PayPeriodEnd DATE NOT NULL,
    GrossPay DECIMAL(10,2) CHECK (GrossPay >= 0),
    TaxDeductions DECIMAL(10,2) DEFAULT 0 CHECK (TaxDeductions >= 0),
    BenefitsDeductions DECIMAL(10,2) DEFAULT 0 CHECK (BenefitsDeductions >= 0),
    NetPay DECIMAL(10,2) CHECK (NetPay >= 0),
    GeneratedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);

-- =========================
-- 5. AuditLogs Table
-- =========================
CREATE TABLE AuditLogs (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TableName NVARCHAR(50) NOT NULL,
    Action NVARCHAR(20) CHECK (Action IN ('INSERT', 'UPDATE', 'DELETE')),
    RecordId INT,
    [Timestamp] DATETIME DEFAULT GETDATE(),
    Details NVARCHAR(MAX),

    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- =========================
-- Indexes (Recommended)
-- =========================
CREATE INDEX IX_Attendance_EmployeeId_WorkDate ON Attendance (EmployeeId, WorkDate);
CREATE INDEX IX_Payroll_EmployeeId_Period ON Payroll (EmployeeId, PayPeriodStart, PayPeriodEnd);
