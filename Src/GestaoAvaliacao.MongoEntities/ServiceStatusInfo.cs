using GestaoAvaliacao.Util;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GestaoAvaliacao.MongoEntities
{
    /// <summary>
    /// Base class for service execution information
    /// </summary>
    public class ServiceStatusInfo
    {
        /// <summary>
        /// Gets or set description
        /// </summary>
        public string Description { get; set; }

        public string ServiceStateDescription
        {
            get
            {
                return ServiceStateCode.GetDescription();
            }
        }

        /// <summary>
        /// The service state code
        /// </summary>

        public EnumServiceState ServiceStateCode { get; set; }

        /// <summary>
        /// The start date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The finish date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// Get or set total milliseconds
        /// </summary>
        public double TotalMilliseconds
        {
            get
            {
                Func<DateTime, double> _ = (x) =>
                {
                    return x.Subtract(StartDate).TotalMilliseconds;
                };
                var date = (FinishDate == DateTime.MinValue) ? DateTime.Now : FinishDate;
                double total = _(date);
                return total;
            }
        }
    }
}