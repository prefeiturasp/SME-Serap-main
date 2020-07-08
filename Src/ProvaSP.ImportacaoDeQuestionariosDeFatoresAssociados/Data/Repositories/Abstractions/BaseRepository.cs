namespace ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions
{
    public abstract class BaseRepository
    {
        protected readonly IDapperContext _dapperContext;

        public BaseRepository()
        {
            _dapperContext = new DapperContext();
        }
    }
}