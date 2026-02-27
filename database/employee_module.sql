CREATE TABLE Employees (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    Salary DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'system',
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

CREATE PROCEDURE sp_Employees_Create
    @Id UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100), @LastName NVARCHAR(100), @Email NVARCHAR(200), @Department NVARCHAR(100),
    @Salary DECIMAL(18,2), @CreatedBy NVARCHAR(100)
AS
BEGIN
    INSERT INTO Employees(Id, FirstName, LastName, Email, Department, Salary, CreatedBy)
    VALUES (@Id, @FirstName, @LastName, @Email, @Department, @Salary, @CreatedBy);
END
GO

CREATE PROCEDURE sp_Employees_GetById @Id UNIQUEIDENTIFIER
AS
BEGIN
    SELECT * FROM Employees WHERE Id=@Id AND IsDeleted=0;
END
GO

CREATE PROCEDURE sp_Employees_GetPaged
    @PageNumber INT, @PageSize INT, @Filter NVARCHAR(100)=NULL, @SortBy NVARCHAR(100)='CreatedAt', @Descending BIT=1
AS
BEGIN
    ;WITH Data AS (
        SELECT * FROM Employees
        WHERE IsDeleted = 0 AND (@Filter IS NULL OR FirstName LIKE '%' + @Filter + '%' OR LastName LIKE '%' + @Filter + '%' OR Department LIKE '%' + @Filter + '%')
    )
    SELECT * FROM Data
    ORDER BY
        CASE WHEN @SortBy='FirstName' THEN FirstName END,
        CASE WHEN @SortBy='CreatedAt' THEN CONVERT(NVARCHAR(30), CreatedAt, 126) END
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) FROM Data;
END
GO

CREATE PROCEDURE sp_Employees_Update
    @Id UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100), @LastName NVARCHAR(100), @Email NVARCHAR(200), @Department NVARCHAR(100), @Salary DECIMAL(18,2),
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    UPDATE Employees
    SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Department=@Department, Salary=@Salary,
        UpdatedAt = SYSUTCDATETIME(), UpdatedBy=@UpdatedBy
    WHERE Id=@Id AND IsDeleted=0;
END
GO

CREATE PROCEDURE sp_Employees_SoftDelete @Id UNIQUEIDENTIFIER, @DeletedBy NVARCHAR(100)
AS
BEGIN
    UPDATE Employees SET IsDeleted=1, UpdatedAt=SYSUTCDATETIME(), UpdatedBy=@DeletedBy WHERE Id=@Id;
END
GO

CREATE PROCEDURE sp_Users_Create @Id UNIQUEIDENTIFIER, @Username NVARCHAR(100), @Email NVARCHAR(200), @PasswordHash NVARCHAR(500), @Role NVARCHAR(50)
AS
BEGIN
    INSERT INTO Users(Id, Username, Email, PasswordHash, Role) VALUES(@Id, @Username, @Email, @PasswordHash, @Role);
END
GO
CREATE PROCEDURE sp_Users_GetByUsername @Username NVARCHAR(100)
AS BEGIN SELECT TOP 1 * FROM Users WHERE Username=@Username AND IsDeleted=0; END
GO
CREATE PROCEDURE sp_Users_GetById @Id UNIQUEIDENTIFIER
AS BEGIN SELECT TOP 1 * FROM Users WHERE Id=@Id AND IsDeleted=0; END
GO

CREATE PROCEDURE sp_RefreshTokens_Save @Id UNIQUEIDENTIFIER, @UserId UNIQUEIDENTIFIER, @Token NVARCHAR(500), @ExpiresAt DATETIME2
AS BEGIN INSERT INTO RefreshTokens(Id, UserId, Token, ExpiresAt) VALUES(@Id, @UserId, @Token, @ExpiresAt); END
GO
CREATE PROCEDURE sp_RefreshTokens_Get @Token NVARCHAR(500)
AS BEGIN SELECT TOP 1 * FROM RefreshTokens WHERE Token=@Token; END
GO
CREATE PROCEDURE sp_RefreshTokens_Revoke @Token NVARCHAR(500)
AS BEGIN UPDATE RefreshTokens SET IsRevoked=1 WHERE Token=@Token; END
GO
