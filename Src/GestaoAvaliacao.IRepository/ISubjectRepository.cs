using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ISubjectRepository
    {
        Subject Save(Subject entity);
        Subject Update(Subject entity);
        void Delete(Subject entity);
        List<Discipline> SearchSubjects(string assunto, string subassunto, Guid EntityId, ref Pager pager);
        Subject GetSubject(long id);
        bool ExistsDescription(string description, Guid ent_id);
        Subject Get(long id);
        List<AJX_Select2> LoadAllSubjects(string description, Guid EntityId);
        List<AJX_Select2> LoadSubsubjectBySubject(string description, string subjects, Guid EntityId);
        Subject LoadSubjectBySubsubject(long idSubsubject);
        bool ExistsItemRelated(Subject subject, Guid ent_id);
        bool IsDeletedSubSubjectBeenUsed(Subject entity);
    }
}
