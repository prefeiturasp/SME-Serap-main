use GestaoAvaliacao;

-- Atualizar tipos existentes
UPDATE dbo.TipoResultadoPsp
SET Nome='Participação - Turma - Geral', State=1, ModeloArquivo='"Edicao";"uad_sigla";"esc_codigo";"AnoEscolar";"tur_codigo";"tur_id";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2021;MP;108;3;3A;2253512;33;29;87,88'
WHERE Codigo = 13;

UPDATE dbo.TipoResultadoPsp
SET Nome='Participação - Turma - Área de Conhecimento', State=1, ModeloArquivo='"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"AnoEscolar";"tur_codigo";"tur_id";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2021;1;MP;108;3;3A;2253512;33;29;87,88'
WHERE Codigo = 14;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Participação - SME - Área de Conhecimento', State=1, ModeloArquivo='"Edicao";"AreaConhecimentoID";"AnoEscolar";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2022;1;3;10;20;30,10'
WHERE Codigo = 20;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Participação - Escola - Geral', State=1, ModeloArquivo='"Edicao";"uad_sigla";"esc_codigo";"AnoEscolar";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2021;MP;108;3;33;29;87,88'
WHERE Codigo = 15;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Participação - Escola - Área de Conhecimento', State=1, ModeloArquivo='"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"AnoEscolar";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2021;1;MP;108;3;33;29;87,88'
WHERE Codigo = 16;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Participação - SME - Geral', State=1, ModeloArquivo='"Edicao";"AnoEscolar";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2021;3;33;29;87,88'
WHERE Codigo = 19;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Participação - DRE - Geral', State=1, ModeloArquivo='"Edicao";"uad_sigla";"AnoEscolar";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2021;MP;3;33;29;87,88'
WHERE Codigo = 17;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Participação - DRE - Área de Conhecimento', State=1, ModeloArquivo=N'"Edicao";"AreaConhecimentoID";"uad_sigla";"AnoEscolar";"TotalPrevisto";"TotalPresente";"PercentualParticipacao"
2022;1;BT;3;10;20;30,30'
WHERE Codigo = 18;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Resultado DRE', State=1
WHERE Codigo = 4;

UPDATE GestaoAvaliacao.dbo.TipoResultadoPsp
SET Nome='Resultado SME', State=1
WHERE Codigo = 5;