using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
	public class DisciplineBusiness : IDisciplineBusiness
	{
		private readonly IDisciplineRepository disciplineRepository;
		private readonly IACA_TipoDisciplinaRepository disciplineTypeRepository;

		public DisciplineBusiness(IDisciplineRepository disciplineRepository, IACA_TipoDisciplinaRepository disciplineTypeRepository)
		{
			this.disciplineRepository = disciplineRepository;
			this.disciplineTypeRepository = disciplineTypeRepository;
		}

		#region Custom

		private Validate Validate(Discipline entity, ValidateAction action, Validate valid)
		{
			valid.Message = null;
			if ((action == ValidateAction.Save) && (entity == null || string.IsNullOrEmpty(entity.Description) || (entity.DisciplineTypeId < 0)))
			{
				valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
			}

			if (action == ValidateAction.Delete)
			{
				Discipline ent = Get(entity.Id);
				if (ent == null)
				{
					valid.Message = "Não foi encontrada a disciplina a ser excluída.";
					valid.Code = 404;
				}

				if (disciplineRepository.ExistsMatrix(entity.Id))
					valid.Message += "<br/>Não é possível excluir esta disciplina, pois a mesma esta sendo usada em uma matriz de avaliação.";
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

		public Discipline Get(long id)
		{
			return disciplineRepository.Get(id);
		}

		public IEnumerable<Discipline> Load(ref Pager pager, Guid EntityId)
		{
			return disciplineRepository.Load(EntityId, ref pager);
		}

		public IEnumerable<Discipline> LoadCustom(Guid EntityId)
		{
			return disciplineRepository.LoadCustom(EntityId);
		}

		public IEnumerable<Discipline> SearchDisciplinesSaves(int typeLevelEducation, Guid EntityId)
		{
			IEnumerable<Discipline> disciplines = disciplineRepository.LoadCustom(EntityId);

			var result = from c in disciplines
						 where c.TypeLevelEducationId == typeLevelEducation
						 select c;

			return result;
		}

		public IEnumerable<Discipline> SearchAllDisciplines(Guid EntityId)
		{
			IEnumerable<Discipline> disciplines = disciplineRepository.SearchAllDisciplines(EntityId);

			return disciplines;
		}

		public IEnumerable<Discipline> LoadComboHasMatrix(Guid entityId)
		{
			return disciplineRepository.LoadComboHasMatrix(entityId);
		}

		public IEnumerable<Discipline> LoadComboByTest(long test_id)
		{
			return disciplineRepository.LoadComboByTest(test_id);
		}
		public IEnumerable<Discipline> GetDisciplinesByTestSubGroup_Id(long TestSubGroup_Id)
		{
			return disciplineRepository.GetDisciplinesByTestSubGroup_Id(TestSubGroup_Id);
		}

		public IEnumerable<Discipline> Search(string search, ref Pager pager, Guid EntityId)
		{
			return disciplineRepository.Search(EntityId, search, ref pager);
		}

		public IEnumerable<Discipline> SearchDisciplines(int typeLevelEducation, Guid EntityId)
		{
			IEnumerable<ACA_TipoDisciplina> disciplinesGestao = disciplineTypeRepository.Load(typeLevelEducation);

			IEnumerable<Discipline> disciplines = disciplineRepository.LoadCustom(EntityId);

			IEnumerable<Discipline> disciplinesSelect = (from dg in disciplinesGestao
														 select new Discipline
														 {
															 Id = dg.tds_id,
															 Description = dg.tds_nome
														 });

			var result = from c in disciplinesSelect
						 where !disciplines.Any(p => p.DisciplineTypeId == c.Id)
						 select c;

			return result;
		}

		public List<AJX_Select2> LoadDisciplineByKnowledgeArea(string description, string knowledgeAreas, Guid EntityId)
		{
			return disciplineRepository.LoadDisciplineByKnowledgeArea(description, knowledgeAreas, EntityId);
		}

		#endregion

		#region Write

		public Discipline Save(Discipline entity, Guid entityid)
		{
			entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
			if (entity.Validate.IsValid)
			{
				entity.EntityId = entityid;
				entity = disciplineRepository.Save(entity);
				entity.Validate.Type = ValidateType.Save.ToString();
				entity.Validate.Message = "Disciplina salva com sucesso.";
			}

			return entity;
		}

		public Discipline Delete(long id)
		{
			Discipline entity = new Discipline { Id = id };
			entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
			if (entity.Validate.IsValid)
			{
				disciplineRepository.Delete(entity);
				entity.Validate.Type = ValidateType.Delete.ToString();
				entity.Validate.Message = "Disciplina excluída com sucesso.";
			}

			return entity;
		}

		public List<Discipline> SaveRange(List<Discipline> listEntity, Guid EntityId)
		{
			foreach (Discipline dc in listEntity)
				dc.EntityId = EntityId;

			disciplineRepository.SaveRange(listEntity);

			return listEntity;
		}

		#endregion
	}
}
