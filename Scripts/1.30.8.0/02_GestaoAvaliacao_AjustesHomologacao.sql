
select * from IDS_ClientCorsOrigins
update IDS_ClientCorsOrigins SET Origin = 'https://hom-coresso.sme.prefeitura.sp.gov.br' where id = 3
update IDS_ClientCorsOrigins SET Origin = 'https://hom-serap.sme.prefeitura.sp.gov.br' where id = 4
update IDS_ClientCorsOrigins SET Origin = 'https://itens-serap.sme.prefeitura.sp.gov.br' where id = 1018
--INSERT INTO IDS_ClientCorsOrigins VALUES (1, 'https://itens-serap.sme.prefeitura.sp.gov.br');


select * from IDS_ClientPostLogoutRedirectUris
update IDS_ClientPostLogoutRedirectUris SET PostLogoutRedirectUri = 'https://hom-coresso.sme.prefeitura.sp.gov.br/Login.ashx' where id = 2
update IDS_ClientPostLogoutRedirectUris SET PostLogoutRedirectUri = 'https://hom-coresso.sme.prefeitura.sp.gov.br/SAML/Logout.ashx' where id = 4


select * from IDS_ClientRedirectUris
update IDS_ClientRedirectUris SET RedirectUri = 'https://hom-coresso.sme.prefeitura.sp.gov.br/Login.ashx' where id = 1
update IDS_ClientRedirectUris SET RedirectUri = 'https://hom-serap.sme.prefeitura.sp.gov.br/Account/LoginSSO' where id = 5
update IDS_ClientRedirectUris SET RedirectUri = 'https://itens-serap.sme.prefeitura.sp.gov.br/Account/LoginSSO' where id = 1026
--INSERT INTO IDS_ClientRedirectUris VALUES (1, 'https://itens-serap.sme.prefeitura.sp.gov.br/Account/LoginSSO');


update CoreSSO.dbo.SYS_Sistema set sis_caminho = 'https://hom-serap.sme.prefeitura.sp.gov.br/Account/LoginSSO'  where sis_id = 204

UPDATE GestaoAvaliacao.dbo.PageConfiguration 
	SET link = REPLACE(link, 'http://10.50.1.54:54127/', 'https://hom-serap.sme.prefeitura.sp.gov.br/')
	WHERE link LIKE 'http://10.50.1.54:54127/%'


UPDATE GestaoAvaliacao_Itens.dbo.PageConfiguration 
	SET link = REPLACE(link, 'http://10.50.1.54:54128/', 'https://itens-serap.sme.prefeitura.sp.gov.br/')
	WHERE link LIKE 'http://10.50.1.54:54128/%'

UPDATE GestaoAvaliacao.dbo.Parameter SET [Value] = 'D:\Sistemas\Files' WHERE [Key] = 'STORAGE_PATH'
UPDATE GestaoAvaliacao.dbo.Parameter SET [Value] = 'https://hom-serap.sme.prefeitura.sp.gov.br/Files' WHERE [Key] = 'VIRTUAL_PATH'

UPDATE GestaoAvaliacao_Itens.dbo.Parameter SET [Value] = 'D:\Sistemas\Files' WHERE [Key] = 'STORAGE_PATH'
UPDATE GestaoAvaliacao_Itens.dbo.Parameter SET [Value] = 'https://itens-serap.sme.prefeitura.sp.gov.br/Files' WHERE [Key] = 'VIRTUAL_PATH'

BEGIN TRAN
UPDATE GestaoAvaliacao.dbo.[File]
	SET [Path] = REPLACE([Path], 'http://serap.sme.prefeitura.sp.gov.br/Files/', 'https://hom-serap.sme.prefeitura.sp.gov.br/Files/')
	WHERE [Path] LIKE 'http://serap.sme.prefeitura.sp.gov.br/Files/Thumbnail_Video%'
	
UPDATE GestaoAvaliacao.dbo.[File]
	SET [Path] = REPLACE([Path], 'http://serap.sme.prefeitura.sp.gov.br/Files/', 'https://hom-serap.sme.prefeitura.sp.gov.br/Files/')
	WHERE [Path] LIKE 'http://serap.sme.prefeitura.sp.gov.br/Files/Icone_Ferramenta_Destaque%'
	
