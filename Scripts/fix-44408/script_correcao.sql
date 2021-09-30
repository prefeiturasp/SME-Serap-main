USE GestaoAvaliacao_SGP

IF OBJECT_ID(N'tempdb..#ra_alunos') IS NOT NULL
BEGIN
DROP TABLE #ra_alunos
END
GO
create table #ra_alunos (ra varchar(20));

insert into #ra_alunos values 
('RA4499492'),
('RA6272622'),
('RA7649601'),
('RA5140434'),
('RA5095535'),
('RA6022647'),
('RA7650028'),
('RA5256757'),
('RA6510754')


declare @ra varchar(20)
DECLARE db_cursor CURSOR FOR 
select ra from #ra_alunos

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @ra  

WHILE @@FETCH_STATUS = 0  
BEGIN  
      
	declare @pes_id UNIQUEIDENTIFIER
	select @pes_id = pes_id from CoreSSO..SYS_Usuario where usu_login = @ra
	declare @ano_letivo int = 2021

	------------------------------------------------------------------------------------------------

	declare @alu_id int, @alu_nome varchar(max), @alu_matricula varchar(20)
	select @alu_id = ISNULL(alu_id,0) from ACA_Aluno where pes_id = @pes_id
	select @alu_nome = pes_nome from CoreSSO..PES_Pessoa where pes_id = @pes_id
	select @alu_matricula = REPLACE(usu_login,'RA','') from CoreSSO..SYS_Usuario where pes_id = @pes_id

	IF not exists (select 1 from ACA_Aluno where pes_id = @pes_id) 
		and exists (select 1 from GestaoPedagogica..ACA_Aluno where pes_id = @pes_id)
	BEGIN	

		select @alu_id = MAX(alu_id) + 1 from ACA_Aluno
		insert into ACA_Aluno
		select @alu_id alu_id,@alu_nome alu_nome,ent_id,@alu_matricula alu_matricula,GETDATE(),GETDATE(),alu_situacao,null MatriculaTurma_alu_id,null MatriculaTurma_mtu_id,pes_id 
		from GestaoPedagogica..ACA_Aluno where pes_id = @pes_id
	
	END

	IF @alu_id != (select alu_id from GestaoPedagogica..ACA_Aluno where pes_id = @pes_id)
	BEGIN

	-- inserir matricula
	select @alu_id alu_id, mtu.mtu_id, tur.esc_id, tur.tur_id, mtu.cur_id, mtu.crr_id, mtu.crp_id,
				mtu.mtu_situacao, mtu.mtu_dataCriacao, mtu.mtu_dataAlteracao, mtu.mtu_numeroChamada
				,mtu.mtu_dataMatricula, mtu.mtu_dataSaida, crp.tcp_id
			into #mtr_inserir
			from GestaoPedagogica..ACA_Aluno alu
				inner join GestaoPedagogica..MTR_MatriculaTurma mtu with (nolock)
				on alu.alu_id = mtu.alu_id
				inner join GestaoPedagogica..TUR_Turma tur
				on mtu.tur_id = tur.tur_id
				inner join ACA_CalendarioAnual ca
				on tur.cal_id = ca.cal_id
				inner join GestaoPedagogica..ACA_CurriculoPeriodo crp
				on mtu.cur_id = crp.cur_id
				and mtu.crr_id = crp.crr_id
				and mtu.crp_id = crp.crp_id
				inner join GestaoPedagogica..ACA_TipoCurriculoPeriodo tpcp
				on crp.tcp_id = tpcp.tcp_id
		where alu.alu_situacao = 1
			and mtu.mtu_situacao <> 3
			and tur.tur_situacao <> 3
			and crp.crp_situacao <> 3
			and tpcp.tcp_situacao <> 3 
			and alu.pes_id = @pes_id
			and ca.cal_ano = @ano_letivo
			and not exists (
				SELECT 1
				FROM GestaoAvaliacao_SGP..MTR_MatriculaTurma mtu_sgp WITH(NOLOCK) 
				INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno AS alu_sgp WITH(NOLOCK) ON alu_sgp.alu_id = mtu_sgp.alu_id
				WHERE alu_sgp.pes_id = @pes_id 
				and mtu_sgp.tur_id = mtu.tur_id
				and mtu_sgp.cur_id = mtu.cur_id
				and YEAR(mtu_sgp.mtu_dataMatricula) = @ano_letivo
				and mtu_sgp.mtu_dataSaida is null
			)
		group by alu.alu_id, mtu.mtu_id, tur.esc_id, tur.tur_id, mtu.cur_id, mtu.crr_id, mtu.crp_id,
				mtu.mtu_situacao, mtu.mtu_dataCriacao, mtu.mtu_dataAlteracao, mtu.mtu_numeroChamada,mtu.mtu_dataMatricula, mtu.mtu_dataSaida, crp.tcp_id

	insert into MTR_MatriculaTurma
	select alu_id,mtu_id,esc_id,tur_id,cur_id,crr_id,crp_id,mtu_situacao,GETDATE(),GETDATE(),mtu_numeroChamada,mtu_dataMatricula,mtu_dataSaida,tcp_id 
	from #mtr_inserir

	drop table #mtr_inserir
	END

	------------------------------------------------------------------------------------------------
	exec SP_REGULARIZAR_MATRICULA_ALUNO @pes_id, 2021

	-- MATRICULA SGP LEGADO
	SELECT 
	mtu.mtu_dataMatricula, 
	mtu.mtu_dataSaida, 
	mtu.cur_id,mtu.crr_id,mtu.crp_id,mtu.alu_id,mtu.tur_id
	,@pes_id pes_id
	FROM GestaoPedagogica..MTR_MatriculaTurma mtu WITH(NOLOCK) 
	INNER JOIN GestaoPedagogica..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
	WHERE alu.pes_id = @pes_id

	-- MATRICULA SERAP
	SELECT 
	mtu.mtu_dataMatricula, 
	mtu.mtu_dataSaida, 
	mtu.cur_id,mtu.crr_id,mtu.crp_id,mtu.alu_id,mtu.tur_id
	,@pes_id pes_id
	FROM GestaoAvaliacao_SGP..MTR_MatriculaTurma mtu WITH(NOLOCK) 
	INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
	WHERE alu.pes_id = @pes_id

	-------------------------------------------------------------------------------------------------
	-- VERIFICA E CORRIGE OS TIPOS DE DEFICIENCIA

	INSERT INTO CoreSSO..PES_PessoaDeficiencia
	select
	DISTINCT
	sgp_aluno.pes_id,
	def.tde_id
	from BD_PRODAM..v_alunos_da_turma_MSTECH  aluno
	INNER JOIN Manutencao..DEPARA_DEFICIENCIA def ON def.id_nee = aluno.id_nee
	INNER JOIN CoreSSO..PES_TipoDeficiencia tipo ON tipo.tde_id = def.tde_id
	INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno sgp_aluno ON sgp_aluno.alu_matricula = aluno.CL_ALU_CODIGO
	LEFT JOIN CoreSSO..Pes_PessoaDeficiencia pesDef ON pesDef.pes_id = sgp_aluno.pes_id
	where 
	CL_DATST >= '2020-01-01'
	AND sgp_aluno.alu_matricula IN (@alu_matricula)
	AND pesDef.pes_id IS NULL

    FETCH NEXT FROM db_cursor INTO @ra
END 

CLOSE db_cursor  
DEALLOCATE db_cursor




