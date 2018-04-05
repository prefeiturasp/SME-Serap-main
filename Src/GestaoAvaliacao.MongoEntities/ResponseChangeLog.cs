using GestaoAvaliacao.MongoEntities.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("ResponseChangeLog")]
    public class ResponseChangeLog : EntityBase
    {
        public Guid Ent_id { get; set; }
        public Guid Usu_id { get; set; }
        public Guid Dre_id { get; set; }
        public int Esc_id { get; set; }
        public long Tur_id { get; set; }
        public long Test_id { get; set; }
        public long Alu_id { get; set; }
        public long Item_Id { get; set; }
        public long Alternative_IdAnterior { get; set; }
        public string Alternative_Anterior { get; set; }
        public long Alternative_IdAtual { get; set; }
        public string Alternative_Atual { get; set; }
        public bool Automatic { get; set; }
        public bool Absence { get; set; }
        public long AbsenceReason_IdAnterior { get; set; }
        public long AbsenceReason_IdAtual { get; set; }

        public ResponseChangeLog()
        {

        }
    }
}
