-- User Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    UserPassword NVARCHAR(255) NOT NULL,
    Avatar NVARCHAR(255) DEFAULT 'https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png',
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);


-- Restaurant Table
CREATE TABLE Restaurants (
    RestaurantId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
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
    MondayOpens NVARCHAR(10) NOT NULL,
    MondayCloses NVARCHAR(10) NOT NULL,
    TuesdayOpens NVARCHAR(10) NOT NULL,
    TuesdayCloses NVARCHAR(10) NOT NULL,
    WednesdayOpens NVARCHAR(10),
    WednesdayCloses NVARCHAR(10),
    ThursdayOpens NVARCHAR(10) NOT NULL,
    ThursdayCloses NVARCHAR(10) NOT NULL,
    FridayOpens NVARCHAR(10) NOT NULL,
    FridayCloses NVARCHAR(10) NOT NULL,
    SaturdayOpens NVARCHAR(10) NOT NULL,
    SaturdayCloses NVARCHAR(10) NOT NULL,
    SundayOpens NVARCHAR(10) NOT NULL,
    SundayCloses NVARCHAR(10) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    INDEX IX_UserId (UserId)
);

-- Product Table
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    RestaurantId INT FOREIGN KEY REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE,
    ProductName NVARCHAR(255) NOT NULL,
    ProductPhoto NVARCHAR(MAX) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    ProductPrice DECIMAL(10, 2) NOT NULL,
    ProductExplanation NVARCHAR(MAX) NOT NULL,
    ProductPreparationTime NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    INDEX IX_RestaurantId (RestaurantId)
);

-- Review Table
CREATE TABLE Reviews (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    RestaurantId INT NOT NULL FOREIGN KEY REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    Comment NVARCHAR(MAX) NOT NULL,
    ReviewPhoto NVARCHAR(MAX),
    Rating INT CHECK(Rating >= 1 AND Rating <= 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    INDEX IX_RestaurantId (RestaurantId),
    INDEX IX_UserId (UserId)
);

-- -- Favorite Table
-- CREATE TABLE Favorites (
--     FavoriteId INT PRIMARY KEY IDENTITY(1,1),
--     RestaurantId INT NOT NULL FOREIGN KEY REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE,
--     UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
--     RestaurantName NVARCHAR(255) NOT NULL,
--     ExtraInfo NVARCHAR(MAX) NOT NULL,
--     RestaurantLogo NVARCHAR(255) DEFAULT 'https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png',
--     CreatedAt DATETIME DEFAULT GETDATE(),
--     UpdatedAt DATETIME DEFAULT GETDATE(),
--     INDEX IX_RestaurantId (RestaurantId),
--     INDEX IX_UserId (UserId)
-- );
-- updated Favorite Table
CREATE TABLE Favorites (
    FavoriteId INT PRIMARY KEY IDENTITY(1,1),
    RestaurantId INT NOT NULL FOREIGN KEY REFERENCES Restaurants(RestaurantId) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    INDEX IX_RestaurantId (RestaurantId),
    INDEX IX_UserId (UserId)
);



-- The ON DELETE CASCADE clause is added to foreign key constraints to automatically delete related 
-- records in child tables when a record in the parent table is deleted.