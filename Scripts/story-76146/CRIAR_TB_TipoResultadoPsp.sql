use GestaoAvaliacao

CREATE TABLE [dbo].[TipoResultadoPsp](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Codigo] [bigint] NOT NULL,
	[Nome] [varchar](500) NOT NULL,
	[NomeTabelaProvaSp] [varchar](100) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL)