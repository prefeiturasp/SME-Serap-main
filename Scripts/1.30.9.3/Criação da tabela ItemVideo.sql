ALTER TABLE [ItemFile] ADD [ConvertedFile_Id] BIGINT NULL;
GO

ALTER TABLE [dbo].[ItemFile]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemFile_dbo.File_Converted_File_Id] FOREIGN KEY([ConvertedFile_Id])
REFERENCES [dbo].[File] ([Id])
GO

ALTER TABLE [dbo].[ItemFile] CHECK CONSTRAINT [FK_dbo.ItemFile_dbo.File_Converted_File_Id]
GO
