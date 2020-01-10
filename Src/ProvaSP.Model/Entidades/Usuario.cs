using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProvaSP.Model.Entidades
{
    public class Usuario
    {
        public  Usuario()
        {
            grupos = new List<Grupo>();
        }

        public string usu_id { get; set; }
        public string usu_login { get; set; }
        public string usu_senha { get; set; }
        public string Ano { get; set; }
        public string Turma { get; set; }
        [JsonIgnore]
        public string usu_criptografia { get; set; }
        public List<Grupo> grupos { get; set; }

        //private ICollection<Grupo> grupos;

        //public virtual ICollection<Grupo> Grupos => grupos ?? (grupos = new HashSet<Grupo>());

        public bool Supervisor
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.Supervisor).Any();
            }
        }

        public bool Diretor
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.Diretor).Any();
            }
        }

        public bool AssistenteDeDiretoria
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.AssistenteDeDiretoria).Any();
            }
        }

        public bool AgenteEscolar
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.AgenteEscolar).Any();
            }
        }

        public bool AuxiliarTecnicoEducacao
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.AuxiliarTecnicoEducacao).Any();
            }
        }

        public bool Coordenador
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.Coordenador).Any();
            }
        }

        public bool Professor
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.Professor).Any();
            }
        }

        public bool Aluno
        {
            get
            {
                return grupos.Where(x => x.gru_id == Grupo.Perfil.Aluno).Any();
            }
        }

        public bool AcessoNivelSME
        {
            get
            {
                return grupos.Where(x => x.AcessoNivelSME).Any();
            }
        }

        public bool AcessoNivelDRE
        {
            get
            {
                return grupos.Where(x => x.AcessoNivelDRE).Any();
            }
        }

        public bool AcessoNivelEscola
        {
            get
            {
                return grupos.Where(x => x.AcessoNivelEscola).Any();
            }
        }
    }
}
