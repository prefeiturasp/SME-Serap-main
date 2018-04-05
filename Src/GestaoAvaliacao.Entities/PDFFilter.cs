using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using System.Collections.Generic;
using System.ComponentModel;

namespace GestaoAvaliacao.Entities
{
	public class PDFFilter
	{
		public PDFFilter()
		{
			SaveToDB = true;
		}

		public Booklet Booklet { get; set; }
		public Test Test { get; set; }
		public List<Item> ItemList { get; set; }
		public List<BlockItem> BlockItemList { get; set; }
		public IEnumerable<Parameter> Parameters { get; set; }
		public EnumGenerateType GenerateType { get; set; }
		public EnumFileType FileType { get; set; }
		public EnumIdentificationType IdentificationType { get; set; }

		public List<AnswerSheetStudentInformation> StudentList { get; set; }
		public AnswerSheetStudentInformation Student { get; set; }
		public int FontSize { get; set; } //fontsize = 20px -> tamanho que mais se aproxima do fontsize 12 do word
		public string HtmlContent { get; set; }
		public bool CDNMathJax { get; set; }
		public bool GenerateAnswerSheet { get; set; }
		public bool PDFPreview { get; set; }

		public byte[] PDFBytes { get; set; }
		public string ContentType { get; set; }
		public string UrlSite { get; set; }
		public string VirtualDirectory { get; set; }
		public string PhysicalDirectory { get; set; }
		public string FolderName { get; set; }
		public string FileName { get; set; }
		public string OriginalName { get; set; }

		public int QtdeItens { get; set; }
		[DefaultValue(true)]
		public bool SaveToDB { get; set; }

		public File Logo { get; set; }
		public long OwnerId { get; set; }
		public long ParentOwnerId { get; set; }
	}
}
