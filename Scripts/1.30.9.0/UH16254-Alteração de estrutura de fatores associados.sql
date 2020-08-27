-- 1.Constructo
ALTER TABLE Constructo ADD Referencia VARCHAR(100) NULL;
ALTER TABLE Constructo ADD AnoEscolar INT NULL;
GO

-- 2. FatoresAssociadosRespostasSME
ALTER TABLE FatorAssociadoQuestionarioRespostaSME ADD FatorAssociadoQuestionarioRespostaSMEIdAux INT NOT NULL IDENTITY(1,1);
ALTER TABLE FatorAssociadoQuestionarioRespostaSME ADD FatorAssociadoQuestionarioRespostaSMEId INT NULL; -- <- PK
GO
UPDATE FatorAssociadoQuestionarioRespostaSME SET FatorAssociadoQuestionarioRespostaSMEId = FatorAssociadoQuestionarioRespostaSMEIdAux;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaSME ALTER COLUMN FatorAssociadoQuestionarioRespostaSMEId INT NOT NULL;
ALTER TABLE FatorAssociadoQuestionarioRespostaSME DROP COLUMN FatorAssociadoQuestionarioRespostaSMEIdAux;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaSME ADD AnoEscolar INT NOT NULL CONSTRAINT DF_AnoEscolar_FatorAssociadoQuestionarioRespostaSME DEFAULT (0);
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaSME DROP CONSTRAINT DF_AnoEscolar_FatorAssociadoQuestionarioRespostaSME;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaSME DROP CONSTRAINT PK_FatorAssociadoQuestionarioRespostaSME;
ALTER TABLE FatorAssociadoQuestionarioRespostaSME ADD CONSTRAINT PK_FatorAssociadoQuestionarioRespostaSME 
	PRIMARY KEY CLUSTERED (FatorAssociadoQuestionarioRespostaSMEId);
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaSME ADD CONSTRAINT UK_FatorAssociadoQuestionarioRespostaSME UNIQUE (Edicao, CicloId, AnoEscolar, FatorAssociadoQuestionarioId, VariavelId, ItemId);
GO

-- 2.1 Relacionamento 1 -> n com Constructo
CREATE TABLE FatorAssociadoQuestionarioRespostaSMEConstructo 
(
	FatorAssociadoQuestionarioRespostaSMEId INT NOT NULL, 
	ConstructoId INT NOT NULL,
	PRIMARY KEY (FatorAssociadoQuestionarioRespostaSMEId, ConstructoId),
	FOREIGN KEY (FatorAssociadoQuestionarioRespostaSMEId) REFERENCES FatorAssociadoQuestionarioRespostaSME(FatorAssociadoQuestionarioRespostaSMEId),
	FOREIGN KEY (ConstructoId) REFERENCES Constructo(ConstructoId)
);
GO

INSERT INTO FatorAssociadoQuestionarioRespostaSMEConstructo
SELECT FatorAssociadoQuestionarioRespostaSMEId, ConstructoId FROM FatorAssociadoQuestionarioRespostaSME WHERE ConstructoId IS NOT NULL;
GO

ALTER TABLE FatorAssociadoQuestionarioRespostaSME DROP CONSTRAINT FK_FatorAssociadoQuestionarioRespostaSME_Constructo;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaSME DROP COLUMN ConstructoId;
GO


-- 3. FatorAssociadoQuestionarioRespostaDRE
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE ADD FatorAssociadoQuestionarioRespostaDREIdAux INT NOT NULL IDENTITY(1,1);
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE ADD FatorAssociadoQuestionarioRespostaDREId INT NULL; -- <- PK
GO
UPDATE FatorAssociadoQuestionarioRespostaDRE SET FatorAssociadoQuestionarioRespostaDREId = FatorAssociadoQuestionarioRespostaDREIdAux;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE ALTER COLUMN FatorAssociadoQuestionarioRespostaDREId INT NOT NULL;
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE DROP COLUMN FatorAssociadoQuestionarioRespostaDREIdAux;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE ADD AnoEscolar INT NOT NULL CONSTRAINT DF_AnoEscolar_FatorAssociadoQuestionarioRespostaDRE DEFAULT (0);
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE DROP CONSTRAINT DF_AnoEscolar_FatorAssociadoQuestionarioRespostaDRE;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE DROP CONSTRAINT PK_FatorAssociadoQuestionarioRespostaDRE;
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE ADD CONSTRAINT PK_FatorAssociadoQuestionarioRespostaDRE 
	PRIMARY KEY CLUSTERED (FatorAssociadoQuestionarioRespostaDREId);
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE ADD CONSTRAINT UK_FatorAssociadoQuestionarioRespostaDRE 
UNIQUE (Edicao, CicloId, AnoEscolar, FatorAssociadoQuestionarioId, VariavelId, ItemId, uad_sigla);
GO

