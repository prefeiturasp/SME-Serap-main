------------------------------------------[SYS_ModuloSiteMap]---------------------------------------------

INSERT INTO [dbo].[SYS_Modulo]
           ([sis_id]
           ,[mod_id]
           ,[mod_nome]
           ,[mod_descricao]
           ,[mod_idPai]
           ,[mod_auditoria]
           ,[mod_situacao]
           ,[mod_dataCriacao]
           ,[mod_dataAlteracao])
     VALUES
           (204,
            48,
            'Acompanhamento de provas',
		    '',		
          17
           ,0
           ,1
           ,GETDATE()
           ,GETDATE())
GO

-------------------------------------MODULOSITEMAP------------------------------------------------

INSERT INTO [dbo].[SYS_ModuloSiteMap]
           ([sis_id]
           ,[mod_id]
           ,[msm_id]
           ,[msm_nome]
           ,[msm_descricao]
           ,[msm_url]
           ,[msm_informacoes]
           ,[msm_urlHelp])
     VALUES
           (204
           ,48
           ,1
           ,'Acompanhamento de provas'
           ,null
           ,'/AdminAcompanhamentoProvas/Index'
           ,null
           ,null)
GO
------------------------------------------VisaoModulo---------------------------------------------I

------------Administração------------------		  
INSERT INTO [dbo].[SYS_VisaoModulo]
           ([vis_id]
           ,[sis_id]
           ,[mod_id])
     VALUES
           ( 1
           ,204
           ,48);

--------------Gestão------------------		   
INSERT INTO [dbo].[SYS_VisaoModulo]
           ([vis_id]
           ,[sis_id]
           ,[mod_id])
     VALUES
           (2
           ,204
           ,48);

-----------Unidade-Administrativa-----------			   
INSERT INTO [dbo].[SYS_VisaoModulo]
           ([vis_id]
           ,[sis_id]
           ,[mod_id])
     VALUES
           (3
           ,204
           ,48);

-------------Individual------------------	
INSERT INTO [dbo].[SYS_VisaoModulo]
           ([vis_id]
           ,[sis_id]
           ,[mod_id])
     VALUES
           ( 4
           ,204
           ,48);
--------------[SYS_VisaoModuloMenu]---------------------------------------------------------------------------
------------Administração------------------		
INSERT INTO [dbo].[SYS_VisaoModuloMenu]
           ([vis_id]
           ,[sis_id]
           ,[mod_id]
           ,[msm_id]
           ,[vmm_ordem])
     VALUES
           (   1 
            ,204
            , 48
            ,  1
            ,  3);

--------------Gestão------------------	
INSERT INTO [dbo].[SYS_VisaoModuloMenu]
           ([vis_id]
           ,[sis_id]
           ,[mod_id]
           ,[msm_id]
           ,[vmm_ordem])
     VALUES
           (   2 
            ,204
            , 48
            ,  1
            ,  3);

-----------Unidade-Administrativa-----------			   
INSERT INTO [dbo].[SYS_VisaoModuloMenu]
           ([vis_id]
           ,[sis_id]
           ,[mod_id]
           ,[msm_id]
           ,[vmm_ordem])
     VALUES
           (   3 
            ,204
            , 48
            ,  1
            ,  3);

-------------Individual------------------	
INSERT INTO [dbo].[SYS_VisaoModuloMenu]
           ([vis_id]
           ,[sis_id]
           ,[mod_id]
           ,[msm_id]
           ,[vmm_ordem])
     VALUES
           (   4 
            ,204
            , 48
            ,  1
            ,  3);
------------------------------------------------------------------------------------------------------




-- Diretor, AD, CP, ADM de DRE, Administrador do NTA e Administrador 

--Administrador             AAD9D772-41A3-E411-922D-782BCB3D218E
--Administrador - NTA       22366A3E-9E4C-E711-9541-782BCB3D218E
--Administrador Serap DRE   104F0759-87E8-E611-9541-782BCB3D218E
--Administrador Serap na UE 4318D329-17DC-4C48-8E59-7D80557F7E77
--Diretor Escolar           75DCAB30-2C1E-E811-B259-782BCB3D2D76
--Coordenador Pedagógico    D4026F2C-1A1E-E811-B259-782BCB3D2D76
-- Assistente de Diretor na UE -   ECF7A20D-1A1E-E811-B259-782BCB3D2D76 


-------------------------Administrador------------------AAD9D772-41A3-E411-922D-782BCB3D218E
INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('AAD9D772-41A3-E411-922D-782BCB3D218E'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO

----------------------------------Administrador - NTA   22366A3E-9E4C-E711-9541-782BCB3D218E    

INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('22366A3E-9E4C-E711-9541-782BCB3D218E'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO

------------------------------Administrador Serap DRE-----------------------------------   

INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('104F0759-87E8-E611-9541-782BCB3D218E'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO


------------------------------Administrador Serap na UE   4318D329-17DC-4C48-8E59-7D80557F7E77--   

INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('4318D329-17DC-4C48-8E59-7D80557F7E77'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO

------------------------------Diretor Escolar           75DCAB30-2C1E-E811-B259-782BCB3D2D76
INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('75DCAB30-2C1E-E811-B259-782BCB3D2D76'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO
------------------------Coordenador Pedagógico    D4026F2C-1A1E-E811-B259-782BCB3D2D76

INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('D4026F2C-1A1E-E811-B259-782BCB3D2D76'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO
-------------------------- Assistente de Diretor na UE
INSERT INTO [dbo].[SYS_GrupoPermissao]
           ([gru_id]
           ,[sis_id]
           ,[mod_id]
           ,[grp_consultar]
           ,[grp_inserir]
           ,[grp_alterar]
           ,[grp_excluir])
     VALUES
           ('ECF7A20D-1A1E-E811-B259-782BCB3D2D76'
           , 204
           ,48
           ,1
           ,1
           ,1
           ,1);
GO
