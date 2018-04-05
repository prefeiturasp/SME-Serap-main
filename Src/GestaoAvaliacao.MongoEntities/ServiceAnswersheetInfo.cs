using GestaoAvaliacao.MongoEntities.Attribute;
using GestaoAvaliacao.Util;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("ServiceAnswersheetInfos")]
    public class ServiceAnswerSheetInfo : EntityBase
    {
        /// <summary>
        /// Gets or sets AnswerSheetLots.
        /// </summary>
        [BsonElement]
        public List<LotServiceAnswerSheetInfo> AnswerSheetLots { get; set; } = new List<LotServiceAnswerSheetInfo>();

        /// <summary>
        /// Gets or sets LotId.
        /// </summary>
        public long LotId { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public ServiceStatusInfo Status { get; set; } = new ServiceStatusInfo();

        /// <summary>
        /// Add or Update LotServiceAnswersheetInfo.
        /// </summary>
        /// <param name="item">
        /// LotServiceAnswersheetInfo
        /// </param>
        public void AddOrUpdateLot(LotServiceAnswerSheetInfo item)
        {
            long lotId = item.LotId;

            var lotIndex = this.AnswerSheetLots.FindIndex(p => p.LotId == lotId);

            if (item != null && lotIndex >= 0)
            {
                this.AnswerSheetLots[lotIndex] = item;
            }
            else
            {
                this.AnswerSheetLots.Add(item);
            }
        }

        /// <summary>
        /// Add or Update SchoolServiceAnswerSheetInfo.
        /// </summary>
        /// <param name="lotId">
        /// LotId
        /// </param>
        /// <param name="item">
        /// SchoolServiceAnswerSheetInfo
        /// </param>
        public void AddOrUpdateSchoolLot(long lotId, SchoolServiceAnswerSheetInfo item)
        {
            var index = this.AnswerSheetLots.FindIndex(p => p.LotId == lotId);

            if (index >= 0 && item != null)
            {
                this.AnswerSheetLots[index].AddOrUpdateSchool(item);
            }
        }

        /// <summary>
        /// Update StatusSchoolLot.
        /// </summary>
        /// <param name="lotId">
        /// LotId
        /// </param>
        /// <param name="schoolId">
        /// SchoolId
        /// </param>
        /// <param name="status">
        /// StatusInfo
        /// </param>
        public void UpdateStatusSchoolLot(long lotId, int schoolId, ServiceStatusInfo status)
        {
            var index = this.AnswerSheetLots.FindIndex(p => p.LotId == lotId);

            if (index >= 0 && status != null)
            {
                this.AnswerSheetLots[index].UpdateSchoolStatus(schoolId, status);
            }
        }

        /// <summary>
        /// Update StatusLot
        /// </summary>
        /// <param name="lotId">
        /// LotId
        /// </param>
        /// <param name="status">
        /// StatusInfo
        /// </param>
        public void UpdateStatusLot(long lotId, ServiceStatusInfo status)
        {
            int index = FindIndexLot(lotId);

            if (index >= 0 && status != null)
            {
                this.AnswerSheetLots[index].UpdateStatus(status);
            }
        }

        public bool LotHasError(long lotId)
        {
            var index = FindIndexLot(lotId);
            var result = false;
            if (index >= 0)
            {
                result =
                    AnswerSheetLots[index]
                            .Schools
                                .Any(p => p.Status.ServiceStateCode == EnumServiceState.Error);
            }
            return result;
        }

        public bool HasError()
        {
            return AnswerSheetLots.Any(p => p.Status.ServiceStateCode == EnumServiceState.Error);
        }

        private int FindIndexLot(long lotId)
        {
            return AnswerSheetLots.FindIndex(p => p.LotId == lotId);
        }
    }
}