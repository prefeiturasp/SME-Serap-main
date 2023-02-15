 ALTER TABLE [dbo].[Test] ADD [TestTai] [bit] NOT NULL DEFAULT 0;

drop table NumberItemsAplicationTai;
CREATE TABLE [dbo].[NumberItemsAplicationTai](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name][varchar](100) not null,
	[Value][varchar](100) not null,
	[CreateDate][datetime] not null,
	[UpdateDate][datetime] not null,
	[State][tinyint] not null
CONSTRAINT [PK_dbo.NumberItemsAplicationTai] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
);
insert into NumberItemsAplicationTai ([Name],[value],[CreateDate],[UpdateDate],[State]) values
('Todos os itens','',getdate(),getdate(),1),('20','20',getdate(),getdate(),1),('30','30',getdate(),getdate(),1),('40','40',getdate(),getdate(),1),('50','50',getdate(),getdate(),1);


drop table NumberItemTestTai;
CREATE TABLE [dbo].[NumberItemTestTai](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TestId][bigint] not null,
	[ItemAplicationTaiId][bigint] not null,
	[CreateDate][datetime] not null,
	[UpdateDate][datetime] not null,
	[State][tinyint] not null
);
ALTER TABLE [dbo].[NumberItemTestTai]  WITH CHECK ADD  CONSTRAINT [FK_NumberItemTestTai_TestId] FOREIGN KEY([TestId])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE;
ALTER TABLE [dbo].[NumberItemTestTai]  WITH CHECK ADD  CONSTRAINT [FK_NumberItemTestTai_ItemAplicationTaiId] FOREIGN KEY([ItemAplicationTaiId])
REFERENCES [dbo].[NumberItemsAplicationTai] ([Id])
ON DELETE CASCADE;

