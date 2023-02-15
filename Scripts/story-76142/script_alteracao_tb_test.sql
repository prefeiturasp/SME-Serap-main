IF COL_LENGTH('Test', 'ProvaComProficiencia') IS NULL
    alter table Test add ProvaComProficiencia [bit] NOT NULL default 0;
IF COL_LENGTH('Test', 'ApresentarResultados') IS NULL
    alter table Test add ApresentarResultados [bit] NOT NULL default 0;
IF COL_LENGTH('Test', 'ApresentarResultadosPorItem') IS NULL
    alter table Test add ApresentarResultadosPorItem [bit] NOT NULL default 0;