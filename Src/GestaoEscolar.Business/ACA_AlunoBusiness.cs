using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_AlunoBusiness : IACA_AlunoBusiness
	{
		readonly IACA_AlunoRepository alunoRepository;

		public ACA_AlunoBusiness()
		{
            this.alunoRepository = new ACA_AlunoRepository();
        }

        public ACA_AlunoBusiness(IACA_AlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository;
        }

        public IEnumerable<ACA_Aluno> GetBySection(long tur_id)
		{
			return alunoRepository.GetBySection(tur_id);
		}

        public ACA_Aluno Get(long id)
        {
            return alunoRepository.Get(id);
        }

        public ACA_Aluno GetStudentByPesId(Guid pes_id)
        {
            return alunoRepository.GetStudentByPesId(pes_id);
        }        

        public IEnumerable<ACA_Aluno> Get(IEnumerable<long> ids)
        {
            return alunoRepository.Get(ids);
        }

    }
}
