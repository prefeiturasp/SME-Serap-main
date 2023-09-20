use GestaoAvaliacao;


UPDATE dbo.TipoResultadoPsp
SET Nome = 'Resultado DRE - Ciclo de Aprendizagem',
	State = 1,
	ModeloArquivo = '"Edicao";"AreaConhecimentoID";"uad_sigla";"CicloID";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado";"PercentualAlfabetizado"
2022;3;"BT";2;209.200;0;0;0.60;0.40;0.10;0.00;1.00;'
where Codigo = 8
