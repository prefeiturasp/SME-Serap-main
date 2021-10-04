

    WITH CteDestinationTurma AS (
        Select * from GestaoAvaliacao_SGP..TUR_Turma
        WHERE tur_id IN ( 
            Select t.tur_id from GestaoAvaliacao_SGP..TUR_Turma t 
            INNER JOIN GestaoAvaliacao_SGP..TUR_TurmaCurriculo tc ON tc.tur_id = t.tur_id
            INNER JOIN GestaoAvaliacao_SGP..ACA_Curso c ON c.cur_id = tc.cur_id 
            where c.tme_id NOT IN (4,6,7,8)
        )
    )
        
    -- TUR_Turma
    MERGE INTO CteDestinationTurma Destino
    USING (select tur.tur_id, tur.esc_id, tur.tur_codigo, tur.tur_descricao, tur.cal_id, trn.ttn_id,
                  tur.tur_situacao, tur.tur_dataCriacao, tur.tur_dataAlteracao, tur.tur_tipo
             from GestaoPedagogica..TUR_Turma tur with (nolock)
                  inner join GestaoPedagogica..ACA_Turno trn with (nolock)
                  on tur.trn_id = trn.trn_id
                  inner join GestaoAvaliacao_SGP..ACA_TipoTurno ttn
                  on trn.ttn_id = ttn.ttn_id
                  inner join GestaoAvaliacao_SGP..ESC_Escola esc
                  on tur.esc_id = esc.esc_id
                  inner join GestaoAvaliacao_SGP..ACA_CalendarioAnual cal
                  on tur.cal_id = cal.cal_id
                  INNER JOIN GestaoAvaliacao_SGP..TUR_TurmaCurriculo tc ON tc.tur_id = tur.tur_id
                  INNER JOIN GestaoAvaliacao_SGP..ACA_Curso c ON c.cur_id = tc.cur_id
            where tur.tur_situacao <> 3
              and trn.trn_situacao <> 3
              and ttn.ttn_situacao <> 3
              and esc.ent_id = '6CF424DC-8EC3-E011-9B36-00155D033206'
              and esc.esc_situacao <> 3
              and cal.cal_ano = 2021
              and cal.ent_id = '6CF424DC-8EC3-E011-9B36-00155D033206'
              and cal.cal_situacao <> 3
              and tur_tipo = 1
              and c.tme_id NOT IN (4,6,7,8)
            group by tur.tur_id, tur.esc_id, tur.tur_codigo, tur.tur_descricao, tur.cal_id, trn.ttn_id,
                  tur.tur_situacao, tur.tur_dataCriacao, tur.tur_dataAlteracao, tur.tur_tipo) Origem
    ON Destino.tur_id = Origem.tur_id
    WHEN MATCHED
         AND ((Destino.tur_codigo COLLATE DATABASE_DEFAULT <> Origem.tur_codigo COLLATE DATABASE_DEFAULT)
               OR
              (Destino.tur_descricao COLLATE DATABASE_DEFAULT <> Origem.tur_descricao COLLATE DATABASE_DEFAULT)
               OR
              (Destino.ttn_id <> Origem.ttn_id)
               OR
              (Destino.tur_situacao <> Origem.tur_situacao) 
              OR
              (Destino.esc_id <> Origem.esc_id))
              
         THEN
         UPDATE SET tur_codigo = Origem.tur_codigo,
                    tur_descricao = Origem.tur_descricao,
                    ttn_id = Origem.ttn_id,
                    tur_situacao = Origem.tur_situacao,
                    tur_dataAlteracao = Origem.tur_dataAlteracao,
                    esc_id = Origem.esc_id,
                    cal_id = Origem.cal_id

 

    WHEN NOT MATCHED THEN
         INSERT (tur_id, esc_id, tur_codigo, tur_descricao, cal_id, ttn_id,
                 tur_situacao, tur_dataCriacao, tur_dataAlteracao, tur_tipo)
         VALUES (Origem.tur_id, Origem.esc_id, Origem.tur_codigo, Origem.tur_descricao, Origem.cal_id, Origem.ttn_id,
                 Origem.tur_situacao, Origem.tur_dataCriacao, Origem.tur_dataAlteracao, Origem.tur_tipo)
    WHEN NOT MATCHED BY SOURCE AND Destino.tur_situacao not in (2,3) THEN -- no caso do status 2, trata-se de turmas que terminaram o ano anterior ativas no SGP e ficaram inativas (situacao = 5 do SGP) e foram acertadas por script
         UPDATE SET tur_situacao = 3,
                    tur_dataAlteracao = GETDATE();



