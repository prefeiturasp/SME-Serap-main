begin tran
insert into ACA_TipoNivelEnsino
select 4,	'EJA - Ensino Fundamental',	1, GETDATE(), GETDATE(), 7 UNION
SELECT 5,	'EJA - CIEJA',	1,GETDATE(), GETDATE(),	8 UNION
SELECT 6,	'EJA ESCOLAS EDUCACAO ESPECIAL', 1,GETDATE(), GETDATE(),	4

-- ROLLBACK
-- COMMIT