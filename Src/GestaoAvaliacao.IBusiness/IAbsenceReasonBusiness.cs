using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAbsenceReasonBusiness
	{
		AbsenceReason Save(AbsenceReason entity, Guid entityid);
		AbsenceReason Update(long id, AbsenceReason entity, Guid entityid);
		AbsenceReason Delete(long id, Guid ent_id);
		AbsenceReason Get(long id, Guid entityid);
		IEnumerable<AbsenceReason> Load(ref Pager pager, Guid entityid);
		IEnumerable<AbsenceReason> Search(string search, ref Pager pager, Guid entityid);
		IEnumerable<AbsenceReason> Get(Guid EntityId);
        AbsenceReason GetDefault(Guid EntityId);

    }
}
