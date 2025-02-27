use GestaoAvaliacao;

UPDATE dbo.TipoResultadoPsp
SET Nome = 'Resultado Escola - Ciclo de Aprendizagem',
	State = 1,
	ModeloArquivo = '"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"CicloID";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado";"PercentualAlfabetizado"
2021;1;"BT";191;2;186,679850746269;134;NA;0,48;0,3;0,18;0,04;NA'
where Codigo = 10