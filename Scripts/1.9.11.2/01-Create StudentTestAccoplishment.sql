CREATE TABLE StudentTestAccoplishment
(
	Id BIGINT PRIMARY KEY IDENTITY(1,1),
	CreateDate DATETIME NOT NULL,
	UpdateDate DATETIME NULL,
	[State] TINYINT NOT NULL,
	AluId BIGINT NOT NULL,
	TurId BIGINT NOT NULL,
	Test_Id BIGINT NOT NULL,
	Situation SMALLINT NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NULL,
	CONSTRAINT UK_Accoplishment UNIQUE (AluId, TurId, Test_Id),
	FOREIGN KEY (Test_Id) REFERENCES Test(Id)
);
GO

CREATE INDEX idx_StudentTestAccoplishment_Test_Turma ON StudentTestAccoplishment (TurId, Test_Id);
GO

CREATE INDEX idx_StudentTestAccoplishment_Test_Situation ON StudentTestAccoplishment (Test_Id, Situation);
GO