using EvoPdf;
using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.HTMLToPDF
{
    public static class HTMLToPDF
    {
        public static byte[] GetHTMLBytes(Test test, string html, PdfPageSize pageSize, string urlImgFile)
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.PdfDocumentOptions.PdfPageSize = pageSize;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;

            pdfConverter.PdfDocumentInfo.Title = test.Description;
            pdfConverter.PdfDocumentInfo.CreatedDate = DateTime.Now;
            pdfConverter.LicenseKey = "Y+3+7P//7P/6++z54vzs//3i/f7i9fX19Q==";

            //Margins
            #region MARGINS

            pdfConverter.PdfDocumentOptions.BottomMargin = (float)28.34;
            pdfConverter.PdfDocumentOptions.LeftMargin = (float)28.34;
            pdfConverter.PdfDocumentOptions.RightMargin = (float)28.34;
            pdfConverter.PdfDocumentOptions.TopMargin = (float)28.34;

            pdfConverter.PdfDocumentOptions.TopSpacing = 5;
            pdfConverter.PdfDocumentOptions.BottomSpacing = 5;

            #endregion
            
            #region CABEÇALHO

            StringBuilder cabecalho = new StringBuilder();

            String htmlCabecalho = "<style type='text/css'> " +
                                       ".prova-img { height: 60px;  margin-left: 30px;  margin-top: 10px;}" +
                                   "</style>" +
                                   "<center>" +
                                        "<img class='prova-img' src='http://" + urlImgFile + "/Assets/images/sme-sp.jpg'/>" +
                                    "</center>";

            cabecalho.Append(htmlCabecalho);
            AddHeader(pdfConverter, cabecalho.ToString());
            #endregion

            #region RODAPÉ

            StringBuilder rodape = new StringBuilder();

            String bimestre = "";
            if (test.Bimester > 0)
            {
                bimestre = (int)test.Bimester+"º bimestre/";
            }

            String htmlRodape = "<style type='text/css'> " +
                                    ".prova-footer    { margin-left: 20px;  font-family: 'Arial'; }" +
                                    ".prova-footer.a  {                     font-size:      16px; }" +
                                    ".prova-footer.b  { margin-top: -15px;  font-size:      18px; }" +
                                "</style>" +
                                "<center>" +
                                    "<p class='prova-footer a'> SME/DOT </p>" +
                                    "<p class='prova-footer b'>" + bimestre + test.ApplicationStartDate.Year + "</p>" +
                                "</center>";

            rodape.Append(htmlRodape);
            AddFooter(pdfConverter, rodape.ToString());
            #endregion

            #region BORDER
 
            Document pdfDocument = pdfConverter.GetPdfDocumentObjectFromHtmlString(html);

            //incluir borda da página
            foreach (PdfPage page in pdfDocument.Pages)
            {
                float width = (page.PageSize.Width - pdfConverter.PdfDocumentOptions.RightMargin - pdfConverter.PdfDocumentOptions.LeftMargin);
                float height = (page.PageSize.Height - pdfConverter.PdfDocumentOptions.TopMargin - pdfConverter.PdfDocumentOptions.BottomMargin);
                RectangleElement borderRectangleElement = new RectangleElement(0, -(page.Header.Height), width, height);
                page.AddElement(borderRectangleElement);
            }

            #endregion
             
            return pdfDocument.Save();
        }

        /// <summary>
        /// Adiciona o cabeçalho do Pdf.
        /// </summary>
        /// <param name="pdfConverter"></param>
        private static void AddHeader(PdfConverter pdfConverter, string cabecalho)
        {
            if (String.IsNullOrEmpty(cabecalho))
            {
                cabecalho = "<p></p>";
            }

            pdfConverter.PdfDocumentOptions.ShowHeader = true;
            pdfConverter.PdfHeaderOptions.HeaderHeight = 50;
            HtmlToPdfElement headerHtml = new HtmlToPdfElement(cabecalho, null);
            headerHtml.FitHeight = true;
            pdfConverter.PdfHeaderOptions.AddElement(headerHtml);
        }

        /// <summary>
        /// Adiciona o rodapé do Pdf.
        /// </summary>
        /// <param name="pdfConverter"></param>
        private static void AddFooter(PdfConverter pdfConverter, string rodape)
        {
            if (String.IsNullOrEmpty(rodape)) {
                rodape = "<p></p>";
            }

            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfFooterOptions.FooterHeight = 50;

            HtmlToPdfElement footerHtml = new HtmlToPdfElement(rodape, null);
            footerHtml.FitHeight = true;
            pdfConverter.PdfFooterOptions.AddElement(footerHtml);


            Font footerFont = new Font(new FontFamily("Arial"), 10, GraphicsUnit.Point);
            TextElement footerTextElement = new TextElement(0, 30, "&p; de &P;  ", footerFont);

            footerTextElement.TextAlign = HorizontalTextAlign.Right;
            pdfConverter.PdfFooterOptions.AddElement(footerTextElement);
        }

        public static string GetHtmlAvaliacao(Test avaliacao, IList<Item> listItens, int tamanhoFonte, string urlCssFile)
        {
            StringBuilder html = new StringBuilder();

            // CSS
            html.Append(MontaCSS(tamanhoFonte, urlCssFile));

            // Capa
            html.Append(MontaCapa(avaliacao));

            // Questões
            html.Append(MontaQuestao(listItens));

            // Fim do HTML
            html.Append("</body></html>");

            return html.ToString();
        }

        /// <summary>
        /// Monta CSS da avaliação.
        /// </summary>
        /// <param name="tamanhoFonte">tamanho da fonte (se menor que zero, tamanho normal)</param>
        /// <param name="urlCssFile">urlbase do projeto</param>
        /// <returns></returns>
        private static StringBuilder MontaCSS(int tamanhoFonte, string urlCssFile)
        {
            StringBuilder html = new StringBuilder();

            try {
                #region init: HTML inicial da avaliação
                    String init = "" +
                        "<html>" +
                        "<head>" +
                            "<link href='http://" + urlCssFile + "/Assets/css/prova.css' rel='stylesheet' />" +
                            "<style type='text/css'> " +
                                ".prova-question{ font-size: " + tamanhoFonte + "px;}" +
                                ".prova-quest{ font-size: " + (tamanhoFonte - 4) + "px; }"+
                            "</style>" +
                        "</head>" +
                        "<body class='prova-body'>";

                #endregion
                html.Append(init);
            }
            catch (Exception ex) {
                throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
            }

            return html;
        }

        private static StringBuilder MontaCapa(Test test)
        {
            StringBuilder html = new StringBuilder();

            try
            {
                #region table: Tabela com as informações básicas do aluno (nome, turma e escola).
                    String table =  "" +
                        "<center>" +
                            "<div class='prova-table-container'>" +
                                "<table class='prova-table'>" +

                                    "<colgroup>" +
                                        "<col width='15%' />" +
                                        "<col width='55%' />" +
                                        "<col width='15%' />" +
                                        "<col width='15%' />" +
                                    "</colgroup>" +

                                    "<tbody>" +
                                        "<tr class='prova-tr-first'>" +
                                            "<td> <b>Escola:</b> </td>" +
                                            "<td></td>" +
                                            "<td> <b style='float: right;'>Turma:</b> </td>" +
                                            "<td></td>"+
                                        "</tr>" +
                                        "<tr>"+
                                            "<td><b>Nome:</b> </td>" +
                                            "<td></td>" +
                                            "<td>  <b style='float: right;'>Nº:</b> </td>" +
                                            "<td></td>" +
                                        "</tr>" +
                                        "<tr>"+
                                            "<td><b>Professor:</b> </td>" +
                                            "<td></td>" +
                                            "<td> <b style='float: right;'>Data:</b> </td>" +
                                            "<td></td>"+
                                        "</tr>" +
                                        "<tr class='prova-tr-last'>" +
                                            "<td></td>" +
                                            "<td></td>" +
                                            "<td></td>" +
                                            "<td></td>" +
                                        "</tr>" +
                                    "</tbody>" +

                                "</table>" +
                            "</div>" +
                        "</center>" +

                        "<br />" +
                        "<h1 class='prova-description'>" +
                            test.Description +
                        "</h1>";

                #endregion
                html.Append(table);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
            }

            return html;
        }

        /// <summary>
        /// Monta as questões da prova.
        /// </summary>
        /// <param name="listaQuestoes"></param>
        /// <returns></returns>
        private static StringBuilder MontaQuestao(IList<Item> itens)
        {
            StringBuilder html = new StringBuilder();

            // Uma pagina A4 sem cabeçario e rodapé tem 1124 points( 1/71 inches ) 
            // 1632px dá uma pagina de conteudo

            try
            {
                int que = 0;

                List<long> textbaseLit = new List<long>();

                List<Item> itensOrder = itens.Where(i => i.BaseText != null).OrderBy(i => i.BaseText.Id).ToList();
                itensOrder.AddRange(itens.Where(i => i.BaseText == null).ToList());

                Item last = itensOrder.Last();

                //Imprimi itens ordenado por texto base
                foreach (var questao in itensOrder)
                {
                    if (questao.BaseText != null && (!textbaseLit.Any(p => p == questao.BaseText.Id)) && (!string.IsNullOrEmpty(questao.BaseText.Description)))
                    {
                        html.Append("<div class='prova-question page-break'>" + questao.BaseText.Description + "</div>");
                        html.Append( "<p class='prova-quest' >" + questao.BaseText.Source + "</p><br />" );
                        textbaseLit.Add(questao.BaseText.Id);
                    }
					
                    //Monta as questões da prova.
                    html.Append("<div class='prova-question page-break'>");
                    html.Append("<h3><b class='prova-question-num'>Questão " + (++que) + "</b></h3>");

                    html.Append((questao.Statement != null ? questao.Statement : "<p>" + string.Empty + "</p>") + "<br>");
                    
                    // Monta as alternativas da questão de acordo com a ordem.
                    html.Append(MontaAlternativas(questao, questao.Id));

                   if (!questao.Equals(last))
                        html.Append("<div class='prova-hr-border'></div>");

                    html.Append("</div>");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
            }

            return html;
        }

        /// <summary>
        /// Monta as alternativas da questão.
        /// </summary>
        /// <param name="questao"></param>
        /// <param name="questao_id"></param>
        /// <returns></returns>
        private static StringBuilder MontaAlternativas(Item item, long que_id)
        {
            StringBuilder html = new StringBuilder();

            try
            {
                IList<Alternative> alternativasOrdenada = item.Alternatives.OrderBy(a => a.Order).ToList();

                char alternativas = 'A';
                //Monta as alternativas da questão.
                foreach (var alternativa in alternativasOrdenada)
                {
                    #region aConteundo
                        String aDescription = alternativa.Description != null ? alternativa.Description : "<br />";
                        String aConteundo = "" +
                            "<div class='prova-answer'>" +
                                "<span class='prova-alternative'>(" + alternativas + ") </span> " + aDescription +
                            "</div>";

                        if (aDescription != "<br />" && aDescription.Substring(0, 7) == "<p><img") {
                            aConteundo += "<br />";
                        }
                    
                    #endregion
                    html.Append(aConteundo);

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

        /// <summary>
        /// Monta o gabarito das questões da prova.
        /// </summary>
        /// <param name="prova"></param>
        /// <param name="modelo"></param>
        /// <returns></returns>
        private static StringBuilder MontaGabarito(Test test, IList<Item> itens)
        {
            StringBuilder html = new StringBuilder();

            try
            {
                #region table
                    String table = "" +
                        "<h2 class='prova-gabarito'> FOLHA DE RESPOSTAS </h2>"+
                        "<h3 class='prova-gabarito-description'>" + test.Description + "</h3>" +
                        "<center>" +
                            "<div class='prova-table-container'>" +
                                "<table class='prova-table'>" +

                                    "<colgroup>" +
                                        "<col width='15%' />" +
                                        "<col width='55%' />" +
                                        "<col width='15%' />" +
                                        "<col width='15%' />" +
                                    "</colgroup>" +

                                    "<tbody>" +
                                        "<tr class='prova-tr-first'>" +
                                            "<td> <b>Escola:</b> </td>" +
                                            "<td></td>" +
                                            "<td> <b style='float: right;'>Turma:</b> </td>" +
                                            "<td></td>"+
                                        "</tr>" +
                                        "<tr>"+
                                            "<td><b>Nome:</b> </td>" +
                                            "<td></td>" +
                                            "<td>  <b style='float: right;'>Nº:</b> </td>" +
                                            "<td></td>" +
                                        "</tr>" +
                                        "<tr>"+
                                            "<td><b>Professor:</b> </td>" +
                                            "<td></td>" +
                                            "<td> <b style='float: right;'>Data:</b> </td>" +
                                            "<td></td>"+
                                        "</tr>" +
                                        "<tr class='prova-tr-last'>" +
                                            "<td></td>" +
                                            "<td></td>" +
                                            "<td></td>" +
                                            "<td></td>" +
                                        "</tr>" +
                                    "</tbody>" +

                                "</table>" +
                            "</div>" +
                        "</center>";

                #endregion
                html.Append(table);

                //Tabela de gabarito das questões da avaliação.
                int tabelas = 0;
                int i = 1;

                html.Append("<table class='prova-gabarito-questions'>");
                html.Append("<tr>");
                html.Append("<td>");
                foreach (Item item in itens)
                {
                    // Quebra o gabarito em tabelas de 10 linhas
                    tabelas++;
                    if (tabelas == 1)
                    {
                        html.Append("<div class='prova-gabarito-container page-break'>");
                        html.Append("<table cellspacing='0' class='prova-gabarito-table'>");
                    }

                    char alternativas = 'A';

                    html.Append("<tr>");
                    html.Append("<td class='td-number'><b>" + (i + 1) + "</b></td>");
                    i++;
                    for (int j = 0; j < item.Alternatives.Count; j++)
                    {
                        html.Append("<td class='td-resp'><span>" + alternativas + "</span></td>");
                        //Incrementa a alternativa.
                        alternativas++;
                    }

                    html.Append("</tr>");

                    // Quebra o gabarito em colunas de 10 linhas
                    if (tabelas == 10 || tabelas == itens.Count)
                    {
                        html.Append("</table>");
                        html.Append("</div>");
                        tabelas = 0;
                    }
                }

                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table></center>");
    
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível gerar o arquivo PDF da avaliação.", ex);
            }

            return html;
        }
    }
}
