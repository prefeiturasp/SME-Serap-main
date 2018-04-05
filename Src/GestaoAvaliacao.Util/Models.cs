using System;
using System.IO;

namespace GestaoAvaliacao.Util
{
    public class UploadModel
    {
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
		public string InputStream { get; set; }
        public Stream Stream { get; set; }
        public string VirtualDirectory { get; set; }
        public string PhysicalDirectory { get; set; }
        public EnumFileType FileType { get; set; }
        public Guid? UsuId { get; set; }
        public byte[] Data { get; set; }
		public FileInfo File { get; set; }
    }
}
