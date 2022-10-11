

select * from [dbo].[IDS_ClientRedirectUris]

update IDS_ClientRedirectUris set RedirectUri = 'https://hom-coresso.sme.prefeitura.sp.gov.br/Login.ashx' where id = 1
update IDS_ClientRedirectUris set RedirectUri = 'https://hom-serap.sme.prefeitura.sp.gov.br/Account/LoginSSO' where id = 1026

select * from [dbo].[IDS_ClientCorsOrigins]

update IDS_ClientCorsOrigins set Origin = 'https://hom-serap.sme.prefeitura.sp.gov.br' where id = 1021
update IDS_ClientCorsOrigins set Origin = 'https://hom-coresso.sme.prefeitura.sp.gov.br' where id = 3



