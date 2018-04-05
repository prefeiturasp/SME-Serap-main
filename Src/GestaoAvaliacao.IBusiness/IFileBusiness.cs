using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IFileBusiness
	{
		File Save(File entity);
		void Update(long id, File entity);
		File Get(long id);
		List<File> GetFilesByOwner(long ownerId, long parentId, EnumFileType ownerType);
		File DeleteFilesNotUsed(double days, int numFiles);
        IEnumerable<File> GetFilesByParent(long parentId, EnumFileType ownerType);
        void AssociateFilesToEntity(long idEntity, List<File> idFiles);
        File Upload(UploadModel model);
		File Delete(long id);
        File Delete(long id, string path);
        File LogicalDelete(long id, Guid UserId, int CoreVisionId);
		void VerifyUnusedFilesByOwner(long ownerId, long parentId, EnumFileType fileType, List<File> usedFiles);
        IEnumerable<File> SearchUploadedFiles(ref Pager pager, FileFilter filter);
        void UpdateOwnerAndParentId(long Id, long ownerId, long? parentId);
        int GetAllFiles(FileFilter filter);
        IEnumerable<string> GetTestNames(long Id);
        bool CheckFileExists(long Id, string physicalDirectory);
        bool CheckFilesExists(IEnumerable<long> Ids, string physicalDirectory);
        File SaveZip(string zipFileName, string folder, IEnumerable<ZipFileInfo> files, string physicalDirectory);
        void ClearFolder(string folderName);
        List<File> DeleteFiles(List<long> filesId, bool exclusionLogic, Guid UserId);
        IEnumerable<EntityFile> GetAllFilesByType(EnumFileType ownerType, DateTime limitDate);
        void DeleteFilesByType(EnumFileType ownerType, DateTime limitDate);
        void DeletePhysicalFiles(List<EntityFile> files, string physicalDirectory);
    }
}
