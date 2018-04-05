using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class FileBusiness : IFileBusiness
	{
		private readonly IFileRepository FileRepository;
		private readonly IStorage storage;
		private readonly IModelTestRepository modelTestRepository;

		public FileBusiness(IFileRepository FileRepository, IStorage storage, IModelTestRepository modelTestRepository)
		{
			this.FileRepository = FileRepository;
			this.storage = storage;
			this.modelTestRepository = modelTestRepository;
		}
		
		#region Custom

		private Validate Validate(EntityFile entity, ValidateAction action, Validate valid, Guid UserId, int CoreVisionId)
		{
			valid.Message = null;
			if (action == ValidateAction.Delete)
			{
				EntityFile ent = Get(entity.Id);
				if (ent == null)
				{
					valid.Message = "Não foi encontrado o arquivo a ser excluído.";
					valid.Code = 404;
				}
				else
				{
					if ((CoreVisionId > 0 && CoreVisionId == (int)EnumSYS_Visao.Individual) && (!ent.CreatedBy_Id.Equals(UserId)))
					{
							valid.Message = "Você não tem permissão para excluir este arquivo.";
							valid.Code = 404;
					}
				}
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
				valid.IsValid = true;

			return valid;
		}

		#endregion

		#region Read

		public EntityFile Get(long Id)
		{
			return FileRepository.Get(Id);
		}

		public List<EntityFile> GetFilesByOwner(long ownerId, long parentId, EnumFileType ownerType)
		{
			return FileRepository.GetFilesByOwner(ownerId, parentId, ownerType);
		}

		public IEnumerable<EntityFile> GetFilesByParent(long parentId, EnumFileType ownerType)
        {
			return FileRepository.GetFilesByParent(parentId, ownerType);
		}

		public IEnumerable<EntityFile> SearchUploadedFiles(ref Pager pager, FileFilter filter)
		{
			return FileRepository.SearchUploadedFiles(ref pager, filter);
		}

		public bool CheckFileExists(long Id, string physicalDirectory)
		{
			EntityFile file = FileRepository.Get(Id);

			if (file != null)
			{
                string filePath = new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty);
                string physicalPath = string.Concat(physicalDirectory, filePath.Replace("/", "\\"));
                string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                if (System.IO.File.Exists(decodedUrl))
				{
					return true;
				}
			}

			return false;
		}

        public bool CheckFilesExists(IEnumerable<long> Ids, string physicalDirectory)
        {
            bool retorno = false;

            foreach (long Id in Ids)
            {
                EntityFile file = FileRepository.Get(Id);

                if (file != null)
                {
                    string filePath = new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty);
                    string physicalPath = string.Concat(physicalDirectory, filePath.Replace("/", "\\"));
                    string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                    if (System.IO.File.Exists(decodedUrl))
                    {
                        retorno = true;
                    }
                }

                if (!retorno)
                    break;
            }

            return retorno;
        }

		public int GetAllFiles(FileFilter filter)
		{
			return FileRepository.GetAllFiles(filter);
		}

		public IEnumerable<string> GetTestNames(long Id)
		{
			return FileRepository.GetTestNames(Id);
		}

        public IEnumerable<EntityFile> GetAllFilesByType(EnumFileType ownerType, DateTime limitDate)
        {
            return FileRepository.GetAllFilesByType(ownerType, limitDate);
        }

        #endregion

        #region Write

        public EntityFile Save(EntityFile entity)
		{
			return FileRepository.Save(entity);
		}

		public void Update(long Id, EntityFile entity)
		{
			entity.Id = Id;
			FileRepository.Update(Id, entity);
		}

		public void AssociateFilesToEntity(long idEntity, List<EntityFile> idFiles)
		{
			FileRepository.AssociateFilesToEntity(idEntity, idFiles);
		}

		/// <summary>
		/// Método de limpeza dos arquivos não utilizados
		/// </summary>
		/// <param name="days">número de dias que serão subtraídos da data atual </param>
		/// <param name="numFiles">número de arquivos que serão deletados</param>
		public EntityFile DeleteFilesNotUsed(double days, int numFiles)
		{
			EntityFile entity = new EntityFile();
			List<EntityFile> files = FileRepository._GetFilesNotUsed(days, numFiles);
			foreach (EntityFile file in files)
			{
				entity = storage.Delete(file.Name);
				if (entity.Validate.IsValid)
					FileRepository.DeleteFilesNotUsed(file);
				else
					break;
			}

			return entity;
		}

		public EntityFile Upload(UploadModel model)
		{
			EntityFile entityFile = new EntityFile();
			if (model != null && model.ContentLength > 0)
			{
				byte[] data = null;
				if (!string.IsNullOrEmpty(model.InputStream))
				{
					string[] stream = model.InputStream.Split(',');
					data = System.Convert.FromBase64String(stream.Length > 1 ? stream[1] : stream[0]);
				}
				else if (model.Stream != null)
				{
					using (MemoryStream ms = new MemoryStream())
					{
						model.Stream.CopyTo(ms);
						data = ms.ToArray();
					}
				}
				else if (model.Data != null)
					data = model.Data;

				if (data != null)
				{
					string name = Guid.NewGuid() + Path.GetExtension(model.FileName);
					string originalName = model.FileName;
					string folderName = string.Format("{0}\\{1}\\{2}", model.FileType.GetDescription(), DateTime.Today.Year, DateTime.Today.Month);

					EntityFile fileUploaded = storage.Save(data, name, model.ContentType, folderName, model.VirtualDirectory, model.PhysicalDirectory, out entityFile);

					if (model.FileType.Equals(EnumFileType.File) || model.FileType.Equals(EnumFileType.AnswerSheetBatch))
					{
						fileUploaded.OriginalName = originalName;
						fileUploaded.CreatedBy_Id = model.UsuId;
					}

					entityFile.Validate = fileUploaded.Validate;
					entityFile.OwnerType = (byte)model.FileType;

					if (fileUploaded.Validate.IsValid)
					{
                        // Get image resolution
                        try
                        {
                            Image image = (Image)new ImageConverter().ConvertFrom(data);
                            entityFile.HorizontalResolution = image.HorizontalResolution;
                            entityFile.VerticalResolution = image.VerticalResolution;
                        }
                        catch
                        {
                        }

                        entityFile = FileRepository.Save(fileUploaded);

						if (!entityFile.Validate.IsValid)
						{
							// problem saving the file
							storage.Delete(null, "\\" + folderName + "\\" + name);
						}
						else
						{
							if (model.File != null)
							{
								try
								{
									model.File.Delete();

									if (model.FileType.Equals(EnumFileType.File))
									{
										UpdateOwnerAndParentId(entityFile.Id, entityFile.Id, entityFile.Id);
									}
								}
								catch (Exception e)
								{
									//  problem deleting the file
									FileRepository.Delete(entityFile.Id);
									storage.Delete(null, "\\" + folderName + "\\" + name);

									entityFile.Validate.IsValid = false;
									entityFile.Validate.Type = ValidateType.alert.ToString();
									entityFile.Validate.Message = e.Message;
								}
							}

                            if (entityFile.Validate.IsValid)
							{
								entityFile.Validate.Message = "Arquivo enviado com sucesso.";
							}
						}
					}
				}
			}
			else
			{
				entityFile.Validate.IsValid = false;
				entityFile.Validate.Type = ValidateType.alert.ToString();
				entityFile.Validate.Code = 400;
				entityFile.Validate.Message = "Não foi encontrado nenhum arquivo para fazer upload.";
			}

			return entityFile;
		}

		public void VerifyUnusedFilesByOwner(long ownerId, long parentId, EnumFileType fileType, List<EntityFile> usedFiles)
		{
			List<EntityFile> allFiles = FileRepository.GetFilesByOwner(ownerId, parentId, fileType);

			foreach (EntityFile file in allFiles.Where(i => !usedFiles.Any(f => f.Id.Equals(i.Id))))
				DeleteFile(file);
		}

		private EntityFile DeleteFile(EntityFile file)
		{
			if (file != null)
			{
				if (file.OwnerType == (byte)EnumFileType.ModelTestHeader || file.OwnerType == (byte)EnumFileType.ModelTestFooter)
					modelTestRepository.RemoveFileFromEntity(file.Id);

				FileRepository.DeleteFilesNotUsed(file);
				storage.Delete(file.Name);
				file.Validate.IsValid = true;
				file.Validate.Message = "Arquivo removido com sucesso.";
			}
			else
			{
				file = new EntityFile();
				file.Validate.IsValid = false;
				file.Validate.Type = ValidateType.error.ToString();
				file.Validate.Message = "Arquivo não encontrado.";
			}
			return file;
		}

		public EntityFile Delete(long id)
		{
			return DeleteFile(FileRepository.Get(id));
		}

        public EntityFile Delete(long id, string path)
        {
            FileRepository.Delete(id);
            return storage.DeleteByPath(path);
        }

		public EntityFile LogicalDelete(long id, Guid UserId, int CoreVisionId)
		{
			EntityFile entity = new EntityFile { Id = id };

			entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate, UserId, CoreVisionId);
			if (entity.Validate.IsValid)
			{
				//Exclui logicamente do banco o arquivo e seus vinculos
				FileRepository.LogicalDelete(entity.Id, UserId);

				//Exclui o arquivo fisico do storage
				EntityFile file = Get(entity.Id);
				storage.Delete(file.Name);

				entity.Validate.IsValid = true;
				entity.Validate.Message = "Arquivo excluído com sucesso.";
			}

			return entity;
		}

		public void UpdateOwnerAndParentId(long Id, long ownerId, long? parentId)
		{
			FileRepository.UpdateOwnerAndParentId(Id, ownerId, parentId);
		}

		public EntityFile SaveZip(string zipFileName, string folder, IEnumerable<ZipFileInfo> files, string physicalDirectory)
		{
            return storage.SaveZip(zipFileName, folder, files, physicalDirectory);
		}

		public void ClearFolder(string folderName)
		{
			storage.ClearFolder(folderName);
		}

		public List<EntityFile> DeleteFiles(List<long> filesId, bool exclusionLogic, Guid UserId)
		{
			List<EntityFile> result = new List<Entities.File>();
			foreach (long id in filesId)
			{
				EntityFile entity = new EntityFile { Id = id };
				if (exclusionLogic)
				{
					//Exclui logicamente do banco o arquivo e seus vinculos
					FileRepository.LogicalDelete(entity.Id, UserId);
				}

				//Exclui o arquivo fisico do storage
				EntityFile file = Get(entity.Id);
				if (file == null)
				{
					entity.Validate.IsValid = false;
					entity.Validate.Message = "Não foi encontrado o arquivo a ser excluído.";
					entity.Validate.Code = 404;
				}
				else {
					var path = file.Path.Split(new string[] { "Files" }, StringSplitOptions.None).Last();
					storage.Delete(null, path);
					entity.Validate.IsValid = true;
					entity.Validate.Message = "Arquivo excluído com sucesso.";
				}

				result.Add(entity);
			}

			return result;
		}

        public void DeleteFilesByType(EnumFileType ownerType, DateTime limitDate)
        {
            FileRepository.DeleteFilesByType(ownerType, limitDate);
        }

        public void DeletePhysicalFiles(List<EntityFile> files, string physicalDirectory)
        {
            foreach (EntityFile file in files)
            {
                if (file != null)
                {
                    string filePath = new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty);
                    string physicalPath = string.Concat(physicalDirectory, filePath.Replace("/", "\\"));
                    string decodedUrl = HttpUtility.UrlDecode(physicalPath);
                    storage.Delete(null, null, decodedUrl);
                }
            }
        }

        #endregion
    }
}
