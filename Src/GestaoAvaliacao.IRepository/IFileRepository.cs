using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IFileRepository
	{
		File Save(File entity);
		void Update(long id, File entity);
		File Get(long id);
		IEnumerable<File> Load(ref Pager pager);
		void Delete(long id);
		void SaveList(List<File> list);
		void UpdateList(List<File> list);
		void DeleteList(List<File> list);
		void DeleteByParentId(long parentId, EnumFileType ownerType);
		void DeleteFilesNotUsed(File file);
        void LogicalDelete(long id, Guid UserId);
		List<File> _GetFilesNotUsed(double days, int numFiles);
		List<File> GetFilesByOwner(long ownerId, long parentId, EnumFileType ownerType);
		List<File> _GetFilesByParent(long parentId);
        IEnumerable<File> GetFilesByParent(long parentId, EnumFileType ownerType);
		void AssociateFilesToEntity(long idEntity, List<File> idFiles);
        IEnumerable<File> SearchUploadedFiles(ref Pager pager, FileFilter filter);
        void UpdateOwnerAndParentId(long Id, long ownerId, long? parentId);
        int GetAllFiles(FileFilter filter);
        IEnumerable<string> GetTestNames(long Id);
        IEnumerable<File> GetAllFilesByType(EnumFileType ownerType, DateTime limitDate);
        void DeleteFilesByType(EnumFileType ownerType, DateTime limitDate);
    }
}
