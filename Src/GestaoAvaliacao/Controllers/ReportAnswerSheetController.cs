using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ReportAnswerSheetController : Controller
    {
        private readonly IAnswerSheetBatchFilesBusiness batchFilesBusiness;

        public ReportAnswerSheetController(IAnswerSheetBatchFilesBusiness batchFilesBusiness)
        {
            this.batchFilesBusiness = batchFilesBusiness;
        }

        public ActionResult FollowUpIdentification()
        {
            EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());
            switch (visao)
            {
                case EnumSYS_Visao.Administracao: 
                case EnumSYS_Visao.Gestao:
                    return RedirectToAction("FollowUpIdentificationDRE");
                case EnumSYS_Visao.UnidadeAdministrativa: 
                case EnumSYS_Visao.Individual:
                    return RedirectToAction("FollowUpIdentificationSchool");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult FollowUpIdentificationDRE()
        {
            EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());
            if (visao != EnumSYS_Visao.Administracao && visao != EnumSYS_Visao.Gestao)
                return RedirectToAction("FollowUpIdentification");

            return View();
        }

        public ActionResult FollowUpIdentificationFiles()
        {
            return View();
        }

        public ActionResult FollowUpIdentificationSchool()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSituationList()
        {
            try
            {
                var listSituation = new List<EnumFollowUpIdentificationDataType>();

                listSituation.Add(EnumFollowUpIdentificationDataType.Identified);
                listSituation.Add(EnumFollowUpIdentificationDataType.NotIdentified);
                listSituation.Add(EnumFollowUpIdentificationDataType.Pending);

                var ret = listSituation.Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v)
                }).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Opções de situação não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar opções de situação." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetIdentificationReportInfo(AnswerSheetBatchFilter filter)
        {
            Validate ret = new Validate();
            AnswerSheetFollowUpIdentification entity = new AnswerSheetFollowUpIdentification();
            try
            {
                entity = batchFilesBusiness.GetIdentificationReportInfo(filter);

                if (filter.SchoolId != null && string.IsNullOrEmpty(entity.SchoolName))
                {
                    ret.IsValid = false;
                    ret.Type = ValidateType.alert.ToString();
                    ret.Message = "Id da Escola inválido.";
                }
                else if (filter.SupAdmUnitId != null && string.IsNullOrEmpty(entity.SupAdmUnitName))
                {
                    ret.IsValid = false;
                    ret.Type = ValidateType.alert.ToString();
                    ret.Message = "Id da DRE inválido.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Type = ValidateType.error.ToString();
                ret.IsValid = false;
                ret.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.IsValid, type = ret.Type, message = ret.Message, entity = entity }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetFollowUpIdentificationDRE(AnswerSheetBatchFilter filter)
        {
            try
            {
                Pager pager = this.GetPager();
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;
                filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
                filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.View = EnumFollowUpIdentificationView.Total;

                var totalFilesList = filter.SupAdmUnitId == null ? batchFilesBusiness.GetIdentificationList(filter) : null;
                var TotalFiles = totalFilesList != null && totalFilesList.Count() > 0 ? totalFilesList.FirstOrDefault() : null;

                filter.View = EnumFollowUpIdentificationView.DRE;
                var DREList = batchFilesBusiness.GetIdentificationList(ref pager, filter);
                if (DREList != null && DREList.Count() > 0)
                {
                    return Json(new { success = true, lista = DREList, totalFiles = TotalFiles, FilterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { success = true, lista = "", totalFiles = "", FilterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter os dados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetFollowUpIdentificationSchool(AnswerSheetBatchFilter filter)
        {
            try
            {
                Pager pager = this.GetPager();
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;
                filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
                filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.View = EnumFollowUpIdentificationView.School;

                var SchoolList = batchFilesBusiness.GetIdentificationList(ref pager, filter);
                if (SchoolList != null && SchoolList.Count() > 0)
                {
                    return Json(new { success = true, lista = SchoolList, FilterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { success = true, lista = "", FilterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter os dados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetFollowUpIdentificationFiles(AnswerSheetBatchFilter filter)
        {
            try
            {
                Pager pager = this.GetPager();
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;
                filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
                filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.View = EnumFollowUpIdentificationView.Files;

                var FilesList = batchFilesBusiness.GetIdentificationFilesList(ref pager, filter);
                if (FilesList != null && FilesList.Count() > 0)
                {
                    return Json(new { success = true, lista = FilesList, FilterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { success = true, lista = "", FilterDateUpdate = filter.FilterDateUpdate }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter os dados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ExportFollowUpIdentification(AnswerSheetBatchFilter filter)
        {
            EntityFile ret = new EntityFile();

            try
            {
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;
                filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
                filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;

                string separator = ";";
                Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
                if (param != null)
                    separator = param.Value;

                ret = batchFilesBusiness.ExportFollowUpIdentification(filter, separator, ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory);
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
        public JsonResult DownloadZipFiles(AnswerSheetBatchFilter filter)
        {
            EntityFile ret = new EntityFile();

            try
            {
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.PesId = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;
                filter.EntId = SessionFacade.UsuarioLogado.Usuario.ent_id;
                filter.CoreGroupId = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.View = EnumFollowUpIdentificationView.Files;

                switch (filter.Type)
                {
                    case EnumFollowUpIdentificationDataType.Identified:
                        var listSituationIdentified = new string[] { Convert.ToString((byte)EnumBatchSituation.Pending), Convert.ToString((byte)EnumBatchSituation.Success), Convert.ToString((byte)EnumBatchSituation.Error), Convert.ToString((byte)EnumBatchSituation.Warning) };
                        filter.Processing = string.Join(",", listSituationIdentified);
                        break;
                    case EnumFollowUpIdentificationDataType.NotIdentified:
                        filter.Processing = Convert.ToString((byte)EnumBatchSituation.NotIdentified);
                        break;
                    case EnumFollowUpIdentificationDataType.Pending:
                        filter.Processing = Convert.ToString((byte)EnumBatchSituation.PendingIdentification);
                        break;
                    default: break;
                }

                ret = batchFilesBusiness.ZipFollowUpIdentification(filter, ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory);

                if(ret == null || (ret != null && ret.Id <= 0))
                {
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.IsValid = false;
                    ret.Validate.Message = "Nenhum resultado encontrado.";
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
    }
}