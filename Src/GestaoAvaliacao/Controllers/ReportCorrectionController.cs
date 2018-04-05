using AvaliaMais.FolhaRespostas.Application.Interfaces;
using AvaliaMais.FolhaRespostas.Application.ViewModels;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    public class ReportCorrectionController : Controller
    {
        private readonly IProcessamentoAppService _procAppService;
        private readonly IExportacaoAppService _expAppService;
        private readonly Guid _userId;
        private readonly Guid _groupId;
        private Pager _pager;

        public ReportCorrectionController(IProcessamentoAppService procAppService, IExportacaoAppService expAppService)
        {
            _procAppService = procAppService;
            _expAppService = expAppService;
            _userId = SessionFacade.UsuarioLogado.Usuario.usu_id;
            _groupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
        }

        public ActionResult Index()
        {
            var visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());
            switch (visao)
            {
                case EnumSYS_Visao.UnidadeAdministrativa: return RedirectToAction("IndexSchool");
                case EnumSYS_Visao.Individual: return RedirectToAction("IndexSection");
            }
            return View();
        }

        public ActionResult IndexSchool()
        {
            return View();
        }

        public ActionResult IndexSection()
        {
            return View();
        }

        public ActionResult IndexStudent()
        {
            return View();
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetDres(int test_id, Guid? uad_id)
        {
            _pager = this.GetPager();
            var result = new DREViewModel();
            if (IsUserGestor())
            {
                result = _procAppService.ObterDresGestor(ref _pager, test_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);
            }
            else
            {
                result = _procAppService.ObterDres(ref _pager, test_id);
            }
            
            if (uad_id != null && uad_id.HasValue && uad_id.Value != Guid.Empty)
            {
                result.lista = FilterDre(result.lista, uad_id);
                result.QuantidadeTotal = null;
            }

            return Json(new { success = true, lista = result.lista, QuantidadeTotal = result.QuantidadeTotal }, JsonRequestBehavior.AllowGet);

        }

        private IEnumerable<DRE> FilterDre(IEnumerable<DRE> dres, Guid? uadId)
        {
            if (uadId.HasValue)
            {
                var result = dres.Where(a => a.DreId == uadId).ToList();
                if (_pager != null)
                {
                    _pager.SetTotalItens(result.Count());
                }
                return result;
            }
            return dres;
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetSchools(int test_id, Guid? uad_id)
        {
            _pager = this.GetPager();
            var result = new EscolaViewModel();
            if (IsUserAdmin() || IsUserGestor())
            {
                result = _procAppService.ObterEscolas(ref _pager, test_id, uad_id.Value);
            }
            else
            {
                result = _procAppService.ObterEscolasDiretor(ref _pager, test_id, 
                    SessionFacade.UsuarioLogado.Usuario,SessionFacade.UsuarioLogado.Grupo);
            }
            return Json(new { success = true, lista = result.lista, QuantidadeTotal = result.QuantidadeTotal }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetSection(int test_id, int? esc_id)
        {
            _pager = this.GetPager();
            var result = new TurmaViewModel();
            if (IsUserTeacher())
            {
                result = _procAppService.ObterTurmasProfessor(ref _pager, test_id, _userId);
            }
            else
            {
                result = _procAppService.ObterTurmas(ref _pager, test_id, esc_id.Value);
            }
            return Json(new { success = true, lista = result.lista, QuantidadeTotal = result.QuantidadeTotal }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetStudent(int test_id, int tur_id)
        {
            _pager = this.GetPager();
            var result = _procAppService.ObterAlunos(ref _pager, tur_id, test_id);
            return Json(new { success = true, lista = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExportDRE(int test_id, Guid? uad_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                IEnumerable<DRE> result = null;
                Quantidade quantidade = null;
                if (IsUserGestor())
                {
                    result = _expAppService.ObterDresGestor(test_id, _userId, _groupId);
                    quantidade = _expAppService.ObterQuantidadeDresGestor(test_id, _userId, _groupId);
                }
                else
                {
                    result = _expAppService.ObterDres(test_id);
                    quantidade = _expAppService.ObterQuantidadeDres(test_id);
                }

                if (uad_id != null && uad_id.HasValue && uad_id.Value != Guid.Empty)
                {
                    result = FilterDre(result, uad_id);
                    quantidade = null;
                }

                if (result != null && result.Count() > 0)
                {
                    ret = _expAppService.ExportReport(result, null, null, null, quantidade, separator,
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Ainda não existem dados para exportar.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Validate.Type = ValidateType.error.ToString();
                ret.Validate.IsValid = false;
                ret.Validate.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExportSchool(int test_id, Guid? uad_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                IEnumerable<Escola> result = null;
                if (IsUserAdmin() || IsUserGestor())
                {
                    result = _expAppService.ObterEscolas(test_id, uad_id.Value);
                }
                else
                {
                    result = _expAppService.ObterEscolasDiretor(test_id, _userId, _groupId);
                }

                if (result != null && result.Count() > 0)
                {
                    ret = _expAppService.ExportReport(null, result, null, null, null, separator, 
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Ainda não existem dados para exportar.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Validate.Type = ValidateType.error.ToString();
                ret.Validate.IsValid = false;
                ret.Validate.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExportSection(int test_id, int? esc_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                IEnumerable<Turma> result = null;
                if (IsUserTeacher())
                {
                    result = _expAppService.ObterTurmasProfessor(test_id,_userId);
                }
                else
                {
                    result = _expAppService.ObterTurmas(test_id, esc_id.Value);
                }

                if (result != null && result.Count() > 0)
                {
                    ret = _expAppService.ExportReport(null, null, result, null, null, separator, 
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Ainda não existem dados para exportar.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Validate.Type = ValidateType.error.ToString();
                ret.Validate.IsValid = false;
                ret.Validate.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExportStudent(int test_id, int tur_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                var result = _expAppService.ObterAlunos(tur_id, test_id);

                if (result != null && result.Count() > 0)
                {
                    ret = _expAppService.ExportReport(null, null, null, result, null, separator, 
                        ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, SessionFacade.UsuarioLogado.Usuario);
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Ainda não existem dados para exportar.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Validate.Type = ValidateType.error.ToString();
                ret.Validate.IsValid = false;
                ret.Validate.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
        }

        // Bounded Context - Core
        private bool IsUserAdmin()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Administracao;
        }

        private bool IsUserGestor()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Gestao;
        }

        private bool IsUserAdmUnity()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.UnidadeAdministrativa;
        }

        private bool IsUserTeacher()
        {
            return (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Individual;
        }
    }
}