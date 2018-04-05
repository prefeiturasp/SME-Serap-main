using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Business
{
	public class GenerateHtmlBusiness : IGenerateHtmlBusiness
	{
		private ModelTest modelTest { get; set; }
		private string UrlCssFile { get; set; }
		private const string CssLink = @"http://{0}/Assets/css/prova.css";
		private const string classLine = @"<div class='line-separation'></div>";

		public string GetContentHtml(PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();
			ModelTest modelT = filter.Test.TestType.ModelTest;

			// CSS
			html.Append(BuildCSS(filter));

			if (filter.PDFPreview)
				html.Append("<div class='pdf-content preview'>");
			else
				html.Append("<div class='pdf-content'>");

			if (filter.GenerateType.Equals(EnumGenerateType.Cover) || filter.GenerateType.Equals(EnumGenerateType.Test))
			{
				#region CAPA

				if (filter.PDFPreview)
				{
					if (modelT.ShowCoverPage && !string.IsNullOrEmpty(modelT.CoverPageText))
					{
						html.Append("<div class='test-cover'><label class='section-title'>Capa</label>");

						//  BORDER
						html.Append(BuildBorder(true, false, filter));

						html.Append(modelT.HeaderHtml.Replace("<hr>", classLine));

						if (modelT.ShowStudentInformationsOnCoverPage)
							html.Append(modelT.StudentInformationHtml.Replace("<hr>", string.Empty));

						if (!string.IsNullOrEmpty(modelT.CoverPageText))
						{
							html.Append("<div class='textoCapa'>");
							html.Append(modelT.CoverPageText);
							html.Append("</div>");
						}

						html.Append(modelT.FooterHtml.Replace("<hr>", classLine));

						//  BORDER
						html.Append(BuildBorder(false, true, filter));

						html.Append("</div>");
					}

					html.Append("<div class='test-content'><label class='section-title'>Conteúdo</label>");

					//  BORDER
					html.Append(BuildBorder(true, false, filter));
					html.Append(modelT.HeaderHtml.Replace("<hr>", classLine));
				}
				else
				{
					html.Append("<div class='pdf-cover'>");

					if (modelT.ShowStudentInformationsOnCoverPage)
						html.Append(modelT.StudentInformationHtml.Replace("<hr>", string.Empty));

					if (modelT.ShowCoverPage && !string.IsNullOrEmpty(modelT.CoverPageText))
						html.AppendFormat("<div class=\"capa\">{0}</div>".Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;"), modelT.CoverPageText);

					html.Append("</div>");
				}

				#endregion
			}

			// IDENTIFICAÇÃO DO ALUNO
			if ((filter.GenerateType.Equals(EnumGenerateType.Items) || filter.GenerateType.Equals(EnumGenerateType.Test))
				&& (!modelT.ShowStudentInformationsOnCoverPage))
			{
				html.Append(modelT.StudentInformationHtml.Replace("<hr>", string.Empty));
			}

			// QUESTOES
			if (filter.GenerateType.Equals(EnumGenerateType.Items) || filter.GenerateType.Equals(EnumGenerateType.Test))
			{
				html.Append("<div class='test-items'>");
				if (filter.ItemList != null)
					html.Append(BuildItems(filter));
				else
					html.Append(BuildBlockItems(filter));
				html.Append("</div>");
			}

			if (filter.PDFPreview)
			{
				// FOOTER                
				html.Append(modelT.FooterHtml.Replace("<hr>", classLine));

				//  BORDER
				html.Append(BuildBorder(false, true, filter));
				html.Append("</div>");
			}

			// FOLHA DE RESPOSTA
			if (filter.GenerateType.Equals(EnumGenerateType.AnswerSheet) || filter.GenerateAnswerSheet)
			{
				html.Append(BuildAnswerSheet(filter));
			}

			// Fim do HTML
			html.Append("</div>");
			if (!filter.PDFPreview)
				html.Append("</body></html>");

			return html.ToString();
		}
		public string GetHeader(PDFFilter filter)
		{
			string cabecalho = string.Empty;
			if (!string.IsNullOrEmpty(filter.Test.TestType.ModelTest.HeaderHtml))
			{
				string cssFile = string.Format(CssLink, filter.UrlSite);
				string cssPdf = string.Format("<link href='{0}' rel='stylesheet' />", cssFile);
				cabecalho = cssPdf;
				cabecalho += @"<div class='pdf-content'>";
				cabecalho += filter.Test.TestType.ModelTest.HeaderHtml.Replace("<hr>", classLine);
				cabecalho += @"</div>";


			}
			return cabecalho;
		}
		public string GetFooter(PDFFilter filter)
		{
			string footer = string.Empty;
			if (!string.IsNullOrEmpty(filter.Test.TestType.ModelTest.FooterHtml))
			{
				string cssFile = string.Format(CssLink, filter.UrlSite);
				string cssPdf = string.Format("<link href='{0}' rel='stylesheet' />", cssFile);
				footer = cssPdf;
				footer += @"<div class='pdf-content'>";
				footer += filter.Test.TestType.ModelTest.FooterHtml.Replace("<hr>", classLine);
				footer += @"</div>";
			}
			return footer;

		}
		public StringBuilder BuildAnswerSheet(PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();
			ModelTest model = filter.Test.TestType.ModelTest;
			ItemType itemType = filter.Test.TestType.ItemType;
			int qtdeItens = 0;

			try
			{
				if (!filter.IdentificationType.Equals(EnumIdentificationType.OnlyWrite) && !filter.IdentificationType.Equals(EnumIdentificationType.QRCode))
				{
					Parameter studentNumberID = filter.Parameters.FirstOrDefault(i => i.Key.Equals("STUDENT_NUMBER_ID"));
					if (studentNumberID != null && Convert.ToBoolean(studentNumberID.Value))
						filter.IdentificationType = EnumIdentificationType.NumberID;
					else
						filter.IdentificationType = EnumIdentificationType.OnlyWrite;
				}

				if (filter.PDFPreview)
				{
					html.Append("<div class='test-answersheet'><label class='section-title'>Folha de resposta</label>");
				}

				bool knowledgeAreaBlock = filter.Booklet.Test.KnowledgeAreaBlock;
				List<BlockItem> blockItems = new List<BlockItem>();

				if (filter.ItemList != null)
				{
					qtdeItens = filter.ItemList.Count;
				}
				else if (filter.BlockItemList != null)
				{
					qtdeItens = filter.BlockItemList.Count;
					blockItems = filter.BlockItemList.ToList();
				}

				if (filter.Booklet != null && blockItems.Count == 0)
				{
					blockItems = filter.Booklet.Blocks.SelectMany(p => p.BlockItems.OfType<BlockItem>()).ToList();
				}

				if (filter.QtdeItens > 0)
				{
					qtdeItens = filter.QtdeItens;
				}

				// Caso a prova seja separada em blocos de área de conhecimento.
				if (knowledgeAreaBlock)
				{
					var knowledgeAreaItens = blockItems.Select(p => p.KnowledgeArea_Id).Distinct();

					qtdeItens += knowledgeAreaItens.Count();
				}

				html.Append(filter.GenerateAnswerSheet ? "<div class='answerSheetPage'>" : "<div id='answerSheetPage'>");

				//IDENTIFICAÇÃO DO ALUNO POR Nº DE CHAMADA OU QRCODE
				if (filter.IdentificationType.Equals(EnumIdentificationType.NumberID) || filter.IdentificationType.Equals(EnumIdentificationType.QRCode))
				{
					html.Append("<div class='sideleft pageTop'>");
				}
				else
					html.Append("<div class='pageTop'>");

				html.Append("<div class='testHeader col-md-12'>");
				//IMAGEM CABEÇALHO
				if (model.ShowLogoHeader && model.FileHeader_Id > 0 && filter.Logo != null && !string.IsNullOrEmpty(filter.Logo.Path))
				{
					html.AppendFormat("<img src='{0}' class='logosmall' />", filter.Logo.Path);
				}

				//TITULO
				html.Append("<div class='testTitleSection'>");
				html.Append("<h2 class='prova-description'>" + filter.Test.Description + "</h2>");

				#region FREQUENCIA DE APLICAÇÃO

				string frequencyApplication = EnumHelper.GetFrenquencyApplication(filter.Test.FrequencyApplication, filter.Test.TestType.FrequencyApplication, true, false);
				if (!string.IsNullOrEmpty(frequencyApplication) && !frequencyApplication.Equals("0"))
					frequencyApplication += " / " + filter.Test.ApplicationStartDate.Year.ToString();
				else
					frequencyApplication = filter.Test.ApplicationStartDate.Year.ToString();

				html.Append(@"<label class='testInfo'>" + frequencyApplication + "</label><br/><br/>");

				#endregion

				html.Append("</div>");
				html.Append("</div>");

				#region IDENTIFICAÇÃO DO ALUNO

				string formLine = @"<div><input class='form-control' type='text'></div>";
				string htmlFormat = "<div><label>{0}</label></div>";
				string studentInformationHtml = string.Empty;
				if (!string.IsNullOrEmpty(model.StudentInformationHtml))
				{
					studentInformationHtml = model.StudentInformationHtml;

					if (filter.Student != null)
					{
						studentInformationHtml = studentInformationHtml.Replace(@"<div><input class=""form-control"" type=""text"" #school#=""""></div>", "#schoolName#");
						studentInformationHtml = studentInformationHtml.Replace(@"<div><input class=""form-control"" type=""text"" #studentname#=""""></div>", "#studentName#");
						studentInformationHtml = studentInformationHtml.Replace(@"<div><input class=""form-control"" type=""text"" #teachername#=""""></div>", "#teacherName#");
						studentInformationHtml = studentInformationHtml.Replace(@"<div><input class=""form-control"" type=""text"" #classname#=""""></div>", "#sectionName#");
						studentInformationHtml = studentInformationHtml.Replace(@"<div><input class=""form-control"" type=""text"" #studentnumber#=""""></div>", "#numberId#");
						studentInformationHtml = studentInformationHtml.Replace(@"<div><input class=""form-control"" type=""text""></div>", "#numberId#");
					}
				}
				else
				{
					html.Append(classLine);
					html.Append("<div class='answerSheetStudentData'>");

					studentInformationHtml = @"<div class='row'><div class='col-md-12'><div class='campoLabel col-md-12'><label>Escola: </label>#schoolName#</div>
						<div class='campoLabel col-md-12'><label>Nome: </label>#studentName#</div><div class='campoLabel col-md-8'><label>Turma: </label>#sectionName#</div>
						<div class='campoLabel col-md-4'><label>Número: </label>#numberId#</div><div class='campoLabel col-md-8'><label>Professor: </label><div><input class='form-control' type='text'></div></div>
						<div class='campoLabel col-md-4'><label>Data: </label><div><input class='form-control' type='text'></div></div></div></div>";

					html.Append("</div>");
				}

				if (!string.IsNullOrEmpty(studentInformationHtml))
				{
					if (filter.Student != null)
					{
						studentInformationHtml = studentInformationHtml.Replace("#schoolName#", string.Format(htmlFormat, filter.Student.SchoolName));
						studentInformationHtml = studentInformationHtml.Replace("#studentName#", string.Format(htmlFormat, filter.Student.Name));
						studentInformationHtml = studentInformationHtml.Replace("#teacherName#", formLine);
						studentInformationHtml = studentInformationHtml.Replace("#sectionName#", string.Format(htmlFormat, filter.Student.SectionName));
						studentInformationHtml = studentInformationHtml.Replace("#numberId#", string.Format(htmlFormat, filter.Student.NumberId));
					}
					else
					{
						studentInformationHtml = studentInformationHtml.Replace("#schoolName#", formLine);
						studentInformationHtml = studentInformationHtml.Replace("#studentName#", formLine);
						studentInformationHtml = studentInformationHtml.Replace("#sectionName#", formLine);
						studentInformationHtml = studentInformationHtml.Replace("#numberId#", formLine);
					}

					html.Append(studentInformationHtml);
				}

				#endregion

				#region ASSINATURA DO ALUNO

				string studentSignature = @"<div class='studentSignature'><div class='row'><div class='col-md-12'><div class='campoLabel'><label>Nome</label><div><input class='form-control' type='text' #studentsignature#=''></div></div></div></div></div><br/><br/>";
				html.Append(studentSignature);

				#endregion

				#region ORIENTAÇÕES

				string answerSheetOrientation = @"<div id='answerSheetOrientation' class='row'><div class='col-md-12'><strong>Instruções:</strong><ul>
												<li><strong>Aluno: </strong>Não rasgue nem utilize corretivo no gabarito.</li>
												<li><strong>Aluno: </strong>Pinte os círculos correspondentes às alternativas assinaladas na prova.</li>
												<li><strong>Aluno: </strong>Para assinalar as alternativas utilize caneta preta ou azul, ou preencha fortemente à lápis.</li>
												<li><strong>Aluno: </strong>Questões em branco, rasuradas ou com mais de uma alternativa assinalada serão consideradas incorretas.</li>
												<li><strong>Professor: </strong>Em caso de ausência do aluno, preencha o circulo 'S' no canto superior direito</li></ul></div></div>";

				html.Append(answerSheetOrientation);

				html.Append("<div class='footer'>");
				html.Append("<span><strong>ATENÇÃO</strong>: Não rasure fora desta área tracejada, assinale apenas as alternativas</span>");
				html.Append("</div>");

				#endregion


				if (filter.IdentificationType.Equals(EnumIdentificationType.NumberID) || filter.IdentificationType.Equals(EnumIdentificationType.QRCode))
				{
					html.Append("</div>");
					html.Append("<div class='sideright'>");

					string absence = @"<div class='absence-container'>
										<div class='mark'></div>
										<div class='mark right'></div>
										<div class='mark bottomLeft'></div>
										<label class='label-absence'>Ausente</label>
										<div class='content-number'>
											<div class='number'>S</div>
										</div>
										</div>";

					html.Append(absence);



					//IDENTIFICAÇÃO DO ALUNO POR Nº DE CHAMADA
					if (filter.IdentificationType.Equals(EnumIdentificationType.NumberID))
						html.Append(Constants.TMPL_HTML_STUDENT_NUMBER_ID);

					//IDENTIFICAÇÃO DO ALUNO POR QRCODE
					if (filter.IdentificationType.Equals(EnumIdentificationType.QRCode))
						html.AppendFormat("<img src='{0}' />", filter.Student.RelativePath);

					html.Append("</div>");
				}
				else
					html.Append("</div>");

				#region QUADRO DE RESPOSTAS
				html.Append("<br clear='all' /><div class='infoarea centerTest'></div>");
				html.Append("<div class='answerFrame'><div class='table-pdf'><div class='mark'></div><div class='mark right'></div><div class='mark bottom'></div>");

				int lines = 0;
				int columns = 0;
				if (qtdeItens <= 20) columns = 1;
				else if (qtdeItens <= 40) columns = 2;
				else if (qtdeItens <= 60) columns = 3;
				else if (qtdeItens <= 80) columns = 4;
				else if (qtdeItens <= 100) columns = 5;

				Parameter columnTemplate = filter.Parameters.FirstOrDefault(i => i.Key.Equals("ANSWERSHEET_USE_COLUMN_TEMPLATE"));

				bool control = true;
				int alternativeNumber = 1;
				string knowledgeArea = "";

				for (int c = 1; c <= columns; c++)
				{
					string cssClass = string.Empty;
					if (c == 1 && columns > c)
						cssClass = "margin-right";
					else if (c != 1 && columns == c)
						cssClass = "margin-left";
					else if (c != 1 && columns > c)
						cssClass = "margin-right margin-left";

					html.AppendFormat("<div class='div-columns'><table class='table-column-{0} {1}'>", c, cssClass);
					html.Append("<tbody><tr><th></th><th>A</th><th>B</th><th>C</th><th>D</th>");
					if (itemType.QuantityAlternative.Equals(5))
						html.Append("<th>E</th></tr>");
					else
						html.Append("</tr>");

					if (c == 1) lines = 20;
					else if (c == 2) lines = 40;
					else if (c == 3) lines = 60;
					else if (c == 4) lines = 80;
					else if (c == 5) lines = 100;

					for (int l = (lines - 20) + 1; l <= lines; l++)
					{
						if (columnTemplate != null && !Convert.ToBoolean(columnTemplate.Value) && l > qtdeItens)
							break;
						else
						{
							bool isKnowledgeAreaBlock = false;
							if (knowledgeAreaBlock)
							{
								if (control)
								{
									BlockItem blockItemAtual = blockItems.ElementAtOrDefault(alternativeNumber - 1);
									BlockItem blockItemAnterior = blockItems.ElementAtOrDefault(alternativeNumber - 2);

									if (blockItemAnterior == null || blockItemAtual.KnowledgeArea_Id != blockItemAnterior.KnowledgeArea_Id)
									{
										isKnowledgeAreaBlock = true;
										knowledgeArea = blockItemAtual.KnowledgeArea_Description;
										control = false;
									}
								}
							}

							if (isKnowledgeAreaBlock)
							{
								html.AppendFormat("<tr><td colspan='100 %' class='knowledgeArea'>{0}</td></tr>", knowledgeArea);
							}
							else
							{
								html.AppendFormat("<tr><td>{0}</td><td><div class='number'>A</div></td><td><div class='number'>B</div></td><td><div class='number'>C</div></td><td><div class='number'>D</div></td>", alternativeNumber);
								if (itemType.QuantityAlternative.Equals(5))
									html.Append("<td><div class='number'>E</div></td></tr>");
								else
									html.Append("</tr>");

								alternativeNumber++;

								control = true;
							}
						}
					}

					html.Append("</tbody></table></div>");
				}

				html.Append("</div>");

				#endregion

				html.Append("</div><br clear='all' /></div>");

				if (filter.PDFPreview) html.Append("</div>");
			}
			catch (Exception ex)
			{
				throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
			}

			return html;
		}
		public string GetHtmlFromItems(PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();

			// CSS
			html.Append(BuildCSS(filter));
			html.Append("<div class='pdf-content'>");
			if (filter.ItemList != null)
				html.Append(BuildItems(filter));
			else
				html.Append(BuildBlockItems(filter));
			html.Append("</div>");
			html.Append("</div></body></html>");

			return html.ToString();
		}

		#region Private Methods

		private static StringBuilder BuildCSS(PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();

			try
			{
				if (!filter.PDFPreview)
					html.Append("<!DOCTYPE html><html><head>");

				string cssFile = string.Format(CssLink, filter.UrlSite);
				html.AppendFormat("<link href='{0}' rel='stylesheet' type='text/css' />", cssFile);

				if (filter.GenerateType.Equals(EnumGenerateType.Test) || filter.GenerateType.Equals(EnumGenerateType.Items))
				{
					#region MathJAX

					string urlMathJax = filter.CDNMathJax ? "http://cdn.mathjax.org/mathjax/latest/MathJax.js" : string.Format("http://{0}/Assets/js/vendor/MathJax.js", filter.UrlSite);

					html.AppendFormat("<script src='http://{0}/Assets/js/vendor/jquery-2.1.1.js' type='text/javascript'></script>", filter.UrlSite);
					html.AppendFormat("<script src='{0}' type='text/javascript'>", urlMathJax);
					html.Append(@"function startMathJax() {
											MathJax.Hub.Config({
												extensions: ['tex2jax.js', 'TeX/AMSmath.js', 'TeX/AMSsymbols.js'],
												tex2jax: { inlineMath: [['$$', '$$'], ['\\(', '\\)']] },
												jax: ['input/TeX', 'output/HTML-CSS'],
												displayAlign: 'center',
												displayIndent: '0.1em',
												showProcessingMessages: false,
											});

											MathJax.Localization.setLocale('pt-br');
											timerRefreshMath();
										};

										function timerRefreshMath() {
											var counter = 0;
											function reloadMathJax() {
												clearInterval(interval);
												interval = setInterval(reloadMathJax, 1);
												counter += 1;
												MathJax.Hub.Queue(['Typeset', MathJax.Hub]);
											}
											var interval = setInterval(reloadMathJax, counter);
										};

										$(document).ready(function () {	   
											startMathJax();
										});");
					html.Append("</script> ");


					#endregion
				}

				if (!filter.PDFPreview)
				{
					html.Append("</head>");
					html.Append("<body class='prova-body'>");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
			}

			return html;
		}
		private static StringBuilder BuildBorder(bool openTag, bool closeTag, PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();

			try
			{
				if ((filter.GenerateType.Equals(EnumGenerateType.Cover) || filter.GenerateType.Equals(EnumGenerateType.Test)) && (filter.Test.TestType.ModelTest.ShowBorder))
				{
					if (filter.PDFPreview)
					{
						if (openTag)
							html.Append("<div class='pdf-border'>");

						if (closeTag)
							html.Append("</div>");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
			}

			return html;
		}
		private static StringBuilder BuildItems(PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();
			IList<Item> items = filter.ItemList;

			try
			{
				string TMPL_Content = string.Empty, TMPL_BaseText = string.Empty, TMPL_Alternatives = string.Empty;
				int que = 0;

				List<long> textbaseLit = new List<long>();

				Item last = items.Last();

				//Imprimir itens ordenado por texto base
				foreach (var questao in items)
				{
					if (questao.ItemNarrated != null && (bool)questao.ItemNarrated)
					{
						TMPL_Content = Constants.TMPL_ItemNarrated_Teacher;
						TMPL_BaseText = Constants.TMPL_BaseText_ItemNarrated_Teacher;
						TMPL_Alternatives = Constants.TMPL_Alternatives_ItemNarrated_Teacher;

						if (questao.BaseText != null && (!textbaseLit.Any(p => p == questao.BaseText.Id)))
						{
							TMPL_BaseText = TMPL_BaseText.Replace("@iconBaseText", questao.BaseText.StudentBaseText != null && !(bool)questao.BaseText.StudentBaseText ? Constants.TMPL_Icon_Narration : string.Empty)
														.Replace("@baseText", !string.IsNullOrEmpty(questao.BaseText.Description) ? questao.BaseText.Description.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") : string.Empty)
														.Replace("@source", !string.IsNullOrEmpty(questao.BaseText.Source) ? questao.BaseText.Source : string.Empty);

							TMPL_Content = TMPL_Content.Replace("@baseText", TMPL_BaseText)
														.Replace("@initialOrientation", !string.IsNullOrEmpty(questao.BaseText.InitialOrientation) ? questao.BaseText.InitialOrientation : string.Empty)
														.Replace("@initialStatement", !string.IsNullOrEmpty(questao.BaseText.InitialStatement) ? questao.BaseText.InitialStatement : string.Empty)
														.Replace("@iconInitialStatement", questao.BaseText.NarrationInitialStatement != null && (bool)questao.BaseText.NarrationInitialStatement ? Constants.TMPL_Icon_Narration : string.Empty)
														.Replace("@baseTextOrientation", !string.IsNullOrEmpty(questao.BaseText.BaseTextOrientation) ? questao.BaseText.BaseTextOrientation : string.Empty);
						}

						StringBuilder alternatives = BuildAlternatives(questao, TMPL_Alternatives);

						TMPL_Content = TMPL_Content.Replace("@question", (++que).ToString())
													.Replace("@tips", !string.IsNullOrEmpty(questao.Tips) ? string.Format("<strong>Comentário: </strong>{0}", questao.Tips) : string.Empty)
													.Replace("@iconStatement", questao.StudentStatement != null && !(bool)questao.StudentStatement ? Constants.TMPL_Icon_Narration : string.Empty)
													.Replace("@statement", !string.IsNullOrEmpty(questao.Statement) ? questao.Statement.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") : string.Empty)
													.Replace("@last", !questao.Equals(last) && filter.Test != null && filter.Test.TestType != null && filter.Test.TestType.ModelTest != null && filter.Test.TestType.ModelTest.ShowItemLine ? classLine : string.Empty)
													.Replace("@alternatives", alternatives.ToString());

						html.Append(TMPL_Content);
					}
					else
					{
						html.Append("<div class='prova-question page-break'>");

						if (questao.BaseText != null && (!textbaseLit.Any(p => p == questao.BaseText.Id)) && (!string.IsNullOrEmpty(questao.BaseText.Description)))
						{
							html.Append("<div class='prova-basetext'>" + questao.BaseText.Description.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") + "</div>");
							if (!string.IsNullOrEmpty(questao.BaseText.Source))
								html.Append("<p class='prova-quest' >" + questao.BaseText.Source + "</p>");
							textbaseLit.Add(questao.BaseText.Id);
						}

						//Monta as questões da prova.
						html.Append("<h3><b class='prova-question-num'>Questão " + (++que) + "</b></h3>");

						html.Append((questao.Statement != null ? questao.Statement.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") : "<p>" + string.Empty + "</p>"));

						// Monta as alternativas da questão de acordo com a ordem.
						html.Append(BuildAlternatives(questao, TMPL_Alternatives));

						if ((!questao.Equals(last)) && (filter.Test != null && filter.Test.TestType != null && filter.Test.TestType.ModelTest != null && filter.Test.TestType.ModelTest.ShowItemLine))
						{
							html.Append(classLine);
						}

						html.Append("</div>");
					}
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
			}

			return html;
		}
		private static StringBuilder BuildBlockItems(PDFFilter filter)
		{
			StringBuilder html = new StringBuilder();
			IList<BlockItem> items = filter.BlockItemList;
			IEnumerable<Parameter> parameters = filter.Parameters;

			try
			{
				int que = 0;

				List<long> textbaseLit = new List<long>();

				items = items.OrderBy(i => i.Order).ToList();

				Item last = items.Select(i => i.Item).Last();

				//Imprimi itens ordenado por texto base
				foreach (var blockItem in items)
				{
					html.Append("<div class='prova-question page-break'>");

					var questao = blockItem.Item;

					if (questao.BaseText != null && (!textbaseLit.Any(p => p == questao.BaseText.Id)) && (!string.IsNullOrEmpty(questao.BaseText.Description)))
					{
						string textoBasePadrao = string.Empty;

						if (parameters != null && parameters.Any())
						{
							var itemsTextoBase = items.Where(i => i.Item.BaseText != null && i.Item.BaseText.Id.Equals(questao.BaseText.Id));
							if (itemsTextoBase != null && itemsTextoBase.Any() && itemsTextoBase.Count() > 1)
							{
								Parameter textoPadrao = parameters.FirstOrDefault(i => i.Key.Equals("BASE_TEXT_DEFAULT"));
								if (textoPadrao != null)
								{
									string intervalo = string.Empty;

									if (itemsTextoBase.Count() == 2)
										intervalo = string.Format("{0} e {1}", que + 1, que + 2);
									else if (itemsTextoBase.Count() > 2)
										intervalo = string.Format("de {0} a {1}", que + 1, ((que + 1) + (itemsTextoBase.Count() - 1)));

									textoBasePadrao = "<span class='baseTextDefault'>" + string.Format("{0} {1}", textoPadrao.Value, intervalo) + ".</span><br/>";
								}
							}
						}

						html.Append("<div class='prova-basetext'>" + textoBasePadrao + questao.BaseText.Description.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") + "</div>");
						if (!string.IsNullOrEmpty(questao.BaseText.Source))
							html.Append("<p class='prova-quest' >" + questao.BaseText.Source + "</p>");
						textbaseLit.Add(questao.BaseText.Id);
					}

					//Monta as questões da prova.
					html.Append("<h3><b class='prova-question-num'>Questão " + (++que) + "</b></h3>");

					html.Append((questao.Statement != null ? questao.Statement.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") : "<p>" + string.Empty + "</p>"));

					// Monta as alternativas da questão de acordo com a ordem.
					html.Append(BuildAlternatives(questao, string.Empty));

					if ((!questao.Equals(last)) && (filter.Test.TestType.ModelTest.ShowItemLine))
					{
						html.Append(classLine);
					}

					html.Append("</div>");
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
			}

			return html;
		}
		private static StringBuilder BuildAlternatives(Item item, string TMPL)
		{
			StringBuilder html = new StringBuilder();

			try
			{
				IList<Alternative> alternativasOrdenada = item.Alternatives.OrderBy(a => a.Order).ToList();

				char alternativas = 'A';
				//Monta as alternativas da questão.
				foreach (var alternativa in alternativasOrdenada)
				{
					string aContent = string.Empty;
					string aDescription = alternativa.Description != null ? alternativa.Description.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;") : "<br />";

					if (!string.IsNullOrEmpty(TMPL) && TMPL.Equals(Constants.TMPL_Alternatives_ItemNarrated_Teacher))
					{
						aContent = TMPL.Replace("@number", alternativas.ToString())
									.Replace("@alternative", aDescription)
									.Replace("@iconAlternatives", item.NarrationAlternatives != null && (bool)item.NarrationAlternatives ? Constants.TMPL_Icon_Narration : string.Empty)
									.Replace("@checkAlternative", Constants.TMPL_Icon_Alternative);
					}
					else
					{
						aContent = "" +
							"<div class='prova-answer'>" +
								"<span class='prova-alternative'>(" + alternativas + ") </span> " + aDescription +
							"</div>";
					}

					html.Append(aContent);

					//Incrementa a alternativa.
					alternativas++;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
			}

			return html;
		}

		#endregion
	}
}
