using GestaoAvaliacao.Dtos.ItemApi;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IPDFConverter;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Extensions;
using GestaoAvaliacao.Util.Videos;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class ItemBusiness : IItemBusiness
    {
        private readonly IItemRepository itemRepository;
        private readonly IAlternativeRepository alternativeRepository;
        private readonly IStorage storage;
        private readonly IFileRepository fileRepository;
        private readonly IParameterBusiness parambusiness;
        private readonly IBaseTextRepository baseTextRepository;
        private readonly IHTMLToPDF htmltopdf;
        private readonly IItemTypeRepository itemTypeRepository;
        private readonly IGenerateHtmlBusiness generateHtmlBusiness;
        private readonly IKnowledgeAreaRepository knowledgeAreaRepository;
        private readonly IDisciplineRepository disciplineRepository;
        private readonly IEvaluationMatrixRepository evaluationMatrixRepository;
        private readonly ISkillRepository skillRepository;
        private readonly ISubjectRepository subjectRepository;
        private readonly IEvaluationMatrixCourseCurriculumGradeBusiness evaluationMatrixCourseCurriculumBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IItemSituationBusiness itemSituationRepository;
        private readonly IFileBusiness fileBusiness;
        private readonly IItemLevelRepository itemLevelRepository;
        private readonly IVideoConverter videoConverter;
        private readonly IACA_TipoNivelEnsinoRepository levelEducationRepository;
        private readonly IItemFileBusiness itemFileBusiness;
        private readonly IItemAudioBusiness itemAudioBusiness;

        const string RESPOSTA_CONSTRUIDA = "Resposta construída";


        private const string VideoConvertedContentType = "video/webm";
        const string TIPO_IMAGENS_PERMITIDOS = "image/jpeg, image/png, image/gif, image/bmp";
        const string TIPO_VIDEOS_PERMITIDOS = "video/mp4, video/webm, video/ogg, application/ogg, video/x-flv, application/x-mpegURL, video/MP2T, video/3gpp, video/quicktime, video/x-msvideo, video/x-ms-wmv";
        const string TIPO_AUDIO_PERMITIDOS = "audio/mpeg, audio/mp4, audio/mp3, audio/vnd.wav, audio/x-ms-wma, audio/ogg";

        public ItemBusiness(IItemRepository itemRepository, IAlternativeRepository alternativeRepository,
                            IStorage storage, IFileRepository fileRepository,
                            IParameterBusiness parambusiness, IBaseTextRepository baseTextRepository,
                            IHTMLToPDF htmltopdf, IItemTypeRepository itemTypeRepository,
                            IGenerateHtmlBusiness generateHtmlBusiness,
                            IKnowledgeAreaRepository knowledgeAreaRepository,
                            IDisciplineRepository disciplineRepository,
                            IEvaluationMatrixRepository evaluationMatrixRepository,
                            ISkillRepository skillRepository,
                            ISubjectRepository subjectRepository,
                            IEvaluationMatrixCourseCurriculumGradeBusiness evaluationMatrixCourseCurriculumBusiness,
                            IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
                            IItemSituationBusiness itemSituationRepository,
                            IFileBusiness fileBusiness,
                            IItemLevelRepository itemLevelRepository,
                            IVideoConverter videoConverter,
                            IACA_TipoNivelEnsinoRepository levelEducationRepository,
                            IItemFileBusiness itemFileBusiness,
                            IItemAudioBusiness itemAudioBusiness
            )
        {
            this.itemRepository = itemRepository;
            this.alternativeRepository = alternativeRepository;
            this.storage = storage;
            this.fileRepository = fileRepository;
            this.parambusiness = parambusiness;
            this.baseTextRepository = baseTextRepository;
            this.htmltopdf = htmltopdf;
            this.itemTypeRepository = itemTypeRepository;
            this.generateHtmlBusiness = generateHtmlBusiness;
            this.knowledgeAreaRepository = knowledgeAreaRepository;
            this.disciplineRepository = disciplineRepository;
            this.evaluationMatrixRepository = evaluationMatrixRepository;
            this.skillRepository = skillRepository;
            this.subjectRepository = subjectRepository;
            this.evaluationMatrixCourseCurriculumBusiness = evaluationMatrixCourseCurriculumBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.itemSituationRepository = itemSituationRepository;
            this.fileBusiness = fileBusiness;
            this.itemLevelRepository = itemLevelRepository;
            this.videoConverter = videoConverter;
            this.levelEducationRepository = levelEducationRepository;
            this.itemFileBusiness = itemFileBusiness;
            this.itemAudioBusiness = itemAudioBusiness;
        }

        #region Custom

        private Validate Validate(Item entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;

            if (action == ValidateAction.Save || action == ValidateAction.Update)
            {
                if (entity.ItemType_Id <= 0
                    || entity.ItemSkills == null || (entity.ItemSkills != null && entity.ItemSkills.Count <= 0)
                    || entity.ItemCurriculumGrades == null || (entity.ItemCurriculumGrades != null && entity.ItemCurriculumGrades.Count <= 0)
                    || entity.EvaluationMatrix_Id <= 0)
                    valid.SetErrorMessage("Não foram preenchidos todos os campos obrigatórios.");


                if (valid.IsValid)
                {
                    //id da página de item = 2
                    var parameters = parambusiness.GetParamsByPage(2);
                    parameters = parameters.Where(p => p.Obligatory == true).ToList();
                    var filled = true;

                    var listTemp = entity.Alternatives != null ? entity.Alternatives.Where(a => a.State == (byte)EnumState.ativo).ToList() : new List<Alternative>();
                    ItemType itemType = null;
                    if (entity.ItemType_Id > 0)
                        itemType = itemTypeRepository.Get(entity.ItemType_Id);

                    foreach (var par in parameters)
                    {
                        switch (par.Key)
                        {
                            case "BASETEXT":
                                if (string.IsNullOrEmpty(entity.BaseText.Description)) filled = false;
                                break;
                            case "SOURCE":
                                if (string.IsNullOrEmpty(entity.BaseText.Description) || string.IsNullOrEmpty(entity.BaseText.Source)) filled = false;
                                break;
                            case "BASETEXT_ORIENTATION":
                                if ((entity.ItemNarrated != null && (bool)entity.ItemNarrated) && (string.IsNullOrEmpty(entity.BaseText.Description) || string.IsNullOrEmpty(entity.BaseText.BaseTextOrientation))) filled = false;
                                break;
                            case "INITIAL_ORIENTATION":
                                if (entity.ItemNarrated != null && (bool)entity.ItemNarrated && string.IsNullOrEmpty(entity.BaseText.InitialOrientation)) filled = false;
                                break;
                            case "INITIAL_STATEMENT":
                                if (entity.ItemNarrated != null && (bool)entity.ItemNarrated && string.IsNullOrEmpty(entity.BaseText.InitialStatement)) filled = false;
                                break;
                            case "DESCRIPTORSENTENCE":
                                if ((par.State != (byte)EnumState.inativo) && (string.IsNullOrEmpty(entity.descriptorSentence))) filled = false;
                                break;
                            case "KEYWORDS":
                                if (string.IsNullOrEmpty(entity.Keywords)) filled = false;
                                break;
                            case "PROFICIENCY":
                                if (entity.proficiency == null) filled = false;
                                break;
                            case "ITEMLEVEL":
                                if (entity.ItemLevel_Id == null) filled = false;
                                break;
                            case "STATEMENT":
                                if (string.IsNullOrEmpty(entity.Statement)) filled = false;
                                break;
                            case "TRI":
                                if (entity.TRICasualSetting == null || entity.TRIDifficulty == null || entity.TRIDiscrimination == null) filled = false;
                                break;
                            case "TIPS":
                                if (string.IsNullOrEmpty(entity.Tips)) filled = false;
                                break;
                            case "ALTERNATIVES":
                                if (!ValidateAlternatives(listTemp, itemType))
                                    filled = false;
                                break;
                            case "TCT":
                                if (listTemp == null || (listTemp != null && listTemp.Count <= 0) ||
                                    (listTemp != null && listTemp.Count > 0 && listTemp.Any(a => a.TCTBiserialCoefficient == null)) ||
                                    (listTemp != null && listTemp.Count > 0 && listTemp.Any(a => a.TCTDificulty == null)) ||
                                    (listTemp != null && listTemp.Count > 0 && listTemp.Any(a => a.TCTDiscrimination == null)))
                                    filled = false;
                                break;
                            case "JUSTIFICATIVE":
                                if (!ValidateJustificatives(listTemp, itemType))
                                    filled = false;
                                break;
                            case "CODE":
                                if (entity.ItemCode.IsNullOrEmptyOrWhiteSpace())
                                    filled = false;
                                break;
                        }
                        if (!filled) break;
                    }

                    if (!filled)
                        valid.Message += "<br/>Não foram preenchidos todos os campos obrigatórios.";

                    if (entity.BaseText != null
                        && (entity.Id > 0 && (entity.BaseText.Id <= 0 || string.IsNullOrEmpty(entity.BaseText.Description))
                        || (entity.Id <= 0 && string.IsNullOrEmpty(entity.BaseText.Description))))
                    {
                        Parameter paramBaseText = parambusiness.GetParamsByPage(2).FirstOrDefault(i => i.Key.Equals("BASETEXT"));
                        if (paramBaseText != null && (!string.IsNullOrEmpty(entity.BaseText.Source) || (entity.ItemNarrated != null && (bool)entity.ItemNarrated && !string.IsNullOrEmpty(entity.BaseText.BaseTextOrientation))))
                        {
                            valid.Message += string.Format("<br/>O campo {0} não pode ficar em branco, pois existe(m) campo(s) preenchido(s) referente(s) a(ao) {0}.<br/>Por favor, verifique.", paramBaseText.Value);
                        }
                    }

                    if (!entity.ItemCode.IsNullOrEmptyOrWhiteSpace())
                    {
                        bool codeAlreadyExists;

                        if (action == ValidateAction.Update)
                            codeAlreadyExists = itemRepository.VerifyItemCodeAlreadyExists(entity.ItemCode, entity.Id);
                        else
                            codeAlreadyExists = itemRepository.VerifyItemCodeAlreadyExists(entity.ItemCode);

                        if (codeAlreadyExists)
                            valid.Message += "<br/>O código do item já existe.";
                    }
                }
            }

            if (action == ValidateAction.Delete)
            {
                Item ent = _GetItemById(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o item a ser excluído.";
                    valid.Code = 404;
                }

                if (ExistsItemBlock(entity.Id))
                    valid.Message += "<br/>Não foi possível excluir o item, pois ele está vinculado a uma ou mais provas.";
            }

            if (!string.IsNullOrEmpty(valid.Message))
            {
                string br = "<br/>";
                valid.Message = valid.Message.TrimStart(br.ToCharArray());

                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }

        private bool ValidateAlternatives(List<Alternative> Alternatives, ItemType itemType)
        {
            if (itemType.Description.Contains(RESPOSTA_CONSTRUIDA))
                return true;

            if (Alternatives == null || (Alternatives != null && Alternatives.Count == 0))
                return false;

            var listTemp = Alternatives.Where(a => a.State == (byte)EnumState.ativo);

            return listTemp.Any(a => a.Correct) &&
                listTemp.Count() == itemType.QuantityAlternative &&
                !listTemp.Any(a => string.IsNullOrEmpty(a.Description));
        }

        private bool ValidateJustificatives(List<Alternative> Alternatives, ItemType itemType)
        {
            if (Alternatives == null || (Alternatives != null && Alternatives.Count == 0))
                return false;

            var listTemp = Alternatives.Where(a => a.State == (byte)EnumState.ativo);

            return listTemp.Any(a => a.Correct) &&
                listTemp.Count() == itemType.QuantityAlternative &&
                !listTemp.Any(a => string.IsNullOrEmpty(a.Justificative));
        }

        private bool ValidateTCT(List<Alternative> Alternatives, ItemType itemType)
        {
            if (Alternatives == null || (Alternatives != null && Alternatives.Count == 0))
                return false;

            var listTemp = Alternatives.Where(a => a.State == (byte)EnumState.ativo);

            return listTemp.Any(a => a.Correct) &&
                listTemp.Count() == itemType.QuantityAlternative &&
                !listTemp.Any(a => string.IsNullOrEmpty(a.Justificative));
        }

        private bool ValidateVersioning(Item entity)
        {
            Item item = itemRepository._GetItemById(entity.Id);

            if (item.ItemSituation.AllowVersion)
            {
                if (item.ItemSituation.Id != entity.ItemSituation.Id) return true;
                //id da página de item = 2
                var parameters = parambusiness.GetParamsByPage(2);

                parameters = parameters.Where(p => p.Versioning == true).ToList();

                foreach (var par in parameters)
                {
                    switch (par.Key)
                    {
                        case "DESCRIPTORSENTENCE":
                            if (item.descriptorSentence != entity.descriptorSentence) return true;
                            break;

                        case "ITEMTYPE":
                            if (item.ItemType.Id != entity.ItemType_Id) return true;
                            break;

                        case "ITEMCURRICULUMGRADE":
                            var groupTypes = new HashSet<int>(item.ItemCurriculumGrades.Select(x => x.TypeCurriculumGradeId));
                            var filteredTypes = entity.ItemCurriculumGrades.Where(x => !groupTypes.Contains(x.TypeCurriculumGradeId)).ToList();
                            if (filteredTypes.Count > 0) return true;
                            break;

                        case "KEYWORDS":
                            if (!GestaoAvaliacao.Util.Compare.ValidateEqualsString(item.Keywords, entity.Keywords)) return true;
                            break;

                        case "PROFICIENCY":
                            if (item.proficiency != entity.proficiency) return true;
                            break;

                        case "ITEMLEVEL":
                            if (GestaoAvaliacao.Util.Compare.ValidateEqualsInt(item.ItemLevel_Id, entity.ItemLevel_Id))
                            {
                                break;
                            }
                            else
                            {
                                return true;
                            }

                        case "STATEMENT":
                            if (item.Statement != entity.Statement) return true;
                            break;

                        case "TRI":
                            if (GestaoAvaliacao.Util.Compare.ValidateEqualsDecimal(item.TRICasualSetting, entity.TRICasualSetting) && GestaoAvaliacao.Util.Compare.ValidateEqualsDecimal(item.TRIDifficulty, entity.TRIDifficulty) && GestaoAvaliacao.Util.Compare.ValidateEqualsDecimal(item.TRIDiscrimination, item.TRIDiscrimination))
                            {
                                break;
                            }
                            else
                            {
                                return true;
                            }
                        case "TIPS":
                            if (item.Tips != entity.Tips) return true;
                            break;

                        case "ALTERNATIVES":
                            for (int i = 0; i < item.Alternatives.Count; i++)
                                if (item.Alternatives[i].Description != entity.Alternatives[i].Description || item.Alternatives[i].Correct != entity.Alternatives[i].Correct)
                                    return true;
                            break;

                        case "TCT":
                            for (int i = 0; i < item.Alternatives.Count; i++)
                            {
                                if (item.Alternatives[i].TCTBiserialCoefficient != entity.Alternatives[i].TCTBiserialCoefficient ||
                                    item.Alternatives[i].TCTDificulty != entity.Alternatives[i].TCTDificulty ||
                                    item.Alternatives[i].TCTDiscrimination != entity.Alternatives[i].TCTDiscrimination) return true;
                            }
                            break;

                        case "JUSTIFICATIVE":
                            for (int i = 0; i < item.Alternatives.Count; i++)
                                if (item.Alternatives[i].Justificative != entity.Alternatives[i].Justificative) return true;
                            break;

                        case "ISRESTRICT":
                            if (item.IsRestrict != entity.IsRestrict) return true;
                            break;
                        case "NIVEISMATRIZ":
                            var itemskills = item.ItemSkills.Where(s => s.State == (Byte)EnumState.ativo);
                            if (itemskills != null && itemskills.Any())
                            {
                                var groupSkills = new HashSet<long>(itemskills.Where(x => x.Skill != null).Select(x => x.Skill.Id));
                                var filteredSkills = entity.ItemSkills.Where(x => x.Skill != null && !groupSkills.Contains(x.Skill.Id)).ToList();
                                if (filteredSkills.Count > 0) return true;
                            }
                            break;
                    }
                }
            }

            return false;
        }

        private bool ValidateBaseText(Item entity, bool updated)
        {
            var parameters = parambusiness.GetParamsByPage(2);

            if (!updated)
            {
                parameters = parameters.Where(p => p.Versioning != null && (bool)p.Versioning).ToList();
            }

            BaseText registeredBaseText = baseTextRepository.Get(entity.BaseText.Id);

            foreach (var par in parameters)
            {
                switch (par.Key)
                {
                    case "BASETEXT":
                        if ((registeredBaseText == null && entity.BaseText.Description != null) || (registeredBaseText != null && registeredBaseText.Description != entity.BaseText.Description))
                        {
                            return true;
                        }
                        break;

                    case "SOURCE":
                        if ((registeredBaseText == null && entity.BaseText.Source != null) || (registeredBaseText != null && registeredBaseText.Source != entity.BaseText.Source))
                        {
                            return true;
                        }
                        break;

                    case "INITIAL_ORIENTATION":
                        if ((entity.ItemNarrated != null && (bool)entity.ItemNarrated)
                            && ((registeredBaseText == null && entity.BaseText.InitialOrientation != null) || (registeredBaseText != null && registeredBaseText.InitialOrientation != entity.BaseText.InitialOrientation)))
                        {
                            return true;
                        }
                        break;

                    case "INITIAL_STATEMENT":
                        if ((entity.ItemNarrated != null && (bool)entity.ItemNarrated)
                            && ((registeredBaseText == null && entity.BaseText.InitialStatement != null) || (registeredBaseText != null && registeredBaseText.InitialStatement != entity.BaseText.InitialStatement)))
                        {
                            return true;
                        }

                        break;

                    case "BASETEXT_ORIENTATION":
                        if ((entity.ItemNarrated != null && (bool)entity.ItemNarrated)
                            && ((registeredBaseText == null && entity.BaseText.BaseTextOrientation != null) || (registeredBaseText != null && registeredBaseText.BaseTextOrientation != entity.BaseText.BaseTextOrientation)))
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        #endregion

        #region Read

        public Item _GetMatrixBytem(long Id)
        {
            return itemRepository._GetMatrixBytem(Id);
        }

        public Item _GetSimpleMatrixBytem(long Id)
        {
            return itemRepository._GetSimpleMatrixBytem(Id);
        }

        public Item _GetBaseTextBytem(long Id)
        {
            return itemRepository._GetBaseTextBytem(Id);
        }

        public Item _GetItemById(long Id)
        {
            return itemRepository._GetItemById(Id);
        }

        public Item GetItemByItemCode(string itemCode)
        {
            return itemRepository.GetItemByItemCode(itemCode);
        }

        public Item _GetGradeByItem(long Id)
        {
            return itemRepository._GetGradeByItem(Id);
        }

        public Item _GetItemSummaryById(long Id)
        {
            return itemRepository._GetItemSummaryById(Id);
        }

        public List<Item> GetItemSummaryById(List<long> idsTest, List<long> ids)
        {
            return itemRepository.GetItemSummaryById(idsTest, ids).ToList();
        }

        public Item _GetAddItemInfos(long Id)
        {
            return itemRepository._GetAddItemInfos(Id);
        }

        public IEnumerable<Item> GetItems(List<long> ItemIds)
        {
            return itemRepository.GetItems(ItemIds);
        }

        public IEnumerable<Item> _GetItemVersions(int itemCodeVersion)
        {
            return itemRepository._GetItemVersions(itemCodeVersion);
        }

        public IEnumerable<Item> GetItemVersions(int itemCodeVersion, int itemVersion)
        {
            return itemRepository.GetItemVersions(itemCodeVersion, itemVersion);
        }

        public IEnumerable<Item> _GetItemsByBaseText(long baseTextId)
        {
            return itemRepository._GetItemsByBaseText(baseTextId);
        }

        public IEnumerable<ItemGroupBaseText> _GetItemGroupBaseTexts(bool? lastVersion = null, params long[] baseTextId)
        {
            return itemRepository._GetItemGroupBaseTexts(lastVersion, baseTextId);
        }

        public IEnumerable<ItemResult> _SearchItems(ItemFilter filter, ref Pager pager)
        {
            return itemRepository._SearchItems(filter, ref pager);
        }

        public List<ItemReportItemType> _GetItemType(int Id, int situacao, Guid EntityId, long typeLevelEducation)
        {
            return itemRepository._GetItemType(Id, situacao, EntityId, typeLevelEducation);
        }

        public List<ItemReportItemLevel> _GetItemLevel(int Id, int situacao, Guid EntityId, long typeLevelEducation)
        {
            return itemRepository._GetItemLevel(Id, situacao, EntityId, typeLevelEducation);
        }

        public List<ItemReportItem> _GetItem(Guid EntityId, long TypeLevelEducation)
        {
            return itemRepository._GetItem(EntityId, TypeLevelEducation);
        }

        public List<ItemReportItemCurriculumGrade> _GetItemCurriculumGrade(int Id, int situacao, Guid EntityId, long typeLevelEducation)
        {
            return itemRepository._GetItemCurriculumGrade(Id, situacao, EntityId, typeLevelEducation);
        }

        public List<ItemReportItemSituation> _GetItemSituation(int Id, string inicio, string fim, Guid EntityId, long typeLevelEducation)
        {
            return itemRepository._GetItemSituation(Id, inicio, fim, EntityId, typeLevelEducation);
        }

        public IEnumerable<ItemBlockResult> _BlockSearchItem(BlockItemFilter filter, ref Pager pager)
        {
            return itemRepository._BlockSearchItem(filter, ref pager);
        }

        private bool VerifyVersionByBaseText(Item entity)
        {
            if (ValidateBaseText(entity, false))
            {
                //Caso tenha sido alterado, verificar se está ligado a outros items que estejam aprovados
                var items = itemRepository._GetItemsByBaseText(entity.BaseText.Id);

                if (items.Count() > 0)
                    return items.Where(i => "Aceito".Equals(i.ItemSituation.Description)).Count() > 0;
                else
                    return false;
            }
            else
                return false;
        }

        public bool ExistsItemBlock(long Id)
        {
            return itemRepository.ExistsItemBlock(Id);
        }

        private Item DuplicateItem(Item entity)
        {
            var _entity = itemRepository.Get(entity.Id);

            _entity.Id = 0;
            _entity.LastVersion = true;
            _entity.ItemVersion++;


            return _entity;
        }

        public byte[] GetItemPreview(long id, string url, List<Parameter> parameters)
        {
            List<Item> items = new List<Item>();
            items.Add(itemRepository.GetToPreview(id));

            PDFFilter filter = new PDFFilter
            {
                ItemList = items,
                FontSize = 20,
                UrlSite = url,
                GenerateType = EnumGenerateType.Items,
                CDNMathJax = bool.Parse(parameters.First(p => p.Key.Equals("UTILIZACDNMATHJAX")).Value),
                SaveToDB = false
            };
            string html = generateHtmlBusiness.GetHtmlFromItems(filter);

            return htmltopdf.ConvertHtml(html, "PreviewItems", (float)18.34, (float)18.34, (float)18.34, (float)18.34, 0, 0);
        }

        public byte[] GetItemPreviewByBaseText(long id, string url, List<Parameter> parameters)
        {
            List<Item> items = itemRepository.GetToPreviewByBaseText(id).ToList();

            PDFFilter filter = new PDFFilter
            {
                ItemList = items,
                FontSize = 20,
                UrlSite = url,
                GenerateType = EnumGenerateType.Items,
                CDNMathJax = bool.Parse(parameters.First(p => p.Key.Equals("UTILIZACDNMATHJAX")).Value),
                SaveToDB = false
            };
            string html = generateHtmlBusiness.GetHtmlFromItems(filter);

            return htmltopdf.ConvertHtml(html, "PreviewItems", (float)18.34, (float)18.34, (float)18.34, (float)18.34, 0, 0);
        }

        #endregion

        #region Write

        public Item Save(long Id, Item entity, List<EntityFile> files = null)
        {
            Item _entity = new Item();
            _entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);

            if (_entity.Validate.IsValid)
            {
                var itemCodeVersion = itemRepository.GetMaxCode() + 1;

                if (entity.ItemCode.IsNullOrEmptyOrWhiteSpace())
                    entity.ItemCode = itemCodeVersion.ToString();

                entity.ItemCodeVersion = itemCodeVersion;
                entity.LastVersion = true;
                entity.ItemVersion = 1;
                _entity = RuleUpdateBaseText(entity);

                entity.BaseText_Id = _entity.BaseText_Id;
                entity.BaseText = null;

                entity.Alternatives = entity.Alternatives.Where(a => a.State == (byte)EnumState.ativo).ToList();

                itemRepository.Save(entity);

                _entity.Validate.Type = ValidateType.Save.ToString();
                _entity.Validate.Message = "Item salvo com sucesso.";

                #region Files

                EntityFile entityFile = new EntityFile();
                if (files != null)
                {
                    entityFile = SaveFiles(Id, entity, _entity, files);
                    if (!entityFile.Validate.IsValid)
                        _entity.Validate = entityFile.Validate;
                }

                #endregion Files
            }

            return _entity;
        }

        public bool VerifyItemCodeAlreadyExists(string itemCode, long? itemId = null)
        {
            return itemRepository.VerifyItemCodeAlreadyExists(itemCode, itemId);
        }

        public Item Update(long Id, Item entity, List<EntityFile> files = null)
        {
            Item oldItem = entity.ShalowCopy();
            Item _entity = new Item();
            _entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);

            if (_entity.Validate.IsValid)
            {
                _entity = RuleUpdateBaseText(entity);

                if (entity.ItemCode.IsNullOrEmptyOrWhiteSpace())
                    entity.ItemCode = entity.ItemCodeVersion.ToString();

                //caso não tenha sido versionado devido ao base Text
                if (_entity.ItemVersion == entity.ItemVersion)
                {
                    //verifica se precisa versionar devido aos outros campos
                    if (ValidateVersioning(entity))
                    {
                        itemRepository.UpdateVersion(Id);
                        entity.Alternatives = entity.Alternatives.Where(i => i.State == (Byte)EnumState.ativo).ToList();
                        entity.Id = 0;
                        entity.ItemVersion = itemRepository.GetMaxVersionByItemCode(entity.ItemCodeVersion) + 1;
                        entity.ItemSituation = null;
                        entity.BaseText_Id = _entity.BaseText.Id > 0 ? _entity.BaseText.Id : new long?();
                        entity.BaseText = null;
                        entity.LastVersion = true;

                        _entity = itemRepository.Save(entity);
                    }
                    else
                    {
                        //Troca do tipo de item
                        long[] alternativesExcluded = entity.Alternatives.Where(i => i.State == (Byte)EnumState.excluido && i.Id > 0).Select(i => i.Id).ToArray();
                        foreach (long alternativeId in alternativesExcluded)
                        {
                            alternativeRepository.Delete(alternativeId);
                        }
                        entity.Alternatives = entity.Alternatives.Where(i => i.State == (Byte)EnumState.ativo).ToList();
                        entity.BaseText = null;
                        entity.ItemSituation = null;
                        _entity = itemRepository.Update(entity);
                    }
                }
                else
                {
                    foreach (Alternative alternative in entity.Alternatives.Where(i => i.State == (Byte)EnumState.ativo).ToList())
                    {
                        _entity.Alternatives.FirstOrDefault(i => i.Order == alternative.Order).Correct = alternative.Correct;
                        _entity.Alternatives.FirstOrDefault(i => i.Order == alternative.Order).Justificative = alternative.Justificative;
                        _entity.Alternatives.FirstOrDefault(i => i.Order == alternative.Order).Description = alternative.Description;
                        _entity.Alternatives.FirstOrDefault(i => i.Order == alternative.Order).Numeration = alternative.Numeration;
                    }

                    entity.Alternatives = _entity.Alternatives;
                    entity.Id = _entity.Id;
                    entity.ItemVersion = _entity.ItemVersion;
                    entity.BaseText_Id = _entity.BaseText_Id;
                    entity.BaseText = null;
                    _entity = itemRepository.Update(entity);
                }

                _entity.Validate.Type = ValidateType.Update.ToString();
                _entity.Validate.Message = "Item alterado com sucesso.";

                #region Files

                EntityFile entityFile = new EntityFile();
                if (files != null)
                {
                    entityFile = SaveFiles(Id, oldItem, _entity, files);
                    if (!entityFile.Validate.IsValid)
                        _entity.Validate = entityFile.Validate;
                }

                #endregion              
            }

            return _entity;
        }

        private Item RuleUpdateBaseText(Item entity)
        {
            Item retorno = entity;

            if (entity.BaseText == null)
            {
                entity.BaseText = new BaseText();
            }

            //Verifica se é um Base text novo, caso for cria e insere o item
            if (entity.BaseText.Id <= 0)
            {
                BaseText bText = entity.BaseText;
                bText.Id = 0;

                bText = baseTextRepository.Save(entity.BaseText);

                retorno.BaseText_Id = bText.Id;
            }
            else if (entity.BaseText.Id > 0)
            {
                //Verifica se texto base foi alterado
                if (ValidateBaseText(entity, true))
                {
                    //Se foi alterado, verifica se é necessário versionar texto base
                    if (VerifyVersionByBaseText(entity))
                    {
                        var oldBaseText = entity.BaseText.Id;
                        BaseText bText = entity.BaseText;
                        bText.Id = 0;

                        bText = baseTextRepository.Save(entity.BaseText);
                        entity.BaseText = null;
                        entity.BaseText_Id = bText.Id;

                        /*Executar a seguinte regra
						* 1 - Criar um novo texto base, para manter o antigo nos ítens ativos
						* 2 - Aplicar nos ítems (método VersioningItemsToNewBaseText)
						*		a. Caso o ítem esteja ativo, criar uma nova versão apontando para o novo texto base
						*		b. Caso o ítem esteja pendente, apenas apontar para o novo text base
						*/
                        var newItem = VersioningItemsToNewBaseText(oldBaseText, bText.Id, entity.ItemCodeVersion);

                        if (newItem != null)
                            retorno = newItem;

                        retorno.BaseText_Id = bText.Id;
                    }
                    else
                    {
                        //Caso não seja necessário versionar, apenas altera o texto base e salva o item
                        BaseText bText = baseTextRepository.Get(entity.BaseText.Id);
                        bText.Description = entity.BaseText.Description;
                        bText.Source = entity.BaseText.Source;
                        bText.InitialOrientation = entity.BaseText.InitialOrientation;
                        bText.InitialStatement = entity.BaseText.InitialStatement;
                        bText.NarrationInitialStatement = entity.BaseText.NarrationInitialStatement;
                        bText.NarrationStudentBaseText = entity.BaseText.NarrationStudentBaseText;
                        bText.StudentBaseText = entity.BaseText.StudentBaseText;

                        baseTextRepository.Update(entity.BaseText);
                        retorno.BaseText_Id = bText.Id;
                    }
                }
                else
                    retorno.BaseText_Id = retorno.BaseText.Id;
            }

            return retorno;
        }

        private Item VersioningItemsToNewBaseText(long oldBaseText, long newBaseText, int? itemCodeVersion = 0)
        {
            Item retorno = null;
            var items = itemRepository._GetItemsByBaseText(oldBaseText);

            foreach (var item in items.Where(i => i.LastVersion))
            {
                Item newItem = item;

                if (item.ItemSituation.AllowVersion)
                {
                    item.LastVersion = false;
                    item.UpdateDate = DateTime.Now;

                    item.ItemSituation = null;
                    itemRepository.UpdateVersion(item.Id);

                    newItem = DuplicateItem(item);
                    newItem.BaseText_Id = newBaseText;

                    itemRepository.Save(newItem);
                }
                else
                {
                    newItem.BaseText_Id = newBaseText;
                    itemRepository.UpdateBaseText(newItem.Id, newBaseText);
                }

                if (itemCodeVersion == newItem.ItemCodeVersion)
                    retorno = newItem;
            }

            return retorno;
        }

        public EntityFile SaveFiles(long Id, Item entity, Item _entity, List<EntityFile> files)
        {
            EntityFile entRetorno = new EntityFile();

            if (_entity != null && _entity.Id > 0)
            {
                List<EntityFile> filesToVersion = new List<EntityFile>();
                if (Id > 0)
                {
                    if (entity.ItemVersion != _entity.ItemVersion)
                    {
                        filesToVersion = files.Where(f => f.ParentOwnerId > 0).ToList();
                        filesToVersion.ToList().ForEach(f => f.ParentOwnerId = _entity.Id);
                        fileRepository.SaveList(filesToVersion);
                    }
                    else
                    {
                        List<EntityFile> filesDB = fileRepository._GetFilesByParent(entity.Id);
                        List<EntityFile> filesToDelete = files != null && files.Count > 0 ? filesDB.Where(f => !files.Select(x => x.Id).Contains(f.Id)).ToList() : filesDB;
                        fileRepository.DeleteList(filesToDelete);
                    }
                }
                if (files != null && files.Count > 0)
                {
                    files.ToList().ForEach(f => f.ParentOwnerId = _entity.Id);
                    files.Where(f => f.OwnerType == (Byte)EnumFileType.Statement).ToList().ForEach(
                        f => f.OwnerId = _entity.Id);

                    if (_entity.BaseText_Id != null && _entity.BaseText_Id.Value > 0)
                        files.Where(f => f.OwnerType == (Byte)EnumFileType.BaseText).ToList().ForEach(
                            f => f.OwnerId = _entity.BaseText_Id.Value);

                    if (_entity.Alternatives != null && _entity.Alternatives.Count > 0)
                        foreach (Alternative alt in _entity.Alternatives)
                        {
                            if (alt != null && alt.Id > 0)
                            {
                                files.Where(
                                    f => f.OwnerType == (Byte)EnumFileType.Alternative && f.OwnerId == alt.Order).
                                    ToList().ForEach(f => f.OwnerId = alt.Id);
                                files.Where(
                                    f => f.OwnerType == (Byte)EnumFileType.Justificative && f.OwnerId == alt.Order).
                                    ToList().ForEach(f => f.OwnerId = alt.Id);
                            }
                        }

                    if (Id > 0 && entity.ItemVersion != _entity.ItemVersion)
                    {
                        List<EntityFile> filesToUpdate =
                            files.Where(f => !filesToVersion.Select(x => x.Id).Contains(f.Id)).ToList();
                        fileRepository.UpdateList(filesToUpdate);
                    }
                    else
                    {
                        fileRepository.UpdateList(files);
                    }
                }
            }

            return entRetorno;
        }

        public Item Delete(long Id)
        {
            Item entity = new Item { Id = Id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);

            if (entity.Validate.IsValid)
            {
                itemRepository.Delete(Id);
                fileRepository.DeleteByParentId(Id, EnumFileType.Alternative);
                fileRepository.DeleteByParentId(Id, EnumFileType.BaseText);
                fileRepository.DeleteByParentId(Id, EnumFileType.Justificative);
                fileRepository.DeleteByParentId(Id, EnumFileType.Statement);

                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Item excluído com sucesso.";
            }

            return entity;
        }

        public Item SaveChangeItem(Item item, long TestId, long itemIdAntigo, long blockId)
        {
            Item entity = new Item { Id = item.Id };

            if (entity.Validate.IsValid)
            {
                itemRepository.SaveChangeItem(item, TestId, itemIdAntigo, blockId);

                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Versão do item alterada com sucesso.";
            }

            return entity;
        }

        public Item SaveChangeBlockChainItem(Item item, long testId, long itemIdAntigo, long blockChainId)
        {
            var entity = new Item { Id = item.Id };

            if (!entity.Validate.IsValid)
                return entity;

            itemRepository.SaveChangeBlockChainItem(item, testId, itemIdAntigo, blockChainId);

            entity.Validate.Type = ValidateType.Delete.ToString();
            entity.Validate.Message = "Versão do item do bloco alterada com sucesso.";

            return entity;
        }

        public EntityFile Upload(Uploader file, string VirtualDirectory, string PhysicalDirectory)
        {
            EntityFile entityFile = new EntityFile();
            if (file != null && file.ContentLength > 0)
            {
                string[] stream = file.InputStream.Split(',');
                byte[] data = System.Convert.FromBase64String(stream.Length > 1 ? stream[1] : stream[0]);
                string name = Guid.NewGuid() + Path.GetExtension(file.FileName);

                EntityFile fileUploaded = storage.Save(data, name, file.ContentType, string.Empty, VirtualDirectory, PhysicalDirectory, out entityFile);
                entityFile.Validate = fileUploaded.Validate;
                if (fileUploaded.Validate.IsValid)
                    fileRepository.Save(fileUploaded);
            }
            else
            {
                entityFile.Validate.IsValid = false;
                entityFile.Validate.Type = ValidateType.alert.ToString();
                entityFile.Validate.Code = 400;
                entityFile.Validate.Message = "Não foi encontrado nenhum arquivo para fazer upload.";
            }

            return entityFile;
        }

        public Item RevokeItem(long Item_Id, bool Revoked)
        {
            Item item = new Item
            {
                Id = Item_Id,
                Revoked = Revoked
            };
            return itemRepository.RevokeItem(item);
        }

        #endregion

        #region ItemsNewApi

        public List<BaseDto> LoadAllKnowledgeAreaActive()
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            return knowledgeAreaRepository.LoadAllKnowledgeAreaActive(string.Empty, new Guid(entidade.Value)).Select(s => new BaseDto
            {
                Id = long.Parse(s.id),
                Descricao = s.text
            }).ToList();
        }

        public List<DisciplineDto> LoadDisciplineByKnowledgeArea(int knowledgeAreas)
        {
            var lista = new List<DisciplineDto>();

            var entidade = parambusiness.GetByKey("ENTIDADE");

            var disciplinas = disciplineRepository.LoadDisciplineByKnowledgeArea(knowledgeAreas.ToString(), new Guid(entidade.Value));
            foreach (var disciplina in disciplinas)
            {
                var nivelEnsino = levelEducationRepository.Get(disciplina.TypeLevelEducationId);

                lista.Add(new DisciplineDto()
                {
                    Id = disciplina.Id,
                    Descricao = disciplina.Description,
                    NivelEnsino = nivelEnsino?.tne_nome
                });
            }

            return lista;
        }

        public List<MatrixDto> LoadMatrixByDiscipline(long idDiscipline)
        {
            return evaluationMatrixRepository.GetComboByDiscipline(idDiscipline).Select(s => new MatrixDto
            {
                Id = s.Id,
                Descricao = s.Description,
                Modelo = s.ModelEvaluationMatrix.Description
            }).ToList();
        }

        public List<SkillDto> LoadSkillByMatrix(long idMatrix)
        {
            var listSkillDto = skillRepository.GetByMatrix(idMatrix)
                .Where(t => t.ModelSkillLevel.Id == 1)
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Descricao = s.Description,
                    Codigo = s.Code
                }).ToList();

            return listSkillDto;
        }

        public List<AbilityDto> LoadAbilityBySkill(long idSkill)
        {
            var listAbilityDto = skillRepository.GetByParent(idSkill)
            .Select(s => new AbilityDto
            {
                Id = s.Id,
                Descricao = s.Description,
                UltimoNivel = s.LastLevel,
                Codigo = s.Code
            }).ToList();
            return listAbilityDto;
        }

        public List<BaseDto> LoadAllSubjects()
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            return subjectRepository.LoadAllSubjects(string.Empty, new Guid(entidade.Value)).Select(s => new BaseDto
            {
                Id = long.Parse(s.id),
                Descricao = s.text
            }).ToList();
        }

        public List<BaseDto> ObterAssuntosPorDisciplina(long DisciplinaId)
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            return subjectRepository.ObterAssuntosPorDisciplinaId(new Guid(entidade.Value), DisciplinaId).Select(s => new BaseDto
            {
                Id = long.Parse(s.id),
                Descricao = s.text
            }).ToList();
        }

        public List<BaseDto> LoadSubsubjectBySubject(string idSubjects)
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            return subjectRepository.LoadSubsubjectBySubject(string.Empty, idSubjects, new Guid(entidade.Value)).Select(s => new BaseDto
            {
                Id = long.Parse(s.id),
                Descricao = s.text
            }).ToList();
        }

        public List<ItemTypeDto> FindForTestType()
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            var list = itemTypeRepository.FindForTestType(new Guid(entidade.Value)).Select(i => new ItemTypeDto
            {
                Id = i.Id,
                Descricao = i.Description,
                EhPadrao = i.IsDefault,
                QuantidadeAlternativa = i.QuantityAlternative
            }).ToList();
            return list;
        }

        public List<CurriculumGradeDto> LoadCurriculumGradesByMatrix(int evaluationMatrixId)
        {
            IEnumerable<ACA_TipoCurriculoPeriodo> listCurriculumGrades = tipoCurriculoPeriodoBusiness.GetAllTypeCurriculumGrades();
            List<EvaluationMatrixCourseCurriculumGrade> list = evaluationMatrixCourseCurriculumBusiness.GetCurriculumGradesByMatrix(evaluationMatrixId);

            if (list != null && list.Count > 0)
            {
                var query = list.Select(i => new CurriculumGradeDto
                {
                    Id = i.TypeCurriculumGradeId,
                    Descricao = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == i.TypeCurriculumGradeId).tcp_descricao,
                    Ordem = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == i.TypeCurriculumGradeId).tcp_ordem
                }).Where(x => x.Id > 0).OrderBy(x => x.Ordem).ToList();
                return query;
            }
            return default;
        }

        public List<ItemLevelDto> LoadAllItemLevel()
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            var list = itemLevelRepository.LoadLevels(new Guid(entidade.Value)).Select(i => new ItemLevelDto()
            {
                Id = i.Id,
                Descricao = i.Description,
                Ordem = i.Value
            }).ToList();

            return list;
        }

        private void ValidateApi(ItemApiDto model, ItemApiResult itemResult)
        {
            itemResult.mensagem = string.Empty;

            if (model.AreaConhecimentoId != 0)
            {
                var knowledgeArea = knowledgeAreaRepository.Get(model.AreaConhecimentoId);
                if (knowledgeArea == null)
                    itemResult.mensagem += "<br/>O id da área de conhecimento é invãlido.";
            }
            else
                itemResult.mensagem += "<br/>O id da área de conhecimento é obrigatório.";


            if (model.MatrizId != 0)
            {
                var matriz = evaluationMatrixRepository.GetByMatriz(model.MatrizId);
                if (!matriz.Any())
                    itemResult.mensagem += "<br/>O id da matriz é inválido.";
            }
            else
                itemResult.mensagem += "<br/>O id da matriz é obrigatório.";

            if (!model.CodigoItem.IsNullOrEmptyOrWhiteSpace())
            {
                bool codigoExiste = itemRepository.VerifyItemCodeAlreadyExists(model.CodigoItem);
                if (codigoExiste)
                    itemResult.mensagem += "<br/>O código do item já existe.";
            }
            else
                itemResult.mensagem += "<br/>O código do item é obrigatório.";

            if (model.Dificuldade != Dificuldade.Nenhum)
            {
                var level = itemLevelRepository.Get((int)model.Dificuldade);
                if (level == null)
                    itemResult.mensagem += "<br/>A dificuldade é inválida [1 - Muito Fácil, 2 - Fácil, 3 - Médio, 4 - Difícil, 5 - Muito Difícil].";
            }
            else
                itemResult.mensagem += "<br/>A dificuldade do item é obrigatório [1 - Muito Fácil, 2 - Fácil, 3 - Médio, 4 - Difícil, 5 - Muito Difícil].";

            ItemType itemType = null;
            if (model.TipoItemId != 0)
            {
                itemType = itemTypeRepository.Get(model.TipoItemId);
                if (itemType == null)
                    itemResult.mensagem += "<br/>O id do tipo do item é inválido.";
            }
            else
                itemResult.mensagem += "<br/>O id do tipo do item é obrigatório.";

            if (model.TipoGradeCurricularId != 0)
            {
                if (model.MatrizId != 0)
                {
                    var grades = evaluationMatrixCourseCurriculumBusiness.GetCurriculumGradesByMatrix((int)model.MatrizId);
                    if (!grades.Any(t => t.TypeCurriculumGradeId == model.TipoGradeCurricularId))
                        itemResult.mensagem += "<br/>O id do tipo de grade curricular é inválido.";
                }
            }
            else
                itemResult.mensagem += "<br/>O id do tipo de grade curricular é obrigatório.";

            if (model.CompetenciaId != 0)
            {
                var listSkillDto = skillRepository.GetByMatrix(model.MatrizId);
                if (!listSkillDto.Any(t => t.Id == model.CompetenciaId && t.ModelSkillLevel.Id == 1))
                    itemResult.mensagem += "<br/>O id da competência é inválido.";
            }
            else
                itemResult.mensagem += "<br/>O id da competência é obrigatório.";

            if (model.HabilidadeId != 0)
            {
                var listAbilityDto = skillRepository.GetByParent(model.CompetenciaId);
                if (listAbilityDto == null || !listAbilityDto.Any(t => t.Id == model.HabilidadeId))
                    itemResult.mensagem += "<br/>O id da habilidade é inválido.";
            }
            else
                itemResult.mensagem += "<br/>O id da habilidade é obrigatório.";

            if (model.SubassuntoId != 0)
            {
                var assunto = subjectRepository.LoadSubjectBySubsubject((long)model.SubassuntoId);
                if (assunto == null)
                    itemResult.mensagem += "<br/>O id do subassunto é inválido.";
            }
            else
                itemResult.mensagem += "<br/>O id do subassunto é obrigatório.";

            if (model.Proficiencia != null)
            {
                if ((int)model.Proficiencia < 100 || (int)model.Proficiencia > 500)
                    itemResult.mensagem += "<br/>Proficiência deve ser de 100 a 500.";
            }

            if (model.Enunciado.IsNullOrEmptyOrWhiteSpace())
                itemResult.mensagem += "<br/>O Enunciado é obrigatório.";

            if (itemType != null && !itemType.Description.Contains(RESPOSTA_CONSTRUIDA))
            {
                if (model.Alternativas != null && model.Alternativas.Any())
                {
                    if (!model.Alternativas.Any(t => t.Correta))
                        itemResult.mensagem += "<br/>O item deve possuir 1 alternativa correta.";

                    if (model.Alternativas.Count(t => t.Correta) > 1)
                        itemResult.mensagem += "<br/>O item deve possuir somente 1 alternativa correta.";

                    if (itemType.QuantityAlternative != model.Alternativas.Count)
                        itemResult.mensagem += $"<br/>O item deve possuir {itemType.QuantityAlternative} alternativas.";

                    if (model.Alternativas.GroupBy(t => t.Ordem).Any(t => t.Count() > 1))
                        itemResult.mensagem += $"<br/>O item não pode conter a ordem duplicada.";

                    if (model.Alternativas.GroupBy(t => t.Numeracao).Any(t => t.Count() > 1))
                        itemResult.mensagem += $"<br/>O item não pode conter a numeração duplicada.";


                    foreach (var alternativa in model.Alternativas)
                    {
                        if (alternativa.Descricao.IsNullOrEmptyOrWhiteSpace())
                            itemResult.mensagem += "<br/>A descrição da alternativa é obrigatório.";

                        if (alternativa.Numeracao.IsNullOrEmptyOrWhiteSpace())
                            itemResult.mensagem += "<br/>A numeração da alternativa é obrigatório.";
                    }

                }
                else
                    itemResult.mensagem += $"<br/>O item deve possuir {itemType.QuantityAlternative} alternativas.";
            }

            if (model.Imagens != null && model.Imagens.Any())
            {
                if (model.Imagens.GroupBy(t => t.Tag).Any(t => t.Count() > 1))
                    itemResult.mensagem += $"<br/>O item não pode conter imagens com a tag duplicada.";

                foreach (var picture in model.Imagens)
                {
                    if (picture.Tag.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>A tag da imagem é obrigatória.";

                    if (picture.Tamanho == 0)
                        itemResult.mensagem += "<br/>O tamanho da imagem é obrigatório.";

                    if (picture.TipoConteudo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O tipo do conteúdo da imagem é obrigatório.";

                    if (picture.Base64.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O arquivo em base64 da imagem é obrigatório.";

                    if (picture.NomeArquivo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O nome do arquivo da imagem é obrigatório.";

                    if (!TIPO_IMAGENS_PERMITIDOS.Contains(picture.TipoConteudo))
                    {
                        itemResult.mensagem += "<br/>Tipo de imagem não permitido. [.jpeg, .png, .gif, .bmp]";
                        continue;
                    }

                    switch (picture.Tipo)
                    {
                        case PictureType.BaseText:
                            if (!model.TextoBase.Contains(picture.Tag))
                                itemResult.mensagem += $"<br/>A tag {picture.Tag} não foi encontrada em texto base.";
                            break;
                        case PictureType.Statement:
                            if (!model.Enunciado.Contains(picture.Tag))
                                itemResult.mensagem += $"<br/>A tag {picture.Tag} não foi encontrada em enunciado.";
                            break;

                        case PictureType.Alternative:
                            var contemTagAlternative = false;
                            foreach (var alternative in model.Alternativas)
                            {
                                if (alternative.Descricao.Contains(picture.Tag)) contemTagAlternative = true;
                            }

                            if (!contemTagAlternative)
                                itemResult.mensagem += $"<br/>A tag {picture.Tag} não foi encontrada em nenhuma alternativa.";
                            break;
                        case PictureType.Justificative:
                            var contemTagJustificative = false;
                            foreach (var alternative in model.Alternativas)
                            {
                                if (alternative.Justificativa.Contains(picture.Tag)) contemTagJustificative = true;
                            }

                            if (!contemTagJustificative)
                                itemResult.mensagem += $"<br/>A tag {picture.Tag} não foi encontrada em nenhuma justificativa das alternativas.";
                            break;
                    }
                }
            }

            if (model.Videos != null && model.Videos.Any())
            {
                foreach (var video in model.Videos)
                {
                    if (video.Tamanho == 0)
                        itemResult.mensagem += "<br/>O tamanho do video é obrigatório.";

                    if (video.TipoConteudo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O tipo do conteúdo do video é obrigatório.";

                    if (video.Base64.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O arquivo em base64 do video é obrigatório.";

                    if (video.NomeArquivo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O nome do arquivo do video é obrigatório.";

                    if (!TIPO_VIDEOS_PERMITIDOS.Contains(video.TipoConteudo))
                        itemResult.mensagem += "<br/>Tipo de video não permitido.";


                    if (video.Tamanho == 0)
                        itemResult.mensagem += "<br/>O tamanho do video é obrigatório.";

                    if (video.TipoConteudo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O tipo do conteúdo do video é obrigatório.";

                    if (video.Base64.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O arquivo em base64 do video é obrigatório.";

                    if (video.NomeArquivo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O nome do arquivo do video é obrigatório.";


                    if (video.MiniaturaTamanho == 0)
                        itemResult.mensagem += "<br/>O tamanho da miniatura do video é obrigatório.";

                    if (video.MiniaturaTipoConteudo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O tipo de conteúdo da miniatura do video é obrigatório.";

                    if (video.MiniaturaBase64.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O arquivo em base64 da miniatura do video é obrigatório.";

                    if (video.NomeArquivo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O nome do arquivo da miniatura do video é obrigatório.";

                    if (!TIPO_IMAGENS_PERMITIDOS.Contains(video.MiniaturaTipoConteudo))
                        itemResult.mensagem += "<br/>Tipo de Imagem não permitido na miniatura do video.";
                }
            }

            if (model.Audios != null && model.Audios.Any())
            {
                foreach (var audio in model.Audios)
                {
                    if (audio.Tamanho == 0)
                        itemResult.mensagem += "<br/>O tamanho do audio é obrigatório.";

                    if (audio.TipoConteudo.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O tipo do conteúdo do audio é obrigatório.";

                    if (audio.Base64.IsNullOrEmptyOrWhiteSpace())
                        itemResult.mensagem += "<br/>O arquivo em base64 do audio é obrigatório.";

                    if (!TIPO_AUDIO_PERMITIDOS.Contains(audio.TipoConteudo))
                        itemResult.mensagem += "<br/>Tipo de audio não permitido.";
                }
            }

            if (!itemResult.mensagem.IsNullOrEmptyOrWhiteSpace())
                itemResult.sucesso = false;
        }

        public List<ItemApiResult> SaveApi(List<ItemApiDto> items)
        {
            var result = new List<ItemApiResult>();

            for (int i = 0; i < items.Count; i++)
            {
                var model = items[i];

                ItemApiResult itemResult = new ItemApiResult
                {
                    sucesso = true,
                    sequencia = i + 1
                };

                try
                {
                    ValidateApi(model, itemResult);

                    if (!itemResult.sucesso) throw new Exception(itemResult.mensagem);

                    var files = new List<EntityFile>();

                    if (model.Imagens != null && model.Imagens.Count > 0)
                    {
                        foreach (var picture in model.Imagens)
                        {
                            switch (picture.Tipo)
                            {
                                case PictureType.BaseText:
                                    if (model.TextoBase.Contains(picture.Tag))
                                    {
                                        string tabImg = UploadPictureTagImg(EnumFileType.BaseText, files, picture);
                                        model.TextoBase = model.TextoBase.Replace(picture.Tag, tabImg);
                                    }
                                    break;
                                case PictureType.Statement:
                                    if (model.Enunciado.Contains(picture.Tag))
                                    {
                                        string tabImg = UploadPictureTagImg(EnumFileType.Statement, files, picture);
                                        model.Enunciado = model.Enunciado.Replace(picture.Tag, tabImg);
                                    }
                                    break;

                                case PictureType.Alternative:
                                    foreach (var alternative in model.Alternativas)
                                    {
                                        if (alternative.Descricao.Contains(picture.Tag))
                                        {
                                            string tabImg = UploadPictureTagImg(EnumFileType.Alternative, files, picture);
                                            alternative.Descricao = alternative.Descricao.Replace(picture.Tag, tabImg);
                                        }
                                    }
                                    break;
                                case PictureType.Justificative:
                                    foreach (var alternative in model.Alternativas)
                                    {
                                        if (alternative.Justificativa.Contains(picture.Tag))
                                        {
                                            string tabImg = UploadPictureTagImg(EnumFileType.Justificative, files, picture);
                                            alternative.Justificativa = alternative.Justificativa.Replace(picture.Tag, tabImg);
                                        }
                                    }
                                    break;
                            }

                            if (files.Any(t => !t.Validate.IsValid)) throw new Exception(files.FirstOrDefault(t => !t.Validate.IsValid).Validate.Message);
                        }
                    }

                    var itemFiles = new List<ItemFile>();
                    if (model.Videos != null && model.Videos.Count > 0)
                    {
                        foreach (var video in model.Videos)
                        {
                            var itemFile = UploadVideo(video);

                            if (!itemFile.Validate.Message.IsNullOrEmptyOrWhiteSpace())
                                throw new Exception(itemFile.Validate.Message);

                            itemFiles.Add(itemFile);
                        }
                    }

                    var itemAudios = new List<ItemAudio>();
                    if (model.Audios != null && model.Audios.Count > 0)
                    {
                        foreach (var audio in model.Audios)
                        {
                            var itemAudio = UploadAudio(audio);

                            if (!itemAudio.Validate.Message.IsNullOrEmptyOrWhiteSpace())
                                throw new Exception(itemAudio.Validate.Message);

                            itemAudios.Add(itemAudio);
                        }
                    }

                    Item item = new Item()
                    {
                        Statement = model.Enunciado,
                        proficiency = model.Proficiencia,
                        EvaluationMatrix_Id = model.MatrizId,
                        Keywords = model.PalavrasChave,
                        Tips = model.Observacao,
                        TRICasualSetting = model.TRIAcertoCasual,
                        TRIDifficulty = model.TRIDificuldade,
                        TRIDiscrimination = model.TRIDiscrimicacao,
                        BaseText = new BaseText()
                        {
                            Description = model.TextoBase,
                            Source = model.Fonte
                        },
                        ItemSituation_Id = 1, // -> Situação 1 = Aceito
                        ItemType_Id = model.TipoItemId,
                        ItemLevel_Id = (long)model.Dificuldade,
                        ItemCode = model.CodigoItem,
                        ItemCurriculumGrades = new List<ItemCurriculumGrade>()
                        {
                            new ItemCurriculumGrade() {
                                TypeCurriculumGradeId = model.TipoGradeCurricularId
                            }
                        },
                        ItemSkills = new List<ItemSkill>(),
                        Alternatives = model.Alternativas != null ? model.Alternativas.Select(t => new Alternative()
                        {
                            Description = t.Descricao,
                            Correct = t.Correta,
                            Order = t.Ordem,
                            Justificative = t.Justificativa,
                            Numeration = t.Numeracao
                        }).ToList() : new List<Alternative>(),
                        IsRestrict = model.Sigiloso,
                        KnowledgeArea_Id = model.AreaConhecimentoId,
                        SubSubject_Id = model.SubassuntoId,
                        ItemFiles = itemFiles,
                        ItemAudios = itemAudios,
                    };

                    item.ItemSkills.Add(new ItemSkill()
                    {
                        Skill_Id = model.CompetenciaId,
                        OriginalSkill = true
                    });

                    item.ItemSkills.Add(new ItemSkill()
                    {
                        Skill_Id = model.HabilidadeId,
                        OriginalSkill = true
                    });

                    var entity = Save(0, item, files);

                    itemResult.sucesso = entity.Validate.IsValid;
                    itemResult.tipo = entity.Validate.Type.ToString();
                    itemResult.mensagem = entity.Validate.Message;
                }
                catch (Exception ex)
                {
                    itemResult.sucesso = false;
                    itemResult.tipo = ValidateType.error.ToString();
                    itemResult.mensagem = "Erro ao salvar item. Erro(s): " + ex.Message;
                }

                result.Add(itemResult);
            }

            return result;
        }

        public List<ItemConsultaApiDto> GetApi(int areaConhecimentoId, long? matrizId)
        {
            try
            {
                var result = new List<ItemConsultaApiDto>();
                var items = itemRepository.GetItemsApi(areaConhecimentoId, matrizId);

                foreach (Item item in items)
                {

                    List<EntityFile> itemImagens = fileRepository._GetFilesByParent(item.Id);
                    var imagens = new List<ArquivoConsultaDto>();
                    if (itemImagens != null && itemImagens.Any())
                    {
                        imagens = itemImagens.Select(x => new ArquivoConsultaDto
                        {
                            Id = x.Id,
                            NomeArquivo = x.Name,
                        }).ToList();
                    }

                    var itemVideos = itemFileBusiness.GetVideosByItemId(item.Id).ToList();
                    var videos = new List<ArquivoConsultaDto>();
                    if (itemVideos != null && itemVideos.Any())
                    {
                        videos = itemVideos.Select(x => new ArquivoConsultaDto
                        {
                            Id = x.Id,
                            NomeArquivo = x.Name,                            
                        }).ToList();
                    }

                    var itemAudios = itemAudioBusiness.GetAudiosByItemId(item.Id).ToList();
                    var audios = new List<ArquivoConsultaDto>();
                    if (itemAudios != null && itemAudios.Any())
                    {
                        audios = itemAudios.Select(x => new ArquivoConsultaDto
                        {
                            Id = x.Id,
                            NomeArquivo = x.Name,
                        }).ToList();
                    }

                    ItemConsultaApiDto itemApiDto = new ItemConsultaApiDto()
                    {
                        Enunciado = item.Statement,
                        Proficiencia = item.proficiency,
                        MatrizId = item.EvaluationMatrix_Id,
                        PalavrasChave = item.Keywords,
                        Observacao = item.Tips,
                        TRIAcertoCasual = item.TRICasualSetting,
                        TRIDificuldade = item.TRIDifficulty,
                        TRIDiscrimicacao = item.TRIDiscrimination,
                        TextoBase = item.BaseText?.Description,
                        Fonte = item.BaseText?.Source,
                        TipoItemId = item.ItemType_Id,
                        Dificuldade = (Dificuldade)item.ItemLevel_Id,
                        CodigoItem = item.ItemCode,
                        TipoGradeCurricularId = item.ItemCurriculumGrades.Any() ? item.ItemCurriculumGrades.FirstOrDefault().TypeCurriculumGradeId : 0,
                        HabilidadeId = item.ItemSkills.Any() ? (int)item.ItemSkills.FirstOrDefault().Skill.Id : 0,
                        Alternativas = item.Alternatives != null ? item.Alternatives.Select(t => new AlternativeDto()
                        {
                            Descricao = t.Description,
                            Correta = t.Correct,
                            Ordem = t.Order,
                            Justificativa = t.Justificative,
                            Numeracao = t.Numeration

                        }).ToList() : new List<AlternativeDto>(),
                        Sigiloso = item.IsRestrict,
                        AreaConhecimentoId = item.KnowledgeArea_Id ?? 0,
                        SubassuntoId = item.SubSubject_Id ?? 0,
                        Imagens = imagens,
                        Videos = videos,
                        Audios = audios,
                    };
                    result.Add(itemApiDto);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string UploadPictureTagImg(EnumFileType type, List<EntityFile> files, PictureDto picture)
        {
            var entidade = parambusiness.GetByKey("ENTIDADE");
            var virtualDirectory = parambusiness.GetByKey(EnumParameterKey.VIRTUAL_PATH.GetDescription(), new Guid(entidade.Value));
            var physicalDirectory = parambusiness.GetByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), new Guid(entidade.Value));

            UploadModel upload = new UploadModel
            {
                ContentLength = picture.Tamanho,
                ContentType = picture.TipoConteudo,
                InputStream = picture.Base64,
                FileName = picture.NomeArquivo,
                Stream = null,
                VirtualDirectory = virtualDirectory.Value,
                PhysicalDirectory = physicalDirectory.Value,
                FileType = type
            };

            var file = fileBusiness.Upload(upload);
            files.Add(file);

            var tabImg = $"<img src='{file.Path}' id='{file.Id}'>";
            return tabImg;
        }

        private ItemFile UploadVideo(VideoDto videoDto)
        {
            var itemFile = new ItemFile();

            var entidade = parambusiness.GetByKey("ENTIDADE");
            var virtualDirectory = parambusiness.GetByKey(EnumParameterKey.VIRTUAL_PATH.GetDescription(), new Guid(entidade.Value));
            var physicalDirectory = parambusiness.GetByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), new Guid(entidade.Value));

            EntityFile entityFileConvert = null;
            var sizeToConvertVideo = parambusiness.GetByKey(EnumParameterKey.SIZE_TO_CONVERT_VIDEO_FILE.GetDescription());
            if (videoDto.Tamanho >= int.Parse(sizeToConvertVideo.Value))
                entityFileConvert = ConvertVideoAsync(videoDto.Base64, videoDto.TipoConteudo, videoDto.NomeArquivo, virtualDirectory.Value, physicalDirectory.Value);

            EntityFile entityThumbnail = null;
            if (videoDto.MiniaturaTamanho >= 0)
            {
                UploadModel uploadThumbnail = new UploadModel
                {
                    ContentLength = videoDto.MiniaturaTamanho,
                    ContentType = videoDto.MiniaturaTipoConteudo,
                    InputStream = videoDto.MiniaturaBase64,
                    FileName = videoDto.NomeArquivo,
                    Stream = null,
                    VirtualDirectory = virtualDirectory.Value,
                    PhysicalDirectory = physicalDirectory.Value,
                    FileType = EnumFileType.ThumbnailVideo
                };

                entityThumbnail = fileBusiness.Upload(uploadThumbnail);
            }

            var upload = new UploadModel
            {
                ContentLength = videoDto.Tamanho,
                ContentType = videoDto.TipoConteudo,
                InputStream = videoDto.Base64,
                FileName = videoDto.NomeArquivo,
                VirtualDirectory = virtualDirectory.Value,
                PhysicalDirectory = physicalDirectory.Value,
                FileType = EnumFileType.Video
            };
            EntityFile entity = fileBusiness.Upload(upload);

            if (entityFileConvert != null && !entityFileConvert.Validate.IsValid)
                itemFile.Validate.Message += entityFileConvert.Validate.Message;

            if (entityThumbnail != null && !entityThumbnail.Validate.IsValid)
                itemFile.Validate.Message += entityThumbnail.Validate.Message;

            if (!entity.Validate.IsValid)
                itemFile.Validate.Message += entity.Validate.Message;

            itemFile.File = entity;

            if (entityFileConvert != null)
            {
                itemFile.ConvertedFileId = entityFileConvert.Id;
                itemFile.ConvertedFile = entityFileConvert;
            }

            if (entityThumbnail != null)
            {
                itemFile.ThumbnailId = entityThumbnail.Id;
                itemFile.Thumbnail = entityThumbnail;
            }
            else
                itemFile.Thumbnail = new EntityFile();

            return itemFile;
        }

        private EntityFile ConvertVideoAsync(string inputStream, string contentType, string fileName, string virtualDirectory, string physicalDirectory)
        {
            var entity = new EntityFile();

            var bytes = Convert.FromBase64String(inputStream);
            var stream = new MemoryStream(bytes);

            var convertedVideoDto = videoConverter.Convert(stream, contentType, fileName, Guid.Empty).Result;
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
                VirtualDirectory = virtualDirectory,
                PhysicalDirectory = physicalDirectory,
                FileType = EnumFileType.Video
            };

            return fileBusiness.Upload(uploadConvertedVideo);
        }

        private ItemAudio UploadAudio(AudioDto audioDto)
        {
            ItemAudio itemAudio = new ItemAudio();

            var entidade = parambusiness.GetByKey("ENTIDADE");
            var virtualDirectory = parambusiness.GetByKey(EnumParameterKey.VIRTUAL_PATH.GetDescription(), new Guid(entidade.Value));
            var physicalDirectory = parambusiness.GetByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), new Guid(entidade.Value));

            UploadModel upload = new UploadModel
            {
                ContentLength = audioDto.Tamanho,
                ContentType = audioDto.TipoConteudo,
                InputStream = audioDto.Base64,
                FileName = audioDto.NomeArquivo,
                VirtualDirectory = virtualDirectory.Value,
                PhysicalDirectory = physicalDirectory.Value,
                FileType = EnumFileType.Audio
            };

            var entity = fileBusiness.Upload(upload);

            if (!entity.Validate.IsValid)
                itemAudio.Validate.Message += entity.Validate.Message;

            itemAudio.File = entity;
            return itemAudio;
        }

        #endregion
    }
}
