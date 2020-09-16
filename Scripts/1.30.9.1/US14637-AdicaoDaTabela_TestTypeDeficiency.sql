ALTER TABLE TestType ADD TargetToStudentsWithDeficiencies BIT NOT NULL CONSTRAINT DF_TestType_TargetToStudentsWithDeficiencies DEFAULT 0;
GO
ALTER TABLE TestType DROP CONSTRAINT DF_TestType_TargetToStudentsWithDeficiencies;
GO

CREATE TABLE TestTypeDeficiency
(
	Id BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	CreateDate DATETIME NOT NULL,
	UpdateDate DATETIME NOT NULL,
	State TINYINT NOT NULL,
	DeficiencyId UNIQUEIDENTIFIER NOT NULL,
	TestType_Id BIGINT NOT NULL,
	FOREIGN KEY(TestType_Id) REFERENCES TestType(Id)
);
GO
ALTER TABLE TestTypeDeficiency ADD UNIQUE (TestType_Id, DeficiencyId);
GO