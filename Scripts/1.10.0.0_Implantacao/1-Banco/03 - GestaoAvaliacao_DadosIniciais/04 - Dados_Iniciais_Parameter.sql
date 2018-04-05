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

USE CoreSSO

DECLARE @ent_id AS UNIQUEIDENTIFIER = ( SELECT  ent_id
										FROM    SYS_Entidade
										WHERE   
										ent_sigla = 'SMESP' and ent_situacao = 1
									  )
									  


USE GestaoAvaliacao




IF EXISTS ( SELECT  [Description] 
			FROM	[Parameter] ) 
BEGIN    
    DELETE  FROM [Parameter]
    DBCC CHECKIDENT ('Parameter', RESEED, 0);
END

IF EXISTS ( SELECT  [Description] 
			FROM	[ParameterType] ) 
BEGIN    
    DELETE  FROM [ParameterType]
    DBCC CHECKIDENT ('ParameterType', RESEED, 0);
END

IF EXISTS ( SELECT  [Description] 
			FROM	[ParameterCategory] ) 
BEGIN    
    DELETE  FROM [ParameterCategory]
    DBCC CHECKIDENT ('ParameterCategory', RESEED, 0);
END

IF EXISTS ( SELECT  [Description] 
			FROM	[ParameterPage] ) 
BEGIN    
    DELETE  FROM [ParameterPage]
    DBCC CHECKIDENT ('ParameterPage', RESEED, 0);
END

    
  
INSERT INTO [ParameterPage]
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State]
			,[pageVersioning]
			,[pageObligatory])
		VALUES
			('Geral'
			,GETDATE()
			,GETDATE()
			,1
			,0
			,0)
	

        
INSERT INTO [ParameterPage]
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State]
			,[pageVersioning]
			,[pageObligatory])
		VALUES
			('Item'
			,GETDATE()
			,GETDATE()
			,1
			,1
			,1)
	


INSERT INTO [ParameterPage] 
			([Description], 
			[pageVersioning], 
			[pageObligatory], 
			[CreateDate], 
			[UpdateDate], 
			[State]) 
			VALUES 
			('Prova', 
			0, 
			0, 
			GETDATE(), 
			GETDATE(), 
			1)
	           


INSERT INTO [dbo].[ParameterPage] 
			([Description], 
			[pageVersioning], 
			[pageObligatory], 
			[CreateDate], 
			[UpdateDate], 
			[State]) 
			VALUES 
			('Sistema', 
			0, 
			0, 
			GETDATE(), 
			GETDATE(), 
			1)
	

-- Insere as categorias dos parâmetros  
 
INSERT INTO [ParameterCategory]
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State])
		VALUES
			('Geral'
			,GETDATE()
			,GETDATE()
			,1)
		

          
INSERT INTO [ParameterCategory]
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State])
		VALUES
			('Extensões de imagens permitidas nos uploads'
			,GETDATE()
			,GETDATE()
			,1)
		
           

INSERT [dbo].[ParameterCategory] 
		([Description], 
		CreateDate, 
		UpdateDate, 
		State) 
VALUES ('Narração', 
		GETDATE(), 
		GETDATE(), 
		1)

INSERT INTO [dbo].[ParameterCategory] 
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State]) 
		VALUES 
			(N'Configuração'
			,GETDATE()
			,GETDATE()
			,1)		
           
-- Insere os tipos dos parâmetros 
   
INSERT INTO [ParameterType]
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State])
		VALUES
			('Input'
			,GETDATE()
			,GETDATE()
			,1)
		

INSERT INTO [ParameterType]
			([Description]
			,[CreateDate]
			,[UpdateDate]
			,[State])
		VALUES
			('Checkbox'
			,GETDATE()
			,GETDATE()
			,1)
		
           
