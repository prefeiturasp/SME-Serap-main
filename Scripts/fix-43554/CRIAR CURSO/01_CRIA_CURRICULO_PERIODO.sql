-- CRIA CURRICULO PERIODO

USE [GestaoPedagogica]
GO
DISABLE TRIGGER TRG_ACA_CurriculoPeriodo_Identity on ACA_CurriculoPeriodo;

INSERT INTO [dbo].[ACA_CurriculoPeriodo]
           ([cur_id]
           ,[crr_id]
           ,[crp_id]
           ,[mep_id]
           ,[crp_ordem]
           ,[crp_descricao]
           ,[crp_idadeIdealAnoInicio]
           ,[crp_idadeIdealMesInicio]
           ,[crp_idadeIdealAnoFim]
           ,[crp_idadeIdealMesFim]
           ,[crp_controleTempo]
           ,[crp_qtdeDiasSemana]
           ,[crp_qtdeTemposDia]
           ,[crp_qtdeTemposSemana]
           ,[crp_qtdeHorasDia]
           ,[crp_qtdeMinutosDia]
           ,[crp_qtdeEletivasAlunos]
           ,[crp_ciclo]
           ,[crp_turmaAvaliacao]
           ,[crp_nomeAvaliacao]
           ,[crp_concluiNivelEnsino]
           ,[crp_situacao]
           ,[crp_dataCriacao]
           ,[crp_dataAlteracao]
           ,[tci_id]
           ,[crp_fundoFrente]
           ,[tcp_id])
     VALUES
           (146
           ,1 
           ,5
           ,null
           ,5
           ,'Infantil I Especial'
           ,null
           ,NULL
           ,NULL
           ,NULL
           ,1
           ,5
           ,4
           ,20
           ,NULL
           ,NULL
           ,25
           ,'Infantil Especial'
           ,0
           ,NULL
           , 0 --<crp_concluiNivelEnsino, bit,>
           ,1
           ,GETDATE()
           ,GETDATE()
           ,12
           , NULL
           ,103);



if @@ERROR <> 0
begin
   PRINT 'Erro na criacao de periodos para os novos curriculos'
   ROLLBACK
   RETURN
end;
ENABLE TRIGGER TRG_ACA_CurriculoPeriodo_Identity on ACA_CurriculoPeriodo;