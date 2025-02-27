use GestaoAvaliacao;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BlockChainBlock' AND COLUMN_NAME = 'Order') BEGIN
	ALTER TABLE BlockChainBlock ADD [Order] int NULL;
END;
GO

update BlockChainBlock
set [Order] = 0
where [Order] is null;
GO

ALTER TABLE BlockChainBlock ALTER COLUMN [Order] int NOT NULL;
GO
