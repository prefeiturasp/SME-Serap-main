using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.MongoEntities.Projections;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Extensions;
using GestaoAvaliacao.Util.Structs;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class ReportItemChoiceBusiness : IReportItemChoiceBusiness
    {
        private readonly ICorrectionResultsBusiness _correctionResultsBusiness;
        private readonly IAlternativeBusiness _alternativeBusiness;
        private readonly IESC_EscolaBusiness _schoolBusiness;
        private readonly ISYS_UnidadeAdministrativaBusiness _uadBusiness;
        private readonly IFileBusiness _fileBusiness;
        private readonly IStorage _storage;

        public ReportItemChoiceBusiness(ICorrectionResultsBusiness correctionResultsBusiness,
                                        IAlternativeBusiness alternativeBusiness,
                                        IESC_EscolaBusiness schoolBusiness,
                                        ISYS_UnidadeAdministrativaBusiness uadBusiness,
                                        IFileBusiness fileBusiness,
                                        IStorage storage)
        {
            _correctionResultsBusiness = correctionResultsBusiness;
            _alternativeBusiness = alternativeBusiness;
            _schoolBusiness = schoolBusiness;
            _uadBusiness = uadBusiness;
            _fileBusiness = fileBusiness;
            _storage = storage;
        }

        /// <summary>
        /// Busca as médias de escolha por item em uma prova
        /// </summary>
        /// <param name="test_Id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Dto com o percentual de escolha de cada alternativa em cada item da prova</returns>
        public async Task<ItemPercentageChoiceByAlternativeWithOrderResult.Test> GetItemPercentageChoiceByAlternative(long test_Id, long? discipline_id, Guid? dre_id, int? esc_id)
        {
            var result = new ItemPercentageChoiceByAlternativeWithOrderResult.Test();

            var itemsPercentageChoiceByAlternative = await _correctionResultsBusiness
                                                                .GetItemPercentageChoiceByAlternative(test_Id, discipline_id, dre_id, esc_id);

            var alternativesOrder = await _alternativeBusiness.GetAlternativesWithNumerationAndOrderByTest(test_Id);

            var dreAndSchoolInformation = GetDreAndSchool(dre_id, esc_id);

            if (itemsPercentageChoiceByAlternative.Any() && alternativesOrder.Any())
            {
                result.Test_Id = test_Id;
                result.DreAndSchoolInformation = dreAndSchoolInformation;
                result.Items = InsertItems(discipline_id != null ? itemsPercentageChoiceByAlternative.Where(p => p.Discipline_Id == discipline_id).ToList() : itemsPercentageChoiceByAlternative, alternativesOrder.ToList());
                result.Alternatives = InsertAlternatives(alternativesOrder.Where(x => x.Item_Id == itemsPercentageChoiceByAlternative.First().Item_Id).ToList());
            }

            return result;
        }

        /// <summary>
        /// Exporta o relatório para CSV
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <param name="separator">Separador do CSV</param>
        /// <param name="virtualDirectory">Diretório virtual</param>
        /// <param name="physicalDirectory">Diretório físico</param>
        /// <returns>Dados para download do CSV</returns>
        public async Task<EntityFile> ExportReport(int test_id, long? discipline_id, Guid? dre_id, int? esc_id,
                                        string separator, string virtualDirectory, string physicalDirectory)
        {
            EntityFile ret = new EntityFile();

            var itemsPercentageChoiceByAlternative = await GetItemPercentageChoiceByAlternative(test_id, discipline_id, dre_id, esc_id);

            if (itemsPercentageChoiceByAlternative != null && itemsPercentageChoiceByAlternative.Items.Any())
            {
                var body = new StringBuilder();
                var fileName = "Rel_Percentage_Item_Choice";

                var dreAndSchoolInformation = GetDreAndSchool(dre_id, esc_id);

                if (!dreAndSchoolInformation.DreName.IsNullOrEmptyOrWhiteSpace())
                {
                    body.Append(string.Concat("DRE:", separator, dreAndSchoolInformation.DreName));
                    body.AppendLine();
                }

                if (!dreAndSchoolInformation.SchoolName.IsNullOrEmptyOrWhiteSpace())
                {
                    body.Append(string.Concat("Escola:", separator, dreAndSchoolInformation.SchoolName));
                    body.AppendLine();
                }

                body.Append(string.Concat("Questões", separator));

                foreach (var alternative in itemsPercentageChoiceByAlternative.Alternatives)
                {
                    body.Append(string.Concat(alternative.Numeration, separator));
                }

                foreach (var item in itemsPercentageChoiceByAlternative.Items)
                {
                    body.AppendLine();
                    body.Append(string.Concat("Item ", item.Order+1, separator));

                    item.Alternatives.ForEach(x =>
                    {
                        body.Append(string.Concat(x.Avg, "%", separator));
                    });
                }

                ret = CreateReportCsvFile(fileName, body.ToString(), virtualDirectory, physicalDirectory);
            }
            else
            {
                ret.Validate.IsValid = false;
                ret.Validate.Type = ValidateType.alert.ToString();
                ret.Validate.Message = "Ainda não existem dados para exportar.";
            }

            return ret;
        }

        /// <summary>
        /// Busca os nomes da DRE e da escola
        /// </summary>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Nome da Dre e da escola</returns>
        private ItemPercentageChoiceByAlternativeWithOrderResult.DreAndSchoolInformation GetDreAndSchool(Guid? dre_id, int? esc_id)
        {
            var dreAndSchoolInformation = new ItemPercentageChoiceByAlternativeWithOrderResult.DreAndSchoolInformation();

            if (esc_id.HasValue)
            {
                var school = _schoolBusiness.GetSchoolAndDRENames((int)esc_id);

                dreAndSchoolInformation.DreName = school.uad_nome;
                dreAndSchoolInformation.SchoolName = school.esc_nome;
            }
            else if (dre_id.HasValue)
            {
                var dre = _uadBusiness.GetByUad_Id((Guid)dre_id);

                dreAndSchoolInformation.DreName = dre.uad_nome;
            }

            return dreAndSchoolInformation;
        }

        /// <summary>
        /// Salva o arquivo e monta o objeto de retorno com os dados para download do CSV
        /// </summary>
        /// <param name="fileName">Nome do arquivo</param>
        /// <param name="body">Conteúdo do CSV</param>
        /// <param name="virtualDirectory">Diretório virtual</param>
        /// <param name="physicalDirectory">Diretório físico</param>
        /// <returns>Dados para download do CSV</returns>
        private EntityFile CreateReportCsvFile(string fileName, string body, string virtualDirectory, string physicalDirectory)
        {
            byte[] buffer = System.Text.Encoding.Default.GetBytes(body);
            string originalName = string.Format("{0}.csv", fileName);
            string name = string.Format("{0}.csv", Guid.NewGuid());
            string contentType = MimeType.CSV.GetDescription();

            var csvFiles = _fileBusiness.GetAllFilesByType(EnumFileType.ExportReportPercentageItemChoice, DateTime.Now.AddDays(-1));
            if (csvFiles != null && csvFiles.Count() > 0)
            {
                _fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
                _fileBusiness.DeleteFilesByType(EnumFileType.ExportReportTestPerformance, DateTime.Now.AddDays(-1));
            }

            EntityFile ret = _storage.Save(buffer, name, contentType, EnumFileType.ExportReportPercentageItemChoice.GetDescription(), virtualDirectory, physicalDirectory, out ret);

            if (ret.Validate.IsValid)
            {
                ret.OriginalName = StringHelper.Normalize(originalName);
                ret.OwnerType = (byte)EnumFileType.ExportReportTestPerformance;

                ret = _fileBusiness.Save(ret);
            }

            return ret;
        }

        /// <summary>
        /// Insere os itens na prova com as alternativas e a porcetagem de escolha por alternativa
        /// </summary>
        /// <param name="itemsPercentageChoicePerAlternative">Lista de itens com a porcentagem de escolha por alternativa (dados do mongodb)</param>
        /// <param name="alternativesOrder">Lista de itens com a ordem correta de cada alternativa (dados do sql server)</param>
        /// <returns>Lista de Dto com os itens da prova</returns>
        private List<ItemPercentageChoiceByAlternativeWithOrderResult.Item> InsertItems(List<ItemPercentageChoiceByAlternativeProjection> itemsPercentageChoicePerAlternative,
                                                                                                    List<AlternativesWithNumerationAndOrderProjection> alternativesOrder)
        {
            var items = new List<ItemPercentageChoiceByAlternativeWithOrderResult.Item>();

            itemsPercentageChoicePerAlternative.ForEach(x =>
            {
                var alternativesByItem = alternativesOrder.Where(y => y.Item_Id == x.Item_Id).ToList();

                var item = new ItemPercentageChoiceByAlternativeWithOrderResult.Item
                {
                    Item_Id = x.Item_Id,
                    Order = alternativesByItem.First().ItemOrder,
                    Revoked = alternativesByItem.First().ItemRevoked,
                    Alternatives = InsertItemAlternatives(x.Alternatives, alternativesByItem)
                };

                items.Add(item);
            });

            return items.OrderBy(x => x.Order).ToList();
        }

        /// <summary>
        /// Insere as alternativas em cada item da prova
        /// </summary>
        /// <param name="alternativesAvg">Lista de alternativas do item com a porcentagem de escolha de cada uma (dados do mongodb)</param>
        /// <param name="alternativesOrder">Lista de alternativas com a ordem correta de cada uma (dados do sql server)</param>
        /// <returns>Lista de alternativas de cada item da prova</returns>
        private List<ItemPercentageChoiceByAlternativeWithOrderResult.Alternative> InsertItemAlternatives(List<AlternativeAverageProjection> alternativesAvg,
                                                                                                                  List<AlternativesWithNumerationAndOrderProjection> alternativesOrder)
        {
            var alternatives = new List<ItemPercentageChoiceByAlternativeWithOrderResult.Alternative>();

            alternativesOrder.ForEach(x =>
            {
                var alternativeAvg = alternativesAvg.Where(y => y.Alternative_Id == x.Alternative_Id).FirstOrDefault();
                var alternative = new ItemPercentageChoiceByAlternativeWithOrderResult.Alternative
                {
                    Alternative_Id = x.Alternative_Id,
                    Correct = x.Correct,
                    Avg = alternativeAvg != null ? new RoundedDecimal(alternativeAvg.Avg) : 0.00,
                    Numeration = x.Numeration,
                    Order = x.Order
                };

                alternatives.Add(alternative);
            });

            var nullableOrShavedAlternatives = alternativesAvg.Where(x => x.Numeration == "N" || x.Numeration == "R")
                                                              .Select(x => new
                                                              {
                                                                  x.Numeration,
                                                                  x.Avg
                                                              }).ToDictionary(t => t.Numeration, t => t.Avg);

            alternatives = InsertNullableAndShavedAlternatives(alternatives, nullableOrShavedAlternatives);

            return alternatives.OrderBy(x => x.Order).ToList();
        }

        /// <summary>
        /// Retorna a ordem correta das alternativas que estão nulas ou rasuradas
        /// </summary>
        /// <param name="numeration">Número/Letra da alternativa</param>
        /// <param name="maxOrder">Quantidade máxima de itens que cada item da prova possui</param>
        /// <returns>Ordem da alternativa</returns>
        private int InsertOrderInNullableOrShavedField(string numeration, int maxOrder)
        {
            int order = 0;

            if (numeration.Equals("N"))
                order = maxOrder + 1;
            else if (numeration.Equals("R"))
                order = maxOrder + 2;

            return order;
        }

        /// <summary>
        /// Insere as alternativas nula e rasura em cada item da prova, com ou sem a porcentagem de escolha
        /// </summary>
        /// <param name="alternatives">Lista de alternativas que já existem no item</param>
        /// <param name="nullableOrShavedAlternatives">Dicionário com o número/letra da alternativa e porcentagem de escolha por ela</param>
        /// <returns>Lista de alternativas</returns>
        private List<ItemPercentageChoiceByAlternativeWithOrderResult.Alternative> InsertNullableAndShavedAlternatives(List<ItemPercentageChoiceByAlternativeWithOrderResult.Alternative> alternatives,
                                                                                                                                    Dictionary<string, double> nullableOrShavedAlternatives = null)
        {
            if (nullableOrShavedAlternatives == null || !nullableOrShavedAlternatives.Any())
            {
                nullableOrShavedAlternatives = new Dictionary<string, double>
                {
                    { "N", 0.0 },
                    { "R", 0.0 }
                };
            }
            else
            {
                if (!nullableOrShavedAlternatives.ContainsKey("R"))
                    nullableOrShavedAlternatives.Add("R", 0.0);
                else if (!nullableOrShavedAlternatives.ContainsKey("N"))
                    nullableOrShavedAlternatives.Add("N", 0.0);
            }

            var maxOrder = alternatives.Max(x => x.Order);

            foreach (var nullableOrShavedAlternative in nullableOrShavedAlternatives.ToList())
            {
                var alternative = new ItemPercentageChoiceByAlternativeWithOrderResult.Alternative
                {
                    Numeration = nullableOrShavedAlternative.Key,
                    Order = InsertOrderInNullableOrShavedField(nullableOrShavedAlternative.Key, maxOrder),
                    Avg = new RoundedDecimal(nullableOrShavedAlternative.Value)
                };

                alternatives.Add(alternative);
            }

            return alternatives;
        }

        /// <summary>
        /// Insere todas as alternativas que a prova possui, somente com a letra/número e a ordem que devem estar em cada item
        /// </summary>
        /// <param name="alternativesOrder">Lista das alternativas que a prova possui com a ordem correta (dados sql server)</param>
        /// <returns>Lista de alternativas da prova com alternativas nula e rasura também</returns>
        private List<ItemPercentageChoiceByAlternativeWithOrderResult.Alternative> InsertAlternatives(List<AlternativesWithNumerationAndOrderProjection> alternativesOrder)
        {
            var alternatives = new List<ItemPercentageChoiceByAlternativeWithOrderResult.Alternative>();

            alternativesOrder.ForEach(x =>
            {
                var alternative = new ItemPercentageChoiceByAlternativeWithOrderResult.Alternative
                {
                    Numeration = x.Numeration.Replace(")", ""),
                    Order = x.Order
                };

                alternatives.Add(alternative);
            });

            alternatives = InsertNullableAndShavedAlternatives(alternatives);

            return alternatives;
        }
    }
}
