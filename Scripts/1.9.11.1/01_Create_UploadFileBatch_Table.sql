CREATE TABLE UploadFileBatch
(
	Id BIGINT PRIMARY KEY IDENTITY(1,1),
	CreatedDate DATETIME NOT NULL,
	BeginDate DATETIME NULL,
	UpdateDate DATETIME NULL,
	UploadFileBatchType SMALLINT NOT NULL,
	Edicao VARCHAR(4) NOT NULL,
	AreaDeConhecimento SMALLINT NOT NULL,
	CicloDeAprendizagem SMALLINT NOT NULL,
	Situation TINYINT NOT NULL,
	UsuId UNIQUEIDENTIFIER NOT NULL,
	UsuName VARCHAR(200) NOT NULL,
	FileCount INT NULL,
	FileErrorCount INT NULL
);
GO

CREATE INDEX idx_UploadFileBatch_Type_Situation ON UploadFileBatch (UploadFileBatchType, Situation);
GO

CREATE INDEX idx_UploadFileBatch_UsuId ON UploadFileBatch (UsuId);
GO

CREATE INDEX idx_UploadFileBatch_Type_Edicao_AreaDeConhecimento_CicloDeAprendizagem ON UploadFileBatch (UploadFileBatchType, Edicao, AreaDeConhecimento, CicloDeAprendizagem);
GO