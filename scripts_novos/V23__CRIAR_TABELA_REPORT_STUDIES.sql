Use GestaoAvaliacao

CREATE TABLE ReportsStudies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255),
    TypeGroup INT,
    Addressee VARCHAR(255),
    Link VARCHAR(500),
    CreateDate DATETIME,
	UpdateDate DATETIME,   
    State TinyInt
);