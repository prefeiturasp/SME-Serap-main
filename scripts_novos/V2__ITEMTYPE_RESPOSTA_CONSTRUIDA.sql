IF NOT EXISTS ( SELECT  Id
                FROM    ItemType
                WHERE   [Description] = 'Resposta construída'
                        AND EntityId = '6CF424DC-8EC3-E011-9B36-00155D033206' ) 
    BEGIN
        INSERT  INTO ItemType ( [Description], [UniqueAnswer], [CreateDate],
                  [UpdateDate], [State], [EntityId], [IsVisibleTestType] ,
                  [IsDefault] , [QuantityAlternative]
                  )
        VALUES  ('Resposta construída',0,getdate(),getdate(),1, '6CF424DC-8EC3-E011-9B36-00155D033206',1,0,0)
    END