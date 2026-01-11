/* -------------------------------------------------------------------------
   UTILITY MANAGEMENT SYSTEM (UMS) - FULL DATABASE SCRIPT
   Module: PUSL2019
   Description: DDL (Tables), DML (Sample Data), and Advanced Objects.
   -------------------------------------------------------------------------
*/

-- =============================================
-- SECTION 1: CREATE TABLES (DDL)
-- =============================================

-- 1. Users Table (Admin, Field Officers, Cashiers)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Admin', 'FieldOfficer', 'Cashier', 'Manager')),
    FullName NVARCHAR(100)
);

-- 2. Utility Types (Electricity, Water, Gas)
CREATE TABLE UtilityTypes (
    UtilityID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(50) NOT NULL UNIQUE
);

-- 3. Tariffs (Pricing Rules)
CREATE TABLE Tariffs (
    TariffID INT PRIMARY KEY IDENTITY(1,1),
    UtilityID INT FOREIGN KEY REFERENCES UtilityTypes(UtilityID),
    TariffName NVARCHAR(100) NOT NULL, 
    UnitRate DECIMAL(10, 2) NOT NULL,
    FixedCharge DECIMAL(10, 2) DEFAULT 0
);

-- 4. Tariff Slabs (Advanced Pricing Logic)
CREATE TABLE TariffSlabs (
    SlabID INT PRIMARY KEY IDENTITY(1,1),
    TariffID INT FOREIGN KEY REFERENCES Tariffs(TariffID),
    FromUnit DECIMAL(10, 2),
    ToUnit DECIMAL(10, 2),
    RatePerUnit DECIMAL(10, 2) NOT NULL
);

-- 5. Customers (Households/Businesses)
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(15),
    AddressLine NVARCHAR(200) NOT NULL,
    RegisteredDate DATETIME DEFAULT GETDATE()
);

-- 6. Premises (Locations)
CREATE TABLE Premises (
    PremisesID INT PRIMARY KEY IDENTITY(1,1),
    AddressLine1 NVARCHAR(200) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL
);

-- 7. Meters (Linking Customers to Utilities)
CREATE TABLE Meters (
    MeterID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    UtilityID INT FOREIGN KEY REFERENCES UtilityTypes(UtilityID),
    MeterSerialNumber NVARCHAR(50) UNIQUE NOT NULL,
    InstallationDate DATE NOT NULL
);

-- 8. Readings (Field Officer enters these)
CREATE TABLE Readings (
    ReadingID INT PRIMARY KEY IDENTITY(1,1),
    MeterID INT FOREIGN KEY REFERENCES Meters(MeterID),
    ReadingDate DATE NOT NULL,
    PreviousReading DECIMAL(10, 2) NOT NULL,
    CurrentReading DECIMAL(10, 2) NOT NULL,
    CHECK (CurrentReading >= PreviousReading)
);

-- 9. Bills (Calculated from Readings)
CREATE TABLE Bills (
    BillID INT PRIMARY KEY IDENTITY(1,1),
    ReadingID INT FOREIGN KEY REFERENCES Readings(ReadingID),
    BillDate DATETIME DEFAULT GETDATE(),
    UnitsConsumed DECIMAL(10, 2) NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    DueDate DATE NOT NULL,
    IsPaid BIT DEFAULT 0
);

-- 10. Payments (Cashier enters these)
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    BillID INT FOREIGN KEY REFERENCES Bills(BillID),
    PaymentDate DATETIME DEFAULT GETDATE(),
    AmountPaid DECIMAL(10, 2) NOT NULL,
    PaymentMethod NVARCHAR(20) CHECK (PaymentMethod IN ('Cash', 'Card', 'Online'))
);

-- 11. Complaints (Admin handles these)
CREATE TABLE Complaints (
    ComplaintID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    Category NVARCHAR(50) CHECK (Category IN ('Billing', 'Meter Fault', 'Service Outage', 'Other')),
    Description NVARCHAR(500) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Open',
    CreatedDate DATETIME DEFAULT GETDATE(),
    ResolvedDate DATETIME NULL
);
GO

-- =============================================
-- SECTION 2: INSERT SAMPLE DATA (DML)
-- =============================================

INSERT INTO UtilityTypes (TypeName) VALUES ('Electricity'), ('Water'), ('Gas');

