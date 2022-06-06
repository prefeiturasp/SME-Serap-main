use GestaoAvaliacao_SGP

insert into ACA_Curriculo values (161,1,null,1,getdate(),getdate())
insert into ACA_CurriculoPeriodo
select
161 cur_id,
crr_id,
crp_id,
crp_ordem,
crp_descricao,
crp_situacao,
getdate() crp_dataCriacao,
getdate() crp_dataAlteracao,
tcp_id
from ACA_CurriculoPeriodo
where cur_id = 139