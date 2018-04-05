USE GestaoAvaliacao
GO

UPDATE [dbo].[Parameter] SET [Description] = 'Habilitar o download das imagens das folhas de respostas dos alunos - OMR' 
WHERE [Key] = 'DOWNLOAD_OMR_FILE'