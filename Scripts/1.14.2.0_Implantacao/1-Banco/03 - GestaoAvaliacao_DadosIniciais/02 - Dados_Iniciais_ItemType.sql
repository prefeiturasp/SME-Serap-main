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
                                        WHERE   
										ent_sigla = 'SMESP' and ent_situacao = 1
                                      )
                                      
USE [GestaoAvaliacao]


UPDATE ItemType SET [Description] = 'Múltipla escolha 4 alternativas', [QuantityAlternative] = 4, [IsVisibleTestType] = 1, [IsDefault] = 1
WHERE [Description] = 'Múltipla escolha';


IF NOT EXISTS ( SELECT  Id
                FROM    ItemType
                WHERE   [Description] = 'Múltipla escolha 4 alternativas'
                        AND EntityId = @ent_id ) 
    BEGIN
        INSERT  INTO ItemType
                ( [Description] ,
                  [UniqueAnswer] ,
                  [CreateDate] ,
                  [UpdateDate] ,
                  [State] ,
                  [EntityId] ,
				  [IsVisibleTestType] ,
				  [IsDefault] ,
				  [QuantityAlternative]
                    
                )
        VALUES  ( 'Múltipla escolha 4 alternativas' ,
                  0 ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @ent_id ,
				  1,
				  1,
				  4    
                )
    END

 
IF NOT EXISTS ( SELECT  Id
                FROM    ItemType
                WHERE   [Description] = 'Múltipla escolha 5 alternativas'
                        AND EntityId = @ent_id ) 
    BEGIN
        INSERT  INTO ItemType
                ( [Description] ,
                  [UniqueAnswer] ,
                  [CreateDate] ,
                  [UpdateDate] ,
                  [State] ,
                  [EntityId] ,
				  [IsVisibleTestType] ,
				  [IsDefault] ,
				  [QuantityAlternative]
                    
                )
        VALUES  ( 'Múltipla escolha 5 alternativas' ,
                  0 ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @ent_id ,
				  1,
				  0,
				  5    
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

