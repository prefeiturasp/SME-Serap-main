using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class StudentTestAbsenceReasonBusiness : IStudentTestAbsenceReasonBusiness
    {
        readonly IStudentTestAbsenceReasonRepository studentTestAbsenceReasonRepository;
        private readonly IResponseChangeLogBusiness responseChangeLogBusiness;

        public StudentTestAbsenceReasonBusiness(IStudentTestAbsenceReasonRepository studentTestAbsenceReasonRepository, IResponseChangeLogBusiness responseChangeLogBusiness)
        {
            this.studentTestAbsenceReasonRepository = studentTestAbsenceReasonRepository;
            this.responseChangeLogBusiness = responseChangeLogBusiness;

        }

		#region Read
		public IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked)
        {
			return studentTestAbsenceReasonRepository.GetByTestSection(test_id, tur_id, aluMongoList, ignoreBlocked);
		}

        public IEnumerable<CorrectionStudentGrid> GetByTestSectionByAluId(long test_id, long tur_id, long alu_id, bool ignoreBlocked)
        {
            return studentTestAbsenceReasonRepository.GetByTestSectionByAluId(test_id, tur_id, alu_id, ignoreBlocked);
        }

        public IEnumerable<StudentTestAbsenceReason> GetAbsencesByTestSection(long test_id, long tur_id)
        {
            return studentTestAbsenceReasonRepository.GetAbsencesByTestSection(test_id, tur_id);
        }

        public IEnumerable<StudentTestAbsenceReason> GetAbsencesByTest(long test_id)
        {
            return studentTestAbsenceReasonRepository.GetAbsencesByTest(test_id);
        }

        public StudentTestAbsenceReason GetByTestStudent(long test_id, long tur_id, long alu_id)
        {
            return studentTestAbsenceReasonRepository.GetByTestStudent(test_id, tur_id, alu_id);
        }

        public int CountAbsencesByTestSection(long test_id, long tur_id)
        {
            return studentTestAbsenceReasonRepository.CountAbsencesByTestSection(test_id, tur_id);
        }

        public SchoolDTO GetEscIdDreIdByTeam(long tur_id)
        {
            return studentTestAbsenceReasonRepository.GetEscIdDreIdByTeam(tur_id);
        }
        #endregion

        #region Write

        public StudentTestAbsenceReason Save(StudentTestAbsenceReason entity, Guid usu_id, Guid ent_id, bool manual)
        {
            var cadastred = studentTestAbsenceReasonRepository.GetByTestStudent(entity.Test_Id, entity.tur_id, entity.alu_id);

            long AbsenceReason_IdAnterior = cadastred != null ? cadastred.AbsenceReason_Id : 0;

            var retornoLog = responseChangeLogBusiness.Save(null, entity.alu_id, entity.Test_Id, entity.tur_id, ent_id, usu_id, manual, null, AbsenceReason_IdAnterior, entity.AbsenceReason_Id);
            
            if (cadastred == null)
            {
                studentTestAbsenceReasonRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
            }
            else
            {
                entity.Id = cadastred.Id;
                //Remover motivo de ausencia
                if (entity.AbsenceReason_Id == 0)
                    studentTestAbsenceReasonRepository.Remove(entity);
                else
                    studentTestAbsenceReasonRepository.Update(entity);
            }

            entity.Validate.Message = "Aluno salvo com sucesso.";

            return entity;
        }

        public StudentTestAbsenceReason Remove(StudentTestAbsenceReason entity)
        {
            var cadastred = studentTestAbsenceReasonRepository.GetByTestStudent(entity.Test_Id, entity.tur_id, entity.alu_id);
            if (cadastred != null)
            {
                entity.Id = cadastred.Id;
                studentTestAbsenceReasonRepository.Remove(entity);
            }

            return entity;
        }

        public IEnumerable<long> StudentAbsencesByTestSection(long test_id, long tur_id)
        {
            return studentTestAbsenceReasonRepository.StudentAbsencesByTestSection(test_id, tur_id);
        }

        #endregion

        #region Private Methods
        private Validate Validate(Test test, Guid usuId, EnumSYS_Visao visao)
        {
            Validate valid = new Validate();

            if (!(test.UsuId.Equals(usuId) || visao == EnumSYS_Visao.UnidadeAdministrativa))
                valid.Message = "Usuário não possui permissão para realizar essa ação.";

            if (!string.IsNullOrEmpty(valid.Message))
            {
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
    }
}