INSERT INTO Tariffs (UtilityID, TariffName, UnitRate, FixedCharge) VALUES 
(1, 'Domestic Electricity', 20.00, 500.00),
(1, 'Commercial Electricity', 35.00, 1000.00),
(2, 'Domestic Water', 5.00, 200.00),
(2, 'Industrial Water', 12.50, 600.00),
(3, 'Domestic Gas', 15.00, 300.00);

INSERT INTO Users (Username, PasswordHash, Role, FullName) VALUES 
('admin', 'admin123', 'Admin', 'System Administrator'),
('reader1', 'pass123', 'FieldOfficer', 'Kamal Perera'),
('cashier1', 'pass123', 'Cashier', 'Sunil Fernando'),
('manager1', 'pass123', 'Manager', 'Mrs. D. Jayasinghe');

INSERT INTO Customers (FullName, Email, PhoneNumber, AddressLine) VALUES 
('John Doe', 'john@example.com', '0771234567', '123 Galle Road, Colombo'),
('Jane Smith', 'jane@example.com', '0719876543', '45 Kandy Road, Kelaniya'),
('Michael Johnson', 'mike@example.com', '0775551234', '89 Havelock Town, Colombo'),
('Emily Davis', 'emily@example.com', '0761112222', '22 Negombo Road, Wattala'),
('David Brown', 'david@example.com', '0703334444', '56 High Level Road, Nugegoda'),
('Sarah Wilson', 'sarah@example.com', '0778889999', '12 Lotus Road, Colombo'),
('James Anderson', 'james@example.com', '0754445555', '33 Beach Road, Mount Lavinia'),
('Patricia Taylor', 'pat@example.com', '0712223333', '78 Main Street, Panadura'),
('Robert Martinez', 'rob@example.com', '0726667777', '90 Temple Road, Maharagama'),
('Linda White', 'linda@example.com', '0779990000', '11 Flower Road, Colombo 07');

INSERT INTO Meters (CustomerID, UtilityID, MeterSerialNumber, InstallationDate) VALUES 
(1, 1, 'ELE-1001', '2023-01-10'),
(1, 2, 'WAT-2001', '2023-01-15'),
(2, 1, 'ELE-1002', '2023-02-20'),
(3, 3, 'GAS-3001', '2023-03-05'),
(4, 1, 'ELE-1003', '2023-03-10'),
(5, 2, 'WAT-2002', '2023-04-01'),
(6, 1, 'ELE-1004', '2023-04-15'),
(7, 1, 'ELE-1005', '2023-05-20'),
(8, 2, 'WAT-2003', '2023-06-01'),
(9, 3, 'GAS-3002', '2023-06-10'),
(10, 1, 'ELE-1006', '2023-07-01');

-- =============================================
-- SECTION 3: STORED PROCEDURES
-- =============================================

CREATE PROCEDURE sp_SubmitReadingAndGenerateBill
    @MeterID INT,
    @CurrentReading DECIMAL(10, 2),
    @ReadingDate DATE
AS
BEGIN
    DECLARE @PreviousReading DECIMAL(10, 2);
    DECLARE @UnitsConsumed DECIMAL(10, 2);
    DECLARE @UnitRate DECIMAL(10, 2);
    DECLARE @FixedCharge DECIMAL(10, 2);
    DECLARE @TotalAmount DECIMAL(10, 2);
    DECLARE @ReadingID INT;

    SELECT TOP 1 @PreviousReading = CurrentReading 
    FROM Readings WHERE MeterID = @MeterID ORDER BY ReadingDate DESC;

    IF @PreviousReading IS NULL SET @PreviousReading = 0;

    IF (@CurrentReading < @PreviousReading)
    BEGIN
        RAISERROR('Error: Current reading cannot be less than previous reading.', 16, 1);
        RETURN;
    END

    SET @UnitsConsumed = @CurrentReading - @PreviousReading;

    SELECT TOP 1 @UnitRate = t.UnitRate, @FixedCharge = t.FixedCharge
    FROM Meters m JOIN Tariffs t ON m.UtilityID = t.UtilityID WHERE m.MeterID = @MeterID;

    SET @TotalAmount = (@UnitsConsumed * @UnitRate) + @FixedCharge;

    INSERT INTO Readings (MeterID, ReadingDate, PreviousReading, CurrentReading)
    VALUES (@MeterID, @ReadingDate, @PreviousReading, @CurrentReading);
    SET @ReadingID = SCOPE_IDENTITY();

    INSERT INTO Bills (ReadingID, BillDate, UnitsConsumed, TotalAmount, DueDate, IsPaid)
    VALUES (@ReadingID, GETDATE(), @UnitsConsumed, @TotalAmount, DATEADD(day, 30, GETDATE()), 0);
