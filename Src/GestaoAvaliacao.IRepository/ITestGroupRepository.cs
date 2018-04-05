using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestGroupRepository
    {
        TestGroup Get(long id);
        TestGroup GetTestGroup(long id);
        IEnumerable<TestGroup> Load(Guid ent_id);
        IEnumerable<TestGroup> LoadPaginate(ref Pager pager, Guid ent_id);
        TestGroup Save(TestGroup entity);
        TestGroup Update(TestGroup entity);
        IEnumerable<TestGroup> Search(ref Pager pager, Guid EntityId, String search = null, int levelqtd = 0);
        void Delete(TestGroup entity);
        bool ExistsModelDescription(string description, Guid ent_id);
        bool ExistsModelDescriptionUpdate(string description, long Id, Guid ent_id);   
        bool ExistsTestRelated(TestGroup entity);
        IEnumerable<TestGroup> LoadGroupsSubGroups(Guid ent_id);
        bool VerifyDeleteSubGroup(long id);
    }
}
