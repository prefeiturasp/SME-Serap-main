using GestaoAvaliacao.Util.Videos.Dtos;
using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaToolkit;
using MediaToolkit.Model;

namespace GestaoAvaliacao.Util.Videos
{
    public class VideoConverter : IVideoConverter
    {
        public async Task<ConvertedVideoDto> Convert(Stream inputStream, string contentType, string filenameInput, Guid usu_id)
        {
            var inputFormat = contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1);
            var pathTemporary = Path.Combine(Path.GetTempPath(), usu_id.ToString());
            var fileNameOutPut = $"{filenameInput.Replace($".{inputFormat}", string.Empty)}.webm";
            var pathTemporaryInputFile = Path.Combine(pathTemporary, filenameInput);
            var pathTemporaryOutputFile = Path.Combine(pathTemporary, fileNameOutPut);
            var outputStream = new MemoryStream();

            CreateDirectory(pathTemporary);
            DeleteTemporaryTemp(pathTemporaryInputFile, pathTemporaryOutputFile, null);
            await CreateFileInput(inputStream, pathTemporaryInputFile);
            ConvertFile(pathTemporaryInputFile, pathTemporaryOutputFile);
            await CreateFileOutPut(pathTemporaryOutputFile, outputStream);
            DeleteTemporaryTemp(pathTemporaryInputFile, pathTemporaryOutputFile, pathTemporary);

            return
                new ConvertedVideoDto
                {
                    FileName = fileNameOutPut,
                    Stream = outputStream
                };
        }

        private async Task CreateFileOutPut(string pathTemporaryOutputFile, MemoryStream outputStream)
        {
            using (Stream fileStream = File.Open(pathTemporaryOutputFile, FileMode.Open))
            {
                await fileStream.CopyToAsync(outputStream);
            }
        }

        private void ConvertFile(string pathTemporaryInputFile, string pathTemporaryOutputFile)
        {
            using (var engine = new Engine())
            {
                engine.Convert(new MediaFile(pathTemporaryInputFile), new MediaFile(pathTemporaryOutputFile));
            }
        }

        private async Task CreateFileInput(Stream inputStream, string pathTemporaryInputFile)
        {
            using (var fileStream = new FileStream(pathTemporaryInputFile, FileMode.Create, FileAccess.Write))
            {
                await inputStream.CopyToAsync(fileStream);
            }
        }

        private void CreateDirectory(string pathTemporary)
        {
            if (!Directory.Exists(pathTemporary))
                Directory.CreateDirectory(pathTemporary);
        }

        private void DeleteTemporaryTemp(string pathTemporaryInputFile, string pathTemporaryOutputFile, string pathTemporary)
        {
            if (File.Exists(pathTemporaryInputFile))
                File.Delete(pathTemporaryInputFile);
            if (File.Exists(pathTemporaryOutputFile))
                File.Delete(pathTemporaryOutputFile);
            if (pathTemporary != null)
                Directory.Delete(pathTemporary);
        }
    }
}
