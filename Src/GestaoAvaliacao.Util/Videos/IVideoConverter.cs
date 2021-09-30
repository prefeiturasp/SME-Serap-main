using GestaoAvaliacao.Util.Videos.Dtos;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Util.Videos
{
    public interface IVideoConverter
    {
        Task<ConvertedVideoDto> Convert(Stream inputStream, string contentType, string filenameInput, Guid usu_id);
    }
}