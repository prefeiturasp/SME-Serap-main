USE [GestaoAvaliacao]
GO


SET XACT_ABORT ON

BEGIN TRANSACTION

IF OBJECT_ID('tempdb..#tmptest') IS NOT NULL DROP TABLE #tmpTest

CREATE TABLE #tmpTest(testID INT, Disciplina VARCHAR(20) COLLATE Latin1_General_CI_AI)

/*valore passados por email*/
INSERT INTO #tmpTest VALUES(148,'Matemática')
INSERT INTO #tmpTest VALUES(149,'Matemática')				
INSERT INTO #tmpTest VALUES(151,'Matemática')				
INSERT INTO #tmpTest VALUES(152,'Matemática')	
INSERT INTO #tmpTest VALUES(154,'Matemática')	
INSERT INTO #tmpTest VALUES(157,'Matemática')	
INSERT INTO #tmpTest VALUES(161,'Matemática')	
INSERT INTO #tmpTest VALUES(163,'Matemática')	
INSERT INTO #tmpTest VALUES(165,'Matemática')	
INSERT INTO #tmpTest VALUES(166,'Matemática')	
INSERT INTO #tmpTest VALUES(168,'Matemática')	
INSERT INTO #tmpTest VALUES(169,'Matemática')	
INSERT INTO #tmpTest VALUES(170,'Matemática')	
INSERT INTO #tmpTest VALUES(172,'Matemática')	
INSERT INTO #tmpTest VALUES(173,'Matemática')	
INSERT INTO #tmpTest VALUES(175,'Matemática')	
INSERT INTO #tmpTest VALUES(178,'Matemática')	

INSERT INTO #tmpTest VALUES(150,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(153,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(155,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(156,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(158,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(159,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(160,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(162,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(164,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(167,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(171,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(174,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(176,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(177,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(179,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(180,'Língua portuguesa')
INSERT INTO #tmpTest VALUES(181,'Língua portuguesa')

DECLARE @EvaluationMatrix_IdLP INT,
		@EvaluationMatrix_IdMT INT,
		@SkillIdNotLastLevelLP INT,
		@SkillIdLastLevelLP INT,
		@SkillIdNotLastLevelMT INT,
		@SkillIdLastLevelMT INT

SET @EvaluationMatrix_IdLP = 16 -- valores passados por email
SET @EvaluationMatrix_IdMT = 29 -- valores passados por email

SELECT @SkillIdNotLastLevelLP = MIN(Id) FROM Skill WHERE EvaluationMatrix_Id = @EvaluationMatrix_IdLP AND LastLevel = 0 AND [State] = 1
SELECT @SkillIdLastLevelLP =  MIN(Id) FROM Skill WHERE EvaluationMatrix_Id = @EvaluationMatrix_IdLP AND LastLevel = 1 AND Parent_Id = @SkillIdNotLastLevelLP AND [State] = 1
SELECT @SkillIdNotLastLevelMT = MIN(Id) FROM Skill WHERE EvaluationMatrix_Id = @EvaluationMatrix_IdMT AND LastLevel = 0 AND [State] = 1
SELECT @SkillIdLastLevelMT = MIN(Id) FROM Skill WHERE EvaluationMatrix_Id = @EvaluationMatrix_IdMT AND LastLevel = 1 AND Parent_Id = @SkillIdNotLastLevelMT AND [State] = 1


-- 1º ItemSkill
-- Skill_Id
UPDATE its SET Skill_Id =  @SkillIdNotLastLevelLP
--SELECT its.Id, its.Item_Id, its.Skill_Id, ski.LastLevel	
FROM ItemSkill its WITH(NOLOCK)
INNER JOIN Item ite WITH(NOLOCK) ON its.Item_Id = ite.Id
INNER JOIN BlockItem bli WITH(NOLOCK) ON ite.Id = bli.Item_Id
INNER JOIN Block blo WITH(NOLOCK) ON bli.Block_Id = blo.Id
INNER JOIN Test tes WITH(NOLOCK) ON blo.Test_Id = tes.Id
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
INNER JOIN Skill ski WITH(NOLOCK) ON its.Skill_Id = ski.Id
WHERE tmp.Disciplina = 'Língua portuguesa'
AND LastLevel = 0

