USE [master]
GO
IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = N'user_gestaoavaliacao')
DROP LOGIN [user_gestaoavaliacao]
GO
CREATE LOGIN [user_gestaoavaliacao] WITH PASSWORD=N'gestaoavaliacao@adm', DEFAULT_DATABASE=[GestaoAvaliacao], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

USE [GestaoAvaliacao]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'user_gestaoavaliacao')
DROP USER [user_gestaoavaliacao]

GO
CREATE USER [user_gestaoavaliacao] FOR LOGIN [user_gestaoavaliacao] 
GO
EXEC sp_addrolemember N'db_datareader', N'user_gestaoavaliacao'
GO
EXEC sp_addrolemember N'db_datawriter', N'user_gestaoavaliacao'
GO

USE [CoreSSO]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'user_gestaoavaliacao')
DROP USER [user_gestaoavaliacao]

GO
CREATE USER [user_gestaoavaliacao] FOR LOGIN [user_gestaoavaliacao] 
GO
EXEC sp_addrolemember N'db_datareader', N'user_gestaoavaliacao'
GO
EXEC sp_addrolemember N'db_datawriter', N'user_gestaoavaliacao'
GO

USE [CoreSSO_Log]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'user_gestaoavaliacao')
DROP USER [user_gestaoavaliacao]

GO
CREATE USER [user_gestaoavaliacao] FOR LOGIN [user_gestaoavaliacao] 
GO
EXEC sp_addrolemember N'db_datareader', N'user_gestaoavaliacao'
GO
EXEC sp_addrolemember N'db_datawriter', N'user_gestaoavaliacao'
GO

