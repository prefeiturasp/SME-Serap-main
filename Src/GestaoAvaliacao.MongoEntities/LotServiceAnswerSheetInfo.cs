using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    /// <summary>
    /// Class for lot answer sheet generation information
    /// </summary>
    public class LotServiceAnswerSheetInfo
    {
        /// <summary>
        /// Gets or sets lotid.
        /// </summary>
        public long LotId { get; set; }

        /// <summary>
        /// Gets or sets parent lotId.
        /// </summary>
        public long? ParentLotId { get; set; }

        /// <summary>
        /// Gets or sets schools.
        /// </summary>
        public List<SchoolServiceAnswerSheetInfo> Schools { get; set; } = new List<SchoolServiceAnswerSheetInfo>();

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        public ServiceStatusInfo Status { get; set; } = new ServiceStatusInfo();

        /// <summary>
        /// Add or update SchoolServiceAnswerSheetInfo.
        /// </summary>
        /// <param name="item">
        /// SchoolServiceAnswerSheetInfo
        /// </param>
        public void AddOrUpdateSchool(SchoolServiceAnswerSheetInfo item)
        {
            var schoolId = item.SchoolId;

            var schoolIndex = this.Schools.FindIndex(p => p.SchoolId == schoolId);

            if ((item != null)
                && (schoolIndex >= 0))
            {
                this.Schools[schoolIndex] = item;
            }
            else
            {
                this.Schools.Add(item);
            }
        }

        /// <summary>
        /// Update Status for school.
        /// </summary>
        /// <param name="schoolId">
        /// SchoolId
        /// </param>
        /// <param name="status">
        /// Status
        /// </param>
        public void UpdateSchoolStatus(int schoolId, ServiceStatusInfo status)
        {
            var schoolIndex = this.Schools.FindIndex(p => p.SchoolId == schoolId);

            if ((status != null)
                && (schoolIndex >= 0))
            {
                this.Schools[schoolIndex].Status = status;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="status">
        /// </param>
        public void UpdateStatus(ServiceStatusInfo status)
        {
            this.Status = status;
        }
    }
}