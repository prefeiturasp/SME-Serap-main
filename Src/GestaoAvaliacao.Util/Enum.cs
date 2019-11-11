using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GestaoAvaliacao.Util
{
    #region Enumerators

    public enum ValidateType
    {
        Save,
        Update,
        Delete,
        alert,
        error
    }

    public enum ValidateAction
    {
        Save = 1,
        Update = 2,
        Delete = 3
    }

    public enum Permission : byte
    {
        All = 1,
        Create = 2,
        Read = 3,
        Update = 4,
        Delete = 5,
        CreateOrUpdate = 6
    }

    public enum EnumDescriptionMenu
	{
		[Description("assignment_turned_in")]
		Cadastros = 1,
		[Description("assignment")]
		Itens = 2,
		[Description("library_books")]
		Relatórios = 3,
		[Description("settings")]
		Parâmetros = 4,
		[Description("description")]
		Prova = 5,
		[Description("file_upload")]
		Arquivos = 6,
        [Description("insert_chart")]
        Resultados = 7,
        [Description("find_in_page")]
        Auditoria = 8
    }

	public enum EnumFileType
	{
		[Description("Texto_Base")]
		BaseText = 1,
		[Description("Alternativa")]
		Alternative = 2,
		[Description("Justificativa")]
		Justificative = 3,
		[Description("Enunciado")]
		Statement = 4,
		[Description("Prova")]
		Test = 5,
		[Description("Modelo_prova")]
		ModelTestHeader = 6,
		[Description("Arquivo")]
		File = 7,
		[Description("Folha_de_resposta")]
        AnswerSheetStudentNumber = 8,
		[Description("Lote")]
		AnswerSheetBatch = 9,
		[Description("Modelo_prova")]
		ModelTestFooter = 10,
		[Description("Modelo_prova")]
		ModelTestCover = 11,
		[Description("Lote_Folha_de_resposta")]
		AnswerSheetLot = 12,
        [Description("Folha_de_resposta")]
		AnswerSheetQRCode = 13,
		[Description("Analise_Item")]
		AnalysisItem = 14,
        [Description("Gabarito_Aluno")]
        AnswerSheetBatchLog = 15,
        [Description("Gabarito_Prova")]
        TestFeedback = 16,
        [Description("Resultado_Prova")]
        ExportCorrectionResult = 17,
        [Description("Dados_Folha_de_resposta")]
        ExportAnswerSheetResult = 18,
        [Description("Rel_Folha_Resposta")]
        ExportFollowUpIdentification = 19,
        [Description("Zip_Rel_Folha_Resposta")]
        ZipFollowUpIdentification = 20,
        [Description("Rel_Processsamento_Correcao")]
        ExportReportCorrection = 21,
        [Description("Rel_Desempenho_Prova")]
        ExportReportTestPerformance = 22,
        [Description("Rel_Desempenho_Item_Prova")]
        ExportReportItemPerformance = 23,
        [Description("Rel_Percentage_Item_Choice")]
        ExportReportPercentageItemChoice = 24,
        [Description("Rel_Student_Performance")]
        ExportReportStudentPerformance = 25,
        [Description("Icone_Link_Externo")]
        IconeLinkExterno = 26,
        [Description("Icone_Ferramenta_Destaque")]
        IconeFerramentaDestaque = 27,
        [Description("Icone_Ferramentas")]
        IconeFerramentas = 28,
        [Description("Video")]
        Video = 29,
        [Description("Thumbnail_Video")]
        ThumbnailVideo = 30,
        [Description("Audio")]
        Audio = 31
    }

	public enum EnumSYS_Visao
	{
		[Description("Administração")]
		Administracao = 1,
		[Description("Gestão")]
		Gestao = 2,
		[Description("Unidade Administrativa")] // Coordenador pedagógico
		UnidadeAdministrativa = 3,
		[Description("Individual")] // Professor
		Individual = 4
	}

    public enum EnumFollowUpIdentificationView : byte
    {
        Total = 1,
        DRE = 2,
        School = 3,
        Files = 4
    }

    public enum EnumFollowUpIdentificationDataType : byte
    {
        Sent = 1,
        [Description("Identificado")]
        Identified = 2,
        [Description("Erro na identificação")]
        NotIdentified = 3,
        [Description("Na fila para identificação")]
        Pending = 4
    }

    public enum EnumFollowUpIdentificationReportDataType : byte
    {
        Sent = 1,
        [Description("Identificado")]
        Identified = 2,
        [Description("Erro na identificação")]
        NotIdentified = 3,
        [Description("Na fila para identificação")]
        Pending = 4,
        [Description("Resolução não adequada")]
        ResolutionNotOk = 5
    }

    //ATENÇÃO: o Name do enum está sendo usado nas classes do css
    //Qualquer alteração afetará o layout
    public enum EnumStatusCorrection : byte
	{
		[Description("Não iniciada")]
		Pending = 0,
		[Description("Em andamento")]
		Processing = 1,
		[Description("Concluída")]
		Success = 2,
        [Description("Parcialmente concluída")]
        PartialSuccess = 3,
        [Description("Processando turma")]
        ProcessingSection = 4
    }

    /// <summary>
    /// Enum para as entidades dos lotes de folhas de respostas
    /// </summary>
    public enum EnumAnswerSheetBatchOwner : byte
    {
        //Lote da prova
        Test = 1,
        //Lote da escola
        School = 2,
        //lote da turma
        Section = 3
    }

    /// <summary>
    /// Enum para categorizar os tipos de lotes de folhas de respostas
    /// </summary>
    public enum EnumAnswerSheetBatchType : byte
    {
        QRCode = 1,
        NumberID = 2,
        OnlyWrite = 3
    }

    public enum EnumTestSituation
	{
		[Description("Pendente")]
		Pending = 1,
		[Description("Cadastrada")]
		Registered = 2,
		[Description("Em andamento")]
		Processing = 3,
		[Description("Aplicada")]
		Applied = 4
	}

	//ATENÇÃO: o Name do enum está sendo usado nas classes do css
	//Qualquer alteração afetará o layout
	public enum EnumBatchProcessing : byte
	{
		[Description("Pendente")]
		Pending = 1,
		[Description("Processando")]
		Processing = 2,
		[Description("Concluído")]
		Success = 3,
		[Description("Falha")]
		Failure = 4,
        [Description("Nova tentativa")]
        Retry = 5,
        [Description("Iniciado")]
        Initiate = 6
    }

	//ATENÇÃO: o Name do enum está sendo usado nas classes do css
	//Qualquer alteração afetará o layout
	public enum EnumBatchSituation : byte
	{
		[Description("Aguardando correção")]
		Pending = 1,
        [Description("Sucesso")]
        Success = 4,
        [Description("Erro na correção")]
        Error = 5,
        [Description("Conferir")]
        Warning = 6,
        [Description("Na fila para identificação")]
        PendingIdentification = 7,
        [Description("Erro na identificação")]
        NotIdentified = 8,
        [Description("Ausente")]
        Absent = 9
    }

    public enum EnumBatchQueueSituation : byte
    {
        [Description("Aguardando descompactação")]
        PendingUnzip = 1,
        [Description("Descompactado com sucesso")]
        Success = 2,
        [Description("Erro na descompactação")]
        NotUnziped = 3,
        [Description("Descompactação em andamento")]
        Processing = 4,
    }

    public enum EnumServiceState : byte
	{
		[Description("Não solicitado")]
		NotRequest = 1,
		[Description("Aguardando execução")]
		Pending = 2,
		[Description("Em andamento")]
		Processing = 3,
		[Description("Processado")]
		Success = 4,
		[Description("Erro")]
		Error = 5,
		[Description("Solicitação cancelada")]
		Canceled = 6
	}

    public enum EnumReportTypeEntity
    {
        [Description("Escola")]
        School = 1,
        [Description("Unidade Administrativa")]
        AdministrativeUnity
    }

    public enum EnumParameterPageId
    {
        Geral = 1,
        Item = 2,
        Prova = 3,
        Sistema = 4
    }

    public enum AnswerSheetServiceMessage : byte
    {
        [Description("Processando lote principal")]
        MainProcessingLot = 0,
        [Description("Lote principal processado")]
        MainProcessCompleted = 1,
        [Description("Lote principal processado com erro")]
        MainProcessCompletedWithError = 2,
        [Description("Processando lote")]
        ChildProcessingLot = 3,
        [Description("Lote processado")]
        ChildProcessCompleted = 4,
        [Description("Lote processado com erro")]
        ChildProcessCompletedWithError = 5,
        [Description("Processando escola")]
        SchoolProcessingAnswerSheet = 6,
        [Description("Escola processada")]
        SchoolProcessAnswerSheetCompleted = 7,
        [Description("Escola processada com erro")]
        SchoolProcessAnswerSheetCompletedWithError = 8,
        [Description("Consolidado os arquivos")]
        SchoolProcessAnswerSheetMerge = 9
    }

    public enum EnumParameterKey : byte
    {
        [Description("BASETEXT")]
        BASETEXT,
        [Description("ITEMSITUATION")]
        ITEMSITUATION,
        [Description("SOURCE")]
        SOURCE,
        [Description("DESCRIPTORSENTENCE")]
        DESCRIPTORSENTENCE,
        [Description("ITEMTYPE")]
        ITEMTYPE,
        [Description("ITEMCURRICULUMGRADE")]
        ITEMCURRICULUMGRADE,
        [Description("KEYWORDS")]
        KEYWORDS,
        [Description("PROFICIENCY")]
        PROFICIENCY,
        [Description("ITEMLEVEL")]
        ITEMLEVEL,
        [Description("STATEMENT")]
        STATEMENT,
        [Description("TRI")]
        TRI,
        [Description("TCT")]
        TCT,
        [Description("TIPS")]
        TIPS,
        [Description("ALTERNATIVES")]
        ALTERNATIVES,
        [Description("JUSTIFICATIVE")]
        JUSTIFICATIVE,
        [Description("ISRESTRICT")]
        ISRESTRICT,
        [Description("NIVEISMATRIZ")]
        NIVEISMATRIZ,
        [Description("JPEG")]
        JPEG,
        [Description("GIF")]
        GIF,
        [Description("PNG")]
        PNG,
        [Description("BMP")]
        BMP,
        [Description("IMAGE_GIF_COMPRESSION")]
        IMAGE_GIF_COMPRESSION,
        [Description("IMAGE_MAX_SIZE_FILE")]
        IMAGE_MAX_SIZE_FILE,
        [Description("IMAGE_QUALITY")]
        IMAGE_QUALITY,
        [Description("IMAGE_MAX_RESOLUTION_HEIGHT")]
        IMAGE_MAX_RESOLUTION_HEIGHT,
        [Description("IMAGE_MAX_RESOLUTION_WIDTH")]
        IMAGE_MAX_RESOLUTION_WIDTH,
        [Description("UTILIZACDNMATHJAX")]
        UTILIZACDNMATHJAX,
        [Description("FILE_MAX_SIZE")]
        FILE_MAX_SIZE,
        [Description("BASE_TEXT_DEFAULT")]
        BASE_TEXT_DEFAULT,
        [Description("INITIAL_ORIENTATION")]
        INITIAL_ORIENTATION,
        [Description("INITIAL_STATEMENT")]
        INITIAL_STATEMENT,
        [Description("BASETEXT_ORIENTATION")]
        BASETEXT_ORIENTATION,
        [Description("CODE_ALTERNATIVE_DUPLICATE")]
        CODE_ALTERNATIVE_DUPLICATE,
        [Description("CODE_ALTERNATIVE_EMPTY")]
        CODE_ALTERNATIVE_EMPTY,
        [Description("SHOW_ITEM_NARRATED")]
        SHOW_ITEM_NARRATED,
        [Description("CHAR_SEP_CSV")]
        CHAR_SEP_CSV,
        [Description("STUDENT_NUMBER_ID")]
        STUDENT_NUMBER_ID,
        [Description("ANSWERSHEET_USE_COLUMN_TEMPLATE")]
        ANSWERSHEET_USE_COLUMN_TEMPLATE,
        [Description("DOWNLOAD_OMR_FILE")]
        DOWNLOAD_OMR_FILE,
        [Description("SHOW_MANUAL")]
        SHOW_MANUAL,
        [Description("ZIP_FILES_ALLOWED")]
        ZIP_FILES_ALLOWED,
        [Description("ZIP_FILES")]
        ZIP_FILES,
        [Description("IMAGE_FILES")]
        IMAGE_FILES,
        [Description("STORAGE_PATH")]
        STORAGE_PATH,
        [Description("GLOBAL_TERM")]
        GLOBAL_TERM,
        [Description("LOCAL_TERM")]
        LOCAL_TERM,
        [Description("VIRTUAL_PATH")]
        VIRTUAL_PATH,
        [Description("DELETE_BATCH_FILES")]
        DELETE_BATCH_FILES,
		[Description("WARNING_UPLOAD_BATCH_DETAIL")]
		WARNING_UPLOAD_BATCH_DETAIL,
        [Description("OCULTAR_BOTAO_FINALIZAR_ENVIAR")]
        OCULTAR_BOTAO_FINALIZAR_ENVIAR,
        [Description("ID_GOOGLE_ANALYTICS")]
        ID_GOOGLE_ANALYTICS,
        [Description("QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA")]
        QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA,
        [Description("HTML_PAGINA_INICIAL")]
        HTML_PAGINA_INICIAL
    }

    public enum MimeType
    {
        [Description("application/csv")]
        CSV,
        [Description("application/x-zip-compressed")]
        ZIP            
    }

    public enum EnumFrenquencyApplication
    {
        [Description("Anual")]
        Yearly = 1,
        [Description("Semestral")]
        Semiannual = 2,
        [Description("Bimestral")]
        Bimonthly = 3,
        [Description("Mensal")]
        Monthly = 4,

        [ParentId(2)]
        [Description("1º Semestre")]
        First_Semester = 5,
        [ParentId(2)]
        [Description("2º Semestre")]
        Second_Semester = 6,

        [ParentId(3)]
        [Description("1º Bimestre")]
        First_Quarter = 7,
        [ParentId(3)]
        [Description("2º Bimestre")]
        Second_Quarter = 8,
        [ParentId(3)]
        [Description("3º Bimestre")]
        Third_Quarter = 9,
        [ParentId(3)]
        [Description("4º Bimestre")]
        Fourth_Quarter = 10,

        [ParentId(4)]
        [Description("Janeiro")]
        January = 11,
        [ParentId(4)]
        [Description("Fevereiro")]
        February = 12,
        [ParentId(4)]
        [Description("Março")]
        March = 13,
        [ParentId(4)]
        [Description("Abril")]
        April = 14,
        [ParentId(4)]
        [Description("Maio")]
        May = 15,
        [ParentId(4)]
        [Description("Junho")]
        June = 16,
        [ParentId(4)]
        [Description("Julho")]
        July = 17,
        [ParentId(4)]
        [Description("Agosto")]
        August = 18,
        [ParentId(4)]
        [Description("Setembro")]
        September = 19,
        [ParentId(4)]
        [Description("Outubro")]
        October = 20,
        [ParentId(4)]
        [Description("Novembro")]
        November = 21,
        [ParentId(4)]
        [Description("Dezembro")]
        December = 22
    }

    public enum PageConfigurationCategory : short
    {
        [Description("Texto principal")]
        MainText = 1,
        [Description("Link de acesso externo")]
        ExternalAccess = 2,
        [Description("Ferramenta de destaque")]
        FeaturedTool = 3,
        [Description("Ferramenta")]
        GeneralTool = 4,
        [Description("Vídeo")]
        Video = 5

    }

    public enum StatusProvaEletronica : byte
    {
        [Description("Não iniciada")]
        NaoIniciada = 1,
        [Description("Em andamento")]
        EmAndamento = 2,
        [Description("Finalizada")]
        Finalizada = 3
    }

    public enum TypeReportsPerformanceExport : byte
    {
        Dre = 1,
        School = 2
    }

    #endregion

    #region EnumHelper

    public static class EnumHelper
	{
		public static string GetDescriptionFromEnumValue(Enum value)
		{
			DescriptionAttribute attribute = value.GetType()
				.GetField(value.ToString())
				.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault() as DescriptionAttribute;
			return attribute == null ? value.ToString() : attribute.Description;
		}

        public static int GetParentIdFromEnumValue(Enum value)
        {
            ParentIdAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(ParentIdAttribute), false)
                .SingleOrDefault() as ParentIdAttribute;
            return attribute == null ? 0 : attribute.Id;
        }

        public static string GetFrenquencyApplication(int Id, int parentId, bool concatenateParent, bool concatenatePipe)
        {
            string ret = null;

            if (Id > 0)
            {
                EnumFrenquencyApplication value = (EnumFrenquencyApplication)Id;
                int parent = GetParentIdFromEnumValue(value);
                bool concatenateChild = (Id != parentId && (Id != 1 && parent == parentId));

                if (parentId > 0)
                    parent = parentId;

                if (concatenatePipe)
                    ret = "| ";

                if (parent > 0 && concatenateParent)
                {
                    EnumFrenquencyApplication parentEnum = (EnumFrenquencyApplication)parent;
                    ret += GetDescriptionFromEnumValue(parentEnum);

                    if (!parentEnum.Equals(EnumFrenquencyApplication.Yearly) && concatenateChild)
                        ret += " - ";
                }

                if(concatenateChild)
                    ret += GetDescriptionFromEnumValue(value);
            }

            return ret;
        }
    }

	#endregion

	#region EnumExtensions

	public static class EnumExtensions
	{
		public static string GetDescription(this Enum enumerator)
		{
			FieldInfo fi = enumerator.GetType().GetField(enumerator.ToString());

			if (null != fi)
			{
				object[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (attributes != null && attributes.Length > 0)
					return ((DescriptionAttribute)attributes[0]).Description;
			}
			return enumerator.ToString();
		}

		public static T FromString<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

        public static string EnumToJson<T>()
        {
            return EnumToJson<T>(false);
        }

        public static string EnumToJson<T>(bool key)
		{
			var t = typeof(T);
			var ret = new StringBuilder("{ ");
			foreach (var item in Enum.GetValues(t))
			{
				var val = (byte)item;
				var description = ((Enum)Enum.Parse(t, item.ToString())).GetDescription();

                if (key)
                {
                    ret.AppendFormat("{0} : '{1}', ", description, description);
                }
                else
                {
                    ret.AppendFormat("{0} : '{1}', ", val, description);
                }
			}

			ret.Append(" }");
			return ret.ToString();
		}
	}

    #endregion

    #region CustomAtributes

    public class ParentIdAttribute : Attribute
    {
        internal ParentIdAttribute(int Id)
        {
            this.Id = Id;
        }

        public int Id { get; private set; }
    }

    #endregion
}