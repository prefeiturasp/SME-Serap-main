using ImportacaoDeQuestionariosSME.Data.Repositories.Alunos;
using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Domain.Alunos;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Domain.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System.Collections.Generic;
using System.Data;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia
{
    public abstract class FatoresAssociadosQuestionarioRespostaFamiliaServices
    {
        protected readonly IAlunoRepository _alunoRepository;
        protected readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository;
        protected readonly IConstructoRepository _constructoRepository;
        protected readonly DataTable _dtRespostas;

        public FatoresAssociadosQuestionarioRespostaFamiliaServices(DataTable dtRespostas)
        {
            _alunoRepository = new AlunoRepository();
            _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
            _constructoRepository = new ConstructoRepository();
            _dtRespostas = dtRespostas;
        }

        protected IEnumerable<QuestionarioItem> MontarQuestoes(ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto dto)
        {
            var dtQuestionario = CsvManager.GetCsvFile(dto.CaminhoDaPlanilhaQuesitonarios);
            if (dtQuestionario.Rows.Count <= 0)
            {
                dto.AddErro("Não existem regitros na planilha para exportação.");
                return null;
            }

            var result = new List<QuestionarioItem>();
            var indice = 0;
            while (indice < dtQuestionario.Rows.Count)
            {
                if (dtQuestionario.Rows[indice]["seq"].ToString() == "0")
                {
                    var questionarioItem = new QuestionarioItem
                    {
                        Enunciado = dtQuestionario.Rows[indice]["texto"].ToString(),
                        Numero = int.Parse(dtQuestionario.Rows[indice]["num_questao"].ToString()),
                    };

                    indice++;
                    while (indice < dtQuestionario.Rows.Count
                        && dtQuestionario.Rows[indice]["seq"].ToString() != "0")
                    {
                        var opcao = new Opcao
                        {
                            Descricao = dtQuestionario.Rows[indice]["texto"].ToString().ReplaceFirst("(", ""),
                            Letra = dtQuestionario.Rows[indice]["alternativa"].ToString(),
                            Numero = int.Parse(dtQuestionario.Rows[indice]["seq"].ToString())
                        };

                        questionarioItem.Opcoes.Add(opcao);
                        indice++;
                    }

                    result.Add(questionarioItem);
                }
                else
                    indice++;
            }

            return result;
        }
    }
}