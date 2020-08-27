DECLARE @anoModificadoAnteriormente AS VARCHAR(4) = '1999';
DECLARE @anoCorreto AS VARCHAR(4) = '2019';

BEGIN TRAN

UPDATE ResultadoSme SET Edicao = @anoCorreto WHERE Edicao = @anoModificadoAnteriormente AND AnoEscolar <> 3;
UPDATE ResultadoDre SET Edicao = @anoCorreto WHERE Edicao = @anoModificadoAnteriormente AND AnoEscolar <> 3;
UPDATE ResultadoEscola SET Edicao = @anoCorreto WHERE Edicao = @anoModificadoAnteriormente AND AnoEscolar <> 3;
UPDATE ResultadoAluno SET Edicao = @anoCorreto WHERE Edicao = @anoModificadoAnteriormente AND AnoEscolar <> 3;

COMMIT