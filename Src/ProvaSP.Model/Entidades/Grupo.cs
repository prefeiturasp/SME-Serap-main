using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Model.Entidades
{
    public class Grupo
    {
        public string gru_id { get; set; }
        public string gru_nome { get; set; }

        public string uad_sigla { get; set; }

        public string esc_codigo { get; set; }

        public struct Perfil
        {
            //NÍVEL SME:
            public static string Administrador = "AAD9D772-41A3-E411-922D-782BCB3D218E";
            public static string AdministradorNTA = "22366A3E-9E4C-E711-9541-782BCB3D218E";
            public static string AdmSerapCOPEDLeitura = "A8CB8D7B-F333-E711-9541-782BCB3D218E";
            public static string GestaoSERApLeitura = "5A98961B-92F3-E611-9541-782BCB3D218E";

            //NÍVEL DRE:
            public static string AdmSerapDRE = "FAB1C330-87E8-E611-9541-782BCB3D218E";
            public static string AdministradorSerapDRE = "104F0759-87E8-E611-9541-782BCB3D218E";
            public static string TecnicoDRE = "48C0129A-9229-E611-8135-782BCB3D218E";
            public static string Supervisor =  "A0B86A81-F233-E711-9541-782BCB3D218E" ;

            //NÍVEL ESCOLA:
            public static string Diretor =  "26552002-FD66-4D63-9FA7-E9B3993D110D" ;
            public static string Coordenador =  "1321EDB1-2499-43CA-ADCB-1FC31D1674B1" ;
            public static string Professor = "067D9B21-A1FF-E611-9541-782BCB3D218E";

            //NÍVEL ALUNO:
            public static string Aluno = "BD6D9CE6-9456-E711-9541-782BCB3D218E";

        };

        public bool AcessoNivelSME
        {
            get
            {
                string[] perfis = { Grupo.Perfil.Administrador, Grupo.Perfil.AdministradorNTA, Grupo.Perfil.AdmSerapCOPEDLeitura, Grupo.Perfil.GestaoSERApLeitura };
                return perfis.Contains(gru_id);
            }   
        }

        public bool AcessoNivelDRE
        {
            get
            {
                string[] perfis = { Grupo.Perfil.AdmSerapDRE, Grupo.Perfil.AdministradorSerapDRE, Grupo.Perfil.TecnicoDRE, Grupo.Perfil.Supervisor };
                return perfis.Contains(gru_id);
            }
        }

        public bool AcessoNivelEscola
        {
            get
            {
                string[] perfis = { Grupo.Perfil.Diretor, Grupo.Perfil.Coordenador, Grupo.Perfil.Professor };
                return perfis.Contains(gru_id);
        }
        }

        public enum TipoQuestionario : int
        {
            QuestionarioSupervisor = 1,
            QuestionarioDiretor = 2,
            QuestionarioCoordenadorPedagogico = 3,
            FichaRegistroAplicadorProva = 8,
            FichaRegistroSupervisor = 9,
            FichaRegistroDiretor = 10,
            FichaRegistroCoordenadorPedagogico = 11,
            QuestionarioProfessor = 12
        };

    }
}
