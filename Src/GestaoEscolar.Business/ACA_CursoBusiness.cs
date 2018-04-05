using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_CursoBusiness : IACA_CursoBusiness
    {
        private readonly IACA_CursoRepository courseRepository;

        public ACA_CursoBusiness(IACA_CursoRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        #region Read

        public IEnumerable<ACA_Curso> Load(Guid ent_id)
        {
            return courseRepository.Load(ent_id);
        }

        public IEnumerable<ACA_Curso> LoadByTipoNivelEnsino(Guid ent_id, int tne_id)
        {
            return courseRepository.LoadByTipoNivelEnsino(ent_id, tne_id);
        }

        public IEnumerable<ACA_Curso> LoadByNivelEnsinoModality(Guid ent_id, int tne_id, int tme_id)
        {
            return courseRepository.LoadByNivelEnsinoModality(ent_id, tne_id, tme_id);
        }

        public ACA_Curso Get(long id, Guid EntityId)
        {
            ACA_Curso result = courseRepository.Get(id, EntityId);
            if (result != null)
                return result;
            else
                return new ACA_Curso();
        }

        public object GetCustom(long id, Guid EntityId)
        {
            ACA_Curso result = courseRepository.Get(id, EntityId);
            if (result != null)
            {
                return new
                {
                    Id = result.cur_id,
                    Description = result.cur_nome
                };
            }
            else
            {
                return new
                {
                    Id = 0,
                    Description = string.Empty
                };
            }
        }

        #endregion
    }
}
