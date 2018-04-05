using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class TestTypeCourseBusiness : ITestTypeCourseBusiness
    {
        private readonly ITestTypeCourseRepository testTypeCourseRepository;
        private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;       

        public TestTypeCourseBusiness(ITestTypeCourseRepository testTypeCourseRepository, ITestCurriculumGradeBusiness testCurriculumGradeBusiness)
        {
            this.testTypeCourseRepository = testTypeCourseRepository;
            this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
        }

        #region Custom

        private Validate Validate(TestTypeCourse entity, int modalityId, long testTypeId, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || entity.CourseId < 0 || entity.TestType == null || entity.TestTypeCourseCurriculumGrades == null)
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
                
                if (testTypeCourseRepository.ExistCourse(entity.CourseId, modalityId, entity.TestType.Id))
                    valid.Message += "<br/>Esse curso já foi associado a esse tipo de prova.";
            }

            if (action == ValidateAction.Update)
            {
                TestTypeCourse ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o curso a ser atualizado.";
                    valid.Code = 404;
                }

                if (entity.CourseId < 0 || entity.TestType == null || entity.TestTypeCourseCurriculumGrades == null)
                    valid.Message += "<br/>Não foram preenchidos todos os campos obrigatórios.";
            }

            if (action == ValidateAction.Delete)
            {
                TestTypeCourse ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o curso a ser excluído.";
                    valid.Code = 404;
                }

                IEnumerable<long> listTypeCurriculumGradeId = entity.TestTypeCourseCurriculumGrades.Select(p => (long)p.TypeCurriculumGradeId);
                if (testCurriculumGradeBusiness.ExistsTestCourse(listTypeCurriculumGradeId, testTypeId))
                    valid.Message += "<br/>Não foi possível excluir esse curso, pois existem uma ou mais provas vinculadas ao(s) ano(s) desse curso.";
            }

            if (!string.IsNullOrEmpty(valid.Message))
            {
                string br = "<br/>";
                valid.Message = valid.Message.TrimStart(br.ToCharArray());

                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }

        #endregion

        #region Read

        public IEnumerable<TestTypeCourse> Search(int evaluationMatrixId, ref Pager pager)
        {
            return testTypeCourseRepository.Search(evaluationMatrixId, ref pager);
        }

        public TestTypeCourse Get(long id)
        {
            return testTypeCourseRepository.Get(id);
        }

        public IEnumerable<TestTypeCourse> Load(ref Pager pager)
        {
            return testTypeCourseRepository.Load(ref pager);
        }

        #endregion

        #region Write

        public TestTypeCourse Save(TestTypeCourse entity, int TypeLevelEducationId, int ModalityId)
        {
            entity.Validate = Validate(entity, ModalityId, 0, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.CreateDate = DateTime.Now;
                entity.UpdateDate = DateTime.Now;
                entity.State = Convert.ToByte(EnumState.ativo);
                entity.TestType_Id = entity.TestType.Id;
                entity.TestType = null;
                entity.TestTypeCourseCurriculumGrades = entity.TestTypeCourseCurriculumGrades.OrderBy(a => a.Ordem).ToList();

                entity = testTypeCourseRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Curso salvo com sucesso.";
            }

            return entity;
        }

        public TestTypeCourse Update(long id, TestTypeCourse entity)
        {
            entity.Validate = Validate(entity, 0, 0, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                TestTypeCourse testTypeCourse =  testTypeCourseRepository.Get(id);
                if (entity.CourseId > 0)
                    testTypeCourse.CourseId = entity.CourseId;
                testTypeCourse.UpdateDate = DateTime.Now;
                testTypeCourseRepository.Update(testTypeCourse);
                entity = testTypeCourse;
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Curso alterado com sucesso.";
            }

            return entity;            
        }

        public TestTypeCourse Delete(long id, long testTypeId)
        {
            TestTypeCourse entity = new TestTypeCourse { Id = id };
            entity.Validate = Validate(entity, 0, testTypeId, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                testTypeCourseRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Curso excluído com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
