using GestaoAvaliacao.Util.Videos.Dtos;
using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Util.Videos
{
    public class VideoConverter : IVideoConverter
    {
        public const string webm = "webm";
        public const string mpeg = "mpeg";
        public const string mp4 = "mp4";
        public const string h265 = "h265";
        public const string h264 = "h264";
        public const string h263 = "h263";
        public const string h261 = "h261";
        public const string wmv = "asf";
        public const string swf = "swf";
        public const string avi = "avi";
        public const string flv = "flv";
        public const string gif = "gif";

        private static IEnumerable<string> _formats = new List<string> { webm, mpeg, mp4, h265, h264, h263, h261, wmv, swf, avi, flv, gif };
        private event EventHandler<ConvertProgressEventArgs> _reportProgress;

        public Task<ConvertedVideoDto> Convert(Stream inputStream, string inputFormat, string fileName, string outputFormat, Action reportProgressAction = null,
            bool appendSilentAudioStream = false)
        {
            if (inputStream is null) throw new ArgumentNullException("O Stream com o vídeo original deve ser informado.");
            if (string.IsNullOrWhiteSpace(inputFormat)) throw new ArgumentException("O formato do vídeo original deve ser informado.");
            if (!_formats.Contains(inputFormat)) throw new InvalidOperationException("O formato do vídeo original é inválido.");
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("O nome do arquivo do vídeo original deve ser informado.");
            if (string.IsNullOrWhiteSpace(outputFormat)) throw new ArgumentException("O formato de conversão deve ser informado.");
            if (!_formats.Contains(inputFormat)) throw new InvalidOperationException("O formato de conversão é inválido.");

            if (reportProgressAction != null)
                _reportProgress += delegate { reportProgressAction(); };

            var settings = new ConvertSettings
            {
                AppendSilentAudioStream = appendSilentAudioStream
            };

            return Convert(inputStream, inputFormat, fileName, outputFormat, settings);
        }

        private Task<ConvertedVideoDto> Convert(Stream inputStream, string inputFormat, string fileName, string outputFormat, ConvertSettings settings) 
            => Task.Run(() =>
                {
                    using (var outputStream = new MemoryStream())
                    {
                        var ffMpeg = new FFMpegConverter();
                        if (_reportProgress != null)
                            ffMpeg.ConvertProgress += _reportProgress;

                        settings = settings ?? new ConvertSettings();
                        var convertTask = ffMpeg.ConvertLiveMedia(inputStream, inputFormat, outputStream, outputFormat, settings);

                        convertTask.Start();
                        convertTask.Wait();

                        var outputFileName = fileName + outputFormat;
                        return new ConvertedVideoDto
                        {
                            FileName = outputFileName,
                            Stream = outputStream
                        };
                    }
                });
    }
}
