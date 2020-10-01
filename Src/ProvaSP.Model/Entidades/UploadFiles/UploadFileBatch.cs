using ProvaSP.Model.Abstractions;
using ProvaSP.Model.Entidades.UploadFiles.Itens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ProvaSP.Model.Entidades.UploadFiles
{
    public class UploadFileBatch : BaseEntity
    {
        private const short EdicaoMaxLength = 4;

        public long Id { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? BeginDate { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public string Edicao { get; set; }
        public UploadFileBatchAreaDeConhecimento AreaDeConhecimento { get; private set; }
        public UploadFileBatchCicloDeAprendizagem CicloDeAprendizagem { get; private set; }
        public UploadFileBatchSituation Situation { get; private set; }
        public ICollection<UploadFileItem> Itens { get; private set; }
        public Guid UsuId { get; private set; }

        public UploadFileBatch(string edicao, UploadFileBatchAreaDeConhecimento areaDeConhecimento, UploadFileBatchCicloDeAprendizagem cicloDeAprendizagem, Guid usuId)
        {
            SetEdicao(edicao);
            SetUsuId(usuId);
            AreaDeConhecimento = areaDeConhecimento;
            CicloDeAprendizagem = cicloDeAprendizagem;

            CreatedDate = DateTime.Now;
            Situation = UploadFileBatchSituation.NotStarted;
            Itens = new List<UploadFileItem>();
        }

        public void Start()
        {
            BeginDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            Situation = UploadFileBatchSituation.InProgress;
        }

        private void SetEdicao(string edicao)
        {
            if(Regex.IsMatch(edicao, "^[0-9]{" + EdicaoMaxLength + "}+$"))
            {
                AddErrorMessage("O ano de edição informado é inválido.");
                return;
            }

            Edicao = edicao;
        }

        private void SetUsuId(Guid usuId)
        {
            if(usuId == Guid.Empty)
            {
                AddErrorMessage("O usuário responsável pelo lote deve ser informado.");
                return;
            }

            UsuId = usuId;
        }
    }

    public enum UploadFileBatchSituation : short
    {
        NotStarted = 0,
        InProgress = 1,
        Done = 2,
        Canceled = 3
    }

    public enum UploadFileBatchAreaDeConhecimento : short
    {
        [Description("Ciências da Natureza")]
        CienciasDaNatureza = 1,
        [Description("Língua Portuguesa")]
        LinguaPortuguesa = 2,
        [Description("Matemática")]
        Matematica = 3,
        [Description("Redação")]
        Redacao = 4
    }

    public enum UploadFileBatchCicloDeAprendizagem : short
    {
        [Description("Alfabetização")]
        Alfabetizacao = 1,
        [Description("Interdisciplinar")]
        Interdisciplinar = 2,
        [Description("Autoral")]
        Autoral = 3
    }
}