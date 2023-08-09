use CoreSSO

declare @modId int = (select MAX(mod_id) + 1 from SYS_Modulo where sis_id = 204);
declare @modIdPai int = 24, @sisId int = 204,
@nome varchar(100) = 'Relatório e Estudos';
insert into SYS_Modulo
(sis_id,mod_id,mod_nome,mod_descricao,mod_idPai,mod_auditoria,mod_situacao,mod_dataCriacao,mod_dataAlteracao)
values
(@sisId,@modId,@nome,null,@modIdPai,0,1,GETDATE(),GETDATE())

insert into SYS_ModuloSiteMap
(sis_id,mod_id,msm_id,msm_nome,msm_descricao,msm_url,msm_informacoes,msm_urlHelp)
values 
(@sisId,@modId,1,@nome,null,'/RelatorioEstudos',null,null)

insert into SYS_VisaoModulo (vis_id, sis_id, mod_id)
values (1,	@sisId,	@modId) -- Administração

insert into SYS_VisaoModuloMenu 
(vis_id,sis_id,mod_id,msm_id,vmm_ordem)
values
(1,@sisId,@modId,1,8) -- Administração

insert into SYS_GrupoPermissao 
(gru_id,sis_id,mod_id,grp_consultar,grp_inserir,grp_alterar,grp_excluir)
values 
('AAD9D772-41A3-E411-922D-782BCB3D218E',@sisId,@modId,1,1,1,1), -- Administrador
('22366A3E-9E4C-E711-9541-782BCB3D218E',@sisId,@modId,1,1,1,1); -- Administrador NTA
