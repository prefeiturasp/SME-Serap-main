using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class QuestionarioUsuario
    {
        public int QuestionarioUsuarioID { get; set; }
        public int QuestionarioID { get; set; }
        public string Guid { get; set; }
        public string esc_codigo { get; set; }
        public int? tur_id { get; set; }
        public string usu_id { get; set; }
        public string DataPreenchimento { get; set; }
        public IEnumerable<QuestionarioRespostaItem> Respostas { get; set; }
    }
}
