using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class ModelTest : EntityBase
	{
		public Guid EntityId { get; set; }
		public string Description { get; set; }
		public bool DefaultModel { get; set; } 

		#region Opções do modelo
		public bool ShowCoverPage { get; set; }
		public bool ShowBorder { get; set; }
		#endregion

		#region Cabeçalho
		public EnumPosition LogoHeaderPosition { get; set; }
		public EnumSize LogoHeaderSize { get; set; }
		public bool LogoHeaderWaterMark { get; set; }
		public EnumPosition MessageHeaderPosition { get; set; }
		public bool ShowMessageHeader { get; set; }
		public string MessageHeader { get; set; }
		public bool MessageHeaderWaterMark { get; set; }
		public bool ShowLineBelowHeader { get; set; }
		public bool ShowLogoHeader { get; set; }
		public File LogoHeader { get; set; }
		public Nullable<long> FileHeader_Id { get; set; }
		public string HeaderHtml { get; set; }
		#endregion

		#region Rodapé
		public EnumPosition LogoFooterPosition { get; set; }
		public EnumSize LogoFooterSize { get; set; }
		public bool LogoFooterWaterMark { get; set; }
		public EnumPosition MessageFooterPosition { get; set; }
		public bool ShowMessageFooter { get; set; }
		public string MessageFooter { get; set; }
		public bool MessageFooterWaterMark { get; set; }
		public bool ShowLineAboveFooter { get; set; }
		public bool ShowLogoFooter { get; set; }
		public File LogoFooter { get; set; }
		public Nullable<long> FileFooter_Id { get; set; }
		public string FooterHtml { get; set; }
		#endregion

		#region  Dados do Estudante / Escola
		public bool ShowSchool { get; set; }
		public bool ShowStudentName { get; set; }
		public bool ShowTeacherName { get; set; }
		public bool ShowClassName { get; set; }
		public bool ShowStudentNumber { get; set; }
		public bool ShowDate { get; set; }
		public bool ShowLineBelowStudentInformation { get; set; }
		public string StudentInformationHtml { get; set; }
		#endregion

		#region DadosCapa
		public string CoverPageText { get; set; }
		public bool ShowStudentInformationsOnCoverPage { get; set; }
		public bool ShowHeaderOnCoverPage { get; set; }
		public bool ShowFooterOnCoverPage { get; set; }
		public bool ShowBorderOnCoverPage { get; set; }
		#endregion

        #region Itens da prova

        public bool ShowItemLine { get; set; }

        #endregion

        public List<File> Files { get; set; }
	}

}
