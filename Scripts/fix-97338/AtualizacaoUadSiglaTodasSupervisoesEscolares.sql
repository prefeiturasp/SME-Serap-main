 UPDATE
   UA
SET
    UA.uad_sigla =  SUBSTRING(UA.uad_nome,22,2)
   
FROM
    SYS_UnidadeAdministrativa AS UA
  
   where UA.uad_nome like '%SUPERVISAO ESCOLAR%'
