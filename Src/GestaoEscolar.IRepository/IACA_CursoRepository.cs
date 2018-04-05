using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_CursoRepository
    {
        IEnumerable<ACA_Curso> Load(Guid EntityId);
        IEnumerable<ACA_Curso> LoadByTipoNivelEnsino(Guid EntityId, int tne_id);
        IEnumerable<ACA_Curso> LoadByNivelEnsinoModality(Guid EntityId, int tne_id, int tme_id);
        ACA_Curso Get(long id, Guid EntityId);
    }
}