-- 3.1 Relacionamento 1 -> n com Constructo
CREATE TABLE FatorAssociadoQuestionarioRespostaDREConstructo 
(
	FatorAssociadoQuestionarioRespostaDREId INT NOT NULL, 
	ConstructoId INT NOT NULL,
	PRIMARY KEY (FatorAssociadoQuestionarioRespostaDREId, ConstructoId),
	FOREIGN KEY (FatorAssociadoQuestionarioRespostaDREId) REFERENCES FatorAssociadoQuestionarioRespostaDRE(FatorAssociadoQuestionarioRespostaDREId),
	FOREIGN KEY (ConstructoId) REFERENCES Constructo(ConstructoId)
);
GO

INSERT INTO FatorAssociadoQuestionarioRespostaDREConstructo
SELECT FatorAssociadoQuestionarioRespostaDREId, ConstructoId FROM FatorAssociadoQuestionarioRespostaDRE WHERE ConstructoId IS NOT NULL;
GO

ALTER TABLE FatorAssociadoQuestionarioRespostaDRE DROP CONSTRAINT FK_FatorAssociadoQuestionarioRespostaDRE_Constructo;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaDRE DROP COLUMN ConstructoId;
GO


-- 4. FatorAssociadoQuestionarioRespostaEscola
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola ADD FatorAssociadoQuestionarioRespostaEscolaIdAux INT NOT NULL IDENTITY(1,1);
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola ADD FatorAssociadoQuestionarioRespostaEscolaId INT NULL; -- <- PK
GO
UPDATE FatorAssociadoQuestionarioRespostaEscola SET FatorAssociadoQuestionarioRespostaEscolaId = FatorAssociadoQuestionarioRespostaEscolaIdAux;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola ALTER COLUMN FatorAssociadoQuestionarioRespostaEscolaId INT NOT NULL;
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola DROP COLUMN FatorAssociadoQuestionarioRespostaEscolaIdAux;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola ADD AnoEscolar INT NOT NULL CONSTRAINT DF_AnoEscolar_FatorAssociadoQuestionarioRespostaEscola DEFAULT (0);
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola DROP CONSTRAINT DF_AnoEscolar_FatorAssociadoQuestionarioRespostaEscola;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola DROP CONSTRAINT PK_FatorAssociadoQuestionarioRespostaEscola;
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola ADD CONSTRAINT PK_FatorAssociadoQuestionarioRespostaEscola 
	PRIMARY KEY CLUSTERED (FatorAssociadoQuestionarioRespostaEscolaId);
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola ADD CONSTRAINT UK_FatorAssociadoQuestionarioRespostaEscola 
UNIQUE (Edicao, CicloId, AnoEscolar, FatorAssociadoQuestionarioId, VariavelId, ItemId, uad_sigla, esc_codigo);
GO

-- 4.1 Relacionamento 1 -> n com Constructo
CREATE TABLE FatorAssociadoQuestionarioRespostaEscolaConstructo 
(
	FatorAssociadoQuestionarioRespostaEscolaId INT NOT NULL, 
	ConstructoId INT NOT NULL,
	PRIMARY KEY (FatorAssociadoQuestionarioRespostaEscolaId, ConstructoId),
	FOREIGN KEY (FatorAssociadoQuestionarioRespostaEscolaId) REFERENCES FatorAssociadoQuestionarioRespostaEscola(FatorAssociadoQuestionarioRespostaEscolaId),
	FOREIGN KEY (ConstructoId) REFERENCES Constructo(ConstructoId)
);
GO

INSERT INTO FatorAssociadoQuestionarioRespostaEscolaConstructo
SELECT FatorAssociadoQuestionarioRespostaEscolaId, ConstructoId FROM FatorAssociadoQuestionarioRespostaEscola WHERE ConstructoId IS NOT NULL;
GO

ALTER TABLE FatorAssociadoQuestionarioRespostaEscola DROP CONSTRAINT FK_FatorAssociadoQuestionarioRespostaEscola_Constructo;
GO
ALTER TABLE FatorAssociadoQuestionarioRespostaEscola DROP COLUMN ConstructoId;
GO

-- 5. Index
CREATE INDEX idx_Aluno_Matricula_Edicao ON Aluno (Edicao, alu_matricula) INCLUDE (AnoEscolar);

