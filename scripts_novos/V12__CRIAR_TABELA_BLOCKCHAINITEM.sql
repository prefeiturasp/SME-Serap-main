use GestaoAvaliacao

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name='BlockChainItem' and type='U')
begin
	CREATE TABLE GestaoAvaliacao.dbo.BlockChainItem (
		BlockChain_Id bigint NOT NULL,
		Item_Id bigint NOT NULL,
		Id bigint IDENTITY(1,1) NOT NULL,
		[Order] int NOT NULL,
		CreateDate datetime NOT NULL,
		UpdateDate datetime NOT NULL,
		State tinyint NOT NULL,
		CONSTRAINT [PK_dbo.BlockChainItem] PRIMARY KEY (Id),
		CONSTRAINT [FK_dbo.BlockChainItem_dbo.BlockChain_BlockChain_Id] FOREIGN KEY (BlockChain_Id) REFERENCES GestaoAvaliacao.dbo.BlockChain(Id) ON DELETE CASCADE,
		CONSTRAINT [FK_dbo.BlockChainItem_dbo.Item_Item_Id] FOREIGN KEY (Item_Id) REFERENCES GestaoAvaliacao.dbo.Item(Id) ON DELETE CASCADE
	);
	 CREATE NONCLUSTERED INDEX IX_BlockChainItem_Id ON dbo.BlockChainItem (  BlockChainItem_Id ASC  )  
		 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
		 ON [PRIMARY ] ;
	 CREATE NONCLUSTERED INDEX IX_Item_Id ON dbo.BlockChainItem (  Item_Id ASC  )  
		 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
		 ON [PRIMARY ] ;
end;