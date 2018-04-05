using AvaliaMais.FolhaRespostas.Application.Factory;
using AvaliaMais.FolhaRespostas.Application.Interfaces;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using System;

namespace GestaoAvaliacao.Services
{
    public class CorrectionResult
    {
        private readonly IProcessamentoAppServiceWrite _procAppWrite;
        private readonly ITestBusiness _testBusiness;

        public CorrectionResult(IProcessamentoAppServiceWrite procAppWrite, ITestBusiness testBusiness)
		{
            _procAppWrite = procAppWrite;
            _testBusiness = testBusiness;
        }

		public void Execute()
		{
            Test test = new Test();

            try
            {
                var result = _testBusiness.GetTestFinishedCorrection(false);
                foreach (var entity in result)
                {
                    test = entity;
                    bool ret = false;
                    if(entity.AllAdhered)
                        ret = _procAppWrite.AdicionarProcessamento(Convert.ToInt32(entity.Id), AdesaoTotalFactory.CriarAdesaoTotal());
                    else
                        ret = _procAppWrite.AdicionarProcessamento(Convert.ToInt32(entity.Id), AdesaoParcialFactory.CriarAdesaoParcial());

                    _testBusiness.UpdateTestProcessedCorrection(entity.Id, ret);
                }
            }
            catch (Exception e)
            {
                if (test != null && test.Id > 0)
                {
                    _testBusiness.UpdateTestProcessedCorrection(test.Id, false);
                }

                LogFacade.LogFacade.SaveError(e);
            }
		}
    }
}
