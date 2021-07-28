USE GestaoAvaliacao_SGP

declare @ra varchar(20) = ''
declare @pes_id UNIQUEIDENTIFIER
select @pes_id = pes_id from CoreSSO..SYS_Usuario where usu_login = @ra


exec SP_REGULARIZAR_MATRICULA_ALUNO @pes_id, 2021

-- MATRICULA SGP LEGADO
SELECT 
mtu.mtu_dataMatricula, 
mtu.mtu_dataSaida, 
mtu.cur_id,mtu.crr_id,mtu.crp_id,mtu.alu_id,mtu.tur_id
FROM GestaoPedagogica..MTR_MatriculaTurma mtu WITH(NOLOCK) 
INNER JOIN GestaoPedagogica..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
WHERE alu.pes_id = @pes_id

-- MATRICULA SERAP
SELECT 
mtu.mtu_dataMatricula, 
mtu.mtu_dataSaida, 
mtu.cur_id,mtu.crr_id,mtu.crp_id,mtu.alu_id,mtu.tur_id
FROM GestaoAvaliacao_SGP..MTR_MatriculaTurma mtu WITH(NOLOCK) 
INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
WHERE alu.pes_id = @pes_id


-------------------------------------------------------------------------------------------------
select *
into #PROVAS
from GestaoAvaliacao.dbo.TestsByStudent(@pes_id)

-- VERIFICA E CORRIGE OS TIPOS DE DEFICIENCIA
select *
into #PessoaDeficiencia
from CoreSSO..PES_PessoaDeficiencia 
where pes_id = @pes_id

select distinct tt.*
into #DeficienciaInserir
from #PROVAS p
inner join GestaoAvaliacao..testTypeDeficiency tt on p.TestTypeId = tt.TestType_Id
where p.TargetToStudentsWithDeficiencies = 1
and tt.DeficiencyId not in(select tde_id from #PessoaDeficiencia)

IF exists (select 1 from #DeficienciaInserir)
BEGIN
insert into CoreSSO..PES_PessoaDeficiencia
select @pes_id pes_id, DeficiencyId tde_id from #DeficienciaInserir
END


drop table #PROVAS
drop table #PessoaDeficiencia
drop table #DeficienciaInserir


