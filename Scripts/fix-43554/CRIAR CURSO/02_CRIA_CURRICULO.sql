-- CRIA CURRICULO
USE [GestaoPedagogica]
GO

BEGIN TRAN 
insert into ACA_Curriculo
       (cur_id, crr_id, crr_codigo, crr_regimeMatricula, crr_periodosNormal, crr_diasLetivos, crr_vigenciaInicio, crr_situacao, crr_dataCriacao, crr_dataAlteracao) Values 
	   (146   ,  1    ,   18      ,        1           ,        3          ,       200      ,      '2021-01-01'   ,     1     ,      GETDATE() , GETDATE())

if @@ERROR <> 0
begin
   PRINT 'Erro na criacao de curriculos'
   ROLLBACK
   RETURN
end;

commit