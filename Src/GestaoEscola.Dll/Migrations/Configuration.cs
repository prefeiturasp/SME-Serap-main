namespace GestaoEscolar.Repository.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<GestaoEscolar.Repository.Context.GestaoEscolarContext>
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
