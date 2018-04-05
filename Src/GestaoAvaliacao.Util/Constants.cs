using System.Web;

namespace GestaoAvaliacao.Util
{
	public static class Constants
	{
		#region TEMPLATES PDF

		public const string TMPL_Icon_Narration = "<i class='material-icons'>chat_bubble</i>";
		public const string TMPL_Icon_Alternative = "<i class='material-icons'>check_box_outline_blank</i>";
		public const string TMPL_ItemNarrated_Teacher = @"<div class='tmpl-item-narrated tmpl-teacher'><div class='prova-question page-break'><h3><b class='prova-question-num'>Questão @question</b></h3>" +
															@"<div class='initial-orientation'>@initialOrientation</div><br/><div class='initial-statement'>@iconInitialStatement<span>@initialStatement</span></div></br><div class='baseText'>@baseText</div><div class='baseTextOrientation'>@baseTextOrientation</div>" +
															@"<br/><div class='statement'>@iconStatement<span>@statement</span></div><div class='alternatives'>@alternatives</div><br/><div class='tips'>@tips</div> @last</div></div>";
		public const string TMPL_BaseText_ItemNarrated_Teacher = @"<div class='prova-question page-break'>@iconBaseText @baseText</div><p class='prova-quest'>@source</p><br />";
		public const string TMPL_Alternatives_ItemNarrated_Teacher = @"@iconAlternatives<div class='prova-answer'><span class='prova-alternative'>(@number) @checkAlternative</span>@alternative</div>";

		#endregion

		#region TEMPLATES ANSWER SHEET

		//FOLHA DE RESPOSTAS 4 ALTERNATIVAS
		//private static string TMPL_TYPE_1_SHEET_20 = "<img src='{0}Assets/images/test/sheet_4alt_20.jpg' class='sheet_20' />";
		//private static string TMPL_TYPE_1_SHEET_40 = "<img src='{0}Assets/images/test/sheet_4alt_40.jpg' class='sheet_40' />";
		//private static string TMPL_TYPE_1_SHEET_60 = "<img src='{0}Assets/images/test/sheet_4alt_60.jpg' class='sheet_60' />";
		//private static string TMPL_TYPE_1_SHEET_80 = "<img src='{0}Assets/images/test/sheet_4alt_80.jpg' class='sheet_80' />";

		//FOLHA DE RESPOSTAS 5 ALTERNATIVAS
		//private static string TMPL_TYPE_2_SHEET_20 = "<img src='{0}Assets/images/test/sheet_5alt_20.jpg' class='sheet_20' />";
		//private static string TMPL_TYPE_2_SHEET_40 = "<img src='{0}Assets/images/test/sheet_5alt_40.jpg' class='sheet_40' />";
		//private static string TMPL_TYPE_2_SHEET_60 = "<img src='{0}Assets/images/test/sheet_5alt_60.jpg' class='sheet_60' />";
		//private static string TMPL_TYPE_2_SHEET_80 = "<img src='{0}Assets/images/test/sheet_5alt_80.jpg' class='sheet_80' />";

		//NÚMERO DE CHAMADA PARA PREENCHIMENTO DO ALUNO
		public const string TMPL_HTML_STUDENT_NUMBER_ID = @"<div class='table-pdf'><div class='mark'></div><div class='mark right'></div><div class='mark bottom'></div>
															<table><tbody><tr><td>0</td><td><div class='number'>0</div></td><td><div class='number'>0</div></td></tr>
															<tr><td>1</td><td><div class='number'>1</div></td><td><div class='number'>1</div></td></tr><tr><td>2</td>
															<td><div class='number'>2</div></td><td><div class='number'>2</div></td></tr><tr><td>3</td>
															<td><div class='number'>3</div></td><td><div class='number'>3</div></td></tr><tr><td>4</td>
															<td><div class='number'>4</div></td><td><div class='number'>4</div></td></tr><tr><td>5</td>
															<td><div class='number'>5</div></td><td><div class='number'>5</div></td></tr><tr><td>6</td>
															<td><div class='number'>6</div></td><td><div class='number'>6</div></td></tr><tr><td>7</td>
															<td><div class='number'>7</div></td><td><div class='number'>7</div></td></tr><tr><td>8</td>
															<td><div class='number'>8</div></td><td><div class='number'>8</div></td></tr><tr><td>9</td>
															<td><div class='number'>9</div></td><td><div class='number'>9</div></td></tr></tbody></table></div>";
		//private static string TMPL_IMG_STUDENT_NUMBER_ID = "background-image: url(\"{0}Assets/images/test/student_id.jpg\")";

		#endregion

		private static string _storageFilePath = System.Configuration.ConfigurationManager.AppSettings["StoragePath"];

		public static string StorageFilePath
		{
			get
			{
				return !string.IsNullOrEmpty(_storageFilePath) ? _storageFilePath : HttpContext.Current.Server.MapPath("/") + "Files";
			}
		}


		public const int IdSistema = 204;

		public const char StringArraySeparator = ';';
		public const char UnderlineChar = '_';
		public const string RegexSpecialChar = @"[^a-zA-Z0-9_]+";

		#region API
		public const string access_token = "Authorization";
		public const string user_info = "user_info";
		#endregion
	}
}
