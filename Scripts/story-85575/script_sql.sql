use GestaoAvaliacao

alter table TipoResultadoPsp add ModeloArquivo varchar(max) default null

-- Resultado Aluno
update TipoResultadoPsp
set ModeloArquivo =
'"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"AnoEscolar";"tur_codigo";"tur_id";"alu_matricula";"alu_nome";"NivelProficienciaID";"Valor";"REDQ1";"REDQ2";"REDQ3";"REDQ4";"REDQ5"
2021;1;"IQ";93459;4;"4A";1010101;1010101;"TESTE ALUNO";4;299,5;NA;NA;NA;NA;NA'
where codigo = 1;

-- Resultado Turma
update TipoResultadoPsp
set ModeloArquivo =
'"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"AnoEscolar";"tur_codigo";"tur_id";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado"
2021;1;"BT";191;4;"4A";2261582;161,1;29;2;48,3;31;17,2;3,4'
where codigo = 2;

-- Resultado Escola
update TipoResultadoPsp
set ModeloArquivo =
'"Edicao";"AreaConhecimentoID";"uad_sigla";"esc_codigo";"AnoEscolar";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado"
2021;1;"BT";191;4;161,1;29;2;48,3;31;17,2;3,4'
where codigo = 3;

-- Resultado Dre
update TipoResultadoPsp
set ModeloArquivo =
'"Edicao";"AreaConhecimentoID";"uad_sigla";"AnoEscolar";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado"
2021;1;"BT";4;162,2;2134;2;44,8;33;16,5;5,6'
where codigo = 4;

-- Resultado Sme
update TipoResultadoPsp
set ModeloArquivo =
'"Edicao";"AreaConhecimentoID";"AnoEscolar";"Valor";"TotalAlunos";"NivelProficienciaID";"PercentualAbaixoDoBasico";"PercentualBasico";"PercentualAdequado";"PercentualAvancado"
2022;1;4;159,3;33728;2;48,3;31,3;15,1;5,3'
where codigo = 5;