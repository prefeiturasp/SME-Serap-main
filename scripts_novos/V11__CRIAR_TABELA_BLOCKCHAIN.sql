use GestaoAvaliacao

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name='BlockChain' and type='U')
begin
	CREATE TABLE GestaoAvaliacao.dbo.BlockChain (
		Id bigint IDENTITY(1,1) NOT NULL,
		Description varchar(10) COLLATE Latin1_General_CI_AS NOT NULL,
		CreateDate datetime NOT NULL,
		UpdateDate datetime NOT NULL,
		State tinyint NOT NULL,
		Test_Id bigint NULL,
		CONSTRAINT [PK_dbo.BlockChain] PRIMARY KEY (Id),
		CONSTRAINT [FK_dbo.BlockChain_dbo.Test_Test_Id] FOREIGN KEY (Test_Id) REFERENCES GestaoAvaliacao.dbo.Test(Id)
	);
	 CREATE NONCLUSTERED INDEX IX_Test_Id ON dbo.BlockChain (  Test_Id ASC  )  
		 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
		 ON [PRIMARY ] ;
end;