-- Insere parâmetros
DECLARE 
	@itempage_id AS INT = (SELECT Id FROM ParameterPage WHERE Description = 'Item'),
	@geralpage_id AS INT = (SELECT Id FROM ParameterPage WHERE Description = 'Geral'),
	@geralcategory_id AS INT = (SELECT Id FROM ParameterCategory WHERE Description = 'Geral'),
	@extensoescategory_id AS INT = (SELECT Id FROM ParameterCategory WHERE Description = 'Extensões de imagens permitidas nos uploads'),
	@inputtype_id AS INT = (SELECT Id FROM ParameterType WHERE Description = 'Input'),
	@ckeckboxtype_id AS INT = (SELECT Id FROM ParameterType WHERE Description = 'Checkbox')

--Insere as páginas dos parâmetros



-- PARAMETROS ITEM
------------------------------------------------------

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
           ('BASETEXT'
           ,'Texto base'
           ,'Texto base item'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)

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
           ,[ParameterType_Id])
     VALUES
           ('ITEMSITUATION'
           ,'Situação do item'
           ,'Situação do item'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id)
                     
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
           ('SOURCE'
           ,'Fonte'
           ,'Fonte texto base item'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
          
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
           ('DESCRIPTORSENTENCE'
           ,'Sentença descritora do item'
           ,'Sentença descritora do item'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,2
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
         
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
           ,[Versioning])
     VALUES
           ('ITEMTYPE'
           ,'Tipo do item'
           ,'Tipo do item'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1)
           
           
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
           ,[Versioning])
     VALUES
           ('ITEMCURRICULUMGRADE'
           ,'Ano'
           ,'Selecione o período'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1)
           
          
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
           ('KEYWORDS'
           ,'Palavras-chave'
           ,'Palavras-chave'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
           
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
           ('PROFICIENCY'
           ,'Proficiência'
           ,'Proficiência'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
           
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
           ('ITEMLEVEL'
           ,'Dificuldade sugerida'
           ,'Dificuldade sugerida'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
          
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
           ('STATEMENT'
           ,'Enunciado'
           ,'Enunciado'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
         
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
           ('TRI'
           ,'TRI'
           ,'TRI'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
                   
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
           ('TCT'
           ,'TCT'
           ,'TCT'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
           
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
           ('TIPS'
           ,'Observação'
           ,'Observação'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
           
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
           ('ALTERNATIVES'
           ,'Alternativas'
           ,'Alternativas'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
           
          
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
           ('JUSTIFICATIVE'
           ,'Justificativa'
           ,'Justificativa'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1
           ,1)
                     
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
           ,[Versioning])
     VALUES
           ('ISRESTRICT'
           ,'Sigiloso'
           ,'Sigiloso'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1)
           
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
           ,[Versioning])
     VALUES
           ('NIVEISMATRIZ'
           ,'Alteração nos níveis da matriz'
           ,'Alteração nos níveis da matriz'
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,1
           ,@ent_id
           ,@geralcategory_id
           ,@itempage_id
           ,@inputtype_id
           ,1)
--------------------------------------------------

-- PARAMETROS GERAL
--------------------------------------------------
    
INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]        
                
        )
VALUES  ( 'JPEG' ,
          'True' ,
          'JPEG' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id 
           ,@extensoescategory_id
           ,@geralpage_id
           ,@ckeckboxtype_id             
        )

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]        
                
        )
VALUES  ( 'GIF' ,
          'True' ,
          'GIF' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id 
           ,@extensoescategory_id
           ,@geralpage_id
           ,@ckeckboxtype_id        
        )
   
INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]        
                
        )
VALUES  ( 'PNG' ,
          'True' ,
          'PNG' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id 
           ,@extensoescategory_id
           ,@geralpage_id
           ,@ckeckboxtype_id        
        )

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]        
                
        )
VALUES  ( 'BMP' ,
          'True' ,
          'BMP' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id 
           ,@extensoescategory_id
           ,@geralpage_id
           ,@ckeckboxtype_id        
        )
         

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]     
        )
