USE [GestaoAvaliacao]
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

PRINT N'UPDATE COLUMN [ModalityId] on [dbo].[TestTypeCourse]...';
UPDATE [dbo].[TestTypeCourse] SET [TestTypeCourse].[ModalityId] = TT.[ModalityId]
FROM [dbo].[TestTypeCourse]
INNER JOIN [dbo].[TestType] TT ON [TestTypeCourse].TestType_Id = TT.[Id]



PRINT N'DROP CONSTRAINT of [ModalityId] on [dbo].[TestType]...';
ALTER TABLE [dbo].[TestType] DROP CONSTRAINT [DF__TestType__Modali__7A672E12]
PRINT N'DROP COLUMN [ModalityId] on [dbo].[TestType]...';
ALTER TABLE [dbo].[TestType] DROP COLUMN [ModalityId]

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