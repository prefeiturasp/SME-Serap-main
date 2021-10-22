--ALTER TABLE Test DROP CONSTRAINT [FK_dbo.Test_dbo.TestTime_TestTime_Id]
--DROP TABLE TestTime

CREATE TABLE TestTime(
Id bigint NOT NULL PRIMARY KEY,
Description VARCHAR(100) NOT NULL,
Segundos int NOT NULL,
CreateDate DATETIME NOT NULL,
UpdateDate DATETIME NOT NULL,
State TINYINT NOT NULL
)

INSERT INTO TestTime VALUES(1,'Sem limite',0,GETDATE(),GETDATE(),1);
INSERT INTO TestTime VALUES(2,'Uma hora',3600,GETDATE(),GETDATE(),1);
INSERT INTO TestTime VALUES(3,'Uma hora e meia',5400,GETDATE(),GETDATE(),1);
INSERT INTO TestTime VALUES(4,'Duas horas',7200,GETDATE(),GETDATE(),1);
INSERT INTO TestTime VALUES(5,'Duas horas e meia',9000,GETDATE(),GETDATE(),1);
INSERT INTO TestTime VALUES(6,'Três horas',10800,GETDATE(),GETDATE(),1);
INSERT INTO TestTime VALUES(7,'Três horas e meia',12600,GETDATE(),GETDATE(),1);
select * from TestTime
select * from Test


ALTER TABLE Test ADD TestTime_Id bigint null
update Test set TestTime_Id=1
ALTER TABLE Test ALTER COLUMN TestTime_Id bigint not null


ALTER TABLE [dbo].[Test]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Test_dbo.TestTime_TestTime_Id] FOREIGN KEY([TestTime_Id])
REFERENCES [dbo].[TestTime] ([Id])