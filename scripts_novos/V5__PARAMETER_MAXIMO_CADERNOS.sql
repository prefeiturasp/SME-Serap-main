
IF NOT EXISTS ( SELECT  Id
                FROM    Parameter
                WHERE   [Key] = 'TESTMAXBLOCK'
                        AND EntityId = '6CF424DC-8EC3-E011-9B36-00155D033206' ) 
    BEGIN
INSERT INTO [Parameter]
           ([Key]
           ,[Value]
           ,[Description]
           ,[StartDate]
           ,[CreateDate]
           ,[UpdateDate]
           ,[State]
           ,[EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]
           ,[Versioning]
           ,[Obligatory])
     VALUES
           ('TESTMAXBLOCK'
           ,'26'
           ,'Máximo de cadernos permitidos'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,'6CF424DC-8EC3-E011-9B36-00155D033206'
           ,1
           ,3
           ,1
           ,1
           ,1);
END
