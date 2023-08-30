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
            try
            {
                var file = fileBusiness.Upload(upload);
                entity.Link = file.Path;
                return reportStudiesRepository.Save(entity);
            }
            catch (System.Exception ex)
            {
                throw ex;
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
            try
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
                            var destinatario = string.Empty;

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

                            destinatario = TrataDestinatario(listaEscolas, listaDres, item, destinatario, entity);

                            entity.Addressee = destinatario;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string TrataDestinatario(IEnumerable<EscolaDto> listaEscolas, IEnumerable<GestaoEscolar.Entities.SYS_UnidadeAdministrativa> listaDres, ReportStudiesCsvDto item, string destinatario, ReportStudies entity)
        {
            if (entity.TypeGroup == (int)EnumTypeGroup.DRE)
            {
                var dre = listaDres.Where(x => x.uad_sigla == item.Destinatario).FirstOrDefault();
                destinatario = $"{dre.uad_sigla} - {dre.uad_nome.Replace("DIRETORIA REGIONAL DE EDUCACAO", "")}";
            }
            if (entity.TypeGroup == (int)EnumTypeGroup.UE)
            {
                var ue = listaEscolas.Where(x => x.EscCodigo == item.Destinatario).FirstOrDefault();
                destinatario = $"{ue.EscCodigo} - {ue.EscNome}";
            }

            return destinatario;
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