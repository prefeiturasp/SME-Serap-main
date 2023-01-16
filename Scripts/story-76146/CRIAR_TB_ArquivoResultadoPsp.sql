use GestaoAvaliacao

CREATE TABLE [dbo].[ArquivoResultadoPsp]
(
[Id] [bigint] IDENTITY(1,1) NOT NULL,
[CodigoTipoResultado] [bigint] NOT NULL,
[NomeArquivo] [varchar](500) NOT NULL,
[NomeOriginalArquivo] [varchar](500) NOT NULL,
[CreateDate] [datetime] NOT NULL,
[UpdateDate] [datetime] NOT NULL,
[State] [tinyint] NOT NULL
)