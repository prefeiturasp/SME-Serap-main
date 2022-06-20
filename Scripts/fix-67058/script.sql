
delete from TestCurriculumGrade where Test_Id = 997 and TypeCurriculumGradeId = 107
delete from TestCurriculumGrade where TypeCurriculumGradeId = 107
insert into TestCurriculumGrade (TypeCurriculumGradeId,CreateDate,UpdateDate,[State],Test_Id) values (64,getdate(),getdate(),1,997)
insert into TestCurriculumGrade (TypeCurriculumGradeId,CreateDate,UpdateDate,[State],Test_Id) values (64,getdate(),getdate(),1,990)
insert into TestCurriculumGrade (TypeCurriculumGradeId,CreateDate,UpdateDate,[State],Test_Id) values (64,getdate(),getdate(),1,987)
insert into TestCurriculumGrade (TypeCurriculumGradeId,CreateDate,UpdateDate,[State],Test_Id) values (64,getdate(),getdate(),1,991)
insert into TestCurriculumGrade (TypeCurriculumGradeId,CreateDate,UpdateDate,[State],Test_Id) values (64,getdate(),getdate(),1,994)

update TestTypeCourseCurriculumGrade set TypeCurriculumGradeId = 64 where id = 1392

insert into SGP_ACA_CurriculoPeriodo values (163,1,3,3,'M - III',1,getdate(),getdate(),64)
delete from SGP_ACA_CurriculoPeriodo where cur_id = 163
