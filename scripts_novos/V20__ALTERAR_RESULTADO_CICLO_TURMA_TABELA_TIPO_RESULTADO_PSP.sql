use GestaoAvaliacao;

UPDATE dbo.TipoResultadoPsp
SET Nome = 'Resultado Turma - Ciclo de Aprendizagem',
	State = 1,
	ModeloArquivo = '"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"tur_codigo";"tur_id";"CicloID";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado";"PercentualAlfabetizado"
2021;1;"BT";191;"4A";2261582;2;161,051724137931;29;NA;0,48;0,31;0,17;0,03;NA'
where Codigo = 12