USE [GestaoAvaliacao_SGP]
GO
/****** Object:  StoredProcedure [dbo].[SP_REGULARIZAR_TURMAS_MATRICULAS]    Script Date: 18/08/2021 19:12:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_REGULARIZAR_TURMAS_MATRICULAS]	
	@esc_id INT,
	@ano_letivo int
AS
BEGIN

-- MATRICULA SGP LEGADO
select mtu.*,alu.pes_id
into #MTR_LEGADO
from GestaoPedagogica..MTR_MatriculaTurma mtu WITH(NOLOCK)
INNER JOIN GestaoPedagogica..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
INNER JOIN GestaoPedagogica..TUR_Turma t WITH(NOLOCK) on mtu.tur_id = t.tur_id
where t.esc_id = @esc_id
and mtu.mtu_situacao = 1
and mtu.mtu_dataSaida is null
and YEAR(mtu.mtu_dataMatricula) = @ano_letivo

-- MATRICULA SERAP
SELECT mtu.*
into #MTR_SERAP
FROM GestaoAvaliacao_SGP..MTR_MatriculaTurma mtu WITH(NOLOCK)
INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
INNER JOIN GestaoAvaliacao_SGP..TUR_Turma t WITH(NOLOCK) on mtu.tur_id = t.tur_id
WHERE t.esc_id = @esc_id
and mtu.mtu_situacao = 1
and mtu.mtu_dataSaida is null
and YEAR(mtu.mtu_dataMatricula) = @ano_letivo

-- PEGA TODAS AS MATRICULAS QUE PRECISA RELARIZAR NO SERAP
select *
into #MTR_ATT
from (
select a.pes_id,a.alu_id,a.tur_id,b.alu_id alu_id_serap, b.tur_id tur_id_serap 
from #MTR_LEGADO a
left join #MTR_SERAP b on a.alu_id = b.alu_id and a.tur_id = b.tur_id) A
where a.alu_id_serap is null and a.tur_id_serap is null

-- REGULARIZA AS MATRICULAS NO SERAP
DECLARE @pes_id uniqueidentifier
DECLARE db_cursor CURSOR FOR 
select pes_id from #MTR_ATT
OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @pes_id
WHILE @@FETCH_STATUS = 0  
BEGIN      
	  exec SP_REGULARIZAR_MATRICULA_ALUNO @pes_id, @ano_letivo;
      FETCH NEXT FROM db_cursor INTO @pes_id 
END
CLOSE db_cursor  
DEALLOCATE db_cursor 



DROP TABLE #MTR_ATT
DROP TABLE #MTR_LEGADO
DROP TABLE #MTR_SERAP
END