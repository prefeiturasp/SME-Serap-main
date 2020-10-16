using AutoMapper;

namespace ProvaSP.Web.Mappers.Config
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ProvaSPApiProfile>();
            });
        }
    }
}