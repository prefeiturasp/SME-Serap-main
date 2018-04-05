using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
	public class AdministrativeUnitTypeRepository : ConnectionReadOnly, IAdministrativeUnitTypeRepository
    {
		#region ReadyOnly

		public IEnumerable<AdministrativeUnitType> Get()
		{
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT tua_id AS AdministrativeUnitTypeId, tua_nome AS Name
                            FROM Synonym_Core_SYS_TipoUnidadeAdministrativa
                            WHERE tua_situacao <> @state";

                var lstAdministrativeUnitType = cn.Query<AdministrativeUnitType>(sql, new { state = (byte)EnumState.excluido });

                return lstAdministrativeUnitType;
            }
        }

		public IEnumerable<AdministrativeUnitType> GetAdministrativeUnitsTypes()
		{
			using (IDbConnection cn = Connection)
			{
				cn.Open();
				var sql = @"SELECT Id, AdministrativeUnitTypeId, Name
                            FROM AdministrativeUnitType
                            WHERE State <> @state";

				var lstAdministrativeUnitType = cn.Query<AdministrativeUnitType>(sql, new { state = (byte)EnumState.excluido });

				return lstAdministrativeUnitType;
			}
		}

        #endregion

        #region CRUD

        public IEnumerable<AdministrativeUnitType> Save(IEnumerable<AdministrativeUnitType> unitTypes)
        {
            using (GestaoAvaliacaoContext context = new GestaoAvaliacaoContext())
            {
                var existingUnitTypes = context.AdministrativeUnitType.ToList();

                // Insere os novos tipos de unidade.                
                var newUnitTypes = unitTypes.Where(p => !existingUnitTypes.Select(x => x.AdministrativeUnitTypeId).Contains(p.AdministrativeUnitTypeId));
                foreach (var unitType in newUnitTypes)
                {
                    context.Entry(unitType).State = EntityState.Added;
                }

                var ids = unitTypes.Select(p => p.AdministrativeUnitTypeId).ToArray();

                //  Reativa/Atualiza os tipos de unidade existentes.
                var updateUnitTypes = existingUnitTypes.Where(p => ids.Contains(p.AdministrativeUnitTypeId));
                foreach (var unitType in updateUnitTypes)
                {
                    context.AdministrativeUnitType.Attach(unitType);

                    unitType.Name = unitTypes.FirstOrDefault(p => p.AdministrativeUnitTypeId == unitType.AdministrativeUnitTypeId).Name;
                    unitType.State = (byte)EnumState.ativo;

                    context.Entry(unitType).State = EntityState.Modified;
                }

                // Exclui os tipos de unidade não utilizados.
                var deletedUnitTypes = existingUnitTypes.Where(p => !ids.Contains(p.AdministrativeUnitTypeId));
                foreach (var unitType in deletedUnitTypes)
                {
                    context.AdministrativeUnitType.Attach(unitType);

                    unitType.State = (byte)EnumState.excluido;

                    context.Entry(unitType).State = EntityState.Modified;
                }

                context.SaveChanges();

                return unitTypes;
            }
        }

        #endregion
    }
}
