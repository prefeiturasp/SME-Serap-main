
-- SGP
UPDATE SYS_Sistema
SET sis_caminho = '$UrlSerapLogin$', sis_caminhoLogout = '$UrlSerapLogout$', sis_situacao = 1
WHERE sis_id = 204

-- Boletim Online
UPDATE SYS_Sistema
SET sis_caminho = '$UrlOmrLogin$', sis_caminhoLogout = '$UrlOmrLogout$', sis_situacao = 1
WHERE sis_id = 217