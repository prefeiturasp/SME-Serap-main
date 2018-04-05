using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.Entities
{
    public class Uploader
	{
		public int ContentLength { get; set; }
		public string ContentType { get; set; }
		public string FileName { get; set; }
		public string InputStream { get; set; }
        public EnumFileType FileType { get; set; }
	}
}