VALUES  ( 'IMAGE_GIF_COMPRESSION' ,
          'False' ,
          'Comprimir GIF (sim ou não), após compressão se tornará imagem estática' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id
           ,@geralcategory_id
           ,@geralpage_id
           ,@ckeckboxtype_id      
        )
    

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
            ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]    
        )
VALUES  ( 'IMAGE_MAX_SIZE_FILE' ,
          '51200' ,
          'Tamanho máximo de imagens permitido (kB)' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id
          ,@geralcategory_id
           ,@geralpage_id
           ,@inputtype_id       
        )
    

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]     
        )
VALUES  ( 'IMAGE_QUALITY' ,
          '100' ,
          'Qualidade das imagens em porcentagem' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id
          ,@geralcategory_id
           ,@geralpage_id
           ,@inputtype_id      
        )
    

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]     
        )
VALUES  ( 'IMAGE_MAX_RESOLUTION_HEIGHT' ,
          '500' ,
          'Resolução das imagens: Altura máxima' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id
          ,@geralcategory_id
           ,@geralpage_id
           ,@inputtype_id       
        )

INSERT  INTO [Parameter]
        ( [Key] ,
          [Value] ,
          [Description] ,
          [StartDate] ,
          [CreateDate] ,
          [UpdateDate] ,
          [State] ,
          [EntityId]
           ,[ParameterCategory_Id]
           ,[ParameterPage_Id]
           ,[ParameterType_Id]     
        )
VALUES  ( 'IMAGE_MAX_RESOLUTION_WIDTH' ,
          '500' ,
          'Resolução das imagens: Largura máxima' ,
          GETDATE() ,
          GETDATE() ,
          GETDATE() ,
          1 ,
          @ent_id
          ,@geralcategory_id
           ,@geralpage_id
           ,@inputtype_id      
        )


INSERT INTO [dbo].[Parameter]
			([Key]
			,[Value]
			,[Description]
			,[StartDate]
			,[EndDate]
			,[CreateDate]
			,[UpdateDate]
			,[State]
			,[EntityId]
			,[Obligatory]
			,[Versioning]
			,[ParameterCategory_Id]
			,[ParameterPage_Id]
			,[ParameterType_Id])
		VALUES
			('UTILIZACDNMATHJAX'
			,'True'
			,'Utilizar CDN na biblioteca MathJax'
			,GETDATE()
			,NULL
			,GETDATE()
			,GETDATE()
			,1
			,@ent_id
			,1
			,0
			,@geralcategory_id
			,@geralpage_id
			,@ckeckboxtype_id)


INSERT INTO [dbo].[Parameter] 
			([Key], 
			[Value], 
			[Description], 
			[EntityId], 
			[StartDate], 
			[EndDate], 
			[Obligatory], 
			[Versioning], 
			[ParameterPage_Id], 
			[CreateDate], 
			[UpdateDate], 
			[State], 
			[ParameterCategory_Id], 
			[ParameterType_Id]) 
		VALUES 
			('FILE_MAX_SIZE',
			'51200', 
			'Tamanho máximo de arquivos permitido (kB)', 
			@ent_id, 
			GETDATE(), 
			NULL, 
			NULL, 
			NULL, 
			1, 
			GETDATE(), 
			GETDATE(), 
			@geralcategory_id, 
			@geralpage_id, 
			@inputtype_id)


DECLARE @PAGE INT = (SELECT Id FROM [dbo].[ParameterPage] WHERE [Description] = N'Prova')


INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('BASE_TEXT_DEFAULT',
		'Utilize o texto a seguir para responder as questões', 
		'Texto de orientação para texto base', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		NULL, 
		NULL, 
		@PAGE, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@inputtype_id)




DECLARE @CAT INT = (SELECT Id FROM [dbo].[ParameterCategory] WHERE [Description] = 'Narração')


INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('INITIAL_ORIENTATION', 
		'Orientação inicial para aplicador', 
		'Orientação inicial para aplicador', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		1, 
		1, 
		@itempage_id, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@CAT, 
		@inputtype_id)
		

INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('INITIAL_STATEMENT', 
		'Enunciado de abertura do item', 
		'Enunciado de abertura do item', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		1, 
		1, 
		@itempage_id, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@CAT, 
		@inputtype_id)


INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('BASETEXT_ORIENTATION', 
		'Orientação complementar sobre o texto base', 
		'Orientação complementar sobre o texto base', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		1, 
		1, 
		@itempage_id, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@CAT, 
		@inputtype_id)



SET @PAGE = (SELECT Id FROM [dbo].[ParameterPage] WHERE [Description] = 'Sistema')


INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('SHOW_ITEM_NARRATED', 
		'True', 
		'Habilitar item narrado', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		NULL, 
		NULL, 
		@PAGE, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@ckeckboxtype_id)

INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate],  
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('ANSWERSHEET_USE_COLUMN_TEMPLATE', 
		'false', 
		'Gerar folha de resposta utilizando o template de colunas (20, 40, 60 e 80 questões)', 
		@ent_id, 
		GETDATE(), 
		NULL,  
		@PAGE, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@ckeckboxtype_id)


SET @PAGE = (SELECT Id FROM [dbo].[ParameterPage] WHERE [Description] = 'Prova')


INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('CODE_ALTERNATIVE_DUPLICATE', 
		'R', 
		'Sigla para Rasurado (Mais de uma resposta preenchida na mesma questão)', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		1, 
		1, 
		@PAGE, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@inputtype_id)


INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate], 
		[Obligatory], 
		[Versioning], 
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('CODE_ALTERNATIVE_EMPTY', 
		'N', 
		'Sigla para Nulo (Nenhuma resposta preenchida)', 
		@ent_id, 
		GETDATE(), 
		NULL, 
		1, 
		1, 
		@PAGE, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@inputtype_id)

INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate],  
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('STUDENT_NUMBER_ID', 
		'False', 
		'Habilitar folha de resposta identificada por número de chamada', 
		@ent_id, 
		GETDATE(), 
		NULL,  
		@PAGE, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@ckeckboxtype_id)

INSERT [dbo].[Parameter] 
		([Key], 
		[Value], 
		[Description], 
		[EntityId], 
		[StartDate], 
		[EndDate],  
		[ParameterPage_Id], 
		[CreateDate], 
		[UpdateDate], 
		[State], 
		[ParameterCategory_Id], 
		[ParameterType_Id]) 
VALUES ('CHAR_SEP_CSV', 
		';', 
		'Caractere utilizado para separar valores no arquivo csv', 
		@ent_id, 
		GETDATE(), 
		NULL,  
		@geralpage_id, 
		GETDATE(), 
		GETDATE(), 
		1, 
		@geralcategory_id, 
		@inputtype_id)



DECLARE @storage_path VARCHAR(MAX) = N'\\caminho_da_rede\'
---
DECLARE @virtual_path VARCHAR(MAX) =  N'http://gestaoavaliacao-dev.build.sistemas/Files/'



DECLARE @page_prova BIGINT = (SELECT [Id] FROM [dbo].[ParameterPage] WHERE [Description] = N'Prova')
DECLARE @page_sistema BIGINT = (SELECT [Id] FROM [dbo].[ParameterPage] WHERE [Description] = N'Sistema')


DECLARE @category_config BIGINT = (SELECT [Id] FROM [dbo].[ParameterCategory] WHERE [Description] = N'Configuração')


INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'DOWNLOAD_OMR_FILE'
		, N'False'
		, N'Habilitar o download das imagens das folhas de respostas dos alunos - OMR'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @geralcategory_id
		, @ckeckboxtype_id)

	

INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id])
VALUES (N'SHOW_MANUAL'
		, N'True'
		, N'Exibir link de manual para download'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @geralcategory_id
		, @ckeckboxtype_id)


INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'ZIP_FILES_ALLOWED'
		, N'image/jpeg;image/png;'
		, N'Arquivos permitidos dentro do zip'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @category_config
		, @inputtype_id)



INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'IMAGE_FILES'
		, N'image/jpeg;image/png;'
		, N'Arquivos de imagens permitidos'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @category_config
		, @inputtype_id)



INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'ZIP_FILES'
		, N'application/x-zip-compressed;application/zip;application/x-rar-compressed;application/rar;zip;rar;'
		, N'Arquivos compactados permitidos'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @category_config
		, @inputtype_id)


INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'STORAGE_PATH'
		, @storage_path
		, N'Caminho fsico responsvel por armazenar os arquivos do sistema'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @category_config
		, @inputtype_id)


INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'GLOBAL_TERM'
		, N'Externo'
		, N'Nomenclatura para o termo Global do sistema'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_prova
		, GETDATE()
		, GETDATE()
		, 1
		, @geralcategory_id
		, @inputtype_id)


INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'LOCAL_TERM'
		, N'Interno'
		, N'Nomenclatura para o termo Local do sistema'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_prova
		, GETDATE()
		, GETDATE()
		, 1
		, @geralcategory_id
		, @inputtype_id)

INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'VIRTUAL_PATH'
		, @virtual_path
		, N'Caminho virtual do armazenamento dos arquivos do sistema'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @category_config
		, @inputtype_id)


INSERT [dbo].[Parameter] 
		([Key]
		, [Value]
		, [Description]
		, [EntityId]
		, [StartDate]
		, [EndDate]
		, [Obligatory]
		, [Versioning]
		, [ParameterPage_Id]
		, [CreateDate]
		, [UpdateDate]
		, [State]
		, [ParameterCategory_Id]
		, [ParameterType_Id]) 
VALUES (N'DELETE_BATCH_FILES'
		, N'false'
		, N'Habilitar excluso dos arquivos da correo automtica de provas'
		, @ent_id
		, GETDATE()
		, NULL
		, NULL
		, NULL
		, @page_sistema
		, GETDATE()
		, GETDATE()
		, 1
		, @geralcategory_id
		, @ckeckboxtype_id)

 

INSERT INTO [dbo].[Parameter]
		([Key]
		,[Value]
		,[Description]
		,[EntityId]
		,[StartDate]
		,[EndDate]
		,[Obligatory]
		,[Versioning]
		,[ParameterPage_Id]
		,[CreateDate]
		,[UpdateDate]
		,[State]
		,[ParameterCategory_Id]
		,[ParameterType_Id])
VALUES  ('CODE'
		,'Código'
		,'Código do item'
		,@ent_id
		,CAST(GETDATE() AS DATE)
		,null
		,0
		,null
		,@itempage_id
		,CAST(GETDATE() AS DATE)
		,CAST(GETDATE() AS DATE)
		,1
		,@geralcategory_id
		,@ckeckboxtype_id)


INSERT INTO [dbo].[Parameter]
		([Key]
		,[Value]
		,[Description]
		,[EntityId]
		,[StartDate]
		,[EndDate]
		,[Obligatory]
		,[Versioning]
		,[ParameterPage_Id]
		,[CreateDate]
		,[UpdateDate]
		,[State]
		,[ParameterCategory_Id]
		,[ParameterType_Id])
VALUES  ('WARNING_UPLOAD_BATCH_DETAIL'
		,'false'
		,'Exibir status conferir como sucesso quando realizado correção de provas'
		, @ent_id
		,CAST(GETDATE() AS DATE)
		,null
		,null
		,null
		,@geralpage_id
		,CAST(GETDATE() AS DATE)
		,CAST(GETDATE() AS DATE)
		,1
		,@category_config
		,@ckeckboxtype_id)

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