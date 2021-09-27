-- INSERT_DEPARA_CURSOS_DIVIDIDOS_tmpTipoSerieEOL
USE [Manutencao]
GO

INSERT INTO [dbo].[DEPARA_CURSOS_DIVIDIDOS]
           ([cd_etapa_ensino]
           ,[cur_id])
     VALUES
           (18
           ,146)
GO

USE [Manutencao]
GO

INSERT INTO [dbo].[tmpTipoSerieEOL]
           ([cd_serie_eol98])
     VALUES
           (414);

GO