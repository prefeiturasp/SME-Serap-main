using ProvaSP.Model.Abstractions;
using ProvaSP.Model.Utils;
using ProvaSP.Model.Utils.Attributes;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace ProvaSP.Model.Entidades.UploadFiles
{
    public class UploadFileBatch : BaseEntity
    {
        public const short EdicaoMaxLength = 4;
        public const int PageItensCount = 20;

        public long Id { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? BeginDate { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public UploadFileBatchType UploadFileBatchType { get; private set; }
        public string Edicao { get; set; }
        public UploadFileBatchAreaDeConhecimento AreaDeConhecimento { get; private set; }
        public UploadFileBatchCicloDeAprendizagem CicloDeAprendizagem { get; private set; }
        public UploadFileBatchSituation Situation { get; private set; }
        public string DirectoryPath => Path.Combine(UploadFileBatchType.GetPath(), Edicao, AreaDeConhecimento.GetPath(), CicloDeAprendizagem.GetPath());
        public long FileCount { get; private set; }
        public Guid UsuId { get; private set; }

        public UploadFileBatch(UploadFileBatchType uploadFileBatchType, string edicao, UploadFileBatchAreaDeConhecimento areaDeConhecimento,
            UploadFileBatchCicloDeAprendizagem cicloDeAprendizagem, Guid usuId)
        {
            UploadFileBatchType = uploadFileBatchType;
            SetEdicao(edicao);
            SetUsuId(usuId);
            UploadFileBatchType = UploadFileBatchType.RevistasEBoletins;
            AreaDeConhecimento = areaDeConhecimento;
            CicloDeAprendizagem = cicloDeAprendizagem;
            CreatedDate = DateTime.Now;
            Situation = UploadFileBatchSituation.NotStarted;
        }

        protected UploadFileBatch()
        {
        }

        public void Start(int fileCount)
        {
            BeginDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            Situation = UploadFileBatchSituation.InProgress;
            FileCount = fileCount;
        }

        private void SetEdicao(string edicao)
        {
            if (!Regex.IsMatch(edicao, "^[0-9]{" + EdicaoMaxLength + "}$"))
            {
                AddErrorMessage("O ano de edição informado é inválido.");
                return;
            }

            Edicao = edicao;
        }

        private void SetUsuId(Guid usuId)
        {
            if (usuId == Guid.Empty)
            {
                AddErrorMessage("O usuário responsável pelo lote deve ser informado.");
                return;
            }

            UsuId = usuId;
        }

        public void CancelBatch()
        {
            UpdateDate = DateTime.Now;
            Situation = UploadFileBatchSituation.Canceled;
        }

        public void FinalizeBatch()
        {
            UpdateDate = DateTime.Now;
            Situation = UploadFileBatchSituation.Done;
        }
    }

    public enum UploadFileBatchType : short
    {
        [Path("Revistas Pedagógicas")]
        RevistasEBoletins = 1
    }

    public enum UploadFileBatchSituation : short
    {
        [Description("Não iniciado")]
        NotStarted = 0,
        [Description("Em execução")]
        InProgress = 1,
        [Description("Finalizado")]
        Done = 2,
        [Description("Cancelado")]
        Canceled = 3
    }

    public enum UploadFileBatchAreaDeConhecimento : short
    {
        [Description("Ciências da Natureza")]
        [Path("Ciências da Natureza")]
        CienciasDaNatureza = 1,

        [Description("Língua Portuguesa")]
        [Path("Língua Portuguesa")]
        LinguaPortuguesa = 2,

        [Description("Matemática")]
        [Path("Matemática")]
        Matematica = 3,

        [Description("Redação")]
        [Path("Redação")]
        Redacao = 4
    }

    public enum UploadFileBatchCicloDeAprendizagem : short
    {
        [Description("Alfabetização")]
        [Path("Ciclo Alfabetização")]
        Alfabetizacao = 1,

        [Description("Interdisciplinar")]
        [Path("Ciclo Interdisciplinar")]
        Interdisciplinar = 2,

        [Description("Autoral")]
        [Path("Ciclo Autoral")]
        Autoral = 3
    }
}