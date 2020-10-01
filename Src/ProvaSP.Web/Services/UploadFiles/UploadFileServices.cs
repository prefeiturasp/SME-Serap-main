using ProvaSP.Data.Data.UploadFiles;
using ProvaSP.Data.Data.UploadFiles.Itens;
using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Model.Entidades.UploadFiles.Itens;
using ProvaSP.Web.Services.UploadFiles.Dtos;
using ProvaSP.Web.Services.UploadFiles.Dtos.Itens;
using ProvaSP.Web.Services.UploadFiles.Dtos.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProvaSP.Web.Services.UploadFiles
{
    public class UploadFileServices : IUploadFileServices
    {
        private readonly IDataUploadFileBatch _dataUploadFileBatch;
        private readonly IDataUploadFileItem _dataUploadFileItem;

        public UploadFileServices()
        {
            _dataUploadFileBatch = new DataUploadFileBatch();
            _dataUploadFileItem = new DataUploadFileItem();
        }

        public async Task<UploadFileBatchDto> AddBatchAsync(AddUploadFileBatchDto dto)
        {
            var resultDto = new UploadFileBatchDto();
            if(dto is null)
            {
                resultDto.AddErrorMessage("Não foram informados os dados para criação do lote para upload.");
                return resultDto;
            }

            var validator = new AddUploadFileBatchDtoValidator();
            var validationResult = validator.Validate(dto);
            if(!validationResult.IsValid)
            {
                resultDto.AddErrorMessages(validationResult.Errors);
                return resultDto;
            }

            try
            {
                if(await _dataUploadFileBatch.AnyBatchActiveAsync())
                {
                    resultDto.AddErrorMessage("Já existe um lote em andameto. Aguarde a finalização.");
                    return resultDto;
                }

                var entity = new UploadFileBatch(dto.Edicao, (UploadFileBatchAreaDeConhecimento)dto.AreaDeConhecimento,
                    (UploadFileBatchCicloDeAprendizagem)dto.CicloDeAprendizagem, dto.UsuId);
                if(!entity.Valid)
                {
                    resultDto.AddErrorMessages(entity.ErrorMessages);
                    return resultDto;
                }

                var id = await _dataUploadFileBatch.AddAsync(entity);
                if(id is null)
                {
                    resultDto.AddErrorMessage("Não foi possível criar o lote para upload. Por favor tente novamente.");
                    return resultDto;
                }

                resultDto.Id = id.GetValueOrDefault();
                resultDto.AreaDeConhecimento = entity.AreaDeConhecimento.ToString();
                resultDto.CicloDeAprendizagem = entity.CicloDeAprendizagem.ToString();
                resultDto.Situacao = entity.Situation.ToString();
                resultDto.UsuId = entity.UsuId;
            }
            catch(Exception ex)
            {
                resultDto.AddErrorMessage(ex);
            }

            return resultDto;
        }

        public async Task<bool> StartBatch(long id)
        {
            if (id <= 0) return false;

            try
            {
                var entity = await _dataUploadFileBatch.GetAsync(id);
                if (entity is null) return false;

                if(!await _dataUploadFileItem.AnyAsync(id)) return false;

                entity.Start();
                await _dataUploadFileBatch.UpdateAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ListUploadFileItemDto> AddBatchItensAsync(long uploadFileBatchId, IEnumerable<AddUploadFileItemDto> itens)
        {
            var resultDto = new ListUploadFileItemDto();
            if(!itens?.Any() ?? true)
            {
                resultDto.AddErrorMessage("");
                return resultDto;
            }

            try
            {
                var taskBatch = _dataUploadFileBatch.GetAsync(uploadFileBatchId);
                var taskNextId = _dataUploadFileItem.GetNextIdAsync();
                await Task.WhenAll(taskBatch, taskNextId);

                var nextId = taskNextId.Result;
                if (nextId <= 0)
                {
                    resultDto.AddErrorMessage("");
                    return resultDto;
                }

                if(taskBatch.Result is null)
                {
                    resultDto.AddErrorMessage("");
                    return resultDto;
                }

                var entities = itens
                    .Select(x => new UploadFileItem(nextId++, x.Path, x.FileName, taskBatch.Result))
                    .ToList();

                await _dataUploadFileItem.AddAsync(entities);

            }
            catch(Exception ex)
            {
                resultDto.AddErrorMessage(ex);
            }

            return resultDto;
        }
    }
}