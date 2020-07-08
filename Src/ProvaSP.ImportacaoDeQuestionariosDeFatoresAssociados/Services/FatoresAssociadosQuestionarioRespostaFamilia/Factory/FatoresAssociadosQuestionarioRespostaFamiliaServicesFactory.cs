using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.DRE;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Escolas;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.SME;
using System.Data;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Factory
{
    public class FatoresAssociadosQuestionarioRespostaFamiliaServicesFactory : IFatoresAssociadosQuestionarioRespostaFamiliaServicesFactory
    {
        public IFatoresAssociadosQuestionarioRespostaFamiliaServices Create(bool isDRE, bool isEscola, bool isSME, DataTable dtRespostas)
        {
            if (isDRE) return new FatoresAssociadosQuestionarioRespostaFamiliaDREServices(dtRespostas);
            if (isEscola) return new FatoresAssociadosQuestionarioRespostaFamiliaEscolaServices(dtRespostas);
            if (isSME) return new FatoresAssociadosQuestionarioRespostaFamiliaSMEServices(dtRespostas);
            return null;
        }
    }
}