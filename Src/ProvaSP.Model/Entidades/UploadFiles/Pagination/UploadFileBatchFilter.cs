using ProvaSP.Model.Abstractions;
using System;

namespace ProvaSP.Model.Entidades.UploadFiles.Pagination
{
    public class UploadFileBatchFilter : Notificable
    {
        public const short RowsPerPage = 10;

        public int Page { get; private set; }
        public string Edicao { get; private set; }
        public UploadFileBatchType UploadFileBatchType { get; private set; }
        public UploadFileBatchAreaDeConhecimento? AreaDeConhecimento { get; private set; }
        public UploadFileBatchCicloDeAprendizagem? CicloDeAprendizagem { get; private set; }

        public UploadFileBatchFilter(int page, UploadFileBatchType uploadFileBatchType, string edicao, UploadFileBatchAreaDeConhecimento? areaDeConhecimento,
            UploadFileBatchCicloDeAprendizagem? cicloDeAprendizagem)
        {
            SetPage(page);
            Edicao = edicao;
            UploadFileBatchType = uploadFileBatchType;
            AreaDeConhecimento = areaDeConhecimento;
            CicloDeAprendizagem = cicloDeAprendizagem;
        }

        public UploadFileBatchFilter(int page, UploadFileBatchType uploadFileBatchType, string edicao, short? areaDeConhecimento, short? cicloDeAprendizagem)
        {
            SetPage(page);
            Edicao = edicao;
            UploadFileBatchType = uploadFileBatchType;
            SetAreaDeConhecimento(areaDeConhecimento);
            SetCicloDeAprendizagem(cicloDeAprendizagem);
        }

        public UploadFileBatchFilter(int page, short uploadFileBatchType, string edicao, short? areaDeConhecimento, short? cicloDeAprendizagem)
        {
            SetPage(page);
            Edicao = edicao;
            SetType(uploadFileBatchType);
            SetAreaDeConhecimento(areaDeConhecimento);
            SetCicloDeAprendizagem(cicloDeAprendizagem);
        }

        private void SetPage(int page)
        {
            if (page <= 0) page = 1;
            Page = page;
        }

        private void SetType(short uploadFileBatchType)
        {
            if (!Enum.IsDefined(typeof(UploadFileBatchType), uploadFileBatchType))
            {
                AddErrorMessage("O tipo de arquivo deve ser informado.");
                return;
            }

            UploadFileBatchType = (UploadFileBatchType)uploadFileBatchType;
        }

        private void SetAreaDeConhecimento(short? areaDeConhecimento)
        {
            if (areaDeConhecimento is null)
            {
                AreaDeConhecimento = null;
                return;
            }

            if (!Enum.IsDefined(typeof(UploadFileBatchAreaDeConhecimento), areaDeConhecimento))
            {
                AddErrorMessage("A área de conhecimento informada é inválida.");
                return;
            }

            AreaDeConhecimento = (UploadFileBatchAreaDeConhecimento)areaDeConhecimento;
        }

        private void SetCicloDeAprendizagem(short? cicloDeAprendizagem)
        {
            if (cicloDeAprendizagem is null)
            {
                CicloDeAprendizagem = null;
                return;
            }

            if (!Enum.IsDefined(typeof(UploadFileBatchCicloDeAprendizagem), cicloDeAprendizagem))
            {
                AddErrorMessage("O ciclo de aprendizagem informado é inválido.");
                return;
            }

            CicloDeAprendizagem = (UploadFileBatchCicloDeAprendizagem)cicloDeAprendizagem;
        }
    }
}