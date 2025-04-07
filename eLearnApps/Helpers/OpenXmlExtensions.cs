using System.IO;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace eLearnApps.Helpers
{

    public static class OpenXmlExtensions
    {
        public static Regex TagRegex = new Regex(@"<[^\\>]+\/>|<\s*([^ >]+)[^>]*>(.*?)<\s*\/\s*\1\s*>|<br>|<br/>", RegexOptions.Singleline);
        public static Regex HighlightRegex = new Regex(@"<span\s+style=['""]background-color:yellow;color:\s*black['""]>\s*(.*?)\s*</span>|<br>|<br/>", RegexOptions.Singleline);
        public const string HrefPattern = @"\bhref\s*=\s*""([^""]*)""";
        public const string HrefReplacement = @"href=""""";
        public static Regex SubscriptRegex = new Regex(@"[ₐ-ₜₓ₀-₉]", RegexOptions.Singleline);
        public static string userCss = @"";
        public const string defaultCss =
            @"html, address,
blockquote,
body, dd, div,
dl, dt, fieldset, form,
frame, frameset,
h1, h2, h3, h4,
h5, h6, noframes,
ol, p, ul, center,
dir, hr, menu, pre { display: block; unicode-bidi: embed }
li { display: list-item }
head { display: none }
table { display: table }
tr { display: table-row }
thead { display: table-header-group }
tbody { display: table-row-group }
tfoot { display: table-footer-group }
col { display: table-column }
colgroup { display: table-column-group }
td, th { display: table-cell }
caption { display: table-caption }
th { font-weight: bolder; text-align: center }
caption { text-align: center }
body { margin: auto; }
h1 { font-size: 2em; margin: auto; }
h2 { font-size: 1.5em; margin: auto; }
h3 { font-size: 1.17em; margin: auto; }
h4, p,
blockquote, ul,
fieldset, form,
ol, dl, dir,
menu { margin: auto }
a { color: blue; }
h5 { font-size: .83em; margin: auto }
h6 { font-size: .75em; margin: auto }
h1, h2, h3, h4,
h5, h6, b,
strong { font-weight: bolder }
blockquote { margin-left: 40px; margin-right: 40px }
i, cite, em,
var, address { font-style: italic }
pre, tt, code,
kbd, samp { font-family: monospace }
pre { white-space: pre }
button, textarea,
input, select { display: inline-block }
big { font-size: 1.17em }
small, sub, sup { font-size: .83em }
sub { vertical-align: sub }
sup { vertical-align: super }
table { border-spacing: 2px; }
thead, tbody,
tfoot { vertical-align: middle }
td, th, tr { vertical-align: inherit }
s, strike, del { text-decoration: line-through }
hr { border: 1px inset }
ol, ul, dir,
menu, dd { margin-left: 40px }
ol { list-style-type: decimal }
ol ul, ul ol,
ul ul, ol ol { margin-top: 0; margin-bottom: 0 }
u, ins { text-decoration: underline }
br:before { content: ""\A""; white-space: pre-line }
center { text-align: center }
:link, :visited { text-decoration: underline }
:focus { outline: thin dotted invert }
/* Begin bidirectionality settings (do not change) */
BDO[DIR=""ltr""] { direction: ltr; unicode-bidi: bidi-override }
BDO[DIR=""rtl""] { direction: rtl; unicode-bidi: bidi-override }
*[DIR=""ltr""] { direction: ltr; unicode-bidi: embed }
*[DIR=""rtl""] { direction: rtl; unicode-bidi: embed }
";
        public static TableCell SetCellWidth(this TableCell tableCell, int width)
        {
            var tableCellProperties = new TableCellProperties();
            var tableCellWidth = new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Pct };
            tableCellProperties.AppendChild(tableCellWidth);
            tableCell.AppendChild(tableCellProperties);
            return tableCell;
        }
        public static bool TextCompareWithHrefRule(string text1, string text2)
        {

            // for testing 
            //string htmlString = @"<a href=""https://example.com/files/Mid%20Terms%20Paper.pdf"">Download File</a>";
            //text1 += htmlString;
            //text2 = htmlString;
            //text1 = @"<p>Please type your answers to all questions in this box before hitting &#39;Submit&#39;. You may access the question paper <a rel=""noopener"" href=""New_Exam_Paper.pdf"" target=""_blank"">here</a>.</p>";

            MatchCollection matches1 = Regex.Matches(text1, HrefPattern);
            Console.WriteLine("Extracted href values:");
            var hrefLinks1 = new List<string>();
            foreach (Match match in matches1)
            {
                var encodedPath = match.Groups[1].Value;

                // Extract only the filename (last part of the path)
                string fileName = System.IO.Path.GetFileName(encodedPath);

                hrefLinks1.Add(fileName);
            }
            MatchCollection matches2 = Regex.Matches(text2, HrefPattern);
            var hrefLinks2 = new List<string>();
            foreach (Match match in matches2)
            {
                var encodedPath = match.Groups[1].Value;

                // Extract only the filename (last part of the path)
                string fileName = System.IO.Path.GetFileName(encodedPath);
                hrefLinks2.Add(fileName);
            }
            var updatedText1 = Regex.Replace(text1, HrefPattern, HrefReplacement);
            var updatedText2 = Regex.Replace(text2, HrefPattern, HrefReplacement);
            bool hrefMatch = hrefLinks1.Count == hrefLinks2.Count;
            if (hrefMatch)
            {
                for (var i = 0; i < hrefLinks1.Count; i++)
                {
                    if (!string.Equals(hrefLinks1[i], hrefLinks2[i], StringComparison.OrdinalIgnoreCase))
                    {
                        hrefMatch = false;
                        break;
                    }
                }
            }
            var textMatch = string.Equals(updatedText1, updatedText2, StringComparison.OrdinalIgnoreCase);
            return textMatch && hrefMatch;
        }
        public static Tuple<int, int> GetLineFromIndex(string text, int minIndex, int maxIndex)
        {
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int currentIndex = 0;
            int startIndex = -1;
            int endIndex = -1;

            // Normalize the maxIndex to ensure it's inclusive and valid
            if (maxIndex >= text.Length)
            {
                maxIndex = text.Length - 1;
            }
            for (int i = 0; i < lines.Length; i++)
            {
                int lineLength = lines[i].Length;
                int lineStartIndex = currentIndex;
                int lineEndIndex = currentIndex + lineLength + System.Environment.NewLine.Length - 1; // End index of the current line
                                                                                                      //var aa = text[lineStartIndex];
                                                                                                      //var bb = text[lineEndIndex];
                                                                                                      //var cc = text.Substring(lineStartIndex, lineEndIndex - lineStartIndex);
                                                                                                      // Check if any part of the line overlaps with the min-max range
                if (minIndex >= lineStartIndex && minIndex <= lineEndIndex)
                {
                    startIndex = lineStartIndex;
                }
                if (maxIndex >= lineStartIndex && maxIndex <= lineEndIndex)
                {
                    endIndex = lineEndIndex;
                    break;
                }


                currentIndex += lineLength + System.Environment.NewLine.Length; // +1 for the newline character
            }


            return new Tuple<int, int>(startIndex, endIndex);
        }
        public static string EscapeContent(Match match)
        {
            var groupIndex = 1;
            // Full matched tag
            string fullTag = match.Value;

            // If the tag has content (non-self-closing), escape the content
            if (match.Groups[groupIndex].Success)
            {
                string content = match.Groups[groupIndex].Value;
                if (string.IsNullOrEmpty(content))
                {
                    return "";
                }
                // Escape the content using HTML encoding
                //string escapedContent = content.Replace("<", "&lt;").Replace(">", "&gt;");
                string escapedContent = HtmlEncode(content);

                // Rebuild the tag with escaped content
                return match.Value.Replace(content, escapedContent);
            }

            // If it's a self-closing tag, return as-is
            return fullTag;
        }

        public static string HtmlEncode(string input)
        {

            if (string.IsNullOrEmpty(input))
                return input;
            // to convert subscript character to match with GPT response
            input = Regex.Replace(input, @"[ₐ-ₜₓ₀-₉]", "â ");
            return input
                .Replace("–", "-")
                .Replace("—", "-")
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("“", "&quot;")
                .Replace("”", "&quot;")
                .Replace("'", "&#39;")
                .Replace("‘", "&#39;")
                .Replace("’", "&#39;");
        }
        public static string EncodeWordCharacter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input
                .Replace("–", "-")
                .Replace("—", "-")
                .Replace("“", "&quot;")
                .Replace("”", "&quot;")
                .Replace("‘", "&#39;")
                .Replace("’", "&#39;");
        }
        public static string ConvertHtmlText(string htmlText)
        {
            var regex = OpenXmlExtensions.HighlightRegex;
            // replaced html content with escaped html character
            //htmlText = regex.Replace(htmlText, new MatchEvaluator(EscapeContent));
            htmlText = htmlText.Replace("\r\n", "<br>").Replace("\n", "<br>");
            // to replace with escaped html character in string outside html tag
            var matches = regex.Matches(htmlText);
            var parts = new List<string>();
            int lastIndex = 0;
            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    parts.Add(HtmlEncode(htmlText.Substring(lastIndex, match.Index - lastIndex)));
                }
                parts.Add(match.Value);
                lastIndex = match.Index + match.Length;
            }
            if (lastIndex < htmlText.Length)
            {
                parts.Add(HtmlEncode(htmlText.Substring(lastIndex)));
            }
            htmlText = string.Join("", parts);
            return htmlText;
        }

        public static Table SetTableAutoFit(this Table table)
        {
            // Set the style and width for the table.
            var tableProp = new TableProperties();
            var tableStyle = new TableStyle { Val = "TableGrid" };

            // Make the table width 100% of the page width.
            var tableWidth = new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct };

            // Apply
            tableProp.Append(tableStyle, tableWidth);
            table.AppendChild(tableProp);

            return table;
        }

        public static Table SetTableColumn(this Table table, int columns)
        {
            var element = new OpenXmlElement[columns];
            // Add 2 columns to the table.
            for (var i = 0; i < columns; i++) element[i] = new GridColumn();
            var tg = new TableGrid(element);
            table.AppendChild(tg);
            return table;
        }

        public static T SetText<T>(this T element, string text, string fontSize = "32", string lineSpace = "240")
        {
            element.CreateParagraph(text, fontSize, lineSpace);
            return element;
        }

        public static void CreateParagraph<T>(this T element, string text = "", string size = "16",
            string lineSpace = "240")
        {
            size = (Convert.ToInt32(size) * 2).ToString();
            var run = new Run();
            var runProperties = new RunProperties();
            var runFonts = new RunFonts
            {
                HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
                ComplexScriptTheme = ThemeFontValues.MinorHighAnsi,
                Ascii = "Arial"
            };
            var fontSize = new FontSize { Val = size };
            var fontSizeComplexScript = new FontSizeComplexScript { Val = size };
            runProperties.AppendChild(runFonts);
            runProperties.AppendChild(fontSize);
            runProperties.AppendChild(fontSizeComplexScript);
            run.AppendChild(runProperties);
            parseTextForOpenXML(run, text);

            var paragraphProperties = new ParagraphProperties();
            var spacingBetweenLines = new SpacingBetweenLines { Line = lineSpace, LineRule = LineSpacingRuleValues.Auto };
            paragraphProperties.AppendChild(spacingBetweenLines);

            var paragraph = new Paragraph();
            paragraph.AppendChild(run);
            paragraph.AppendChild(paragraphProperties);

            (element as OpenXmlCompositeElement)?.AppendChild(paragraph);
        }

        public static Paragraph CreateParagraph(string text = "", string size = "16", string lineSpace = "240")
        {
            size = (Convert.ToInt32(size) * 2).ToString();
            var run = new Run();
            var runProperties = new RunProperties();
            var runFonts = new RunFonts
            {
                HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
                ComplexScriptTheme = ThemeFontValues.MinorHighAnsi,
                Ascii = "Arial"
            };
            var fontSize = new FontSize { Val = size };
            var fontSizeComplexScript = new FontSizeComplexScript { Val = size };
            runProperties.AppendChild(runFonts);
            runProperties.AppendChild(fontSize);
            runProperties.AppendChild(fontSizeComplexScript);
            run.AppendChild(runProperties);
            parseTextForOpenXML(run, text);

            var paragraphProperties = new ParagraphProperties();
            var spacingBetweenLines = new SpacingBetweenLines { Line = lineSpace, LineRule = LineSpacingRuleValues.Auto };
            paragraphProperties.AppendChild(spacingBetweenLines);

            var paragraph = new Paragraph();
            paragraph.AppendChild(run);
            paragraph.AppendChild(paragraphProperties);

            return paragraph;
        }

        public static void CreateTableRow(this Table table)
        {
            var row = new TableRow();
            table.AppendChild(row);
        }

        public static TableCell SetBottomBorder(this TableCell tableCell, BorderValues? val = null,
            string? color = null, UInt32Value? size = null, UInt32Value? space = null)
        {
            if (val is null)
            {
                val = BorderValues.Single;
            }
            if (color is null)
            {
                color = "auto";
            }
            var tableCellBorderProperties = new TableCellProperties();
            var tableCellBorders = new TableCellBorders();

            var bottomBorder = new BottomBorder
            {
                Val = val,
                Color = color,
                Size = size == null ? (UInt32Value)4U : size,
                Space = space == null ? (UInt32Value)0U : space
            };
            tableCellBorders.AppendChild(bottomBorder);
            tableCellBorderProperties.AppendChild(tableCellBorders);
            tableCell.AppendChild(tableCellBorderProperties);
            return tableCell;
        }

        public static TableCell SetTopBorder(this TableCell tableCell, BorderValues? val = null,
            string? color = null, UInt32Value? size = null, UInt32Value? space = null)
        {
            if (val is null)
            {
                val = BorderValues.Single;
            }
            if (color is null)
            {
                color = "auto";
            }

            var tableCellBorderProperties = new TableCellProperties();
            var tableCellBorders = new TableCellBorders();

            var topBorder = new TopBorder
            {
                Val = val,
                Color = color,
                Size = size == null ? (UInt32Value)4U : size,
                Space = space == null ? (UInt32Value)0U : space
            };
            tableCellBorders.AppendChild(topBorder);
            tableCellBorderProperties.AppendChild(tableCellBorders);
            tableCell.AppendChild(tableCellBorderProperties);
            return tableCell;
        }

        public static TableCellProperties AddTableCellProperties(this TableCellProperties properties,
            TableWidthUnitValues? tableWidthUnit, int? gridSpan = null)
        {
            if (tableWidthUnit is null)
            {
                tableWidthUnit = TableWidthUnitValues.Auto;
            }
            var tableCellResponseWidth = new TableCellWidth { Type = tableWidthUnit };
            properties.AppendChild(tableCellResponseWidth);
            if (gridSpan.HasValue)
            {
                var gridSpanResponse = new GridSpan { Val = gridSpan };
                properties.AppendChild(gridSpanResponse);
            }

            return properties;
        }

        public static Run SetFontSize(this Run run, string size = "16", bool bold = false)
        {
            size = (Convert.ToInt32(size) * 2).ToString();
            var runProperties = new RunProperties();
            var runFonts = new RunFonts
            {
                HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
                ComplexScriptTheme = ThemeFontValues.MinorHighAnsi,
                Ascii = "Arial"
            };
            var fontSize = new FontSize { Val = size };
            var fontSizeComplexScript = new FontSizeComplexScript { Val = size };
            if (bold)
            {
                Bold boldStyle = new Bold();
                runProperties.Append(boldStyle);
            }
            runProperties.AppendChild(runFonts);
            runProperties.AppendChild(fontSize);
            runProperties.AppendChild(fontSizeComplexScript);
            run.AppendChild(runProperties);
            return run;
        }

        public static ParagraphProperties SetLineSpace(this ParagraphProperties paragraphProperties,
            string space = "240")
        {
            var spacingBetweenLines = new SpacingBetweenLines { Line = space, LineRule = LineSpacingRuleValues.Auto };
            paragraphProperties.AppendChild(spacingBetweenLines);
            return paragraphProperties;
        }
        public static string CleanupTag(this string input)
        {
            input = input.Replace("\n", "<br />");
            input = input.Replace("&amp;", "&");
            input = input.Replace("&nbsp;", "\xA0");
            input = input.Replace("&quot;", "\"");
            input = input.Replace("&lt;", "~lt;");
            input = input.Replace("&gt;", "~gt;");
            input = input.Replace("&#", "~#");
            input = input.Replace("&", "&amp;");
            input = input.Replace("~lt;", "&lt;");
            input = input.Replace("~gt;", "&gt;");
            input = input.Replace("~#", "&#");
            return input;
        }
        public static XElement HtmlToXElement(this string html)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html.Replace("<p><span>&nbsp;</span></p>", ""));
            RemoveEmptyNodes(doc.DocumentNode);

            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                using (StringReader reader = new StringReader(writer.ToString()))
                {
                    return XElement.Load(reader);
                }
            }
        }
        public static string RemoveSpecialCharacter(string input)
        {
            string result = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");
            return result;
        }   
        static void RemoveEmptyNodes(HtmlNode containerNode)
        {
            if (containerNode.Attributes.Count == 0 && string.IsNullOrEmpty(containerNode.InnerText))
            {
                containerNode.Remove();
            }
            else
            {
                for (int i = containerNode.ChildNodes.Count - 1; i >= 0; i--)
                {
                    RemoveEmptyNodes(containerNode.ChildNodes[i]);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="styleVal"></param>
        /// <param name="input"></param>
        public static void CreateParagraphWithStyle<T>(this T element, string styleVal, string input)
        {
            var paragraph = new Paragraph();

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId = new ParagraphStyleId() { Val = styleVal };
            paragraphProperties.AppendChild(paragraphStyleId);

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            Languages languages = new Languages { Val = "en-US" };
            paragraphMarkRunProperties.AppendChild(languages);
            paragraphProperties.AppendChild(paragraphMarkRunProperties);

            Run run = new Run();
            Text text = new Text { Space = SpaceProcessingModeValues.Preserve, Text = input };
            run.AppendChild(text);

            paragraph.AppendChild(paragraphProperties);
            paragraph.AppendChild(run);

            (element as OpenXmlCompositeElement)?.AppendChild(paragraph);
        }
        public static Paragraph CreateParagraphWithStyle(string styleVal, string input)
        {
            var paragraph = new Paragraph();

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId = new ParagraphStyleId() { Val = styleVal };
            paragraphProperties.AppendChild(paragraphStyleId);

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            Languages languages = new Languages { Val = "en-US" };
            paragraphMarkRunProperties.AppendChild(languages);
            paragraphProperties.AppendChild(paragraphMarkRunProperties);

            Run run = new Run();
            Text text = new Text { Space = SpaceProcessingModeValues.Preserve, Text = input };
            run.AppendChild(text);

            paragraph.AppendChild(paragraphProperties);
            paragraph.AppendChild(run);

            return paragraph;
        }
        public static Paragraph CreateParagraphWithTab(string input, bool addTab = false)
        {
            var paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            Run run = new Run();
            Text text = new Text { Text = input };
            run.AppendChild(text);
            if (addTab)
            {
                var indentation = new Indentation { Left = "720" };
                paragraphProperties.AppendChild(indentation);
            }
            paragraph.AppendChild(paragraphProperties);
            paragraph.AppendChild(run);
            return paragraph;
        }

        public static void parseTextForOpenXML(Run run, string textualData)
        {
            textualData = textualData ?? "";
            string[] newLineArray = { Environment.NewLine };
            string[] textArray = textualData.Split(newLineArray, StringSplitOptions.None);

            bool first = true;
            foreach (string line in textArray)
            {
                if (!first)
                {
                    run.Append(new Break());
                }

                first = false;

                Text txt = new Text();
                txt.Text = line;
                run.Append(txt);
            }
        }
    }
}