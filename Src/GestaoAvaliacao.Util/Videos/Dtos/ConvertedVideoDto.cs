using System.IO;

namespace GestaoAvaliacao.Util.Videos.Dtos
{
    public class ConvertedVideoDto
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }
    }
}