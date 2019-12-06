
BEGIN TRAN

SELECT distinct qri.*, e.esc_nome
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.QuestionarioUsuario qu ON qu.Edicao = '2019' AND qu.esc_codigo = e.esc_codigo
	INNER JOIN ProvaSP.dbo.QuestionarioRespostaItem qri ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID
	LEFT JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	LEFT JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	LEFT JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

SELECT distinct qu.*, e.esc_nome
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.QuestionarioUsuario qu ON qu.Edicao = '2019' AND qu.esc_codigo = e.esc_codigo
	LEFT JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	LEFT JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	LEFT JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
	--INNER JOIN ProvaSP.dbo.QuestionarioRespostaItem qri ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')


SELECT distinct pf.*, e.esc_nome
--DELETE pf
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.PessoaPerfil pf				ON pf.Edicao = '2019' AND pf.esc_codigo = e.esc_codigo
	LEFT JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	LEFT JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	LEFT JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')


SELECT distinct aap.*, e.esc_nome
--delete aap
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.AcompanhamentoAplicacaoPessoa aap ON aap.Edicao = '2019' AND aap.esc_codigo = e.esc_codigo
	LEFT JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	left JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	left JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')


SELECT distinct aae.*, e.esc_nome
--delete aae
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.AcompanhamentoAplicacaoEscola aae ON aae.Edicao = '2019' AND aae.esc_codigo = e.esc_codigo
	left JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	left JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	left JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')
--COMMIT

---------------------------------------------------------------------------

BEGIN TRAN
select * 
--UPDATE qu SET qu.esc_codigo = pf.esc_codigo
FROM QuestionarioUsuario qu
  JOIN PessoaPerfil pf ON pf.Edicao = qu.Edicao AND pf.usu_id = qu.usu_id AND pf.PerfilID = 4
WHERE qu.Edicao = '2019' AND
  qu.QuestionarioID = 23 AND
  pf.esc_codigo <> qu.esc_codigo

select * 
--UPDATE qu SET qu.esc_codigo = pf.esc_codigo
FROM QuestionarioUsuario qu
  JOIN PessoaPerfil pf ON pf.Edicao = qu.Edicao AND pf.usu_id = qu.usu_id AND pf.PerfilID = 2
WHERE qu.Edicao = '2019' AND
  qu.QuestionarioID = 24 AND
  pf.esc_codigo <> qu.esc_codigo

select * 
--UPDATE qu SET qu.esc_codigo = pf.esc_codigo
FROM QuestionarioUsuario qu
  JOIN PessoaPerfil pf ON pf.Edicao = qu.Edicao AND pf.usu_id = qu.usu_id AND pf.PerfilID = 2
WHERE qu.Edicao = '2019' AND
  qu.QuestionarioID = 25 AND
  pf.esc_codigo <> qu.esc_codigo
ROLLBACK--COMMIT

--------------------------------------------------------------------------------------------------------

BEGIN TRAN

select * 
--UPDATE qu SET qu.esc_codigo = '0'
from QuestionarioUsuario qu
  LEFT JOIN PessoaPerfil pf ON pf.Edicao = qu.Edicao AND pf.usu_id = qu.usu_id AND pf.PerfilID = 4
WHERE qu.Edicao = '2019' AND
  qu.QuestionarioID = 23 AND
  pf.usu_id IS NULL;

select * 
--UPDATE qu SET qu.esc_codigo = '0'
from QuestionarioUsuario qu
  LEFT JOIN PessoaPerfil pf ON pf.Edicao = qu.Edicao AND pf.usu_id = qu.usu_id AND pf.PerfilID = 2
WHERE qu.Edicao = '2019' AND
  qu.QuestionarioID = 24 AND
  pf.usu_id IS NULL;

select * 
--UPDATE qu SET qu.esc_codigo = '0'
from QuestionarioUsuario qu
  LEFT JOIN PessoaPerfil pf ON pf.Edicao = qu.Edicao AND pf.usu_id = qu.usu_id AND pf.PerfilID = 2
WHERE qu.Edicao = '2019' AND
  qu.QuestionarioID = 25 AND
  pf.usu_id IS NULL;

COMMIT

--------------------------------------------------------------------------------------------------------

