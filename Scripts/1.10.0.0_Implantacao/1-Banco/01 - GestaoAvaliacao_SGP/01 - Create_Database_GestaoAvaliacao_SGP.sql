DECLARE @databaseName VARCHAR(120)

-- Banco de dados do Sistema
SET @databaseName = 'GestaoAvaliacao_SGP'

--------------------------------------------------
-- Caminho do arquivo do banco de dados			--
-- Por Padrão é pego o caminho do bd CoreSSO.	--
--------------------------------------------------                  
DECLARE @data_path nvarchar(512);
DECLARE @data_path_log nvarchar(512);
SET @data_path = (SELECT SUBSTRING(physical_name, 1, CHARINDEX(N'CoreSSO.mdf', LOWER(physical_name)) - 1)
					FROM master.sys.master_files
					WHERE database_id = DB_ID('CoreSSO') AND file_id = 1); 
SET @data_path_log = @data_path

IF DB_ID (@databaseName) IS NOT NULL 
BEGIN
	--EXECUTE('DROP DATABASE ' + @databaseName)
	--PRINT 'Banco de dados ' + @databaseName + ' excluído.' 
	PRINT 'Banco de dados já existente.'
END
ELSE
BEGIN
	PRINT 'Criando banco de dados: '+ @databaseName + '...'
	EXECUTE
	(
	'CREATE DATABASE '+ @databaseName + ' ON  PRIMARY
	( NAME = '+ @databaseName +', FILENAME = '''+ @data_path + @databaseName 
	+ '.mdf'', SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
	 LOG ON 
	( NAME = '+ @databaseName +'_log, FILENAME = '''+ @data_path_log + @databaseName 
	+ '_log.ldf'' , SIZE = 1024KB , FILEGROWTH = 10% ) 
	 ALTER DATABASE '+ @databaseName +' SET RECOVERY FULL 
	 ALTER DATABASE '+ @databaseName +' COLLATE Latin1_General_CI_AS
	' )
END

IF DB_ID (@databaseName) IS NOT NULL
BEGIN
	PRINT 'Banco de dados: ' + @databaseName + ' criado com sucesso.'
	EXECUTE('ALTER DATABASE ' + @databaseName + ' SET TRUSTWORTHY ON')
	PRINT 'Banco de dados: ' + @databaseName + ' configurado com sucesso.'
END