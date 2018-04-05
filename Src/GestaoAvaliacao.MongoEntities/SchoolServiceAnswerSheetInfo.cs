using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    /// <summary>
    /// Class for school answer sheet generation information
    /// </summary>
    public class SchoolServiceAnswerSheetInfo
    {
        /// <summary>
        /// Gets or sets school id.
        /// </summary>
        public int SchoolId { get; set; }

        /// <summary>
        /// Gets or sets school name.
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        public ServiceStatusInfo Status { get; set; } = new ServiceStatusInfo();

        /// <summary>
        /// Gets or sets answer sheet quantity to generate.
        /// </summary>
        public int AnswerSheetQuantityToGenerate { get; set; }

        /// <summary>
        /// Gets or sets answer sheet count generated
        /// </summary>
        public int AnswerSheetCountGenerated { get; set; }

        public List<SectionInfo> Sections { get; set; } = new List<SectionInfo>();


    }

    public class SectionInfo
    {
        public long tur_id { get; set; }
        public double TotalMillisecondsPdf { get; set; }
        public double TotalMillisecondsQrCode { get; set; }

        public int QuantityToGenerate;

    }
}