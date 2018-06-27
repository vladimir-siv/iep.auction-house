CREATE TABLE [SystemParameters]
(
	[ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	[RecentAuctions] INT NOT NULL,
	[DefaultAuctionTime] INT NOT NULL,
	[SilverPackage] DECIMAL(24, 12) NOT NULL,
	[GoldPackage] DECIMAL(24, 12) NOT NULL,
	[PlatinumPackage] DECIMAL(24, 12) NOT NULL,
	[Currency] VARCHAR(32) NOT NULL,
	[PriceRate] DECIMAL(24, 12) NOT NULL,
);

CREATE TABLE [Users]
(
	[ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	[FirstName] VARCHAR(64) NOT NULL,
	[LastName] VARCHAR(64) NOT NULL,
	[Email] VARCHAR(64) UNIQUE NOT NULL,
	[Password] VARCHAR(64) NOT NULL,
	[Balance] DECIMAL(24, 12) NOT NULL DEFAULT 0.00,
);

CREATE TABLE [Administrators]
(
	[ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL
		FOREIGN KEY REFERENCES [Users](ID)
		ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE [TokenOrders]
(
	[ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	[Buyer] UNIQUEIDENTIFIER NOT NULL
		FOREIGN KEY REFERENCES [Users](ID)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	[Amount] DECIMAL(24, 12) NOT NULL,
	[Currency] VARCHAR(32) NOT NULL,
	[PriceRate] DECIMAL(24, 12) NOT NULL,
	[Status] [bit] NULL DEFAULT NULL,
);

CREATE TABLE [Auctions]
(
	[ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	[Title] VARCHAR(64) NOT NULL,
	[AuctionTime] INT NOT NULL,
	[CreatedOn] [DATETIME] NOT NULL DEFAULT GETDATE(),
	[OpenedOn] [DATETIME] NULL DEFAULT NULL,
	[CompletedOn] [DATETIME] NULL DEFAULT NULL,
	[StartingPrice] DECIMAL(24, 12) NOT NULL,
	[Currency] VARCHAR(32) NOT NULL,
	[PriceRate] DECIMAL(24, 12) NOT NULL,
);

CREATE TABLE [Bids]
(
	[ID] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	[Bidder] UNIQUEIDENTIFIER NOT NULL
		FOREIGN KEY REFERENCES [Users](ID)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	[Auction] UNIQUEIDENTIFIER NOT NULL
		FOREIGN KEY REFERENCES [Auctions](ID)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	[BidOn] [DATETIME] NOT NULL DEFAULT GETDATE(),
	[Amount] DECIMAL(24, 12) NOT NULL
);

INSERT INTO [SystemParameters](RecentAuctions, DefaultAuctionTime, SilverPackage, GoldPackage, PlatinumPackage, Currency, PriceRate) VALUES
	(10, 3600, 30.00, 50.00, 100.00, 'EUR', 0.25);

INSERT INTO [Users](FirstName, LastName, Email, Password) VALUES
	('Vladimir', 'Sivèev', 'vladimirsi@nordeus.com', '191fc174ed9bd33973e55f7b5a948c8b'),
	('Radovan', 'Silaški', 'rsilaski@gmail.com', '6e99cd25256d4351f3fb776027e8d271'),
	('Marko', 'Francuski', 'francmarko@gmail.com', '12f46699c76e0669c77de31a13d77943'),
	('Predrag', 'Mitroviæ', 'pedja1996@gmail.com', 'e04952e2f8d85f604967caf07080bc8c');

INSERT INTO [Administrators](ID)
	SELECT ID FROM [Users] WHERE Email = 'vladimirsi@nordeus.com';