using ProvaSP.Model.Abstractions;
using System;

namespace ProvaSP.Model.Entidades.UploadFiles.Itens
{
    public class UploadFileItem : BaseEntity
    {
        public long Id { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public string OriginPath { get; private set; }
        public string FileName { get; private set; }
        public UploadFileItemSituation Situation { get; private set; }
        public string NotificationMessage { get; private set; }
        public UploadFileBatch UploadFileBatch { get; private set; }

        public UploadFileItem(long id, string originPath, string fileName, UploadFileBatch uploadFileBatch)
            : base()
        {
            SetId(id);
            SetOriginPath(originPath);
            SetFileName(fileName);
            SetUploadFileBatch(uploadFileBatch);

            CreatedDate = DateTime.Now;
            Situation = UploadFileItemSituation.NotStarted;
        }

        private void SetId(long id)
        {
            if(id <= 0)
            {
                AddErrorMessage("O id do item deve ser informado.");
                return;
            }

            Id = id;
        }

        private void SetOriginPath(string originPath)
        {
            if(string.IsNullOrWhiteSpace(originPath))
            {
                AddErrorMessage("O caminho do arquivo deve ser informado.");
                return;
            }

            OriginPath = originPath;
        }

        private void SetFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                AddErrorMessage("O nome do arquivo deve ser informado.");
                return;
            }

            FileName = fileName;
        }

        private void SetUploadFileBatch(UploadFileBatch uploadFileBatch)
        {
            if (uploadFileBatch is null)
            {
                AddErrorMessage("O id do lote deve ser informado.");
                return;
            }

            if(!uploadFileBatch.Valid)
            {
                AddErrorMessage("O lote informado não é válido.");
                return;
            }

            UploadFileBatch = uploadFileBatch;
        }

        public void SetSituation(UploadFileItemSituation situation, string resultMessage = "")
        {
            Situation = situation;
            if (!string.IsNullOrWhiteSpace(resultMessage))
                NotificationMessage = resultMessage;
        }
    }

    public enum UploadFileItemSituation : short
    {
        NotStarted = 0,
        InProgress = 1,
        Done = 2,
        Canceled = 3,
        Error = 4
    }
}