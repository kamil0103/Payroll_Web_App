USE Payroll_DB;
GO

-- Employees Table
CREATE TABLE dbo.Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
);
GO

--  Users Table
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NULL,
    UserName NVARCHAR(60) NOT NULL UNIQUE,
    PasswordHash VARBINARY(500) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT ('Employee'),
    IsActive BIT NOT NULL DEFAULT (1),
    CONSTRAINT FK_Users_Employees FOREIGN KEY (EmployeeId)
        REFERENCES dbo.Employees(EmployeeId)
);
GO

--  Attendance Table
CREATE TABLE dbo.Attendance (
    AttendanceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    WorkDate DATE NOT NULL,
    HoursWorked DECIMAL(5,2) NULL,
    Status TINYINT NOT NULL DEFAULT (1),
    Notes NVARCHAR(200) NULL,
    CONSTRAINT FK_Attendance_Employees FOREIGN KEY (EmployeeId)
        REFERENCES dbo.Employees(EmployeeId),
    CONSTRAINT UQ_Attendance_Emp_Date UNIQUE (EmployeeId, WorkDate)
);
GO

--  Payroll Table
CREATE TABLE dbo.Payroll (
    PayrollId BIGINT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    GrossPay DECIMAL(12,2) NOT NULL DEFAULT (0),
    Deductions DECIMAL(12,2) NOT NULL DEFAULT (0),
    NetPay AS (GrossPay - Deductions) PERSISTED,
    PayDate DATE NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT FK_Payroll_Employees FOREIGN KEY (EmployeeId)
        REFERENCES dbo.Employees(EmployeeId)
);
GO
