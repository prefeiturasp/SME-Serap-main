CREATE TABLE UploadFileItem
(
	Id BIGINT PRIMARY KEY IDENTITY(1,1),
	CreatedDate DATETIME NOT NULL,
	OriginPath VARCHAR(MAX) NOT NULL,
	[FileName] VARCHAR(100) NOT NULL,
	Situation TINYINT NOT NULL,
	NotificationMessage VARCHAR(MAX) NULL,
	UploadFileBatchId BIGINT NOT NULL,
	FOREIGN KEY(UploadFileBatchId) REFERENCES UploadFileBatch(Id)
);
GO

CREATE INDEX idx_UploadFileItem_Batch ON UploadFileItem (UploadFileBatchId);
GO
