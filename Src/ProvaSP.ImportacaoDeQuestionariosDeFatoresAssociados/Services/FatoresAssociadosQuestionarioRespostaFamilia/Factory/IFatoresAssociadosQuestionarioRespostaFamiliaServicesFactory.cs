using System.Data;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Factory
{
    public interface IFatoresAssociadosQuestionarioRespostaFamiliaServicesFactory
    {
        IFatoresAssociadosQuestionarioRespostaFamiliaServices Create(bool isDRE, bool isEscola, bool isSME, DataTable dtRespostas);
    }
}