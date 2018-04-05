namespace GestaoEscolar.Repository.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialDatabase_SGP : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ACA_Aluno",
                c => new
                    {
                        alu_id = c.Long(nullable: false),
                        alu_nome = c.String(nullable: false, maxLength: 200, unicode: false),
                        ent_id = c.Guid(nullable: false),
                        alu_matricula = c.String(maxLength: 50, unicode: false),
                        alu_dataCriacao = c.DateTime(nullable: false),
                        alu_dataAlteracao = c.DateTime(nullable: false),
                        alu_situacao = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.alu_id);
            
            CreateTable(
                "dbo.ACA_CalendarioAnual",
                c => new
                    {
                        cal_id = c.Int(nullable: false),
                        ent_id = c.Guid(nullable: false),
                        cal_padrao = c.Boolean(nullable: false),
                        cal_ano = c.Int(nullable: false),
                        cal_descricao = c.String(nullable: false, maxLength: 200, unicode: false),
                        cal_dataInicio = c.DateTime(nullable: false, storeType: "date"),
                        cal_dataFim = c.DateTime(nullable: false, storeType: "date"),
                        cal_situacao = c.Byte(nullable: false),
                        cal_dataCriacao = c.DateTime(nullable: false),
                        cal_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.cal_id);
            
            CreateTable(
                "dbo.ACA_Curriculo",
                c => new
                    {
                        cur_id = c.Int(nullable: false),
                        crr_id = c.Int(nullable: false),
                        crr_nome = c.String(maxLength: 200, unicode: false),
                        crr_situacao = c.Byte(nullable: false),
                        crr_dataCriacao = c.DateTime(nullable: false),
                        crr_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.cur_id, t.crr_id })
                .ForeignKey("dbo.ACA_Curso", t => t.cur_id);
            
            CreateTable(
                "dbo.ACA_Curso",
                c => new
                    {
                        cur_id = c.Int(nullable: false),
                        ent_id = c.Guid(nullable: false),
                        tne_id = c.Int(nullable: false),
                        tme_id = c.Int(nullable: false),
                        cur_codigo = c.String(maxLength: 10, unicode: false),
                        cur_nome = c.String(nullable: false, maxLength: 200, unicode: false),
                        cur_nome_abreviado = c.String(maxLength: 20, unicode: false),
                        cur_situacao = c.Byte(nullable: false),
                        cur_dataCriacao = c.DateTime(nullable: false),
                        cur_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.cur_id)
                .ForeignKey("dbo.ACA_TipoModalidadeEnsino", t => t.tme_id)
                .ForeignKey("dbo.ACA_TipoNivelEnsino", t => t.tne_id)
                .Index(t => t.ent_id, name: "IX_ACA_Curso_ent_id");
            
            CreateTable(
                "dbo.ACA_TipoModalidadeEnsino",
                c => new
                    {
                        tme_id = c.Int(nullable: false),
                        tme_nome = c.String(nullable: false, maxLength: 100, unicode: false),
                        tme_situacao = c.Byte(nullable: false),
                        tme_dataCriacao = c.DateTime(nullable: false),
                        tme_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.tme_id);
            
            CreateTable(
                "dbo.ACA_TipoNivelEnsino",
                c => new
                    {
                        tne_id = c.Int(nullable: false),
                        tne_nome = c.String(nullable: false, maxLength: 100, unicode: false),
                        tne_situacao = c.Byte(nullable: false),
                        tne_dataCriacao = c.DateTime(nullable: false),
                        tne_dataAlteracao = c.DateTime(nullable: false),
                        tne_ordem = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.tne_id);
            
            CreateTable(
                "dbo.ACA_CurriculoDisciplina",
                c => new
                    {
                        cur_id = c.Int(nullable: false),
                        crr_id = c.Int(nullable: false),
                        crp_id = c.Int(nullable: false),
                        tds_id = c.Int(nullable: false),
                        crd_tipo = c.Byte(nullable: false),
                        crd_situacao = c.Byte(nullable: false),
                        crd_dataCriacao = c.DateTime(nullable: false),
                        crd_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.cur_id, t.crr_id, t.crp_id, t.tds_id })
                .ForeignKey("dbo.ACA_CurriculoPeriodo", t => new { t.cur_id, t.crr_id, t.crp_id })
                .ForeignKey("dbo.ACA_TipoDisciplina", t => t.tds_id);
            
            CreateTable(
                "dbo.ACA_CurriculoPeriodo",
                c => new
                    {
                        cur_id = c.Int(nullable: false),
                        crr_id = c.Int(nullable: false),
                        crp_id = c.Int(nullable: false),
                        crp_ordem = c.Int(nullable: false),
                        crp_descricao = c.String(nullable: false, maxLength: 200, unicode: false),
                        crp_situacao = c.Byte(nullable: false),
                        crp_dataCriacao = c.DateTime(nullable: false),
                        crp_dataAlteracao = c.DateTime(nullable: false),
                        tcp_id = c.Int(),
                    })
                .PrimaryKey(t => new { t.cur_id, t.crr_id, t.crp_id })
                .ForeignKey("dbo.ACA_Curriculo", t => new { t.cur_id, t.crr_id })
                .ForeignKey("dbo.ACA_TipoCurriculoPeriodo", t => t.tcp_id);
            
            CreateTable(
                "dbo.ACA_TipoCurriculoPeriodo",
                c => new
                    {
                        tcp_id = c.Int(nullable: false),
                        tne_id = c.Int(nullable: false),
                        tme_id = c.Int(nullable: false),
                        tcp_descricao = c.String(nullable: false, maxLength: 100, unicode: false),
                        tcp_ordem = c.Byte(nullable: false),
                        tcp_situacao = c.Byte(nullable: false),
                        tcp_dataCriacao = c.DateTime(nullable: false),
                        tcp_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.tcp_id)
                .ForeignKey("dbo.ACA_TipoModalidadeEnsino", t => t.tme_id)
                .ForeignKey("dbo.ACA_TipoNivelEnsino", t => t.tne_id);
            
            CreateTable(
                "dbo.ACA_TipoDisciplina",
                c => new
                    {
                        tds_id = c.Int(nullable: false),
                        tne_id = c.Int(nullable: false),
                        tds_nome = c.String(nullable: false, maxLength: 100, unicode: false),
                        tds_situacao = c.Byte(nullable: false),
                        tds_dataCriacao = c.DateTime(nullable: false),
                        tds_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.tds_id)
                .ForeignKey("dbo.ACA_TipoNivelEnsino", t => t.tne_id);
            
            CreateTable(
                "dbo.ACA_Docente",
                c => new
                    {
                        doc_id = c.Long(nullable: false),
                        pes_id = c.Guid(nullable: false),
                        ent_id = c.Guid(nullable: false),
                        doc_nome = c.String(nullable: false, maxLength: 200, unicode: false),
                        doc_situacao = c.Byte(nullable: false),
                        doc_dataCriacao = c.DateTime(nullable: false),
                        doc_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.doc_id);
            
            CreateTable(
                "dbo.ACA_TipoTurno",
                c => new
                    {
                        ttn_id = c.Int(nullable: false),
                        ttn_nome = c.String(nullable: false, maxLength: 100, unicode: false),
                        ttn_situacao = c.Byte(nullable: false),
                        ttn_dataCriacao = c.DateTime(nullable: false),
                        ttn_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ttn_id);
            
            CreateTable(
                "dbo.ESC_Escola",
                c => new
                    {
                        esc_id = c.Int(nullable: false),
                        ent_id = c.Guid(nullable: false),
                        uad_id = c.Guid(nullable: false),
                        esc_codigo = c.String(maxLength: 20, unicode: false),
                        esc_nome = c.String(nullable: false, maxLength: 200, unicode: false),
                        esc_situacao = c.Byte(nullable: false),
                        esc_dataCriacao = c.DateTime(nullable: false),
                        esc_dataAlteracao = c.DateTime(nullable: false),
                        uad_idSuperiorGestao = c.Guid(),
                    })
                .PrimaryKey(t => t.esc_id)
                .ForeignKey("dbo.SYS_UnidadeAdministrativa", t => new { t.ent_id, t.uad_idSuperiorGestao });
            
            CreateTable(
                "dbo.SYS_UnidadeAdministrativa",
                c => new
                    {
                        ent_id = c.Guid(nullable: false),
                        uad_id = c.Guid(nullable: false),
                        uad_codigo = c.String(maxLength: 20, unicode: false),
                        uad_nome = c.String(nullable: false, maxLength: 200, unicode: false),
                        uad_sigla = c.String(maxLength: 50, unicode: false),
                        uad_situacao = c.Byte(nullable: false),
                        uad_dataCriacao = c.DateTime(nullable: false),
                        uad_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ent_id, t.uad_id });
            
            CreateTable(
                "dbo.MTR_MatriculaTurma",
                c => new
                    {
                        alu_id = c.Long(nullable: false),
                        mtu_id = c.Int(nullable: false),
                        esc_id = c.Int(nullable: false),
                        tur_id = c.Long(nullable: false),
                        cur_id = c.Int(nullable: false),
                        crr_id = c.Int(nullable: false),
                        crp_id = c.Int(nullable: false),
                        mtu_situacao = c.Byte(nullable: false),
                        mtu_dataCriacao = c.DateTime(nullable: false),
                        mtu_dataAlteracao = c.DateTime(nullable: false),
                        mtu_numeroChamada = c.Int(),
                    })
                .PrimaryKey(t => new { t.alu_id, t.mtu_id })
                .ForeignKey("dbo.ACA_Aluno", t => t.alu_id)
                .ForeignKey("dbo.ESC_Escola", t => t.esc_id)
                .ForeignKey("dbo.TUR_TurmaCurriculo", t => new { t.tur_id, t.cur_id, t.crr_id, t.crp_id });
            
            CreateTable(
                "dbo.TUR_TurmaCurriculo",
                c => new
                    {
                        tur_id = c.Long(nullable: false),
                        cur_id = c.Int(nullable: false),
                        crr_id = c.Int(nullable: false),
                        crp_id = c.Int(nullable: false),
                        tcr_situacao = c.Byte(nullable: false),
                        tcr_dataCriacao = c.DateTime(nullable: false),
                        tcr_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.tur_id, t.cur_id, t.crr_id, t.crp_id })
                .ForeignKey("dbo.ACA_CurriculoPeriodo", t => new { t.cur_id, t.crr_id, t.crp_id })
                .ForeignKey("dbo.TUR_Turma", t => t.tur_id);
            
            CreateTable(
                "dbo.TUR_Turma",
                c => new
                    {
                        tur_id = c.Long(nullable: false),
                        esc_id = c.Int(nullable: false),
                        tur_codigo = c.String(maxLength: 30, unicode: false),
                        tur_descricao = c.String(maxLength: 2000, unicode: false),
                        cal_id = c.Int(nullable: false),
                        ttn_id = c.Int(),
                        tur_situacao = c.Byte(nullable: false),
                        tur_dataCriacao = c.DateTime(nullable: false),
                        tur_dataAlteracao = c.DateTime(nullable: false),
                        tur_tipo = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.tur_id)
                .ForeignKey("dbo.ACA_CalendarioAnual", t => t.cal_id)
                .ForeignKey("dbo.ACA_TipoTurno", t => t.ttn_id)
                .ForeignKey("dbo.ESC_Escola", t => t.esc_id);
            
            CreateTable(
                "dbo.MTR_MatriculaTurmaDisciplina",
                c => new
                    {
                        alu_id = c.Long(nullable: false),
                        mtu_id = c.Int(nullable: false),
                        mtd_id = c.Int(nullable: false),
                        tud_id = c.Long(nullable: false),
                        mtd_numeroChamada = c.Int(),
                        mtd_situacao = c.Byte(nullable: false),
                        mtd_dataCriacao = c.DateTime(nullable: false),
                        mtd_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.alu_id, t.mtu_id, t.mtd_id })
                .ForeignKey("dbo.MTR_MatriculaTurma", t => new { t.alu_id, t.mtu_id })
                .ForeignKey("dbo.TUR_TurmaDisciplina", t => t.tud_id);
            
            CreateTable(
                "dbo.TUR_TurmaDisciplina",
                c => new
                    {
                        tud_id = c.Long(nullable: false),
                        tur_id = c.Long(nullable: false),
                        tds_id = c.Int(nullable: false),
                        tud_codigo = c.String(nullable: false, maxLength: 30, unicode: false),
                        tud_nome = c.String(nullable: false, maxLength: 200, unicode: false),
                        tud_tipo = c.Byte(nullable: false),
                        tud_situacao = c.Byte(nullable: false),
                        tud_dataCriacao = c.DateTime(nullable: false),
                        tud_dataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.tud_id)
                .ForeignKey("dbo.ACA_TipoDisciplina", t => t.tds_id)
                .ForeignKey("dbo.TUR_Turma", t => t.tur_id);
            
            CreateTable(
                "dbo.TUR_TurmaDocente",
                c => new
                    {
                        tud_id = c.Long(nullable: false),
                        tdt_id = c.Int(nullable: false),
                        doc_id = c.Long(nullable: false),
                        tdt_situacao = c.Byte(nullable: false),
                        tdt_dataCriacao = c.DateTime(nullable: false),
                        tdt_dataAlteracao = c.DateTime(nullable: false),
                        tdt_posicao = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.tud_id, t.tdt_id })
                .ForeignKey("dbo.ACA_Docente", t => t.doc_id)
                .ForeignKey("dbo.TUR_TurmaDisciplina", t => t.tud_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TUR_TurmaDocente", "tud_id", "dbo.TUR_TurmaDisciplina");
            DropForeignKey("dbo.TUR_TurmaDocente", "doc_id", "dbo.ACA_Docente");
            DropForeignKey("dbo.MTR_MatriculaTurmaDisciplina", "tud_id", "dbo.TUR_TurmaDisciplina");
            DropForeignKey("dbo.TUR_TurmaDisciplina", "tur_id", "dbo.TUR_Turma");
            DropForeignKey("dbo.TUR_TurmaDisciplina", "tds_id", "dbo.ACA_TipoDisciplina");
            DropForeignKey("dbo.MTR_MatriculaTurmaDisciplina", new[] { "alu_id", "mtu_id" }, "dbo.MTR_MatriculaTurma");
            DropForeignKey("dbo.MTR_MatriculaTurma", new[] { "tur_id", "cur_id", "crr_id", "crp_id" }, "dbo.TUR_TurmaCurriculo");
            DropForeignKey("dbo.TUR_TurmaCurriculo", "tur_id", "dbo.TUR_Turma");
            DropForeignKey("dbo.TUR_Turma", "esc_id", "dbo.ESC_Escola");
            DropForeignKey("dbo.TUR_Turma", "ttn_id", "dbo.ACA_TipoTurno");
            DropForeignKey("dbo.TUR_Turma", "cal_id", "dbo.ACA_CalendarioAnual");
            DropForeignKey("dbo.TUR_TurmaCurriculo", new[] { "cur_id", "crr_id", "crp_id" }, "dbo.ACA_CurriculoPeriodo");
            DropForeignKey("dbo.MTR_MatriculaTurma", "esc_id", "dbo.ESC_Escola");
            DropForeignKey("dbo.MTR_MatriculaTurma", "alu_id", "dbo.ACA_Aluno");
            DropForeignKey("dbo.ESC_Escola", new[] { "ent_id", "uad_idSuperiorGestao" }, "dbo.SYS_UnidadeAdministrativa");
            DropForeignKey("dbo.ACA_CurriculoDisciplina", "tds_id", "dbo.ACA_TipoDisciplina");
            DropForeignKey("dbo.ACA_TipoDisciplina", "tne_id", "dbo.ACA_TipoNivelEnsino");
            DropForeignKey("dbo.ACA_CurriculoDisciplina", new[] { "cur_id", "crr_id", "crp_id" }, "dbo.ACA_CurriculoPeriodo");
            DropForeignKey("dbo.ACA_CurriculoPeriodo", "tcp_id", "dbo.ACA_TipoCurriculoPeriodo");
            DropForeignKey("dbo.ACA_TipoCurriculoPeriodo", "tne_id", "dbo.ACA_TipoNivelEnsino");
            DropForeignKey("dbo.ACA_TipoCurriculoPeriodo", "tme_id", "dbo.ACA_TipoModalidadeEnsino");
            DropForeignKey("dbo.ACA_CurriculoPeriodo", new[] { "cur_id", "crr_id" }, "dbo.ACA_Curriculo");
            DropForeignKey("dbo.ACA_Curriculo", "cur_id", "dbo.ACA_Curso");
            DropForeignKey("dbo.ACA_Curso", "tne_id", "dbo.ACA_TipoNivelEnsino");
            DropForeignKey("dbo.ACA_Curso", "tme_id", "dbo.ACA_TipoModalidadeEnsino");
            DropIndex("dbo.ACA_Curso", "IX_ACA_Curso_ent_id");
            DropTable("dbo.TUR_TurmaDocente");
            DropTable("dbo.TUR_TurmaDisciplina");
            DropTable("dbo.MTR_MatriculaTurmaDisciplina");
            DropTable("dbo.TUR_Turma");
            DropTable("dbo.TUR_TurmaCurriculo");
            DropTable("dbo.MTR_MatriculaTurma");
            DropTable("dbo.SYS_UnidadeAdministrativa");
            DropTable("dbo.ESC_Escola");
            DropTable("dbo.ACA_TipoTurno");
            DropTable("dbo.ACA_Docente");
            DropTable("dbo.ACA_TipoDisciplina");
            DropTable("dbo.ACA_TipoCurriculoPeriodo");
            DropTable("dbo.ACA_CurriculoPeriodo");
            DropTable("dbo.ACA_CurriculoDisciplina");
            DropTable("dbo.ACA_TipoNivelEnsino");
            DropTable("dbo.ACA_TipoModalidadeEnsino");
            DropTable("dbo.ACA_Curso");
            DropTable("dbo.ACA_Curriculo");
            DropTable("dbo.ACA_CalendarioAnual");
            DropTable("dbo.ACA_Aluno");
        }
    }
}
