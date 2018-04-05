using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class AnswerSheetLotBusiness : IAnswerSheetLotBusiness
	{
        private readonly IAnswerSheetLotRepository answerSheetLotRepository;
        private readonly IFileBusiness fileBusiness;
        private readonly IStorage storage;
        private readonly IParameterBusiness parameterBusiness;

        #region Constructor

        public AnswerSheetLotBusiness(IAnswerSheetLotRepository answerSheetLotRepository, IFileBusiness fileBusiness, IStorage storage, IParameterBusiness parameterBusiness)
		{
			this.answerSheetLotRepository = answerSheetLotRepository;
			this.fileBusiness = fileBusiness;
            this.storage = storage;
            this.parameterBusiness = parameterBusiness;
		}

		#endregion

		#region Read

		public IEnumerable<AnswerSheetLotDTO> GetTestLot(ref Pager pager, AnswerSheetLotFilter filter)
		{
			return answerSheetLotRepository.GetTestLot(ref pager, filter);
		}

        public IEnumerable<AnswerSheetLot> GetLotList(AnswerSheetLotFilter filter, ref Pager pager)
        {
            return answerSheetLotRepository.GetLotList(filter, ref pager);
        }

        public IEnumerable<AnswerSheetLotDTO> GetLotFiles(AnswerSheetLotFilter filter, ref Pager pager)
        {
            return answerSheetLotRepository.GetLotFiles(filter, ref pager);
        }

        public IEnumerable<AnswerSheetLotDTO> GetAdheredTests(AnswerSheetLotFilter filter, ref Pager pager)
        {
            return answerSheetLotRepository.GetAdheredTests(filter, ref pager);
        }

        public IEnumerable<AnswerSheetLot> GetByExecutionState(EnumServiceState state)
		{
			return answerSheetLotRepository.GetByExecutionState(state);
		}

        public IEnumerable<AnswerSheetLot> GetParentByExecutionState(EnumServiceState state)
        {
            return answerSheetLotRepository.GetParentByExecutionState(state);
        }

        public AnswerSheetLot GetById(long Id)
		{
			return answerSheetLotRepository.GetById(Id);
		}

        public IEnumerable<AnswerSheetLot> GetByParentId(long ParentId)
        {
            return answerSheetLotRepository.GetByParentId(ParentId);
        }

        public IEnumerable<Test> GetTestList(long Id)
        {
            return answerSheetLotRepository.GetTestList(Id);
        }

        public int GetTestCount(long Id)
        {
            return answerSheetLotRepository.GetTestCount(Id);
        }

        public AnswerSheetLotHistory GetLotFolderSize(long Id, SYS_Usuario usuarioLogado)
        {
            AnswerSheetLotHistory entity = new AnswerSheetLotHistory { tempFolderSize = "Indisponível", mainFolderSize = "Indisponível" };
            AnswerSheetLot lot = GetById(Id);
            if (lot != null)
            {
                var paramPath = parameterBusiness.GetParamByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), usuarioLogado.ent_id);
                var phisicalPath = paramPath != null ? paramPath.Value : null;

                if (!string.IsNullOrEmpty(phisicalPath))
                {
                    string typeLotFolder = lot.Type.Equals(EnumAnswerSheetBatchOwner.Test) ? "P_" + lot.TestId : "E_" + lot.Id;
                    string path = string.Format("{0}\\{1}\\{2}\\{3}", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, typeLotFolder);

                    if (lot.StateExecution.Equals(EnumServiceState.Processing))
                    {
                        entity.tempFolderSize = storage.GetDirectorySize(path, true);
                    }

                    typeLotFolder = lot.Type.Equals(EnumAnswerSheetBatchOwner.Test) ? "Prova" : "Escola";
                    path = string.Format("{0}\\{1}\\{2}\\{3}", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, typeLotFolder);
                    entity.mainFolderSize = storage.GetDirectorySize(path, true);
                }
            }

            return entity;            
        }

        #endregion

        #region Persist

        public AnswerSheetLot Save(AnswerSheetLot entity, List<AnswerSheetLot> list)
		{
            AnswerSheetLot valid = Validate(entity, ValidateAction.Save, list);
            entity.Validate = valid.Validate;

            if (entity.Validate.IsValid)
            {
                if (entity.Type.Equals(EnumAnswerSheetBatchOwner.Test))
                {
                    entity = answerSheetLotRepository.Save(entity);
                }
                else if (entity.Type.Equals(EnumAnswerSheetBatchOwner.School))
                {
                    entity = answerSheetLotRepository.SaveLot(entity, list);
                }
                else
                {
                    entity = null;
                }

                if (entity != null)
                {
                    entity.Validate.Type = ValidateType.Save.ToString();
                    entity.Validate.Message = entity.StateExecution == EnumServiceState.NotRequest ? "Lote de folha de resposta salvo com sucesso." : entity.StateExecution == EnumServiceState.Pending ? "Lote de folha de resposta solicitado com sucesso."
                    : "Solicitação de lote de folha de resposta cancelada.";
                }
                else
                {
                    entity.Validate.IsValid = false;
                    entity.Validate.Type = ValidateType.alert.ToString();
                    entity.Validate.Message = "Não foi possível atender a solicitação.";
                }
			}

			return entity;
		}

		public AnswerSheetLot GenerateAgain(AnswerSheetLot entity, Guid userId)
		{
            AnswerSheetLot cadastred = ValidateGenerateAgain(entity);
            if (entity.Type.Equals(EnumAnswerSheetBatchOwner.Test))
            {
                if (cadastred != null && cadastred.Validate.IsValid)
                {
                    cadastred.Type = entity.Type;
                    DeleteLotFiles(cadastred, userId);
                    answerSheetLotRepository.Delete(cadastred);
                }

                entity = Save(entity, null);
            }
            else if (entity.Type.Equals(EnumAnswerSheetBatchOwner.School))
            {
                if (cadastred != null && cadastred.Validate.IsValid)
                {
                    cadastred.Type = entity.Type;
                    DeleteLotFiles(cadastred, userId);
                }
                entity = Update(entity, false);
            }

			return entity;
		}

		public AnswerSheetLot Update(AnswerSheetLot entity, bool service, bool updateSubLot = true)
		{
            if (!service)
            {
                AnswerSheetLot valid = Validate(entity, ValidateAction.Update, null);
                entity.Validate = valid.Validate;
            }

            if (entity.Validate.IsValid || service)
            {
                if (entity.Type.Equals(EnumAnswerSheetBatchOwner.Test))
                {
                    entity = answerSheetLotRepository.Update(entity);
                }
                else if (entity.Type.Equals(EnumAnswerSheetBatchOwner.School))
                {
                    if (updateSubLot)
                    {
                        answerSheetLotRepository.UpdateLot(entity);
                    }
                    entity = answerSheetLotRepository.Update(entity);
                }
                else
                {
                    entity = null;
                }

                if (entity != null)
                {
                    entity.Validate.Type = ValidateType.Update.ToString();
                    entity.Validate.Message = "Lote de folha de resposta alterado com sucesso.";
                }
                else
                {
                    entity.Validate.IsValid = false;
                    entity.Validate.Type = ValidateType.alert.ToString();
                    entity.Validate.Message = "Não foi possível alterar o lote de folha de resposta.";
                }
			}

			return entity;
		}

        public AnswerSheetLot Delete(AnswerSheetLot entity)
        {
            AnswerSheetLot valid = Validate(entity, ValidateAction.Delete, null);
            entity.Validate = valid.Validate;

            if (entity.Validate.IsValid)
            {
                if (entity.Type.Equals(EnumAnswerSheetBatchOwner.Test))
                {
                    answerSheetLotRepository.Delete(entity);
                }
                else if (entity.Type.Equals(EnumAnswerSheetBatchOwner.School))
                {
                    answerSheetLotRepository.DeleteLot(entity.Id);
                    answerSheetLotRepository.Delete(entity);
                }
                else
                {
                    entity = null;
                }

                if (entity != null)
                {
                    entity.Validate.Type = ValidateType.Delete.ToString();
                    entity.Validate.Message = "Lote de folha de resposta excluído com sucesso.";
                }
                else
                {
                    entity.Validate.IsValid = false;
                    entity.Validate.Type = ValidateType.alert.ToString();
                    entity.Validate.Message = "Não foi possível excluir o lote de folha de resposta.";
                }
            }

            return entity;
        }

        #endregion

        #region Private Methods

        private AnswerSheetLot Validate(AnswerSheetLot entity, ValidateAction action, List<AnswerSheetLot> list)
		{
            AnswerSheetLot cadastred = new AnswerSheetLot();
            if (entity.Type.Equals(EnumAnswerSheetBatchOwner.Test))
            {
                cadastred = answerSheetLotRepository.GetByTest(entity.TestId);
            }
            else if (entity.Type.Equals(EnumAnswerSheetBatchOwner.School))
            {
                cadastred = answerSheetLotRepository.GetById(entity.Id);
            }

            if (action == ValidateAction.Save || action == ValidateAction.Update)
            {
                if (cadastred != null && cadastred.State == (byte)EnumState.ativo)
                {
                    if (cadastred.StateExecution.Equals(EnumServiceState.Processing) && entity.StateExecution.Equals(EnumServiceState.Canceled))
                        entity.Validate.Message = @"Não é permitido desfazer a geração do lote, pois a situação está ""Em andamento"".";
                    else if (cadastred.StateExecution.Equals(EnumServiceState.Pending) && !entity.StateExecution.Equals(EnumServiceState.Canceled))
                        entity.Validate.Message = "Já existe uma solicitação para esse lote.";
                }

                if ((action == ValidateAction.Save && cadastred == null && entity.Type.Equals(EnumAnswerSheetBatchOwner.School)) 
                        && (list == null || list.Count <= 0 || !list.Any(i => i.Test_Id > 0)))
                {
                    entity.Validate.Message = "Não existem provas selecionadas.";
                }
            }
            else if (action == ValidateAction.Delete)
            {
                cadastred = answerSheetLotRepository.GetById(entity.Id);
                if (cadastred == null)
                {
                    entity.Validate.Message = "Não foi encontrado o lote a ser excluído.";
                    entity.Validate.Code = 404;
                }
            }

            if (!string.IsNullOrEmpty(entity.Validate.Message))
            {
                string br = "<br/>";
                entity.Validate.Message = entity.Validate.Message.TrimStart(br.ToCharArray());

                entity.Validate.IsValid = false;

                if (entity.Validate.Code <= 0)
                    entity.Validate.Code = 400;

                entity.Validate.Type = ValidateType.alert.ToString();
            }
            else
                entity.Validate.IsValid = true;

            return entity;
        }

        private AnswerSheetLot ValidateGenerateAgain(AnswerSheetLot entity)
        {
            var cadastred = entity.Type.Equals(EnumAnswerSheetBatchOwner.Test) ? answerSheetLotRepository.GetByTest(entity.TestId)
                    : answerSheetLotRepository.GetById(entity.Id);
            if (cadastred == null)
            {
                entity.Validate.Message = "Não foi possível encontrar o lote.";
            }

            if (!string.IsNullOrEmpty(entity.Validate.Message))
            {
                string br = "<br/>";
                entity.Validate.Message = entity.Validate.Message.TrimStart(br.ToCharArray());

                entity.Validate.IsValid = false;

                if (entity.Validate.Code <= 0)
                    entity.Validate.Code = 400;

                entity.Validate.Type = ValidateType.alert.ToString();
            }
            else
                entity.Validate.IsValid = true;

            return entity;
        }

        private void DeleteLotFiles(AnswerSheetLot entity, Guid userId)
        {
            //remover os arquivos atuais de lotes da prova
            if (entity.Type.Equals(EnumAnswerSheetBatchOwner.Test))
            {
                var file = fileBusiness.GetFilesByOwner(entity.Id, entity.TestId, EnumFileType.AnswerSheetLot).FirstOrDefault();
                if (file != null)
                    fileBusiness.Delete(file.Id);
            }
            else if (entity.Type.Equals(EnumAnswerSheetBatchOwner.School))
            {
                var file = answerSheetLotRepository.GetFiles(entity.Id);
                if (file != null)
                {
                    fileBusiness.DeleteFiles(file.ToList(), true, userId);
                }
            }
        }

        #endregion
    }
}
