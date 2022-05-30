USE [GestaoAvaliacao]
GO

/****** Object:  Table [dbo].[TestTaiCurriculumGrade]    Script Date: 27/05/2022 12:43:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TestTaiCurriculumGrade](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Discipline_Id] [bigint] NOT NULL,
	[EvaluationMatrix_Id] [bigint] NOT NULL,
	[TypeCurriculumGradeId] [bigint] NOT NULL,
	[Percentage] [int] NOT NULL,
	[Test_Id] [bigint] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TestTaiCurriculumGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TestTaiCurriculumGrade]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestTaiCurriculumGrade_dbo.EvaluationMatrix_EvaluationMatrix_Id] FOREIGN KEY([EvaluationMatrix_Id])
REFERENCES [dbo].[EvaluationMatrix] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TestTaiCurriculumGrade] CHECK CONSTRAINT [FK_dbo.TestTaiCurriculumGrade_dbo.EvaluationMatrix_EvaluationMatrix_Id]
GO

ALTER TABLE [dbo].[TestTaiCurriculumGrade]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestTaiCurriculumGrade_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TestTaiCurriculumGrade] CHECK CONSTRAINT [FK_dbo.TestTaiCurriculumGrade_dbo.Test_Test_Id]
GO


