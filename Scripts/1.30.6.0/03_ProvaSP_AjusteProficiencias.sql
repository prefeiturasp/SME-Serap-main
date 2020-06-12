BEGIN TRAN
UPDATE ProvaSP.dbo.NivelProficienciaAnoEscolar SET Nome = 'Abaixo do Básico'
	WHERE AnoEscolar = 5 and Nome = 'Nivel basico'
COMMIT