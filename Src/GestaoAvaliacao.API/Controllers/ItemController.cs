using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Controllers
{
    [CustomAuthorization]
    public class ItemController : ApiController
    {
        private readonly IItemBusiness itemBusiness;
        private readonly IFileBusiness fileBusiness;

        public ItemController(IItemBusiness itemBusiness, IFileBusiness fileBusiness)
        {
            this.itemBusiness = itemBusiness;
            this.fileBusiness = fileBusiness;
        }

        [Route("api/Item")]
        [HttpPost]
        [ResponseType(typeof(ItemApiResult))]
        public HttpResponseMessage ItemSave([FromBody] ItemModel model)
        {
            ItemApiResult itemResult = new ItemApiResult();
            try
            {
                var files = new List<File>();
                if (model.Pictures != null && model.Pictures.Count > 0)
                {
                    foreach (var picture in model.Pictures)
                    {
                        switch (picture.Type)
                        {
                            case PictureType.BaseText:
                                if (model.BaseText.Description.Contains(picture.Tag))
                                {
                                    string tabImg = UploadPictureTagImg(EnumFileType.BaseText, files, picture);
                                    model.BaseText.Description = model.BaseText.Description.Replace(picture.Tag, tabImg);
                                }
                                break;
                            case PictureType.Statement:
                                if (model.Statement.Contains(picture.Tag))
                                {
                                    string tabImg = UploadPictureTagImg(EnumFileType.Statement, files, picture);
                                    model.Statement = model.Statement.Replace(picture.Tag, tabImg);
                                }
                                break;

                            case PictureType.Alternative:
                                foreach (var alternative in model.Alternatives)
                                {
                                    if (alternative.Description.Contains(picture.Tag))
                                    {
                                        string tabImg = UploadPictureTagImg(EnumFileType.Alternative, files, picture);
                                        alternative.Description = alternative.Description.Replace(picture.Tag, tabImg);
                                    }
                                }
                                break;
                            case PictureType.Justificative:
                                foreach (var alternative in model.Alternatives)
                                {
                                    if (alternative.Justificative.Contains(picture.Tag))
                                    {
                                        string tabImg = UploadPictureTagImg(EnumFileType.Justificative, files, picture);
                                        alternative.Justificative = alternative.Justificative.Replace(picture.Tag, tabImg);
                                    }
                                }
                                break;
                        }
                    }
                }

                Item item = new Item()
                {
                    ItemCodeVersion = model.ItemCodeVersion,
                    Statement = model.Statement,
                    descriptorSentence = model.DescriptorSentence,
                    proficiency = model.Proficiency,
                    EvaluationMatrix_Id = model.EvaluationMatrix_Id,
                    Keywords = model.Keywords,
                    Tips = model.Tips,
                    TRICasualSetting = model.TRICasualSetting,
                    TRIDifficulty = model.TRIDifficulty,
                    TRIDiscrimination = model.TRIDiscrimination,
                    BaseText = new BaseText()
                    {
                        Description = model.BaseText.Description,
                        Source = model.BaseText.Description
                    },
                    ItemSituation_Id = model.ItemSituation_Id,
                    ItemType_Id = model.ItemType_Id,
                    ItemLevel_Id = model.ItemLevel_Id,
                    ItemCode = model.ItemCode,
                    ItemVersion = model.ItemVersion,
                    ItemCurriculumGrades = new List<ItemCurriculumGrade>()
                    {
                        new ItemCurriculumGrade() {
                            TypeCurriculumGradeId = model.TypeCurriculumGradeId
                        }
                    },
                    ItemSkills = model.ItemSkills.Select(t => new ItemSkill()
                    {
                        Skill_Id = t
                    }).ToList(),
                    Alternatives = model.Alternatives.Select(t => new Alternative()
                    {
                        Description = t.Description,
                        Correct = t.Correct,
                        Order = t.Order,
                        Justificative = t.Justificative,
                        Numeration = t.Numeration
                    }).ToList(),
                    IsRestrict = model.IsRestrict,
                    ItemNarrated = model.ItemNarrated,
                    StudentStatement = model.StudentStatement,
                    NarrationStudentStatement = model.NarrationStudentStatement,
                    NarrationAlternatives = model.NarrationAlternatives,
                    KnowledgeArea_Id = model.KnowledgeArea_Id,
                    SubSubject_Id = model.SubSubject_Id
                };

                var entity = itemBusiness.Save(0, item, files);

                itemResult.success = entity.Validate.IsValid;
                itemResult.type = entity.Validate.Type.ToString();
                itemResult.message = entity.Validate.Message;
                itemResult.item_id = entity.Id;
            }
            catch (Exception ex)
            {
                itemResult.success = false;
                itemResult.type = ValidateType.error.ToString();
                itemResult.message = "Erro ao salvar item.";

                LogFacade.SaveError(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, itemResult);
        }

        private string UploadPictureTagImg(EnumFileType type, List<File> files, PictureModel picture)
        {
            UploadModel upload = new UploadModel
            {
                ContentLength = picture.ContentLength,
                ContentType = picture.ContentType,
                InputStream = picture.InputStream,
                Stream = null,
                FileName = picture.FileName,
                VirtualDirectory = ApplicationFacade.VirtualDirectory,
                PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
                FileType = type,
                UsuId = this.UserId()
            };

            var file = fileBusiness.Upload(upload);
            files.Add(file);


            var tabImg = $"<img src='{file.Path}' id='{file.Id}'>";
            return tabImg;
        }
    }
}