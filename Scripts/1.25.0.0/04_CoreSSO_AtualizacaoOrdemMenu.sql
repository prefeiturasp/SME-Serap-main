USE [CoreSSO]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
	
	DECLARE @sis_id INT = (SELECT sis_id FROM SYS_Sistema WITH(NOLOCK) WHERE sis_nome = 'SERAp' AND sis_situacao <> 3)

	DECLARE @mod_idModelo INT 
	DECLARE @ordemModelo INT 
	DECLARE @mod_idTipo INT 
	DECLARE @ordemTipo INT 

	DECLARE @dados AS TABLE (
		sis_id INT NOT NULL,
		mod_id INT NOT NULL,
		vis_id INT NOT NULL,
		ordem INT NOT NULL,
		ordemNova INT NULL
	)

	--SELECT @mod_idModelo = M.mod_id, @ordemModelo = Vmm.vmm_ordem 
	INSERT INTO @dados (sis_id, mod_id, vis_id, ordem)
	SELECT M.sis_id, M.mod_id, Vmm.vis_id, Vmm.vmm_ordem
	FROM 
		SYS_Modulo AS M WITH(NOLOCK)
		INNER JOIN SYS_VisaoModulo Vm WITH(NOLOCK)
			ON M.sis_id = Vm.sis_id
			AND M.mod_id = Vm.mod_id
		INNER JOIN SYS_VisaoModuloMenu Vmm WITH(NOLOCK)
			ON Vm.sis_id = Vmm.sis_id
			AND Vm.mod_id = Vmm.mod_id
			AND Vm.vis_id = Vmm.vis_id
	WHERE 
		M.sis_id = @sis_id
		AND mod_nome IN ('Modelo de prova', 'Tipo de prova')
	ORDER BY
		M.mod_id, Vmm.vis_id

	UPDATE D1 SET D1.ordemNova = D2.ordem
	FROM 
		@dados D1
		INNER JOIN @dados D2
			ON D1.sis_id = D2.sis_id
			AND D1.mod_id <> D2.mod_id
			AND D1.vis_id = D2.vis_id


	UPDATE Vmm SET Vmm.vmm_ordem = D.ordemNova
	FROM 
		@dados D
		INNER JOIN SYS_VisaoModuloMenu AS Vmm 
			ON D.sis_id = Vmm.sis_id
			AND D.mod_id = Vmm.mod_id
			AND D.vis_id = Vmm.vis_id

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION