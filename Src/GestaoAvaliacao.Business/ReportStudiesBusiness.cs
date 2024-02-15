using CsvHelper;
using CsvHelper.Configuration;
using GestaoAvaliacao.Business.DTO;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.IBusiness;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GestaoAvaliacao.Business
{
    public class ReportStudiesBusiness : IReportStudiesBusiness
    {
        private readonly IReportStudiesRepository reportStudiesRepository;
        private readonly IFileBusiness fileBusiness;
        private readonly IESC_EscolaBusiness _schoolBusiness;
        private readonly ISYS_UnidadeAdministrativaBusiness _uadBusiness;
        public ReportStudiesBusiness(IReportStudiesRepository reportStudiesRepository, IFileBusiness fileBusiness, IESC_EscolaBusiness _schoolBusiness,
           ISYS_UnidadeAdministrativaBusiness _uadBusiness)
        {
            this.reportStudiesRepository = reportStudiesRepository;
            this.fileBusiness = fileBusiness;
            this._schoolBusiness = _schoolBusiness;
            this._uadBusiness = _uadBusiness;
        }

        public Util.Validate Validate(ReportStudies entity, long evaluationMatrixId, ValidateAction action, Util.Validate valid)
        {
            valid.Message = null;

            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Name) || string.IsNullOrEmpty(entity.Addressee))
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
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
            {
                valid.IsValid = true;
            }

            return valid;
        }

        public bool Save(ReportStudies entity, UploadModel upload)
        {
            entity.Id = 0;
            TrataGrupoDestinatario(entity);
            TrataDescricaoDestinatario(entity);
            var file = fileBusiness.Upload(upload);
            entity.Link = file.Path;
            return reportStudiesRepository.Save(entity);
        }

        public bool Update(ReportStudies entity)
        {
            TrataGrupoDestinatario(entity);
            TrataDescricaoDestinatario(entity);
            var entityDb = reportStudiesRepository.GetById(entity.Id);
            if (entityDb == null || entityDb?.Id == 0) throw new Exception("arquivo não encontrado.");
            return reportStudiesRepository.Update(entity);
        }

        private void TrataGrupoDestinatario(in ReportStudies entity)
        {
            if (entity.TypeGroup == null || entity.TypeGroup == 0)
            {
                entity.Addressee = entity.UadCodigoDestinatario = null;
                entity.TypeGroup = null;
            }
            else if (entity.TypeGroup != (int)EnumTypeGroup.DRE && entity.TypeGroup != (int)EnumTypeGroup.UE)
                entity.Addressee = entity.UadCodigoDestinatario = null;

            if (entity.TypeGroup == (int)EnumTypeGroup.DRE || entity.TypeGroup != (int)EnumTypeGroup.UE)
            {
                if (string.IsNullOrEmpty(entity.UadCodigoDestinatario) || entity.UadCodigoDestinatario.Trim() == "0")
                    entity.Addressee = entity.UadCodigoDestinatario = null;
            }
        }

        private void TrataDescricaoDestinatario(in ReportStudies entity)
        {
            if (entity.TypeGroup == (int)EnumTypeGroup.DRE)
            {
                var dre = _uadBusiness.GetByUad_Codigo(entity.UadCodigoDestinatario);
                if (dre == null) throw new Exception($"DRE não encontrada: {entity.UadCodigoDestinatario}");
                entity.Addressee = dre.uad_nome.Replace("DIRETORIA REGIONAL DE EDUCACAO", "");
            }

            if (entity.TypeGroup == (int)EnumTypeGroup.UE)
            {
                var escola = _schoolBusiness.ObterEscolaPorCodigo(entity.UadCodigoDestinatario);
                if (escola == null) throw new Exception($"ESCOLA não encontrada: {entity.UadCodigoDestinatario}");
                entity.Addressee = $"{escola.EscCodigo} - {escola.EscNome}";
            }
        }

        public IEnumerable<ReportStudies> ListAll()
        {
            return reportStudiesRepository.ListAll();
        }

        public IEnumerable<ReportStudies> ListPaginated(ref Pager pager, string searchFilter)
        {
            if (!string.IsNullOrEmpty(searchFilter))
                return reportStudiesRepository.ListWithFilter(searchFilter);
            return reportStudiesRepository.ListPaginated(ref pager);
        }
        public void Delete(long id)
        {
            reportStudiesRepository.Delete(id);
        }
        public bool DeleteById(long id)
        {
            return reportStudiesRepository.DeleteById(id);
        }

        public void ImportCsv(HttpPostedFileBase arquivo, SYS_Usuario usuario, SYS_Grupo sysGrupo, out CsvImportDTO retornoCsv)
        {

            using (var leitorAquivo = new StreamReader(arquivo.InputStream, encoding: Encoding.UTF8))
            {
                using (var csv = new CsvReader(leitorAquivo, config))
                {
                    var listaArquivoEstudoCsvDto = csv.GetRecords<ReportStudiesCsvDto>().ToList();
                    var listaErros = new List<ErrosImportacaoCSV>();
                    var linha = 1;
                    var codigosAtualizados = new List<long>();
                    var listaEscolas = _schoolBusiness.LoadAllSchoollsActiveDto();
                    var listaDres = _uadBusiness.LoadDRESimple(usuario, sysGrupo);
                    var listaCodigosDre = listaDres.Select(x => x.uad_sigla).ToList();
                    var listaGrupos = CarregaGrupos();
                    foreach (var item in listaArquivoEstudoCsvDto)
                    {
                        linha++;
                        var entity = reportStudiesRepository.GetById(item.Codigo);

                        if (ExisteErroItemCsv(listaErros,
                                                  linha,
                                     codigosAtualizados,
                                           listaEscolas,
                                        listaCodigosDre,
                                            listaGrupos,
                                                   item,
                                                 entity))
                        { continue; }



                        if (Enum.TryParse(RemoveAcentos(item.TipoGrupo.ToUpper()), out EnumTypeGroup enumTypeGroup))
                            entity.TypeGroup = (int)enumTypeGroup;

                        TrataDestinatario(listaEscolas, listaDres, item, entity);

                        codigosAtualizados.Add(item.Codigo);

                        reportStudiesRepository.Update(entity);

                    }
                    retornoCsv = new CsvImportDTO
                    {
                        QtdeSucesso = codigosAtualizados.Count(),
                        QtdeErros = listaErros.Count
                    };

                    retornoCsv.Erros.AddRange(listaErros);

                }
            }
        }

        private static void TrataDestinatario(IEnumerable<EscolaDto> listaEscolas, IEnumerable<GestaoEscolar.Entities.SYS_UnidadeAdministrativa> listaDres, ReportStudiesCsvDto item, in ReportStudies entity)
        {
            if (entity.TypeGroup == (int)EnumTypeGroup.DRE)
            {
                var dre = listaDres.Where(x => x.uad_sigla == item.Destinatario).FirstOrDefault();
                entity.Addressee = $"{dre.uad_sigla} - {dre.uad_nome.Replace("DIRETORIA REGIONAL DE EDUCACAO", "")}";
                entity.UadCodigoDestinatario = dre.uad_codigo;
            }
            if (entity.TypeGroup == (int)EnumTypeGroup.UE)
            {
                var ue = listaEscolas.Where(x => x.EscCodigo == item.Destinatario).FirstOrDefault();
                entity.Addressee = $"{ue.EscCodigo} - {ue.EscNome}";
                entity.UadCodigoDestinatario = ue.EscCodigo;
            }
        }

        private static bool ExisteErroItemCsv(List<ErrosImportacaoCSV> listaErros, int linha, List<long> codigosAtualizados, IEnumerable<EscolaDto> listaEscolas, List<string> listaAbreviacaoDres, List<string> listaGrupos, ReportStudiesCsvDto item, ReportStudies entity)
        {
            var contemErro = false;
            if (entity == null)
            {
                listaErros.Add(new ErrosImportacaoCSV
                {
                    Linha = linha,
                    Erro = "Código inválido"
                });
                contemErro = true;
            }

            if (!listaGrupos.Contains(RemoveAcentos(item.TipoGrupo.ToUpper())))
            {
                listaErros.Add(new ErrosImportacaoCSV
                {
                    Linha = linha,
                    Erro = "Grupo inválido."
                });
                contemErro = true;
            }

            if (item.TipoGrupo.ToUpper() == "DRE" && !listaAbreviacaoDres.Contains(item.Destinatario))
            {
                listaErros.Add(new ErrosImportacaoCSV
                {
                    Linha = linha,
                    Erro = "Destinatário inválido."
                });
                contemErro = true;

            }

            if (item.TipoGrupo.ToUpper() == "UE" && !listaEscolas.Select(x => x.EscCodigo).Contains(item.Destinatario))
            {
                listaErros.Add(new ErrosImportacaoCSV
                {
                    Linha = linha,
                    Erro = "Destinatário inválido."
                });
                contemErro = true;
            }

            if (codigosAtualizados.Contains(item.Codigo))

            {
                listaErros.Add(new ErrosImportacaoCSV
                {
                    Linha = linha,
                    Erro = "Código em duplicidade"
                });
                contemErro = true;
            }

            return contemErro;
        }

        private List<string> CarregaGrupos()
        {
            var listaGrupo = new List<string>
            {
                "UE",
                "DRE",
                "GERAL",
                "SME",
                "PUBLICO"
            };

            return listaGrupo;
        }

        public IEnumerable<AJX_Select2> ListarGrupos()
        {
            var enumType = typeof(EnumTypeGroup);
            var listaGrupos = new List<AJX_Select2>();

            foreach (var value in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(enumType, value);
                var fieldInfo = enumType.GetField(name);
                var descriptionAttribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                    .FirstOrDefault() as DescriptionAttribute;

                if (descriptionAttribute != null)
                {
                    var description = descriptionAttribute.Description;
                    var code = (int)value;
                    listaGrupos.Add(new AJX_Select2 { text = description, id = code.ToString() });
                }

            }

            return listaGrupos.OrderBy(x => x.text);
        }

        public IEnumerable<AJX_Select2> ListarDestinatarios(SYS_Usuario usuario, SYS_Grupo sysGrupo, EnumTypeGroup? tipoGrupo, string filtroDesc = null)
        {
            var listaDestinatarios = new List<AJX_Select2>();

            var listaDres = _uadBusiness.LoadDRESimple(usuario, sysGrupo);
            var listaCodigosDre = listaDres.Select(x => x.uad_sigla).ToList();

            if (tipoGrupo == EnumTypeGroup.DRE)
            {
                listaDestinatarios = listaDres.Select(x => new AJX_Select2
                {
                    id = x.uad_codigo,
                    text = x.uad_nome.Replace("DIRETORIA REGIONAL DE EDUCACAO", "")
                }).ToList();
            }

            if (tipoGrupo == EnumTypeGroup.UE)
            {
                var listaEscolas = _schoolBusiness.LoadSimple(usuario, sysGrupo, Guid.Empty);
                listaDestinatarios = listaEscolas.Select(x => new AJX_Select2
                {
                    id = x.esc_codigo,
                    text = $"{x.esc_codigo} - {x.esc_nome}"
                }).ToList();
            }

            if (!string.IsNullOrEmpty(filtroDesc))
                listaDestinatarios = listaDestinatarios.Where(x => x.text.Contains(filtroDesc.ToUpper())).ToList();

            return listaDestinatarios;
        }

        private static CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            ShouldSkipRecord = records =>
            {
                var linha = records.Row.Parser.RawRecord.Replace(Environment.NewLine, string.Empty);
                linha = linha.Trim().Replace("\r", string.Empty);
                linha = linha.Trim().Replace("\n", string.Empty);
                linha = linha.Trim().Replace("\0", string.Empty);

                var arrayLinha = records.Row.Parser.Record;
                return string.IsNullOrEmpty(linha) || arrayLinha == null || arrayLinha.Length == 0 ||
                       (arrayLinha.Length > 0 && string.IsNullOrEmpty(arrayLinha[0]));
            }
        };


        private static string RemoveAcentos(string valor)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in valor.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}