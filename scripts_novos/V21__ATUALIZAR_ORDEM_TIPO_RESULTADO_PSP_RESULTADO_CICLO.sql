use gestaoavaliacao

update TipoResultadoPsp 
set codigo = 7
where NomeTabelaProvaSp = 'ResultadoCicloAluno';

update TipoResultadoPsp 
set codigo = 8
where NomeTabelaProvaSp = 'ResultadoCicloTurma';

update TipoResultadoPsp 
set codigo = 9
where NomeTabelaProvaSp = 'ResultadoCicloEscola';

update TipoResultadoPsp 
set codigo = 10
where NomeTabelaProvaSp = 'ResultadoCicloDre';

update TipoResultadoPsp 
set codigo = 11
where NomeTabelaProvaSp = 'ResultadoCicloSme';

update TipoResultadoPsp 
set codigo = 12
where NomeTabelaProvaSp = 'ResultadoCicloEnturmacaoAtual';