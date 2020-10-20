using AutoMapper;
using ProvaSP.Data;
using ProvaSP.Data.Data.UploadFiles;
using ProvaSP.Model.Entidades;
using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Model.Entidades.UploadFiles.Pagination;
using ProvaSP.Web.Services.UploadFiles.Dtos;
using ProvaSP.Web.Services.UploadFiles.Dtos.Files;
using ProvaSP.Web.Services.UploadFiles.Dtos.Search;
using ProvaSP.Web.Services.UploadFiles.Dtos.Validators;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProvaSP.Web.Services.UploadFiles
{
    public abstract class UploadFileServices
    {
        private readonly IFileManagerServices _fileManagerServices;
        private readonly IDataUploadFileBatch _dataUploadFileBatch;

        protected abstract UploadFileBatchType Type { get; }

        public UploadFileServices()
        {
            _fileManagerServices = new FileManagerServices();
            _dataUploadFileBatch = new DataUploadFileBatch();
        }

        public async Task<UploadFileSearchPageDto> GetAsync(UploadFileSearchDto dto)
        {
            var resultDto = new UploadFileSearchPageDto();
            if (dto is null)
            {
                resultDto.AddErrorMessage("Os dados do lote para ser finalizado não foram informados.");
                return resultDto;
            }

            try
            {
                var filter = new UploadFileBatchFilter(dto.Page, Type, dto.Edicao, dto.AreaDeConhecimento, dto.CicloDeAprendizagem);
                if(!filter.Valid)
                {
                    resultDto.AddErrorMessages(filter.ErrorMessages);
                    return resultDto;
                }

                var entitiesPaginated = await _dataUploadFileBatch.GetAsync(filter);
                if (entitiesPaginated is null || (!entitiesPaginated.Entities?.Any() ?? true)) return resultDto;

                resultDto = Mapper.Map<UploadFileSearchPageDto>(entitiesPaginated);
            }
            catch(Exception ex)
            {
                resultDto.AddErrorMessage(ex);
            }

            return resultDto;
        }

        public async Task<UploadFileBatchDto> AddBatchAsync(AddUploadFileBatchDto dto)
        {
            var resultDto = new UploadFileBatchDto();
            if (dto is null)
            {
                resultDto.AddErrorMessage("Não foram informados os dados para criação do lote para upload.");
                return resultDto;
            }

            var validator = new AddUploadFileBatchDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                resultDto.AddErrorMessages(validationResult.Errors);
                return resultDto;
            }

            try
            {
                if (await _dataUploadFileBatch.AnyBatchActiveAsync(Type))
                {
                    resultDto.AddErrorMessage("Já existe um lote em andameto. Aguarde a finalização.");
                    return resultDto;
                }

                var entity = new UploadFileBatch(
                    Type,
                    dto.Edicao,
                    (UploadFileBatchAreaDeConhecimento)dto.AreaDeConhecimento,
                    (UploadFileBatchCicloDeAprendizagem)dto.CicloDeAprendizagem,
                    dto.UsuId,
                    dto.UsuName);
                if (!entity.Valid)
                {
                    resultDto.AddErrorMessages(entity.ErrorMessages);
                    return resultDto;
                }

                var id = await _dataUploadFileBatch.AddAsync(entity);
                if (id is null)
                {
                    resultDto.AddErrorMessage("Não foi possível criar o lote para upload. Por favor tente novamente.");
                    return resultDto;
                }

                resultDto = Mapper.Map<UploadFileBatchDto>(entity);
                resultDto.Id = id.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                resultDto.AddErrorMessage(ex);
            }

            return resultDto;
        }

        public async Task<StartBatchDto> StartBatchAsync(StartBatchDto dto)
        {
            if (dto is null)
            {
                dto = new StartBatchDto();
                dto.AddErrorMessage("Não foram informados os dados para cancelamento do lote para upload.");
                return dto;
            }

            var validator = new StartBatchDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddErrorMessages(validationResult.Errors);
                return dto;
            }

            try
            {
                var entity = await _dataUploadFileBatch.GetAsync(dto.Id);
                if (entity is null)
                {
                    dto.AddErrorMessage("O lote informado não existe.");
                    return dto;
                }

                entity.Start(dto.FileCount);
                await _dataUploadFileBatch.UpdateAsync(entity);
            }
            catch(Exception ex)
            {
                dto.AddErrorMessage(ex);
            }

            return dto;
        }

        public async Task<CancelBatchDto> CancelBatchAsync(CancelBatchDto dto)
        {
            if(dto is null)
            {
                dto = new CancelBatchDto();
                dto.AddErrorMessage("Não foram informados os dados para cancelamento do lote para upload.");
                return dto;
            }

            var validator = new CancelBatchDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddErrorMessages(validationResult.Errors);
                return dto;
            }

            try
            {
                var uploadFileBatch = await _dataUploadFileBatch.GetAsync(dto.Id);
                if(uploadFileBatch is null)
                {
                    dto.AddErrorMessage("O lote informado não existe.");
                    return dto;
                }

                if (uploadFileBatch.Situation == UploadFileBatchSituation.Canceled) return dto;

                uploadFileBatch.CancelBatch();
                await _dataUploadFileBatch.UpdateAsync(uploadFileBatch);
            }
            catch(Exception ex)
            {
                dto.AddErrorMessage(ex);
            }

            return dto;
        }

        public async Task<CancelOpenedBatchesDto> CancelActiveBatchesAsync(CancelOpenedBatchesDto dto)
        {
            if (dto is null)
            {
                dto = new CancelOpenedBatchesDto();
                dto.AddErrorMessage("Não foram informados os dados para cancelamento dos lotes em aberto.");
                return dto;
            }

            var validator = new CancelOpenedBatchesDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddErrorMessages(validationResult.Errors);
                return dto;
            }

            try
            {
                var uploadFileBatches = await _dataUploadFileBatch.GetActiveBatchesAsync(dto.UsuId, Type);
                if (!uploadFileBatches?.Any() ?? true) return dto;

                foreach(var uploadFileBatch in uploadFileBatches)
                {
                    uploadFileBatch.CancelBatch();
                }
                 
                await _dataUploadFileBatch.UpdateAsync(uploadFileBatches);
            }
            catch (Exception ex)
            {
                dto.AddErrorMessage(ex);
            }

            return dto;
        }

        public async Task<UploadFileItemDto> UploadFileAsync(UploadFileItemDto dto, HttpPostedFile httpPostedFile)
        {
            if(dto is null)
            {
                dto = new UploadFileItemDto();
                dto.AddErrorMessage("Os detalhes do arquivos não foram informados.");
                return dto;
            }

            try
            {
                var configuracao = DataConfiguracao.RetornarConfiguracao(Configuracao.ChaveFileDirectoryPath);

                byte[] dataFile;
                using (var ms = new MemoryStream())
                {
                    httpPostedFile.InputStream.Position = 0;
                    httpPostedFile.InputStream.CopyTo(ms);
                    dataFile = ms.ToArray();
                }

                var path = Path.Combine(configuracao.Valor, dto.DirectoryPath);
                _fileManagerServices.Save(dataFile, FormatFileName(httpPostedFile.FileName), path);
            }
            catch(Exception ex)
            {
                dto.AddErrorMessage(ex);
            }

            return await Task.FromResult(dto);
        }

        public async Task<FinalizeBatchDto> FinalizeBatchAsync(FinalizeBatchDto dto)
        {
            if(dto is null)
            {
                dto = new FinalizeBatchDto();
                dto.AddErrorMessage("Os dados do lote para ser finalizado não foram informados.");
                return dto;
            }

            var validator = new FinalizeBatchDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddErrorMessages(validationResult.Errors);
                return dto;
            }

            try
            {
                var uploadFileBatch = await _dataUploadFileBatch.GetAsync(dto.Id);
                if (uploadFileBatch is null)
                {
                    dto.AddErrorMessage("O lote informado não existe.");
                    return dto;
                }

                if (uploadFileBatch.Situation == UploadFileBatchSituation.Done) return dto;

                uploadFileBatch.FinalizeBatch(dto.FileErrorCount);
                await _dataUploadFileBatch.UpdateAsync(uploadFileBatch);
            }
            catch(Exception ex)
            {
                dto.AddErrorMessage(ex);
            }

            return dto;
        }

        private static string FormatFileName(string fileName)
        {
            if (!fileName.Contains("/")) return fileName;
            var index = fileName.LastIndexOf("/") + 1;
            return fileName.Substring(index, fileName.Length - index);
        }
    }
}