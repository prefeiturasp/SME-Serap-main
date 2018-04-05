using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class BaseText : EntityBase
    {
        public string Description { get; set; }
        public string Source { get; set; }
        public string InitialOrientation { get; set; }
        public string InitialStatement { get; set; }
        public bool? NarrationInitialStatement { get; set; }
        public bool? StudentBaseText { get; set; }
        public bool? NarrationStudentBaseText { get; set; }
        public string BaseTextOrientation { get; set; }
    }
}
