USE master;
GO

IF DB_ID(N'DB_Denisov_CoffeeShop') IS NULL
    CREATE DATABASE [DB_Denisov_CoffeeShop];
GO

USE [DB_Denisov_CoffeeShop];
GO

DROP TABLE IF EXISTS dbo.Payment;
DROP TABLE IF EXISTS dbo.Category;
DROP TABLE IF EXISTS dbo.[User];
GO

CREATE TABLE dbo.[User] (
    ID       INT IDENTITY(1, 1) NOT NULL,
    Login    NVARCHAR(50)       NOT NULL,
    Password NVARCHAR(100)      NOT NULL, 
    Role     NVARCHAR(10)       NOT NULL, 
    FIO      NVARCHAR(150)      NOT NULL, 
    Photo    NVARCHAR(260)      NULL,     
    CONSTRAINT PK_User       PRIMARY KEY (ID),
    CONSTRAINT UQ_User_Login UNIQUE (Login),
    CONSTRAINT CK_User_Role  CHECK (Role IN (N'Admin', N'User'))
);
GO

CREATE TABLE dbo.Category (
    ID   INT IDENTITY(1, 1) NOT NULL,
    Name NVARCHAR(100)      NOT NULL,
    CONSTRAINT PK_Category      PRIMARY KEY (ID),
    CONSTRAINT UQ_Category_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.Payment (
    ID         INT IDENTITY(1, 1) NOT NULL,
    UserID     INT                NOT NULL,
    CategoryID INT                NOT NULL,
    [Date]     DATE               NOT NULL,
    Name       NVARCHAR(150)      NOT NULL, 
    Num        INT                NOT NULL, 
    Price      DECIMAL(10, 2)     NOT NULL, 
    CONSTRAINT PK_Payment PRIMARY KEY (ID),
    CONSTRAINT FK_Payment_User FOREIGN KEY (UserID)
        REFERENCES dbo.[User] (ID) ON DELETE CASCADE,
    CONSTRAINT FK_Payment_Category FOREIGN KEY (CategoryID)
        REFERENCES dbo.Category (ID) ON DELETE CASCADE,
    CONSTRAINT CK_Payment_Num   CHECK (Num BETWEEN 1 AND 100),
    CONSTRAINT CK_Payment_Price CHECK (Price > 0 AND Price <= 100000)
);
GO


INSERT INTO dbo.[User] (Login, Password, Role, FIO, Photo)
VALUES
    (N'admin', N'E8509EBCE57B831FE321D6EC2B5A1AE00FD87188', N'Admin', N'Смирнова Елена Викторовна', NULL),
    (N'manager', N'0C833EF7630B5D9B6ADC60B2173606D25684F8D2', N'Admin', N'Козлов Андрей Петрович', NULL),
    (N'barista_anna', N'7C9C93C192B2B3814F2484DBCC9E6F9AAE6CE3F9', N'User', N'Морозова Анна Сергеевна', NULL),
    (N'barista_oleg', N'07A86293E5005F5C97BF1808CAA1694C7B891AAF', N'User', N'Волков Олег Дмитриевич', NULL),
    (N'cashier_maria', N'4FF0751EAB494DC322E53A8918B26CD40C1C286B', N'User', N'Новикова Мария Игоревна', NULL),
    (N'cashier_pavel', N'E4DDBC04DC07A9F6799065E77AD1EE9DEB432B50', N'User', N'Лебедев Павел Андреевич', NULL);
GO

INSERT INTO dbo.Category (Name)
VALUES
    (N'Кофе'),
    (N'Чай'),
    (N'Десерты'),
    (N'Выпечка'),
    (N'Напитки');
GO

INSERT INTO dbo.Payment (UserID, CategoryID, [Date], Name, Num, Price)
VALUES
    (3, 1, '2026-09-14', N'Капучино', 2, 250.00),
    (3, 4, '2026-09-14', N'Круассан классический', 2, 150.00),
    (4, 1, '2026-09-15', N'Американо', 1, 180.00),
    (5, 3, '2026-09-16', N'Чизкейк Нью-Йорк', 2, 280.00),
    (6, 2, '2026-09-17', N'Масала-чай', 3, 250.00),
    (3, 5, '2026-09-18', N'Лимонад домашний', 2, 250.00),
    (4, 3, '2026-09-19', N'Тирамису', 1, 310.00),
    (5, 1, '2026-09-20', N'Латте', 4, 270.00),
    (6, 4, '2026-09-21', N'Синнабон', 2, 210.00),
    (1, 1, '2026-09-22', N'Флэт уайт', 1, 290.00),
    (2, 2, '2026-09-23', N'Чай зелёный с жасмином', 2, 170.00),
    (4, 5, '2026-09-24', N'Айс-латте', 2, 290.00),
    (3, 3, '2026-09-25', N'Брауни', 3, 220.00),
    (5, 4, '2026-09-26', N'Булочка с корицей', 5, 140.00),
    (6, 1, '2026-09-27', N'Раф', 2, 320.00),
    (1, 3, '2026-09-28', N'Медовик', 1, 260.00),
    (2, 5, '2026-09-29', N'Свежевыжатый апельсиновый сок', 2, 300.00),
    (4, 2, '2026-10-01', N'Облепиховый чай', 2, 230.00),
    (3, 1, '2026-10-02', N'Латте', 3, 270.00),
    (5, 5, '2026-10-03', N'Милкшейк ванильный', 2, 320.00),
    (6, 3, '2026-10-04', N'Макарон ассорти', 6, 120.00),
    (1, 4, '2026-10-06', N'Круассан с миндалём', 2, 190.00),
    (2, 1, '2026-10-07', N'Эспрессо', 2, 150.00),
    (4, 4, '2026-10-08', N'Кекс морковный', 3, 170.00),
    (3, 2, '2026-10-10', N'Чай чёрный классический', 2, 160.00),
    (5, 2, '2026-10-12', N'Масала-чай', 2, 250.00),
    (6, 5, '2026-10-14', N'Вода без газа', 4, 80.00),
    (1, 5, '2026-10-16', N'Лимонад домашний', 1, 250.00),
    (2, 3, '2026-10-18', N'Тирамису', 2, 310.00),
    (4, 1, '2026-10-20', N'Капучино', 5, 250.00);
GO


SELECT * FROM dbo.[User];
SELECT * FROM dbo.Category;
SELECT * FROM dbo.Payment;

SELECT p.ID,
       p.[Date],
       u.FIO      AS Пользователь,
       c.Name     AS Категория,
       p.Name     AS Позиция,
       p.Num      AS Количество,
       p.Price    AS Цена,
       p.Num * p.Price AS Сумма
FROM dbo.Payment AS p
JOIN dbo.[User]  AS u ON u.ID = p.UserID
JOIN dbo.Category AS c ON c.ID = p.CategoryID
ORDER BY p.[Date], p.ID;
GO
