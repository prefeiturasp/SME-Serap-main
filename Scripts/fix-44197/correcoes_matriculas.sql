USE GestaoAvaliacao_SGP

IF OBJECT_ID(N'tempdb..#ra_alunos') IS NOT NULL
BEGIN
DROP TABLE #ra_alunos
END
GO
create table #ra_alunos (ra varchar(20));

insert into #ra_alunos values 
('RA4481692'),
('RA4876747'),
('RA2945283'),
('RA4077525'),
('RA2945283'),
('RA2960037'),
('RA4239126'),
('RA4728201'),
('RA3828272'),
('RA3543412'),
('RA4066368'),
('RA3926539'),
('RA3679827'),
('RA3672777'),
('RA3979579'),
('RA7536391'),
('RA3898492'),
('RA3402468'),
('RA4084497'),
('RA3934605'),
('RA4414239'),
('RA4371832'),
('RA4640413'),
('RA4538820'),
('RA3230082'),
('RA2689322'),
('RA3960482'),
('RA3420855'),
('RA4571376'),
('RA4021093'),
('RA4527176'),
('RA4547517'),
('RA4518686'),
('RA4071460'),
('RA3938243'),
('RA3948834'),
('RA3574710'),
('RA4233582'),
('RA2689267'),
('RA7700658'),
('RA4260805'),
('RA3397858'),
('RA4779748'),
('RA4800374'),
('RA3898347'),
('RA4518727'),
('RA2811143'),
('RA4518658'),
('RA6300773'),
('RA5933258'),
('RA4534160'),
('RA4849613'),
('RA3686175'),
('RA4953113'),
('RA7672639'),
('RA4578130'),
('RA3502975'),
('RA6768723'),
('RA3482002'),
('RA4266674'),
('RA6182114'),
('RA3364573'),
('RA4214506'),
('RA4253651'),
('RA3547389'),
('RA4281143'),
('RA4266669'),
('RA4539759'),
('RA3923315'),
('RA4021118'),
('RA6419760'),
('RA3565400'),
('RA4394546'),
('RA3852253'),
('RA4218534'),
('RA3605863'),
('RA4783350'),
('RA3689125'),
('RA5328647'),
('RA4759791'),
('RA4847458'),
('RA2464583'),
('RA3589291'),
('RA4258062'),
('RA4232267'),
('RA3870647'),
('RA3677481'),
('RA4266615'),
('RA4492794'),
('RA4806109'),
('RA3947161'),
('RA4276273'),
('RA4066374'),
('RA4066401'),
('RA3681258'),
('RA3898697'),
('RA4096187'),
('RA4214906'),
('RA3934749'),
('RA4728989'),
('RA3894765'),
('RA3343094'),
('RA3928141'),
('RA4782716'),
('RA4728776'),
('RA4004431'),
('RA3927899'),
('RA4266684'),
('RA4208712'),
('RA3684266'),
('RA3934985'),
('RA7738620'),
('RA7738620'),
('RA3938310'),
('RA3538835'),
('RA4274605'),
('RA3356803'),
('RA2709634'),
('RA3388014'),
('RA4182650'),
('RA5350950')


declare @ra varchar(20)
DECLARE db_cursor CURSOR FOR 
select ra from #ra_alunos

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @ra  

WHILE @@FETCH_STATUS = 0  
BEGIN  
      
	declare @pes_id UNIQUEIDENTIFIER

	select top 1 
	@pes_id = pes_id 
	from GestaoPedagogica..ACA_Aluno alu
	inner join GestaoPedagogica..ACA_AlunoCurriculo alc on alu.alu_id = alc.alu_id
	where alc.alc_matricula = REPLACE(@ra,'RA','')
	order by alc.alc_id desc

IF @pes_id is null
BEGIN
	SELECT 'ALUNO NÃO LOCALIZADO NO BANCO: GestaoPedagogica' MSG
