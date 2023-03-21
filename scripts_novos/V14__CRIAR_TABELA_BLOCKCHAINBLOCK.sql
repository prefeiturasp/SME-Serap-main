use GestaoAvaliacao

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name='BlockChainBlock' and type='U')
begin
	CREATE TABLE GestaoAvaliacao.dbo.BlockChainBlock (
		Id bigint IDENTITY(1,1) NOT NULL,
		Description varchar(10) COLLATE Latin1_General_CI_AS NOT NULL,
		CreateDate datetime NOT NULL,
		UpdateDate datetime NOT NULL,
		State tinyint NOT NULL,
		BlockChain_Id bigint NULL,
		CONSTRAINT [PK_dbo.BlockChainBlock] PRIMARY KEY (Id),
		CONSTRAINT [FK_dbo.BlockChainBlock_dbo.BlockChain_BlockChain_Id] FOREIGN KEY (BlockChain_Id) REFERENCES GestaoAvaliacao.dbo.BlockChain(Id)
	);
	 CREATE NONCLUSTERED INDEX IX_BlockChain_Id ON dbo.BlockChainBlock (  BlockChain_Id ASC  )  
		 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
		 ON [PRIMARY ] ;
end;