using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Services
{
    public class UnzipAnswerSheetQueue
	{
        #region Dependences
        readonly IAnswerSheetBatchQueueBusiness answerSheetBatchQueueBusiness;
        readonly IAnswerSheetBatchFilesBusiness answerSheetBatchFilesBusiness;
        readonly IFileBusiness fileBusiness;
		#endregion

		public UnzipAnswerSheetQueue(IFileBusiness fileBusiness, IAnswerSheetBatchQueueBusiness answerSheetBatchQueueBusiness, 
            IAnswerSheetBatchFilesBusiness answerSheetBatchFilesBusiness)
		{
			this.fileBusiness = fileBusiness;
            this.answerSheetBatchQueueBusiness = answerSheetBatchQueueBusiness;
            this.answerSheetBatchFilesBusiness = answerSheetBatchFilesBusiness;
        }

		public void Execute()
		{
            try
            {
                IEnumerable<AnswerSheetBatchQueue> list = answerSheetBatchQueueBusiness.GetAnswerSheetBatchBySituation(EnumBatchQueueSituation.Processing, 0);
                if (list != null && list.Count() < 1)
                {
                    IEnumerable<AnswerSheetBatchQueue> ListZip = answerSheetBatchQueueBusiness.GetAnswerSheetBatchBySituation(EnumBatchQueueSituation.PendingUnzip, 1);
                    Init(ListZip);
                }
            }
            catch (OutOfMemoryException ex)
            {
                RollbackZipProcessing(new AnswerSheetBatchQueue
                {
                    Situation = EnumBatchQueueSituation.NotUnziped,
                    Description = "Erro na descompactação"
                });
                LogFacade.LogFacade.SaveError(ex);
            }
            catch (Exception e)
            {
                RollbackZipProcessing(new AnswerSheetBatchQueue
                {
                    Situation = EnumBatchQueueSituation.NotUnziped,
                    Description = "Erro na descompactação"
                });
                LogFacade.LogFacade.SaveError(e);
            }
		}

        private void Init(IEnumerable<AnswerSheetBatchQueue> ListZip)
        {
            foreach (var zip in ListZip)
            {
                try
                {
                    zip.Situation = EnumBatchQueueSituation.Processing;
                    Update(zip);

                    Entities.File file = fileBusiness.Get(zip.File_Id);
                    AnswerSheetBatchQueue answerSheetBatchQueue = Unzip(zip, file);
                    Update(answerSheetBatchQueue);
                }
                catch(OutOfMemoryException ex)
                {
                    zip.Situation = EnumBatchQueueSituation.NotUnziped;
                    zip.Description = "Erro na descompactação";
                    Update(zip);
                    LogFacade.LogFacade.SaveError(ex, string.Format("Estouro de memória | Fila Id: {0} | Arquivo Id: {1}", zip.Id, zip.File_Id));
                }
                catch (Exception e)
                {
                    zip.Situation = EnumBatchQueueSituation.NotUnziped;
                    zip.Description = "Erro na descompactação";
                    Update(zip);
                    LogFacade.LogFacade.SaveError(e, string.Format("Erro na descompactação | Fila Id: {0} | Arquivo Id: {1}", zip.Id, zip.File_Id));
                }
            }
        }

        private AnswerSheetBatchQueue Unzip(AnswerSheetBatchQueue zip, Entities.File file)
        {
            AnswerSheetBatchQueue answerSheetBatchQueue = answerSheetBatchFilesBusiness.Unzip(file, zip, zip.EntityId, zip.CreatedBy_Id);
            if ((answerSheetBatchQueue.Validate.IsValid) && (answerSheetBatchQueue.AnswerSheetBatchFiles != null && answerSheetBatchQueue.AnswerSheetBatchFiles.Count > 0))
            {
                Validate valid = answerSheetBatchFilesBusiness.SavePendingIdentification(answerSheetBatchQueue.AnswerSheetBatchFiles, answerSheetBatchQueue.Id, answerSheetBatchQueue.AnswerSheetBatch_Id, answerSheetBatchQueue.School_Id, answerSheetBatchQueue.SupAdmUnit_Id);
                answerSheetBatchQueue.Validate = valid;
                if (valid.IsValid)
                {
                    answerSheetBatchFilesBusiness.AssociateFilesToEntity(answerSheetBatchQueue.AnswerSheetBatchFiles, 0);
                    answerSheetBatchQueue.Situation = EnumBatchQueueSituation.Success;
                }  
                else
                {
                    answerSheetBatchQueue.Situation = EnumBatchQueueSituation.NotUnziped;
                    answerSheetBatchQueue.Description = answerSheetBatchQueue.Validate.Message;
                }
            }
            else
            {
                answerSheetBatchQueue.Situation = EnumBatchQueueSituation.NotUnziped;
                answerSheetBatchQueue.Description = answerSheetBatchQueue.Validate.Message;
            }
                
            return answerSheetBatchQueue;
        }

        private void Update(AnswerSheetBatchQueue entity)
        {
            answerSheetBatchQueueBusiness.Update(entity.Id, entity);
        }

        private void RollbackZipProcessing(AnswerSheetBatchQueue entity)
        {
            answerSheetBatchQueueBusiness.UpdateZipProcessing(entity);
        }
    }
}
