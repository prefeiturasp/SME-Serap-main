using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class AdminSerapEstudantesController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (!SessionFacade.UsuarioLogadoIsValid)
            {
                throw new NotImplementedException();
            };

            string urlApiSerapEstudantes = WebConfigurationManager.AppSettings["URL_ADMIN_SERAP_ESTUDANTES"];
            string chaveApi = WebConfigurationManager.AppSettings["ChaveSerapProvaApi"];
            UsuarioLogado user = SessionFacade.UsuarioLogado;
            string urlAdminEstudantes = $"{urlApiSerapEstudantes}{user.Usuario?.usu_login}/{user.Nome}/{user.Grupo?.gru_id}/{chaveApi}";
            AdminSerapEstudantesDTO adminSerapEstudantesDTO = new AdminSerapEstudantesDTO();
            adminSerapEstudantesDTO.UrlApiSerapEstudantes = urlAdminEstudantes;
            return View(adminSerapEstudantesDTO);
        }
    }
}
