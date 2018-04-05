USE GestaoAvaliacao
GO


SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping foreign keys from [dbo].[AnswerSheetLot]'
GO
ALTER TABLE [dbo].[AnswerSheetLot] DROP CONSTRAINT [FK_dbo.AnswerSheetLot_dbo.AnswerSheetLot_Parent_Id]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping foreign keys from [dbo].[Parameter]'
GO
ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_dbo.Parameter_dbo.ParameterPage_ParameterPage_Id]
GO
ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_dbo.Parameter_dbo.ParameterCategory_ParameterCategory_Id]
GO
ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_dbo.Parameter_dbo.ParameterType_ParameterType_Id]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping foreign keys from [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [FK_dbo.Test_dbo.Discipline_Discipline_Id]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[AbsenceReason]'
GO
ALTER TABLE [dbo].[AbsenceReason] DROP CONSTRAINT [DF__AbsenceRe__IsDef__6FE99F9F]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[AnswerSheetBatchFiles]'
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] DROP CONSTRAINT [DF__AnswerShe__Situa__72C60C4A]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[AnswerSheetBatch]'
GO
ALTER TABLE [dbo].[AnswerSheetBatch] DROP CONSTRAINT [DF__AnswerShe__Batch__70DDC3D8]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[AnswerSheetBatch]'
GO
ALTER TABLE [dbo].[AnswerSheetBatch] DROP CONSTRAINT [DF__AnswerShe__Owner__71D1E811]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[AnswerSheetLot]'
GO
ALTER TABLE [dbo].[AnswerSheetLot] DROP CONSTRAINT [DF__AnswerShee__Type__73BA3083]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[AnswerSheetLot]'
GO
ALTER TABLE [dbo].[AnswerSheetLot] DROP CONSTRAINT [DF__AnswerShe__Paren__74AE54BC]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Item]'
GO
ALTER TABLE [dbo].[Item] DROP CONSTRAINT [DF__Item__IsRestrict__76969D2E]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Item]'
GO
ALTER TABLE [dbo].[Item] DROP CONSTRAINT [DF__Item__descriptor__75A278F5]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Item]'
GO
ALTER TABLE [dbo].[Item] DROP CONSTRAINT [DF__Item__ItemCodeVe__778AC167]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[ModelTest]'
GO
ALTER TABLE [dbo].[ModelTest] DROP CONSTRAINT [DF__ModelTest__ShowI__787EE5A0]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[ParameterPage]'
GO
ALTER TABLE [dbo].[ParameterPage] DROP CONSTRAINT [DF__Parameter__pageV__7C4F7684]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[ParameterPage]'
GO
ALTER TABLE [dbo].[ParameterPage] DROP CONSTRAINT [DF__Parameter__pageO__7D439ABD]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Parameter]'
GO
ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [DF__Parameter__Param__7A672E12]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Parameter]'
GO
ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [DF__Parameter__Param__797309D9]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Parameter]'
GO
ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [DF__Parameter__Param__7B5B524B]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[TestTypeCourse]'
GO
ALTER TABLE [dbo].[TestTypeCourse] DROP CONSTRAINT [DF__TestTypeC__Modal__06CD04F7]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[TestType]'
GO
ALTER TABLE [dbo].[TestType] DROP CONSTRAINT [DF__TestType__Bib__02FC7413]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[TestType]'
GO
ALTER TABLE [dbo].[TestType] DROP CONSTRAINT [DF__TestType__Global__03F0984C]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[TestType]'
GO
ALTER TABLE [dbo].[TestType] DROP CONSTRAINT [DF__TestType__TypeLe__04E4BC85]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[TestType]'
GO
ALTER TABLE [dbo].[TestType] DROP CONSTRAINT [DF__TestType__Freque__05D8E0BE]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF__Test__PublicFeed__7E37BEF6]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF__Test__ProcessedC__7F2BE32F]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF__Test__Visible__00200768]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF__Test__FrequencyA__01142BA1]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF__Test__Multidisci__02084FDA]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping index [IX_AnswerSheetBatchLog_State] from [dbo].[AnswerSheetBatchLog]'
GO
DROP INDEX [IX_AnswerSheetBatchLog_State] ON [dbo].[AnswerSheetBatchLog]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping index [IX_Parent_Id] from [dbo].[AnswerSheetLot]'
GO
DROP INDEX [IX_Parent_Id] ON [dbo].[AnswerSheetLot]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping index [UN_Item_ItemCodeVersion_ItemVersion] from [dbo].[Item]'
GO
DROP INDEX [UN_Item_ItemCodeVersion_ItemVersion] ON [dbo].[Item]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping index [IX_ParameterPage_Id] from [dbo].[Parameter]'
GO
DROP INDEX [IX_ParameterPage_Id] ON [dbo].[Parameter]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping index [IX_ParameterCategory_Id] from [dbo].[Parameter]'
GO
DROP INDEX [IX_ParameterCategory_Id] ON [dbo].[Parameter]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping index [IX_ParameterType_Id] from [dbo].[Parameter]'
GO
DROP INDEX [IX_ParameterType_Id] ON [dbo].[Parameter]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[Test]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Test] ADD
[TestSubGroup_Id] [bigint] NULL
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_TestSubGroup_Id] on [dbo].[Test]'
GO
CREATE NONCLUSTERED INDEX [IX_TestSubGroup_Id] ON [dbo].[Test] ([TestSubGroup_Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[Parameter]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Parameter] ALTER COLUMN [Value] [varchar] (max) COLLATE Latin1_General_CI_AS NOT NULL
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_ParameterPage_Id] on [dbo].[Parameter]'
GO
CREATE NONCLUSTERED INDEX [IX_ParameterPage_Id] ON [dbo].[Parameter] ([ParameterPage_Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_ParameterCategory_Id] on [dbo].[Parameter]'
GO
CREATE NONCLUSTERED INDEX [IX_ParameterCategory_Id] ON [dbo].[Parameter] ([ParameterCategory_Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_ParameterType_Id] on [dbo].[Parameter]'
GO
CREATE NONCLUSTERED INDEX [IX_ParameterType_Id] ON [dbo].[Parameter] ([ParameterType_Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[TestSubGroup]'
GO
CREATE TABLE [dbo].[TestSubGroup]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[Description] [varchar] (500) COLLATE Latin1_General_CI_AS NOT NULL,
[CreateDate] [datetime] NOT NULL,
[UpdateDate] [datetime] NOT NULL,
[State] [tinyint] NOT NULL,
[TestGroup_Id] [bigint] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_dbo.TestSubGroup] on [dbo].[TestSubGroup]'
GO
ALTER TABLE [dbo].[TestSubGroup] ADD CONSTRAINT [PK_dbo.TestSubGroup] PRIMARY KEY CLUSTERED  ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_TestGroup_Id] on [dbo].[TestSubGroup]'
GO
CREATE NONCLUSTERED INDEX [IX_TestGroup_Id] ON [dbo].[TestSubGroup] ([TestGroup_Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[TestGroup]'
GO
CREATE TABLE [dbo].[TestGroup]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[Description] [varchar] (500) COLLATE Latin1_General_CI_AS NOT NULL,
[EntityId] [uniqueidentifier] NOT NULL,
[CreateDate] [datetime] NOT NULL,
[UpdateDate] [datetime] NOT NULL,
[State] [tinyint] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_dbo.TestGroup] on [dbo].[TestGroup]'
GO
ALTER TABLE [dbo].[TestGroup] ADD CONSTRAINT [PK_dbo.TestGroup] PRIMARY KEY CLUSTERED  ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_Parent_Id] on [dbo].[AnswerSheetLot]'
GO
CREATE NONCLUSTERED INDEX [IX_Parent_Id] ON [dbo].[AnswerSheetLot] ([Parent_Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [UN_Item_ItemCodeVersion_ItemVersion] on [dbo].[Item]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [UN_Item_ItemCodeVersion_ItemVersion] ON [dbo].[Item] ([ItemCodeVersion], [ItemVersion])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[AbsenceReason]'
GO
ALTER TABLE [dbo].[AbsenceReason] ADD CONSTRAINT [DF__AbsenceRe__IsDef__50F0E28A] DEFAULT ((0)) FOR [IsDefault]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[AnswerSheetBatchFiles]'
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] ADD CONSTRAINT [DF__AnswerShe__Situa__5AAF56EE] DEFAULT ((0)) FOR [Situation]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[AnswerSheetBatch]'
GO
ALTER TABLE [dbo].[AnswerSheetBatch] ADD CONSTRAINT [DF__AnswerShe__Batch__58C70E7C] DEFAULT ((0)) FOR [BatchType]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[AnswerSheetBatch] ADD CONSTRAINT [DF__AnswerShe__Owner__59BB32B5] DEFAULT ((0)) FOR [OwnerEntity]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[AnswerSheetLot]'
GO
ALTER TABLE [dbo].[AnswerSheetLot] ADD CONSTRAINT [DF__AnswerShee__Type__3B01A16B] DEFAULT ((0)) FOR [Type]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[Item]'
GO
ALTER TABLE [dbo].[Item] ADD CONSTRAINT [DF__Item__ItemCodeVe__58920452] DEFAULT ((0)) FOR [ItemCodeVersion]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[TestTypeCourse]'
GO
ALTER TABLE [dbo].[TestTypeCourse] ADD CONSTRAINT [DF__TestTypeC__Modal__27EECCF7] DEFAULT ((0)) FOR [ModalityId]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[TestType]'
GO
ALTER TABLE [dbo].[TestType] ADD CONSTRAINT [DF__TestType__Freque__61274A53] DEFAULT ((1)) FOR [FrequencyApplication]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [DF__Test__PublicFeed__4F089A18] DEFAULT ((0)) FOR [PublicFeedback]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [DF__Test__ProcessedC__56A9BBE0] DEFAULT ((0)) FOR [ProcessedCorrection]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [DF__Test__Visible__579DE019] DEFAULT ((0)) FOR [Visible]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [DF__Test__FrequencyA__621B6E8C] DEFAULT ((1)) FOR [FrequencyApplication]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [DF__Test__Multidisci__743A1EC7] DEFAULT ((0)) FOR [Multidiscipline]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[AnswerSheetLot]'
GO
ALTER TABLE [dbo].[AnswerSheetLot] ADD CONSTRAINT [FK_dbo.AnswerSheetLot_dbo.AnswerSheetLot_Parent_Id] FOREIGN KEY ([Parent_Id]) REFERENCES [dbo].[AnswerSheetLot] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Parameter]'
GO
ALTER TABLE [dbo].[Parameter] ADD CONSTRAINT [FK_dbo.Parameter_dbo.ParameterPage_ParameterPage_Id] FOREIGN KEY ([ParameterPage_Id]) REFERENCES [dbo].[ParameterPage] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Parameter] ADD CONSTRAINT [FK_dbo.Parameter_dbo.ParameterCategory_ParameterCategory_Id] FOREIGN KEY ([ParameterCategory_Id]) REFERENCES [dbo].[ParameterCategory] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Parameter] ADD CONSTRAINT [FK_dbo.Parameter_dbo.ParameterType_ParameterType_Id] FOREIGN KEY ([ParameterType_Id]) REFERENCES [dbo].[ParameterType] ([Id]) ON DELETE CASCADE
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[TestSubGroup]'
GO
ALTER TABLE [dbo].[TestSubGroup] ADD CONSTRAINT [FK_dbo.TestSubGroup_dbo.TestGroup_TestGroup_Id] FOREIGN KEY ([TestGroup_Id]) REFERENCES [dbo].[TestGroup] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Test]'
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [FK_dbo.Test_dbo.TestSubGroup_TestSubGroup_Id1] FOREIGN KEY ([TestSubGroup_Id]) REFERENCES [dbo].[TestSubGroup] ([Id])
GO
ALTER TABLE [dbo].[Test] ADD CONSTRAINT [FK_dbo.Test_dbo.Discipline_Discipline_Id] FOREIGN KEY ([Discipline_Id]) REFERENCES [dbo].[Discipline] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO