USE [GestaoAvaliacao_SGP]
GO
/****** Object:  StoredProcedure [dbo].[SP_REGULARIZAR_MATRICULA_ALUNO]    Script Date: 20/07/2021 16:56:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_REGULARIZAR_MATRICULA_ALUNO]	
	@pes_id uniqueidentifier,
	@ano_letivo int
AS
BEGIN

-- Passo 1:
MERGE INTO ACA_CurriculoPeriodo Destino
USING (select mtu.cur_id,
			  mtu.crr_id,
			  mtu.crp_id,
			  cp.crp_ordem,
			  cp.crp_descricao,
			  1 crp_situacao,
			  getdate() crp_dataCriacao,
			  getdate() crp_dataAlteracao,
			  cp.tcp_id
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
				  inner join GestaoPedagogica..ACA_CurriculoPeriodo cp
				  on mtu.cur_id = cp.cur_id and
				  	 mtu.crr_id = cp.crr_id and
				  	 mtu.crp_id = cp.crp_id and
					 tpcp.tcp_id = cp.tcp_id
            where alu.alu_situacao = 1
              and mtu.mtu_situacao <> 3 -- Alterado de 1 para 3 - 07-06/16
              and tur.tur_situacao <> 3
              and crp.crp_situacao <> 3
			  and tpcp.tcp_situacao <> 3 
			  and alu.pes_id = @pes_id
			  and ca.cal_ano = @ano_letivo
            group by mtu.cur_id,
					 mtu.crr_id,
					 mtu.crp_id,
					 cp.crp_ordem,
					 cp.crp_descricao,					 
					 cp.tcp_id
			) Origem	
	ON	Destino.cur_id = Origem.cur_id
	AND Destino.crr_id = Origem.crr_id
	AND Destino.crp_id = Origem.crp_id
	AND Destino.tcp_id = Origem.tcp_id
WHEN NOT MATCHED THEN
         INSERT (cur_id,
				 crr_id,
				 crp_id,
				 crp_ordem,
				 crp_descricao,
				 crp_situacao,
				 crp_dataCriacao,
				 crp_dataAlteracao,
				 tcp_id)
         VALUES (Origem.cur_id,
				 Origem.crr_id,
				 Origem.crp_id,
				 Origem.crp_ordem,
				 Origem.crp_descricao,
				 Origem.crp_situacao,
				 Origem.crp_dataCriacao,
				 Origem.crp_dataAlteracao,
				 Origem.tcp_id);

-- Passo 2:
MERGE INTO TUR_TurmaCurriculo Destino
USING (select tur.tur_id,
			  mtu.cur_id,
			  mtu.crr_id,
			  mtu.crp_id,
			  1 tcr_situacao,
			  getdate() tcr_dataCriacao,
			  getdate() tcr_dataAlteracao,
			  crp.tcp_id			  
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
              and mtu.mtu_situacao <> 3 -- Alterado de 1 para 3 - 07-06/16
              and tur.tur_situacao <> 3
              and crp.crp_situacao <> 3
			  and tpcp.tcp_situacao <> 3 
			  and alu.pes_id = @pes_id
			  and ca.cal_ano = @ano_letivo
            group by tur.tur_id,
					 mtu.cur_id,
					 mtu.crr_id,
					 mtu.crp_id,
					 crp.tcp_id) Origem
	ON Destino.tur_id = Origem.tur_id
	AND Destino.cur_id = Origem.cur_id
	AND Destino.crr_id = Origem.crr_id
	AND Destino.crp_id = Origem.crp_id
	AND Destino.tcp_id = Origem.tcp_id
WHEN NOT MATCHED THEN
         INSERT ([tur_id]
				,[cur_id]
				,[crr_id]
				,[crp_id]
				,[tcr_situacao]
				,[tcr_dataCriacao]
				,[tcr_dataAlteracao]
				,[tcp_id])
         VALUES (Origem.[tur_id]
				,Origem.[cur_id]
				,Origem.[crr_id]
				,Origem.[crp_id]
				,Origem.[tcr_situacao]
				,Origem.[tcr_dataCriacao]
				,Origem.[tcr_dataAlteracao]
				,Origem.[tcp_id]);

-- Passo 3:
MERGE INTO MTR_MatriculaTurma Destino
    USING (select alu.alu_id, mtu.mtu_id, tur.esc_id, tur.tur_id, mtu.cur_id, mtu.crr_id, mtu.crp_id,
                  mtu.mtu_situacao, mtu.mtu_dataCriacao, mtu.mtu_dataAlteracao, mtu.mtu_numeroChamada
                 ,mtu.mtu_dataMatricula, mtu.mtu_dataSaida, crp.tcp_id -- Add 07-06/16
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
              and mtu.mtu_situacao <> 3 -- Alterado de 1 para 3 - 07-06/16
              and tur.tur_situacao <> 3
              and crp.crp_situacao <> 3
			  and tpcp.tcp_situacao <> 3 
			  and alu.pes_id = @pes_id
			  and ca.cal_ano = @ano_letivo
            group by alu.alu_id, mtu.mtu_id, tur.esc_id, tur.tur_id, mtu.cur_id, mtu.crr_id, mtu.crp_id,
                  mtu.mtu_situacao, mtu.mtu_dataCriacao, mtu.mtu_dataAlteracao, mtu.mtu_numeroChamada,mtu.mtu_dataMatricula, mtu.mtu_dataSaida, crp.tcp_id) Origem
     ON Destino.alu_id = Origem.alu_id
    AND Destino.mtu_id = Origem.mtu_id	
    WHEN NOT MATCHED THEN
         INSERT (alu_id, mtu_id, esc_id, tur_id, cur_id, crr_id, crp_id,
                 mtu_situacao, mtu_dataCriacao, mtu_dataAlteracao, mtu_numeroChamada,  mtu_dataMatricula, mtu_dataSaida, tcp_id)
         VALUES (Origem.alu_id, Origem.mtu_id, Origem.esc_id, Origem.tur_id, Origem.cur_id,
                 Origem.crr_id, Origem.crp_id, Origem.mtu_situacao,
                 Origem.mtu_dataCriacao, Origem.mtu_dataAlteracao, Origem.mtu_numeroChamada,Origem.mtu_dataMatricula, Origem.mtu_dataSaida, Origem.tcp_id);
END
