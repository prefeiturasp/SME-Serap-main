using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ISubjectBusiness
    {
        Subject Save(Subject entity, Guid ent_id);
        Subject Update(Subject entity, Guid ent_id);
        Subject Delete(long id, Guid ent_id);
        List<Discipline> SearchSubjects(string assunto, string subassunto, Guid ent_id, ref Pager pager);
        Subject GetSubject(long id);
        Subject Get(long id);
        List<AJX_Select2> LoadAllSubjects(string description, Guid EntityId);
        List<AJX_Select2> LoadSubsubjectBySubject(string description, string subjects, Guid EntityId);
        Subject LoadSubjectBySubsubject(long idSubsubject);
    }
}
