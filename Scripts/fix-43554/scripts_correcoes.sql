 
 insert into GestaoPedagogica..MTR_MatriculaTurma values (1662359,	4,	405343,	108,	1,	5,	'2021-02-04',	NULL,	NULL,	NULL,	NULL,	NULL,	1,	GETDATE(),	GETDATE(),	1,	5,	NULL)

 delete from GestaoPedagogica..TUR_TurmaCurriculo where tur_id = 405343

 insert into GestaoPedagogica..TUR_TurmaCurriculo values (405343,108,1,5,1,1,GETDATE(),GETDATE())

 update GestaoPedagogica..MTR_MatriculaTurma set cur_id = 108 where alu_id = 1662359 and tur_id = 405343


 update MTR_MatriculaTurma set cur_id = 108 where alu_id = 1662359 and tur_id = 405343

 update GestaoPedagogica..ACA_AlunoCurriculo set alc_dataSaida = '2021-02-04', alc_situacao = 4 where alu_id = 1662359 and cur_id = 108

 update GestaoPedagogica..ACA_AlunoCurriculo set cur_id = 108 where alu_id = 1662359 and cur_id = 146 and esc_id = 561


insert into ACA_AlunoCurriculo
(alu_id ,alc_id,esc_id,uni_id,cur_id,crr_id,crp_id,alc_matricula,alc_codigoInep,alc_dataPrimeiraMatricula,alc_dataSaida,alc_dataColacao,alc_matriculaEstadual,alc_qtdeImpressoesHistorico,alc_situacao,alc_dataCriacao,alc_dataAlteracao,alc_registroGeral) values
(1662359,5     ,561   ,1     ,108   ,1     ,5     ,'6678520'    ,null          ,'2021-02-04'             ,null         ,null           ,null                 ,null                       ,1           ,GETDATE()      ,GETDATE()        ,null)


 delete from ACA_CurriculoPeriodo where cur_id = 146 and crp_id = 5 and crp_ordem = 5

 insert into TUR_TurmaCurriculo values (405343,108,1,5,1,getdate(),getdate(),103);

 update MTR_MatriculaTurma set mtu_dataSaida = '2021-02-04', mtu_situacao = 5 where alu_id = 1662359 and tur_id = 332603
 update GestaoAvaliacao_SGP..MTR_MatriculaTurma set mtu_dataSaida = '2021-02-04', mtu_situacao = 5 where alu_id = 1662359 and tur_id = 332603

INSERT INTO Mtr_MatriculaTurma 
	  (alu_id ,mtu_id,tur_id,cur_id,crr_id,crp_id,mtu_dataMatricula,mtu_avaliacao,mtu_frequencia,mtu_relatorio,mtu_resultado,mtu_dataSaida,mtu_situacao,mtu_dataCriacao,mtu_dataAlteracao,mtu_numeroChamada,alc_id,usu_idResultado) 
VALUES(1662359,4     ,405343,108   ,1     ,5     ,'2021-02-04'     ,null         ,null          ,null         ,null         ,null         ,1           ,GETDATE()      ,GETDATE()        ,1                ,5     ,null)


 insert into MTR_MatriculaTurma values (1662359,	4,	561,	405343,	108,	1,	5,	1,	GETDATE(),	GETDATE(),	5,	'2021-02-04',	null,	103)

update MTR_MatriculaTurma set mtu_situacao = 5, mtu_dataSaida = '2021-02-04' where alu_id = 2196759 and esc_id = 998 and cur_id = 108
insert into MTR_MatriculaTurma values (2196759,	5,	561,	405343,	108,	1,	5,	1,	GETDATE(),	GETDATE(),	5,	'2021-02-04',	null,	103)
update GestaoPedagogica..MTR_MatriculaTurma set mtu_situacao = 5, mtu_dataSaida = '2021-02-04' where alu_id = 2196759 and cur_id = 108 and tur_id = 336709
insert into GestaoPedagogica..MTR_MatriculaTurma values (2196759,	4,	405343,	108,	1,	5,	'2021-02-04',	NULL,	NULL,	NULL,	NULL,	NULL,	1,	GETDATE(),	GETDATE(),	1,	5,	NULL)


