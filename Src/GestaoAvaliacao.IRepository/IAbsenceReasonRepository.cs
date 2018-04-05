using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IAbsenceReasonRepository
	{
		AbsenceReason Save(AbsenceReason entity);
		void Update(AbsenceReason entity);
		void Delete(long id);
		AbsenceReason Get(long id, Guid EntityId);
        AbsenceReason GetDefault(Guid EntityId);
        List<AbsenceReason> GetAll(Guid EntityId);
        IEnumerable<AbsenceReason> Load(ref Pager pager, Guid EntityId);
		IEnumerable<AbsenceReason> Search(string search, ref Pager pager, Guid EntityId);
		bool ExistsDescriptionNamed(string description, Guid ent_id);
		bool ExistsDescriptionNamedAlter(String Description, int id, Guid ent_id);
		IEnumerable<AbsenceReason> Get(Guid EntityId);
        void VerifyDefault(Guid EntityId);

    }
}