END;
GO

CREATE PROCEDURE sp_RegisterNewCustomer
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Address NVARCHAR(200)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Customers WHERE Email = @Email)
    BEGIN
        RAISERROR('Error: Email already registered.', 16, 1);
        RETURN;
    END
    INSERT INTO Customers (FullName, Email, PhoneNumber, AddressLine, RegisteredDate)
    VALUES (@FullName, @Email, @Phone, @Address, GETDATE());
END;
GO

-- =============================================
-- SECTION 4: TRIGGERS
-- =============================================

CREATE TRIGGER trg_UpdateBillStatus
ON Payments
AFTER INSERT
AS
BEGIN
    DECLARE @BillID INT;
    DECLARE @TotalPaid DECIMAL(10, 2);
    DECLARE @BillAmount DECIMAL(10, 2);

    SELECT @BillID = i.BillID FROM inserted i;
    SELECT @TotalPaid = SUM(AmountPaid) FROM Payments WHERE BillID = @BillID;
    SELECT @BillAmount = TotalAmount FROM Bills WHERE BillID = @BillID;

    IF (@TotalPaid >= @BillAmount)
    BEGIN
        UPDATE Bills SET IsPaid = 1 WHERE BillID = @BillID;
    END
END;
GO

CREATE TRIGGER trg_PreventBillDeletion
ON Bills
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR('Security Warning: Bills cannot be deleted.', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO

-- =============================================
-- SECTION 5: FUNCTIONS & VIEWS
-- =============================================

CREATE FUNCTION fn_GetTotalDebt(@CustomerID INT)
RETURNS DECIMAL(10, 2)
AS
BEGIN
    DECLARE @TotalDebt DECIMAL(10, 2);
    SELECT @TotalDebt = SUM(b.TotalAmount)
    FROM Bills b
    JOIN Readings r ON b.ReadingID = r.ReadingID
    JOIN Meters m ON r.MeterID = m.MeterID
    WHERE m.CustomerID = @CustomerID AND b.IsPaid = 0;
    RETURN ISNULL(@TotalDebt, 0);
END;
GO

CREATE VIEW v_MonthlyRevenue AS
SELECT 
    u.TypeName AS UtilityType,
    YEAR(b.BillDate) AS BillYear,
    MONTH(b.BillDate) AS BillMonth,
    SUM(b.TotalAmount) AS TotalRevenue,
    COUNT(b.BillID) AS TotalBillsGenerated
FROM Bills b
JOIN Readings r ON b.ReadingID = r.ReadingID
JOIN Meters m ON r.MeterID = m.MeterID
JOIN UtilityTypes u ON m.UtilityID = u.UtilityID
GROUP BY u.TypeName, YEAR(b.BillDate), MONTH(b.BillDate);
GO

CREATE VIEW v_UnpaidBillsReport AS
SELECT 
    c.FullName, c.PhoneNumber, u.TypeName AS Utility,
    b.BillDate, b.TotalAmount, b.DueDate,
    DATEDIFF(day, b.DueDate, GETDATE()) AS DaysOverdue
FROM Bills b
JOIN Readings r ON b.ReadingID = r.ReadingID
JOIN Meters m ON r.MeterID = m.MeterID
JOIN Customers c ON m.CustomerID = c.CustomerID
JOIN UtilityTypes u ON m.UtilityID = u.UtilityID
WHERE b.IsPaid = 0;
GO

CREATE VIEW v_TopConsumers AS
SELECT TOP 10 
    c.FullName, 
    u.TypeName AS Utility, 
    SUM(b.UnitsConsumed) AS TotalUsage, 
    SUM(b.TotalAmount) AS TotalSpent
FROM Bills b
JOIN Readings r ON b.ReadingID = r.ReadingID
JOIN Meters m ON r.MeterID = m.MeterID
JOIN Customers c ON m.CustomerID = c.CustomerID
JOIN UtilityTypes u ON m.UtilityID = u.UtilityID
GROUP BY c.FullName, u.TypeName
ORDER BY TotalUsage DESC;
GO