update GestaoPedagogica..ACA_AlunoCurriculo set alc_dataSaida = '2021-02-04', alc_situacao = 4 where alu_id = 2196759 and alc_id = 4
update GestaoPedagogica..ACA_AlunoCurriculo set alc_dataPrimeiraMatricula = '2021-02-04', crp_id = 5, esc_id = 561 where alu_id = 2196759 and alc_id = 5

 insert into GestaoAvaliacao_SGP..TUR_TurmaCurriculo values (428690,130,1,7,1,GETDATE(),GETDATE(),72)

use CoreSSO

--RA7747646
update SYS_Usuario set usu_senha = 'DZQuStWqpPY=' where usu_id = '27DD7FBE-5C9E-EB11-9CEB-782BCB3D2D76'

--RA7613752
update SYS_Usuario set usu_senha = 'PIpjaV+2h2Y=', usu_criptografia = 1 where usu_id = (select usu_id from SYS_Usuario where usu_login = 'RA7613752')

update SYS_Usuario set usu_senha = 'vutV6DnCC1Y=', usu_criptografia = 1 where usu_id = 'B7F45CFA-DFEA-E611-9541-782BCB3D218E'

-- bryan
insert into PES_PessoaDeficiencia values ('F4055727-33AF-4E50-B99B-6506EAA667BE','F28D3D1C-6276-4CEF-BD61-11984FE91B29')

-- David
insert into PES_PessoaDeficiencia values ('E209CED2-A88D-483B-872D-D9289E8E10CC','A991B05D-A078-41E5-AEF2-A33DF5192481')
insert into PES_PessoaDeficiencia values ('E209CED2-A88D-483B-872D-D9289E8E10CC','D41C5E1B-4C69-4301-8BA6-3A152AE975B5')

-- Caique
insert into PES_PessoaDeficiencia values ('3A51C496-2957-439C-9242-70E85DD80A46','D41C5E1B-4C69-4301-8BA6-3A152AE975B5')

-- Alicio
insert into PES_PessoaDeficiencia values ('FCD179FB-8839-40E2-8120-C7595200C74F','A991B05D-A078-41E5-AEF2-A33DF5192481')

-- Stehany
 insert into PES_PessoaDeficiencia values ('71D8174E-6454-40DF-9890-DC6C1940433E','A991B05D-A078-41E5-AEF2-A33DF5192481')
 insert into PES_PessoaDeficiencia values ('71D8174E-6454-40DF-9890-DC6C1940433E','D41C5E1B-4C69-4301-8BA6-3A152AE975B5')

-- Ingrid
insert into PES_PessoaDeficiencia values ('1CA39D35-7183-4FAC-ACC3-30813419CBED','A991B05D-A078-41E5-AEF2-A33DF5192481')

-- Renan
 insert into PES_PessoaDeficiencia values ('81F2F213-97A0-49B2-BF01-FC0FAAE5D46C','A991B05D-A078-41E5-AEF2-A33DF5192481')
 insert into PES_PessoaDeficiencia values ('81F2F213-97A0-49B2-BF01-FC0FAAE5D46C','D41C5E1B-4C69-4301-8BA6-3A152AE975B5')

-- GUILHERMY SANTOS SILVA
 insert into PES_PessoaDeficiencia values ('F385393A-2D8E-E311-B1FE-782BCB3D2D76','A991B05D-A078-41E5-AEF2-A33DF5192481')
 insert into PES_PessoaDeficiencia values ('F385393A-2D8E-E311-B1FE-782BCB3D2D76','D41C5E1B-4C69-4301-8BA6-3A152AE975B5')

-- Mellany dos Santos Lima
 insert into PES_PessoaDeficiencia values ('87EFDCDA-AAC2-4A38-8744-BDCA274FB862','A991B05D-A078-41E5-AEF2-A33DF5192481')
 insert into PES_PessoaDeficiencia values ('87EFDCDA-AAC2-4A38-8744-BDCA274FB862','D41C5E1B-4C69-4301-8BA6-3A152AE975B5')





