using System;
using System.Collections.Generic;
using System.Text;

namespace ProvaSP.Model.Entidades
{
    public class QuestionarioItem
    {
        public int QuestionarioItemID { get; set; }
        public int QuestionarioID { get; set; }
        public string Titulo { get; set; }
        public string Numero { get; set; }
    }
}
