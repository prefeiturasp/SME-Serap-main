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
    public class EvaluationMatrixCourseBusiness : IEvaluationMatrixCourseBusiness
	{
		private readonly IEvaluationMatrixCourseRepository evaluationMatrixCourseRepository;       

        public EvaluationMatrixCourseBusiness(IEvaluationMatrixCourseRepository evaluationMatrixCourseRepository)
		{
			this.evaluationMatrixCourseRepository = evaluationMatrixCourseRepository;
		}

		#region Custom

		private Validate Validate(EvaluationMatrixCourse entity, ValidateAction action, Validate valid)
		{
			valid.Message = null;
			if (action == ValidateAction.Save)
			{
				if (entity == null || entity.CourseId < 0 || entity.EvaluationMatrix == null || entity.EvaluationMatrixCourseCurriculumGrades == null)
					valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

				if (evaluationMatrixCourseRepository.ExistCourse(entity.CourseId, entity.ModalityId, entity.EvaluationMatrix.Id))
					valid.Message += "<br/>Esse curso já foi associado a essa matriz de avaliação.";
			}

			if (action == ValidateAction.Update)
			{
				EvaluationMatrixCourse ent = Get(entity.Id);
				if (ent == null)
				{
					valid.Message = "Não foi encontrado o curso a ser atualizado.";
					valid.Code = 404;
				}

				if (entity.CourseId < 0 || entity.EvaluationMatrix == null || entity.EvaluationMatrixCourseCurriculumGrades == null)
					valid.Message += "<br/>Não foram preenchidos todos os campos obrigatórios.";
			}

			if (action == ValidateAction.Delete)
			{
				EvaluationMatrixCourse ent = Get(entity.Id);
				if (ent == null)
				{
					valid.Message = "Não foi encontrado o curso a ser excluído.";
					valid.Code = 404;
				}

				if (evaluationMatrixCourseRepository.ExistsItemAndLastCourse(entity.Id, entity.EvaluationMatrix.Id))
					valid.Message += "<br/>Não foi possível excluir esse curso, pois existem um ou mais itens vinculados ao(s) ano(s) desse curso.";
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

		public EvaluationMatrixCourse Get(long id)
		{
			return evaluationMatrixCourseRepository.Get(id);
		}

		public IEnumerable<EvaluationMatrixCourse> Load(ref Pager pager)
		{
			return evaluationMatrixCourseRepository.Load(ref pager);
		}

		public IEnumerable<EvaluationMatrixCourse> Search(int evaluationMatrixId, ref Pager pager)
		{
			return evaluationMatrixCourseRepository.Search(evaluationMatrixId, ref pager);
		}

		#endregion

		#region Write

		public EvaluationMatrixCourse Save(EvaluationMatrixCourse entity)
		{
			entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
			if (entity.Validate.IsValid)
			{
                entity.CreateDate = DateTime.Now;
                entity.UpdateDate = DateTime.Now;
                entity.State = Convert.ToByte(EnumState.ativo);
                entity.EvaluationMatrix_Id = entity.EvaluationMatrix.Id;
                entity.EvaluationMatrix = null;
                entity.EvaluationMatrixCourseCurriculumGrades = entity.EvaluationMatrixCourseCurriculumGrades.OrderBy(a => a.Ordem).ToList();

				entity = evaluationMatrixCourseRepository.Save(entity);
				entity.Validate.Type = ValidateType.Save.ToString();
				entity.Validate.Message = "Curso salvo com sucesso.";
			}

			return entity;
		}

		public EvaluationMatrixCourse Update(long id, EvaluationMatrixCourse entity)
		{
			entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);
			if (entity.Validate.IsValid)
			{
                EvaluationMatrixCourse evaluationMatrixCourse = evaluationMatrixCourseRepository.Get(id);
                if (entity.CourseId > 0)
                    evaluationMatrixCourse.CourseId = entity.CourseId;
                evaluationMatrixCourse.UpdateDate = DateTime.Now;

                evaluationMatrixCourseRepository.Update(evaluationMatrixCourse);
                entity = evaluationMatrixCourse;
				entity.Validate.Type = ValidateType.Update.ToString();
				entity.Validate.Message = "Curso alterado com sucesso.";
			}

			return entity;
		}

		public EvaluationMatrixCourse Delete(long id, long evaluationMatrixId)
		{
			EvaluationMatrixCourse entity = new EvaluationMatrixCourse { Id = id };
			entity.EvaluationMatrix = new EvaluationMatrix() { Id = evaluationMatrixId };

			entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
			if (entity.Validate.IsValid)
			{
				evaluationMatrixCourseRepository.Delete(entity);
				entity.Validate.Type = ValidateType.Delete.ToString();
				entity.Validate.Message = "Curso excluído com sucesso.";
			}

			return entity;
		}

		#endregion
	}
}
