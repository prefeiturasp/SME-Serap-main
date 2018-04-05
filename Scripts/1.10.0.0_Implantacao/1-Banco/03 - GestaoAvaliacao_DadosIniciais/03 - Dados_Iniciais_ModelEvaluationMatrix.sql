USE CoreSSO

GO
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

DECLARE @ent_id AS UNIQUEIDENTIFIER = ( SELECT  ent_id
                                        FROM    SYS_Entidade
                                        WHERE   ent_sigla = 'SMESP'
                                      )

USE GestaoAvaliacao

IF NOT EXISTS ( SELECT  Id
                FROM    ModelEvaluationMatrix
                WHERE   [Description] = 'Matriz SMESP' AND EntityId = @ent_id ) 
    BEGIN
        INSERT  INTO ModelEvaluationMatrix
                ( [Description] ,
                  CreateDate ,
                  UpdateDate ,
                  State ,
                  EntityId
                    
                )
        VALUES  ( 'Matriz SMESP' ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @ent_id  
                    
                )
    END
        
DECLARE @modelId AS BIGINT = ( SELECT   Id
                               FROM     ModelEvaluationMatrix
                               WHERE    Description = 'Matriz SMESP' AND EntityId = @ent_id
                             )


IF NOT EXISTS ( SELECT  Id
                FROM    ModelSkillLevel
                WHERE   [Description] = 'Eixo'
                        AND ModelEvaluationMatrix_Id = @modelId ) 
    BEGIN
        INSERT  INTO dbo.ModelSkillLevel
                ( [Description] ,
                  Level ,
                  LastLevel ,
                  CreateDate ,
                  UpdateDate ,
                  State ,
                  ModelEvaluationMatrix_Id
                    
                )
        VALUES  ( 'Eixo' ,
                  1 ,
                  0 ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @modelId  
                    
                )
    END 

IF NOT EXISTS ( SELECT  Id
                FROM    ModelSkillLevel
                WHERE   [Description] = 'Habilidade'
                        AND ModelEvaluationMatrix_Id = @modelId ) 
    BEGIN        
        INSERT  INTO ModelSkillLevel
                ( Description ,
                  Level ,
                  LastLevel ,
                  CreateDate ,
                  UpdateDate ,
                  State ,
                  ModelEvaluationMatrix_Id
                )
        VALUES  ( 'Habilidade' ,
                  2 ,
                  1 ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @modelId  
                )
    END

 GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) 
  ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 
  BEGIN
    PRINT 'The database update succeeded'
    COMMIT TRANSACTION
  END
ELSE 
  PRINT 'The database update failed - ROLLBACK aplied'
GO
DROP TABLE #tmpErrors 
GO 