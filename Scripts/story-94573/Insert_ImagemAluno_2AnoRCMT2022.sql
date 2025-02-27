 INSERT INTO   PROVASP..IMAGEMALUNO 

    select            '2022' as edicao,
                        3 as AreaConhecimentoId, 
		                REPLICATE('0', 6 - LEN(insc.esc_eol)) + RTrim(insc.esc_eol) as esc_codigo,
		                alu.alu_matricula, 
		                 questao,
		                pagina, 
		                alu.alu_nome,
                       'Imagens/2022/'+'2AnoRC'+'/'+insc.tipo_esc+ '/' + insc.DRE + '/' +
					   insc.esc_eol + '/' + insc.ano_escolar + '/' + turma + '/' +
					   insc.nome_arquivo as Caminho
		 from Manutencao..[base_psp_MT_2ano - recorte] insc   -- 213876
         inner join    GestaoAvaliacao_SGP..[ACA_Aluno] as alu on alu.alu_matricula = insc.eol_aluno-- 235943
		 inner join ProvaSP..Escola e on REPLICATE('0', 6 - LEN(insc.esc_eol)) + RTrim(insc.esc_eol)  = e.esc_codigo 