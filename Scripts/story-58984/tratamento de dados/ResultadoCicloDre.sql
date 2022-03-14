use ProvaSP_Resultado2021;


update ResultadoCicloDre set totalAlunos = 0 where totalAlunos is null
update ResultadoCicloDre set PercentualAlfabetizado = 0 where PercentualAlfabetizado is null
update ResultadoCicloDre set Valor = ROUND(Valor, 1);
update ResultadoCicloDre set PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1);
update ResultadoCicloDre set PercentualBasico = ROUND(PercentualBasico, 1);
update ResultadoCicloDre set PercentualAdequado = ROUND(PercentualAdequado, 1);
update ResultadoCicloDre set PercentualAvancado = ROUND(PercentualAvancado, 1);
update ResultadoCicloDre set PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1);

