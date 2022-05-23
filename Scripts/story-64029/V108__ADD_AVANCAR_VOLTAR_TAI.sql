  
ALTER TABLE [dbo].[NumberItemTestTai] ADD  [AdvanceWithoutAnswering] [bit];


ALTER TABLE [dbo].[NumberItemTestTai] ADD  [BackToPreviousItem] [bit];

 UPDATE [dbo].[NumberItemTestTai] SET AdvanceWithoutAnswering = 0 , BackToPreviousItem = 0; 
