using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_AlunoRepository
	{
		IEnumerable<ACA_Aluno> GetBySection(long tur_id);
        ACA_Aluno Get(long alu_id);
        IEnumerable<ACA_Aluno> Get(IEnumerable<long> alu_ids);
        ACA_Aluno GetStudentByPesId(Guid pes_id);
    }
}
