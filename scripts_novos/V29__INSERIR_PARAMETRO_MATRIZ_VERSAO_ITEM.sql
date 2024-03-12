use GestaoAvaliacao;

insert into [Parameter] ([Key], Value, Description, StartDate, EndDate, CreateDate, UpdateDate, State, EntityId, Obligatory, Versioning, ParameterCategory_Id, ParameterPage_Id, ParameterType_Id)
values ('EVALUATION_MATRIX', 'Matriz', 'Matriz de avaliação', GETDATE(), null, GETDATE(), GETDATE(), 1, '6CF424DC-8EC3-E011-9B36-00155D033206', 1, 1, 1, 2, 1);