UPDATE GestaoAvaliacao.dbo.[File]
	SET [Path] = REPLACE([Path], 'http://serap.sme.prefeitura.sp.gov.br/Files/', 'https://hom-serap.sme.prefeitura.sp.gov.br/Files/')
	WHERE [Path] LIKE 'http://serap.sme.prefeitura.sp.gov.br/Files/Icone_Ferramentas%'
COMMIT

BEGIN TRAN
UPDATE GestaoAvaliacao_Itens.dbo.[File]
	SET [Path] = REPLACE([Path], 'http://serap.sme.prefeitura.sp.gov.br/Files/', 'https://hom-serap.sme.prefeitura.sp.gov.br/Files/')
	WHERE [Path] LIKE 'http://serap.sme.prefeitura.sp.gov.br/Files/Thumbnail_Video%'
	
UPDATE GestaoAvaliacao_Itens.dbo.[File]
	SET [Path] = REPLACE([Path], 'http://serap.sme.prefeitura.sp.gov.br/Files/', 'https://hom-serap.sme.prefeitura.sp.gov.br/Files/')
	WHERE [Path] LIKE 'http://serap.sme.prefeitura.sp.gov.br/Files/Icone_Ferramenta_Destaque%'
	
UPDATE GestaoAvaliacao_Itens.dbo.[File]
	SET [Path] = REPLACE([Path], 'http://serap.sme.prefeitura.sp.gov.br/Files/', 'https://hom-serap.sme.prefeitura.sp.gov.br/Files/')
	WHERE [Path] LIKE 'http://serap.sme.prefeitura.sp.gov.br/Files/Icone_Ferramentas%'
COMMIT


insert into CoreSSO.dbo.SYS_UsuarioGrupo values ( '185E3380-61CA-E911-87E1-782BCB3D2D76', '5A98961B-92F3-E611-9541-782BCB3D218E', 1)
insert into CoreSSO.dbo.SYS_UsuarioGrupo values ( '185E3380-61CA-E911-87E1-782BCB3D2D76', 'ECF7A20D-1A1E-E811-B259-782BCB3D2D76', 1)
insert into CoreSSO.dbo.SYS_UsuarioGrupo values ( '185E3380-61CA-E911-87E1-782BCB3D2D76', '66C70452-1A1E-E811-B259-782BCB3D2D76', 1)
insert into CoreSSO.dbo.SYS_UsuarioGrupo values ( '185E3380-61CA-E911-87E1-782BCB3D2D76', '75DCAB30-2C1E-E811-B259-782BCB3D2D76', 1)

insert into CoreSSO.dbo.SYS_UsuarioGrupoUA values ('185E3380-61CA-E911-87E1-782BCB3D2D76', '66C70452-1A1E-E811-B259-782BCB3D2D76',	'B5639424-EA89-E311-B1FE-782BCB3D2D76',	'6CF424DC-8EC3-E011-9B36-00155D033206',	'8CBDB758-2FA1-E111-BE16-00155D02E702')
insert into CoreSSO.dbo.SYS_UsuarioGrupoUA values ('185E3380-61CA-E911-87E1-782BCB3D2D76', '75DCAB30-2C1E-E811-B259-782BCB3D2D76',	'52906CBE-C819-E911-87E1-782BCB3D2D76',	'6CF424DC-8EC3-E011-9B36-00155D033206',	'DE5F6DCF-A26D-4830-AC28-CAB36C85D5B5')
insert into CoreSSO.dbo.SYS_UsuarioGrupoUA values ( '185E3380-61CA-E911-87E1-782BCB3D2D76', '75DCAB30-2C1E-E811-B259-782BCB3D2D76',	'7A4F9424-EA89-E311-B1FE-782BCB3D2D76',	'6CF424DC-8EC3-E011-9B36-00155D033206',	'A358F015-3F1A-4252-BD24-CB5BAB297F84')
insert into CoreSSO.dbo.SYS_UsuarioGrupoUA values ( '185E3380-61CA-E911-87E1-782BCB3D2D76', '75DCAB30-2C1E-E811-B259-782BCB3D2D76',	'B5639424-EA89-E311-B1FE-782BCB3D2D76',	'6CF424DC-8EC3-E011-9B36-00155D033206',	'19C59CCA-168F-4DCD-A641-B93D115792F0')
