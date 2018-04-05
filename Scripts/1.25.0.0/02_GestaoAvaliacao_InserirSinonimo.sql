USE [GestaoAvaliacao_SGP]
GO

/****** Object:  Synonym [dbo].[SGP_ESC_Escola]    Script Date: 27/06/2017 17:14:23 ******/
CREATE SYNONYM [dbo].[Synonym_AdministrativeUnitType] FOR [GestaoAvaliacao].[dbo].[AdministrativeUnitType]
GO

USE [GestaoAvaliacao]
GO

/****** Object:  Synonym [dbo].[Synonym_Core_SYS_TipoUnidadeAdministrativa]    Script Date: 28/06/2017 10:28:03 ******/
CREATE SYNONYM [dbo].[Synonym_Core_SYS_TipoUnidadeAdministrativa] FOR [CoreSSO].[dbo].[SYS_TipoUnidadeAdministrativa]
GO
