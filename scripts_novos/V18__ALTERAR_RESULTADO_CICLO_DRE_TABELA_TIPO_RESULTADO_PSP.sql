use GestaoAvaliacao;

UPDATE dbo.TipoResultadoPsp
SET Nome = 'Resultado DRE - Ciclo de Aprendizagem',
	State = 1,
	ModeloArquivo = '"Edicao";"AreaConhecimentoID";"uad_sigla";"CicloID";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado";"PercentualAlfabetizado"  2021;1;"BT",2;176,332920347322;106299;"NA";0,51;0,31;0,14;0,04;"NA"'
where Codigo = 8
