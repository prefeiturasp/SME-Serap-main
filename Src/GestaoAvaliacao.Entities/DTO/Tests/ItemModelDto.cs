using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO.Tests
{
    public class ItemModelDto
    {
        public ItemModelDto()
        {
            Alternatives = new List<AlternativeModelDto>();
        }

        public long Item_Id { get; set; }
        public string ItemCode { get; set; }
        public int ItemVersion { get; set; }
        public int ItemOrder { get; set; }
        public string BaseTextDescription { get; set; }
        public long BaseTextId { get; set; }
        public string Statement { get; set; }
        public EnumSituation Revoked { get; set; }
        public EnumSituation ItemSituation { get; set; }
        public long RequestRevoke_Id { get; set; }
        public long BlockItem_Id { get; set; }
        public string Justification { get; set; }
        public IEnumerable<ItemFile> Videos { get; set; }
        public IEnumerable<ItemAudio> Audios { get; set; }
        public bool ShowVideoFiles { get; set; }
        public bool ShowAudioFiles { get; set; }
        public List<AlternativeModelDto> Alternatives { get; set; }
    }
}