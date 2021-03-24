using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Videos;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class FileController : Controller
    {
        private readonly IFileBusiness fileBusiness;
        private readonly ITestBusiness testBusiness;
        private readonly IParameterBusiness parameterBusiness;
        private readonly IVideoConverter videoConverter;
        private const string VideoConvertedContentType = "video/webm";

        public FileController(IFileBusiness fileBusiness, ITestBusiness testBusiness, IParameterBusiness parameterBusiness, IVideoConverter videoConverter)
        {
            this.fileBusiness = fileBusiness;
            this.testBusiness = testBusiness;
            this.parameterBusiness = parameterBusiness;
            this.videoConverter = videoConverter;
        }

        public ActionResult Index(long? Id)
        {
            if (Id.HasValue)
            {
                Validate valid = testBusiness.CanEdit(Id.Value, SessionFacade.UsuarioLogado.Usuario.usu_id,
                    (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));

                if (!valid.IsValid)
                    Response.Redirect(Url.Action("Index", "Test"));
            }
            return View();
        }

        #region Read

        [HttpGet]
        [Paginate]
        public JsonResult SearchUploadedFiles(FileFilter filter)
        {
            try
            {
                #region Filters

                if (filter == null)
                    filter = new FileFilter();

                Pager pager = this.GetPager();
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;

                #endregion

                IEnumerable<EntityFile> result = fileBusiness.SearchUploadedFiles(ref pager, filter);

                if (result != null)
                {
                    var ret = result.Select(entity => new
                    {
                        Id = entity.Id,
                        Path = entity.Path,
                        OriginalName = entity.OriginalName,
                        CreateDate = entity.CreateDate.ToShortDateString(),
                        AllLinks = fileBusiness.GetTestNames(entity.Id)
                    });

                    return Json(new { success = true, lista = ret, pageSize = pager.PageSize }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, lista = "" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar arquivos pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckFileExists(long Id)
        {
            Validate valid = new Validate();

            try
            {
                valid.IsValid = fileBusiness.CheckFileExists(Id, ApplicationFacade.PhysicalDirectory);
                if (!valid.IsValid)
                {
                    valid.Type = ValidateType.alert.ToString();
                    valid.Message = "Arquivo não existe.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                valid.IsValid = false;
                valid.Type = ValidateType.error.ToString();
                valid.Message = "Erro ao tentar encontrar arquivo.";
            }

            return Json(new { success = valid.IsValid, type = valid.Type, message = valid.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckFilePathExists(string path)
        {
            Validate valid = new Validate();

            try
            {
                string filePath = new Uri(path).AbsolutePath.Replace("Files/", string.Empty);
                string physicalPath = path.StartsWith("http") ? System.Web.HttpContext.Current.Server.MapPath(filePath) : path;
                string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                valid.IsValid = System.IO.File.Exists(decodedUrl);
                if (!valid.IsValid)
                {
                    valid.Type = ValidateType.alert.ToString();
                    valid.Message = "Arquivo não existe.";
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                valid.IsValid = false;
                valid.Type = ValidateType.error.ToString();
                valid.Message = "Erro ao tentar encontrar arquivo.";
            }

            return Json(new { success = valid.IsValid, type = valid.Type, message = valid.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void DownloadFile(long Id)
        {
            bool redirect = false;
            try
            {
                EntityFile file = fileBusiness.Get(Id);
                if (file != null)
                {
                    string filePath = new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty);
                    string physicalPath = string.Concat(ApplicationFacade.PhysicalDirectory, filePath.Replace("/", "\\"));
                    string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                    if (System.IO.File.Exists(decodedUrl))
                    {
                        System.IO.FileStream fs = System.IO.File.Open(decodedUrl, System.IO.FileMode.Open);
                        byte[] btFile = new byte[fs.Length];
                        fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                        fs.Close();

                        Response.Clear();
                        Response.AddHeader("Content-disposition", "attachment; filename=" + file.OriginalName);
                        Response.ContentType = file.ContentType;
                        Response.BinaryWrite(btFile);
                        Response.End();
                        redirect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
            }

            if (!redirect && Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.PathAndQuery))
                Response.Redirect(Request.UrlReferrer.PathAndQuery, false);
        }

        [HttpGet]
        public void DownloadFilePath(string path)
        {
            bool redirect = false;
            try
            {
                string filePath = new Uri(path).AbsolutePath.Replace("Files/", string.Empty);
                string physicalPath = path.StartsWith("http") ? System.Web.HttpContext.Current.Server.MapPath(filePath) : path;
                string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                if (System.IO.File.Exists(decodedUrl))
                {
                    System.IO.FileStream fs = System.IO.File.Open(decodedUrl, System.IO.FileMode.Open);
                    byte[] btFile = new byte[fs.Length];
                    fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                    fs.Close();

                    Response.Clear();
                    Response.AddHeader("Content-disposition", "attachment; filename=" + Path.GetFileName(path));
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(btFile);
                    Response.End();
                    redirect = true;
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
            }

            if (!redirect && Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.PathAndQuery))
                Response.Redirect(Request.UrlReferrer.PathAndQuery, false);
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Upload(Uploader file)
        {
            EntityFile entity = new EntityFile();

            try
            {
                UploadModel upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = file.InputStream,
                    Stream = null,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectory,
                    PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                    FileType = file.FileType,
                    UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
                };

                entity = fileBusiness.Upload(upload);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload da imagem.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, filelink = entity.Path, idFile = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UploadVideoAsync(HttpPostedFileBase file)
        {
            var entity = new EntityFile();
            var entityFileConvert = new EntityFile();

            try
            {
                var sizeToConvertVideo = parameterBusiness.GetByKey(EnumParameterKey.SIZE_TO_CONVERT_VIDEO_FILE.GetDescription());
                if(file.ContentLength >= int.Parse(sizeToConvertVideo.Value))
                    entityFileConvert = await ConvertVideoAsync(file.InputStream, file.ContentType, file.FileName);

                var upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = null,
                    Stream = file.InputStream,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectory,
                    PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                    FileType = EnumFileType.Video,
                    UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
                };

                entity = fileBusiness.Upload(upload);

                if (!entityFileConvert.Validate.IsValid)
                    entity.Validate.Message += entityFileConvert.Validate.Message;

                return Json(new
                {
                    success = entity.Validate.IsValid,
                    type = entity.Validate.Type,
                    message = entity.Validate.Message,
                    filelink = entity.Path,
                    idFile = entity.Id,
                    idConvertedFile = entityFileConvert.Id,
                    convertedVideoLink = entityFileConvert.Path
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload do arquivo.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, filelink = entity.Path, idFile = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        private async Task<EntityFile> ConvertVideoAsync(Stream inputStream, string contentType, string fileName)
        {
            var entity = new EntityFile();

            var convertedVideoDto = await videoConverter.Convert(inputStream, contentType, fileName, SessionFacade.UsuarioLogado.Usuario.usu_id);
            if (convertedVideoDto is null)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message =
                    "Não foi possível realizar a conversão do vídeo para um tamanho menor. O vídeo origianl será mantido.";
                return entity;
            }

            var uploadConvertedVideo = new UploadModel
            {
                ContentLength = (int)convertedVideoDto.Stream.Length,
                ContentType = VideoConvertedContentType,
                InputStream = null,
                Stream = convertedVideoDto.Stream,
                FileName = convertedVideoDto.FileName,
                VirtualDirectory = ApplicationFacade.VirtualDirectory,
                PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                FileType = EnumFileType.Video,
                UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
            };

            return fileBusiness.Upload(uploadConvertedVideo);
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, EnumFileType fileType)
        {
            EntityFile entity = new EntityFile();

            try
            {
                UploadModel upload = new UploadModel
                {
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    InputStream = null,
                    Stream = file.InputStream,
                    FileName = file.FileName,
                    VirtualDirectory = ApplicationFacade.VirtualDirectory,
                    PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                    FileType = fileType,
                    UsuId = SessionFacade.UsuarioLogado.Usuario.usu_id
                };

                entity = fileBusiness.Upload(upload);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload do arquivo.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, filelink = entity.Path, idFile = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            EntityFile entity = new EntityFile();

            try
            {
                entity = fileBusiness.Delete(id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o arquivo.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteVideoAsync(long id, long? convertedFileId)
        {
            EntityFile entity = new EntityFile();

            try
            {
                entity = fileBusiness.Delete(id);

                if (convertedFileId != null)
                    fileBusiness.Delete(convertedFileId.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o arquivo.";
                LogFacade.SaveError(ex);
            }

            return await Task.FromResult(Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public JsonResult LogicalDelete(long id)
        {
            EntityFile entity = new EntityFile();

            try
            {
                entity = fileBusiness.LogicalDelete(id, SessionFacade.UsuarioLogado.Usuario.usu_id, SessionFacade.UsuarioLogado.Grupo.vis_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o arquivo.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}