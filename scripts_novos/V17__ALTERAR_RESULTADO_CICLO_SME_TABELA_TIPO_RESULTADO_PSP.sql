use GestaoAvaliacao;

UPDATE dbo.TipoResultadoPsp
SET Nome = 'Resultado SME - Ciclo de Aprendizagem',
	State = 1,
	ModeloArquivo = '"Edicao";"AreaConhecimentoID";"CicloID";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado";"PercentualAlfabetizado"
2021;1;2;176,332920347322;106299;"NA";0,51;0,31;0,14;0,04;"NA"'
where Codigo = 11