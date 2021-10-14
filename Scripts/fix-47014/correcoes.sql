 
update SYS_Usuario set usu_senha = 'aVaRuQCBzDg=', usu_criptografia = 1 where usu_id = (select usu_id from SYS_Usuario where usu_login = 'RA3694862')
insert into PES_PessoaDeficiencia values ('ED4DC593-B19E-4C48-BB22-704ADEC7D5CC','D0B500CC-34F7-4777-A399-B22584016386') -- SURDEZ SEVERA/PROFUNDA
insert into PES_PessoaDeficiencia values ('ED4DC593-B19E-4C48-BB22-704ADEC7D5CC','E7CC6BC6-03E0-4434-AAC4-817B464DA059') -- DEFICIENCIA INTELECTUAL
insert into PES_PessoaDeficiencia values ('ED4DC593-B19E-4C48-BB22-704ADEC7D5CC','39E6DC61-2A3B-436E-872D-ABB22AA161E7') -- DEFICIENCIA MULTIPLA
insert into PES_PessoaDeficiencia values ('14B90D00-56F8-4F9D-B846-F951A78D070F','D0B500CC-34F7-4777-A399-B22584016386') -- SURDEZ SEVERA/PROFUNDA
insert into PES_PessoaDeficiencia values ('829A5B3D-2D8E-E311-B1FE-782BCB3D2D76','D0B500CC-34F7-4777-A399-B22584016386') -- SURDEZ SEVERA/PROFUNDA
update GestaoAvaliacao_SGP..MTR_MatriculaTurma set tur_id = 429958, cur_id = 145, crp_id = 1, tcp_id = 38 where alu_id = 1193284 and mtu_situacao = 1 and mtu_dataSaida is null
insert into PES_PessoaDeficiencia values ('B33344E5-85E7-4CD3-A1D8-03E3F31175B8','D0B500CC-34F7-4777-A399-B22584016386') -- SURDEZ SEVERA/PROFUNDA
insert into PES_PessoaDeficiencia values ('3EFF05CF-46C3-4851-9B52-45DF8BC6B475','A991B05D-A078-41E5-AEF2-A33DF5192481') -- SURDEZ LEVE/MODERADA
