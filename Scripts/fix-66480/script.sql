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

insert into ACA_CurriculoPeriodo values (163,1,7,7,'M III',1,getdate(),getdate(),107)
insert into ACA_Curriculo values (163,1,null,1,getdate(),getdate())

begin tran
update cp
set cp.crp_ordem = _tcp.tcp_ordem,
cp.crp_id = _tcp.tcp_ordem
from ACA_CurriculoPeriodo cp
inner join ACA_TipoCurriculoPeriodo _tcp on cp.tcp_id = _tcp.tcp_id 
where cp.cur_id = 161 and cp.crp_situacao = 1
and _tcp.tcp_situacao = 1
commit

insert into ACA_Curriculo values (161,1,null,1,getdate(),getdate())
insert into ACA_CurriculoPeriodo


insert into ACA_CurriculoPeriodo values 
(161,1,10,10,'EJA COMPLEMENTAR II',1,getdate(),getdate(),117)
(161,1,9,9,'EJA COMPLEMENTAR I',1,getdate(),getdate(),123)
(161,1,2,2,'EJA FINAL I',1,getdate(),getdate(),124)
(161,1,6,6,'EJA FINAL II',1,getdate(),getdate(),125)

update ACA_TipoCurriculoPeriodo 
set tcp_situacao = 1
where 
tcp_id = 107

update ACA_TipoCurriculoPeriodo 
set tcp_situacao = 1
where 
tcp_id in (130,110,124,121,118,129,125,122,109,123,117)





