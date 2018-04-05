USE [GestaoAvaliacao_SGP]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	UPDATE ESC_Escola 
	SET tua_id = (SELECT tua_id FROM CoreSSO..SYS_TipoUnidadeAdministrativa AS Tua WITH(NOLOCK) WHERE Tua.tua_situacao <> 3 AND Tua.tua_nome = SUBSTRING(esc_nome, 0, CHARINDEX('-', esc_nome) - 1))

	INSERT INTO [dbo].[Synonym_AdministrativeUnitType] (AdministrativeUnitTypeId, Name, CreateDate, UpdateDate, State)
	SELECT DISTINCT Esc.tua_id, Tua.tua_nome, GETDATE(), GETDATE(), 1 AS situacao
	FROM 
		ESC_Escola Esc WITH(NOLOCK)
		INNER JOIN CoreSSO..SYS_TipoUnidadeAdministrativa AS Tua WITH(NOLOCK)
			ON Esc.tua_id = Tua.tua_id
		

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION