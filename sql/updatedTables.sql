-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Active BIT NOT NULL DEFAULT 1
);


-- from youtube
CREATE TABLE Users
(
    UserId INT IDENTITY(1, 1) PRIMARY KEY
    , FirstName NVARCHAR(50)
    , LastName NVARCHAR(50)
    , Email NVARCHAR(50)
    , Gender NVARCHAR(50)
    , Active BIT
);

-- Auth Table
CREATE TABLE Auth(
    Email NVARCHAR(255) PRIMARY KEY,
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX),
    FOREIGN KEY (Email) REFERENCES Users(Email) -- Connect Auth to Users via Email
);

-- Restaurants Table
CREATE TABLE Restaurants (
    RestaurantId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL, -- Foreign Key
    RestaurantName NVARCHAR(255) NOT NULL,
    RestaurantCode INT,
    FullAddress NVARCHAR(255) NOT NULL,
    Latitude NVARCHAR(100) NOT NULL,
    Longitude NVARCHAR(100) NOT NULL,
    EstablishmentType NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    ExtraInfo NVARCHAR(MAX) NOT NULL,
    AboutUs NVARCHAR(MAX) NOT NULL,
    CreditCard BIT NOT NULL,
    Cash BIT NOT NULL,
    FoodCards BIT NOT NULL,
    AppleGooglePay BIT NOT NULL,
    OutdoorSitting BIT NOT NULL,
    PetsAllowed BIT NOT NULL,
    Alcohol BIT NOT NULL,
    Parking BIT NOT NULL,
    OffersDelivery BIT NOT NULL,
    OffersTakeout BIT NOT NULL,
    GoodForGroups BIT NOT NULL,
    GoodForKids BIT NOT NULL,
    FullBar BIT NOT NULL,
    TakesReservation BIT NOT NULL,
    WaiterService BIT NOT NULL,
    SelfService BIT NOT NULL,
    HasTV BIT NOT NULL,
    FreeWifi BIT NOT NULL,
    StreetParking BIT NOT NULL,
    BeerAndWineOnly BIT NOT NULL,
    Italian BIT NOT NULL,
    Hookah BIT NOT NULL,
    Burger BIT NOT NULL,
    HotDogs BIT NOT NULL,
    FastFood BIT NOT NULL,
    Breakfast BIT NOT NULL,
    Doner BIT NOT NULL,
    HalalFood BIT NOT NULL,
    ImageUrls NVARCHAR(MAX) NOT NULL,
    MondayOpens TIME NOT NULL,
    MondayCloses TIME NOT NULL,
    TuesdayOpens TIME NOT NULL,
    TuesdayCloses TIME NOT NULL,
    WednesdayOpens TIME,
    WednesdayCloses TIME,
    ThursdayOpens TIME NOT NULL,
    ThursdayCloses TIME NOT NULL,
    FridayOpens TIME NOT NULL,
    FridayCloses TIME NOT NULL,
    SaturdayOpens TIME NOT NULL,
    SaturdayCloses TIME NOT NULL,
    SundayOpens TIME NOT NULL,
    SundayCloses TIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    Active BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE -- Foreign Key to Users
);

-- Products Table
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    RestaurantId INT NOT NULL, -- Foreign Key
    ProductName NVARCHAR(255) NOT NULL,
    ProductPhoto NVARCHAR(MAX) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    ProductPrice DECIMAL(10, 2) NOT NULL,
    ProductExplanation NVARCHAR(MAX) NOT NULL,
    ProductPreparationTime NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RestaurantId) REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE -- Foreign Key to Restaurants
);

-- Reviews Table
CREATE TABLE Reviews (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    RestaurantId INT NOT NULL, -- Foreign Key
    UserId INT NOT NULL, -- Foreign Key
    Comment NVARCHAR(MAX) NOT NULL,
    ReviewPhoto NVARCHAR(MAX),
    Rating INT CHECK(Rating >= 1 AND Rating <= 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RestaurantId) REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE, -- Foreign Key to Restaurants
    FOREIGN KEY (UserId) REFERENCES Users(UserId) -- Foreign Key to Users
);

-- Favorites Table
CREATE TABLE Favorites (
    FavoriteId INT PRIMARY KEY IDENTITY(1,1),
    RestaurantId INT NOT NULL, -- Foreign Key
    UserId INT NOT NULL, -- Foreign Key
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RestaurantId) REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE, -- Foreign Key to Restaurants
    FOREIGN KEY (UserId) REFERENCES Users(UserId) -- Foreign Key to Users
);
