CREATE TABLE StudentTestSession
(
	Id BIGINT PRIMARY KEY IDENTITY(1,1),
	CreateDate DATETIME NOT NULL,
	UpdateDate DATETIME NULL,
	[State] TINYINT NOT NULL,
	StudentTestAccoplishment_Id BIGINT NOT NULL,
	ConnectionId UNIQUEIDENTIFIER NOT NULL,
	Situation SMALLINT NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NULL,
	FOREIGN KEY (StudentTestAccoplishment_Id) REFERENCES StudentTestAccoplishment(Id)
);
GO