CREATE TABLE [dbo].[ItemAudio](
[Id] [bigint] IDENTITY(1,1) NOT NULL,
[CreateDate] [datetime] NOT NULL,
[UpdateDate] [datetime] NOT NULL,
[State] [tinyint] NOT NULL,
[File_Id] [bigint] NULL,
[Item_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.ItemAudio] PRIMARY KEY CLUSTERED 
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ItemAudio]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemAudio_dbo.File_File_Id] FOREIGN KEY([File_Id])
REFERENCES [dbo].[File] ([Id])
GO

ALTER TABLE [dbo].[ItemAudio] CHECK CONSTRAINT [FK_dbo.ItemAudio_dbo.File_File_Id]
GO

ALTER TABLE [dbo].[ItemAudio]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemAudio_dbo.Item_Item_Id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Item] ([Id])
GO

ALTER TABLE [dbo].[ItemAudio] CHECK CONSTRAINT [FK_dbo.ItemAudio_dbo.Item_Item_Id]
GO