UPDATE its SET Skill_Id =  @SkillIdLastLevelLP
--SELECT its.Id, its.Item_Id, its.Skill_Id, ski.LastLevel
FROM ItemSkill its WITH(NOLOCK)
INNER JOIN Item ite WITH(NOLOCK) ON its.Item_Id = ite.Id
INNER JOIN BlockItem bli WITH(NOLOCK) ON ite.Id = bli.Item_Id
INNER JOIN Block blo WITH(NOLOCK) ON bli.Block_Id = blo.Id
INNER JOIN Test tes WITH(NOLOCK) ON blo.Test_Id = tes.Id
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
INNER JOIN Skill ski WITH(NOLOCK) ON its.Skill_Id = ski.Id
WHERE tmp.Disciplina = 'Língua portuguesa'
AND LastLevel = 1

UPDATE its SET Skill_Id =  @SkillIdNotLastLevelMT
--SELECT its.Id, its.Item_Id, its.Skill_Id, ski.LastLevel
FROM ItemSkill its WITH(NOLOCK)
INNER JOIN Item ite WITH(NOLOCK) ON its.Item_Id = ite.Id
INNER JOIN BlockItem bli WITH(NOLOCK) ON ite.Id = bli.Item_Id
INNER JOIN Block blo WITH(NOLOCK) ON bli.Block_Id = blo.Id
INNER JOIN Test tes WITH(NOLOCK) ON blo.Test_Id = tes.Id
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
INNER JOIN Skill ski WITH(NOLOCK) ON its.Skill_Id = ski.Id
WHERE tmp.Disciplina = 'Matemática'
AND LastLevel = 0

UPDATE its SET Skill_Id =  @SkillIdLastLevelMT
--SELECT its.Id, its.Item_Id, its.Skill_Id, ski.LastLevel
FROM ItemSkill its WITH(NOLOCK)
INNER JOIN Item ite WITH(NOLOCK) ON its.Item_Id = ite.Id
INNER JOIN BlockItem bli WITH(NOLOCK) ON ite.Id = bli.Item_Id
INNER JOIN Block blo WITH(NOLOCK) ON bli.Block_Id = blo.Id
INNER JOIN Test tes WITH(NOLOCK) ON blo.Test_Id = tes.Id
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
INNER JOIN Skill ski WITH(NOLOCK) ON its.Skill_Id = ski.Id
WHERE tmp.Disciplina = 'Matemática'
AND LastLevel = 1


-- 2º Item
-- EvaluationMatrix_Id
UPDATE ite SET EvaluationMatrix_Id = @EvaluationMatrix_IdLP
--SELECT ite.* 
FROM Item ite WITH(NOLOCK)
INNER JOIN BlockItem bli WITH(NOLOCK) ON ite.Id = bli.Item_Id
INNER JOIN Block blc WITH(NOLOCK) ON bli.Block_Id = blc.Id
INNER JOIN Test tes WITH(NOLOCK) ON blc.Test_Id = tes.Id
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
WHERE tmp.Disciplina = 'Língua portuguesa'

UPDATE ite SET EvaluationMatrix_Id = @EvaluationMatrix_IdMT
--SELECT ite.* 
FROM Item ite WITH(NOLOCK)
INNER JOIN BlockItem bli WITH(NOLOCK) ON ite.Id = bli.Item_Id
INNER JOIN Block blc WITH(NOLOCK) ON bli.Block_Id = blc.Id
INNER JOIN Test tes WITH(NOLOCK) ON blc.Test_Id = tes.Id
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
WHERE tmp.Disciplina = 'Matemática'

-- 3º Test
-- Discipline_Id
UPDATE tes SET Discipline_Id = dis.Id
--SELECT dis.* 
FROM Test tes WITH(NOLOCK)
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
INNER JOIN Discipline dis WITH(NOLOCK) ON tmp.Disciplina COLLATE Latin1_General_CI_AI = dis.[Description] COLLATE Latin1_General_CI_AI
WHERE tmp.Disciplina = 'Língua portuguesa'
AND dis.[State] = 1

UPDATE tes SET Discipline_Id = dis.Id
--SELECT dis.* 
FROM Test tes WITH(NOLOCK)
INNER JOIN #tmpTest tmp ON tes.Id = tmp.testID
INNER JOIN Discipline dis WITH(NOLOCK) ON tmp.Disciplina COLLATE Latin1_General_CI_AI = dis.[Description] COLLATE Latin1_General_CI_AI
WHERE tmp.Disciplina = 'Matemática'
AND dis.[State] = 1

COMMIT