USE CoreSSO

DECLARE @ent_id AS UNIQUEIDENTIFIER = ( SELECT  ent_id
                                        FROM    SYS_Entidade
                                        WHERE   ent_sigla = 'SMESP'
                                      )
USE [GestaoAvaliacao]

IF NOT EXISTS ( SELECT  Id
                FROM    [dbo].[ItemSituation]
                WHERE   [Description] = 'Aceito'
                        AND EntityId = @ent_id ) 
    BEGIN
        INSERT  INTO [dbo].[ItemSituation]
                ( [Description] ,
                  [CreateDate] ,
                  [UpdateDate] ,
                  [State] ,
                  [EntityId],
                  [AllowVersion]
                )
        VALUES  ( 'Aceito' ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @ent_id ,
                  1
                )
    END


IF NOT EXISTS ( SELECT  Id
                FROM    [dbo].[ItemSituation]
                WHERE   [Description] = 'Pendente'
                        AND EntityId = @ent_id ) 
    BEGIN
        INSERT  INTO [dbo].[ItemSituation]
                ( [Description] ,
                  [CreateDate] ,
                  [UpdateDate] ,
                  [State] ,
                  [EntityId] ,
                  [AllowVersion]
                )
        VALUES  ( 'Pendente' ,
                  GETDATE() ,
                  GETDATE() ,
                  1 ,
                  @ent_id ,
                  0
                )
    END


