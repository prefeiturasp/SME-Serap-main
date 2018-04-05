USE [GestaoAvaliacao]
GO

IF OBJECT_ID('tempdb..#tmpExcluir') IS NOT NULL
	DROP TABLE #tmpExcluir

CREATE TABLE #tmpExcluir (id BIGINT) 

PRINT 'Registros à serem atualizados...'
INSERT INTO #tmpExcluir SELECT Id FROM Test WITH(NOLOCK) WHERE id IN (183,182,167,166,139,140,138,103,101)

PRINT 'Updating Test...'
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM Test exc
INNER JOIN #tmpExcluir tmp ON exc.Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating Adherence...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM Adherence exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating AnswerSheetBatch...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM AnswerSheetBatch exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating AnswerSheetLot...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM AnswerSheetLot exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating Block...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM Block exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating Booklet...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM Booklet exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating ExportAnalysis...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM ExportAnalysis exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating RequestRevoke...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM RequestRevoke exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating StudentTestAbsenceReason...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM StudentTestAbsenceReason exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating TestCurriculumGrade...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM TestCurriculumGrade exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating TestFiles...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM TestFiles exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating TestItemLevel...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM TestItemLevel exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating TestPerformanceLevel...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM TestPerformanceLevel exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating TestSectionStatusCorrection...'-- 
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM TestSectionStatusCorrection exc
INNER JOIN #tmpExcluir tmp ON exc.Test_Id = tmp.id
WHERE exc.[State] <> 3


PRINT 'Updating File...'-- 
-- File
--OwnerType= 5,8 (Tipo relacionado à Prova, tipo relacionado à folha de resposta)
--ParentOwnerId será o Id da Prova Test_ID
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM [File] exc
INNER JOIN #tmpExcluir tmp ON exc.ParentOwnerId = tmp.id AND exc.OwnerType IN (5,8)
WHERE exc.[State] <> 3


PRINT 'Updating File...'-- 
-- OwnerType = 9 (Tipo relacionado ao Lote)
--ParentOwnerId será o id do lote AnswerSheetbatchId
UPDATE exc SET [State] = 3,
  			 UpdateDate = GETDATE()
FROM [File] exc
INNER JOIN AnswerSheetBatch asb ON exc.ParentOwnerId = asb.Id AND exc.OwnerType = 9
INNER JOIN #tmpExcluir tmp ON asb.Test_Id = tmp.Id
WHERE exc.[State] <> 3
