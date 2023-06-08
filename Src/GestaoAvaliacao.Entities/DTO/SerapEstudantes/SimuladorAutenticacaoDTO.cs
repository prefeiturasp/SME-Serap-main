using System;

namespace GestaoAvaliacao.Entities.DTO.SerapEstudantes
{
    public class SimuladorAutenticacaoDTO
    {
        public SimuladorAutenticacaoDTO(string login, Guid perfil)
        {
            Login = login;
            Perfil = perfil;
        }

        public string Login { get; }
        public Guid Perfil { get; }
        public string ChaveApi { get; set; }
    }
}
