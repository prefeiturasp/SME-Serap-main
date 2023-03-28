use CoreSSO

IF EXISTS (SELECT 1 FROM SYS_Modulo sm  WHERE sm.sis_id = 204 and sm.mod_nome = 'Cadastrar novo item')  
BEGIN  
	UPDATE SYS_Modulo 
	SET mod_nome = 'Banco de itens (Novo)',
		mod_descricao = null,
		mod_dataAlteracao = CURRENT_TIMESTAMP 
	WHERE sis_id = '204'
	and mod_nome = 'Cadastrar novo item'
END  
ELSE  
BEGIN  
	INSERT INTO SYS_Modulo values (204,
		(select MAX(mod_id) + 1 from SYS_Modulo where sis_id = 204),
		'Banco de itens (Novo)',
		null,
		14,
		0,
		1,
		CURRENT_TIMESTAMP,
		CURRENT_TIMESTAMP
	)
END  