SELECT distinct qri.*
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
	INNER JOIN ProvaSP.dbo.QuestionarioUsuario qu ON qu.Edicao = '2019' AND qu.esc_codigo = e.esc_codigo
	INNER JOIN ProvaSP.dbo.QuestionarioRespostaItem qri ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID
WHERE tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

SELECT distinct qu.*, e.esc_nome
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
	INNER JOIN ProvaSP.dbo.QuestionarioUsuario qu ON qu.Edicao = '2019' AND qu.esc_codigo = e.esc_codigo
	--INNER JOIN ProvaSP.dbo.QuestionarioRespostaItem qri ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID
WHERE tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

BEGIN TRAN

SELECT distinct pf.*
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
	INNER JOIN ProvaSP.dbo.PessoaPerfil pf				ON pf.Edicao = '2019' AND pf.esc_codigo = e.esc_codigo
	--INNER JOIN ProvaSP.dbo.QuestionarioUsuario qu ON qu.Edicao = '2019' AND qu.esc_codigo = e.esc_codigo
	--INNER JOIN ProvaSP.dbo.QuestionarioRespostaItem qri ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID
WHERE tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

COMMIT



SELECT distinct aap.*, e.esc_nome
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
	INNER JOIN ProvaSP.dbo.AcompanhamentoAplicacaoPessoa aap ON aap.Edicao = '2019' AND aap.esc_codigo = e.esc_codigo
WHERE tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

select * from PessoaPerfil pf where pf.usu_id = '78AADA57-43B6-E111-B597-00155D02E702'

select * from ProvaSP.dbo.QuestionarioUsuario qu WHERE qu.Edicao = '2019' AND qu.usu_id = '78AADA57-43B6-E111-B597-00155D02E702'


select *
--delete aap
FROM QuestionarioUsuario qu
	JOIN AcompanhamentoAplicacaoPessoa aap
	ON aap.Edicao = qu.Edicao
		AND aap.usu_id = qu.usu_id
		AND aap.AtributoID = 30 --QuestionarioDeDiretorPreenchido
	LEFT JOIN AcompanhamentoAplicacaoPessoa aapDUP
	ON aap.Edicao = aapDUP.Edicao
		AND aap.usu_id = aapDUP.usu_id
		AND aap.AtributoID = aapDUP.AtributoID
		AND coalesce(qu.esc_codigo, '-1') = coalesce(aapDUP.esc_codigo, '-1')
WHERE qu.Edicao = '2019'
	AND qu.QuestionarioID = 24 -- Diretor
	AND coalesce(qu.esc_codigo, '-1') <> coalesce(aap.esc_codigo, '-1')
	AND aapDUP.usu_id IS NOT NULL


select *
--DELETE AAP
FROM QuestionarioUsuario qu
	JOIN AcompanhamentoAplicacaoPessoa aap
	ON aap.Edicao = qu.Edicao
		AND aap.usu_id = qu.usu_id
		AND aap.AtributoID = 83 --QuestionarioAssistenteDiretoriaPreenchido
	LEFT JOIN AcompanhamentoAplicacaoPessoa aapDUP
	ON aap.Edicao = aapDUP.Edicao
		AND aap.usu_id = aapDUP.usu_id
		AND aap.AtributoID = aapDUP.AtributoID
		AND coalesce(qu.esc_codigo, '-1') = coalesce(aapDUP.esc_codigo, '-1')
WHERE qu.Edicao = '2019'
	AND qu.QuestionarioID = 25 -- Assist. Diretor
	AND coalesce(qu.esc_codigo, '-1') <> coalesce(aap.esc_codigo, '-1')
	AND aapDUP.usu_id IS NOT NULL
	
SELECT distinct aap.*, e.esc_nome
--delete aap
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.AcompanhamentoAplicacaoPessoa aap ON aap.Edicao = '2019' AND aap.esc_codigo = e.esc_codigo
	LEFT JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	left JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	left JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

SELECT distinct aae.*, e.esc_nome
--delete aae
FROM GestaoPedagogica.dbo.ESC_Escola e
	INNER JOIN ProvaSP.dbo.AcompanhamentoAplicacaoEscola aae ON aae.Edicao = '2019' AND aae.esc_codigo = e.esc_codigo
	left JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
	left JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
	left JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
WHERE tce.tce_nome is null OR tce.tce_nome NOT IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

