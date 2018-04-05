namespace GestaoAvaliacao.Repository.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<GestaoAvaliacao.Repository.Context.GestaoAvaliacaoContext>
    {
        public Configuration()
        {  
            //Habilita o Migrations automatico
            AutomaticMigrationDataLossAllowed = true;
            //Habilita o Migrations automatico
            AutomaticMigrationsEnabled = true;  
        }
    }
}
