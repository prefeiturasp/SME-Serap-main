
    SELECT uad_sigla, *
                                                FROM SYS_UnidadeAdministrativa
                                                WHERE uad_id IN(
                                                        SELECT TOP 1 uad_id
                                                        FROM SYS_UsuarioGrupoUA
                                                        WHERE gru_id ='66C70452-1A1E-E811-B259-782BCB3D2D76' 
														AND usu_id = '51A7DA57-43B6-E111-B597-00155D02E702');

	update 	SYS_UnidadeAdministrativa set uad_sigla = 'BT'  where uad_id = 'CEFE49B6-16A1-E111-BE16-00155D02E702';									