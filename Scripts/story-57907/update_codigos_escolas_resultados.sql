use ProvaSP;

-- ---------------------------------------------------------
SELECT distinct ra.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ResultadoAluno ra WITH (NOLOCK)
left join Escola e WITH (NOLOCK) on ra.esc_codigo = e.esc_codigo
WHERE ra.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ResultadoAluno set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC
-- ---------------------------------------------------------
SELECT distinct re.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ResultadoEscola re WITH (NOLOCK)
left join Escola e WITH (NOLOCK) on re.esc_codigo = e.esc_codigo
WHERE re.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ResultadoEscola set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC
-- ---------------------------------------------------------
SELECT distinct rt.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ResultadoTurma rt WITH (NOLOCK)
left join Escola e WITH (NOLOCK) on rt.esc_codigo = e.esc_codigo
WHERE rt.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ResultadoTurma set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC
-- ---------------------------------------------------------