END
ELSE
BEGIN

	
	declare @ano_letivo int = 2021

	------------------------------------------------------------------------------------------------
	select p.*, u.usu_id
	into #PessoaUsuario
	from CoreSSO..PES_Pessoa p
	left join CoreSSO..SYS_Usuario u on p.pes_id = u.pes_id and REPLACE(u.usu_login,'RA','') = REPLACE(@ra,'RA','')
	where p.pes_id = @pes_id

	if (select usu_id from #PessoaUsuario) is null
	begin

		declare @new_usu_id UNIQUEIDENTIFIER = newid()
		insert into CoreSSO..SYS_Usuario (usu_id,usu_login,usu_senha,usu_criptografia,usu_situacao,usu_dataCriacao,usu_dataAlteracao,pes_id,usu_integridade,ent_id,usu_integracaoAD,usu_dataAlteracaoSenha)
		select
		@new_usu_id, @ra usu_login, @ra usu_senha, 1 usu_criptografia, 1 usu_situacao, GETDATE() usu_dataCriacao, GETDATE() usu_dataAlteracao, 
		pu.pes_id, 0 usu_integridade, alu.ent_id, 0 usu_integracaoAD, GETDATE() usu_dataAlteracaoSenha
		from #PessoaUsuario pu
		inner join GestaoPedagogica..ACA_Aluno alu on pu.pes_id = alu.pes_id

		declare @gru_alu_serap UNIQUEIDENTIFIER = 'BD6D9CE6-9456-E711-9541-782BCB3D218E' -- GRUPO ALUNO SERAP
		declare @gru_alu_boletim UNIQUEIDENTIFIER = 'BDE3EA03-08E5-E311-A825-782BCB3D218E' -- GRUPO ALUNO BOLETIM ON LINE

		insert into CoreSSO..SYS_UsuarioGrupo (usu_id, gru_id, usg_situacao) values (@new_usu_id,@gru_alu_serap,1),(@new_usu_id,@gru_alu_boletim,1)		

	end

	drop table #PessoaUsuario

	declare @alu_id int, @alu_nome varchar(max), @alu_matricula varchar(20)
	select @alu_id = ISNULL(alu_id,0) from ACA_Aluno where pes_id = @pes_id
	select @alu_nome = pes_nome from CoreSSO..PES_Pessoa where pes_id = @pes_id
	select @alu_matricula = REPLACE(@ra,'RA','')

	IF not exists (select 1 from ACA_Aluno where pes_id = @pes_id and alu_situacao = 1) 
		and exists (select 1 from GestaoPedagogica..ACA_Aluno where pes_id = @pes_id and alu_situacao = 1)
	BEGIN	

		select @alu_id = MAX(alu_id) + 1 from ACA_Aluno
		insert into ACA_Aluno
		select @alu_id alu_id,@alu_nome alu_nome,ent_id,@alu_matricula alu_matricula,GETDATE(),GETDATE(),alu_situacao,null MatriculaTurma_alu_id,null MatriculaTurma_mtu_id,pes_id 
		from GestaoPedagogica..ACA_Aluno where pes_id = @pes_id and alu_situacao = 1
	
	END

	update ACA_Aluno set alu_situacao = 1 where pes_id = @pes_id
	--------------------------------------------

	
	IF @alu_id != (select alu_id from GestaoPedagogica..ACA_Aluno where pes_id = @pes_id and alu_situacao = 1)
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
	INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno sgp_aluno ON sgp_aluno.alu_matricula = aluno.CL_ALU_CODIGO and sgp_aluno.alu_situacao = 1
	LEFT JOIN CoreSSO..Pes_PessoaDeficiencia pesDef ON pesDef.pes_id = sgp_aluno.pes_id
	where 
	CL_DATST >= '2020-01-01'
	AND sgp_aluno.alu_matricula IN (@alu_matricula)
	AND pesDef.pes_id IS NULL
	-------------------------------------------------------------------------------------------------

	-- MATRICULA SGP LEGADO
	SELECT * 
	FROM
	(SELECT 
	mtu.mtu_dataMatricula mtu_dataMatricula_LEGADO, 
	mtu.mtu_dataSaida mtu_dataSaida_LEGADO, 
	mtu.alu_id alu_id_LEGADO, mtu.tur_id tur_id_LEGADO
	,@pes_id pes_id
	FROM GestaoPedagogica..MTR_MatriculaTurma mtu WITH(NOLOCK) 
	INNER JOIN GestaoPedagogica..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
	WHERE alu.pes_id = @pes_id and alu.alu_situacao = 1) A
	LEFT JOIN
	-- MATRICULA SERAP
	(SELECT 
	mtu.mtu_dataMatricula mtu_dataMatricula_SERAP, 
	mtu.mtu_dataSaida mtu_dataSaida_SERAP, 
	mtu.alu_id alu_id_SERAP, mtu.tur_id tur_id_SERAP
	,@pes_id pes_id, @alu_matricula alu_matricula
	FROM GestaoAvaliacao_SGP..MTR_MatriculaTurma mtu WITH(NOLOCK) 
	INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
	WHERE alu.pes_id = @pes_id and alu.alu_situacao = 1) B
	ON A.pes_id = B.pes_id

END

    FETCH NEXT FROM db_cursor INTO @ra
 
END

CLOSE db_cursor  
DEALLOCATE db_cursor




