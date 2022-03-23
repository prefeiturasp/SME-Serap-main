using System;

namespace GestaoAvaliacao.Entities.DTO.SerapEstudantes
{
    public class AdminAutenticacaoDTO
    {
        public AdminAutenticacaoDTO(string login, Guid perfil)
        {
            Login = login;
            Perfil = perfil;
        }

        public string Login { get; set; }
        public Guid Perfil { get; set; }
        public string ChaveApi { get; set; }
    }
}
