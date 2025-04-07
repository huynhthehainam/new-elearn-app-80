using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using eLearnApps.Models;
using eLearnApps.Valence;
using eLearnApps.ViewModel.Valence;
using iText.Kernel.XMP.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TimeZoneConverter;
using ExtendedProperties = DocumentFormat.OpenXml.ExtendedProperties;

namespace eLearnApps.Helpers
{
    // TODO: need to move this file to core
    public class ExtractQuestion
    {
        public long QuestionNumber { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
    }
    public class OpenXmlHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Unique Identifier prefix. openxml uses unique identifier to reference component
        /// </summary>
        private const string PrefixRid = "rId";

        /// <summary>
        /// Unique Identifier auto incrementid. openxml uses unique identifier to reference component
        /// </summary>
        private int autorId;
        private Regex _tagRegex = OpenXmlExtensions.TagRegex;
        private ValenceApi valenceApi;
        private readonly IConfiguration _configuration;
        public OpenXmlHelper(IConfiguration configuration)
        {
            valenceApi = new ValenceApi(configuration);
            _configuration = configuration;
        }

        private string GetRID()
        {
            autorId += 1;
            var rid = $"{PrefixRid}{autorId}";
            return rid;
        }

        public void CreateDoc(MemoryStream ms, List<UserResponse> userResponses, List<ExtractQuestion> fullQuestions, ExtractionInfoModel options)
        {
            using (WordprocessingDocument myDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
            {
                CreateParts(myDoc, userResponses, fullQuestions, options);
            }
        }

        public void CreateParts(WordprocessingDocument document, List<UserResponse> userResponses, List<ExtractQuestion> fullQuestions, ExtractionInfoModel options)
        {
            //This part generate the document.
            MainDocumentPart mainDocumentPart1 = document.AddMainDocumentPart();
            GenerateMainDocumentPart1Content(mainDocumentPart1, userResponses, fullQuestions, options);

            //Some settings for the document.
            var docSettingId = GetRID();
            DocumentSettingsPart documentSettingsPart1 = mainDocumentPart1.AddNewPart<DocumentSettingsPart>(docSettingId);
            GenerateDocumentSettingsPart1Content(documentSettingsPart1);
        }

        public void GenerateFooterPart1Content(FooterPart footerPart1)
        {
            Footer footer1 = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            footer1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "006F1606", RsidRunAdditionDefault = "00C55D79" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Justification justification1 = new Justification() { Val = JustificationValues.Right };

            paragraphProperties1.Append(justification1);

            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = "Page";

            run1.Append(text1);

            Run run2 = new Run() { RsidRunAddition = "00661845" };
            Text text2 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text2.Text = " ";

            run2.Append(text2);

            Run run3 = new Run();
            FieldChar fieldChar1 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

            run3.Append(fieldChar1);

            Run run4 = new Run();
            FieldCode fieldCode1 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            fieldCode1.Text = "PAGE ";

            run4.Append(fieldCode1);

            Run run5 = new Run();
            FieldChar fieldChar2 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

            run5.Append(fieldChar2);

            Run run6 = new Run() { RsidRunAddition = "00661845" };

            RunProperties runProperties1 = new RunProperties();
            NoProof noProof1 = new NoProof();

            runProperties1.Append(noProof1);
            Text text3 = new Text();
            text3.Text = "7";

            run6.Append(runProperties1);
            run6.Append(text3);

            Run run7 = new Run();
            FieldChar fieldChar3 = new FieldChar() { FieldCharType = FieldCharValues.End };

            run7.Append(fieldChar3);

            Run run8 = new Run() { RsidRunAddition = "00661845" };
            Text text4 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text4.Text = " ";

            run8.Append(text4);

            Run run9 = new Run();
            Text text5 = new Text();
            text5.Text = "of";

            run9.Append(text5);

            Run run10 = new Run() { RsidRunAddition = "00661845" };
            Text text6 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text6.Text = " ";

            run10.Append(text6);

            Run run11 = new Run();
            FieldChar fieldChar4 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

            run11.Append(fieldChar4);

            Run run12 = new Run();
            FieldCode fieldCode2 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            fieldCode2.Text = "NUMPAGES  ";

            run12.Append(fieldCode2);

            Run run13 = new Run();
            FieldChar fieldChar5 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

            run13.Append(fieldChar5);

            Run run14 = new Run() { RsidRunAddition = "00661845" };

            RunProperties runProperties2 = new RunProperties();
            NoProof noProof2 = new NoProof();

            runProperties2.Append(noProof2);
            Text text7 = new Text();
            text7.Text = "10";

            run14.Append(runProperties2);
            run14.Append(text7);

            Run run15 = new Run();
            FieldChar fieldChar6 = new FieldChar() { FieldCharType = FieldCharValues.End };

            run15.Append(fieldChar6);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            paragraph1.Append(run2);
            paragraph1.Append(run3);
            paragraph1.Append(run4);
            paragraph1.Append(run5);
            paragraph1.Append(run6);
            paragraph1.Append(run7);
            paragraph1.Append(run8);
            paragraph1.Append(run9);
            paragraph1.Append(run10);
            paragraph1.Append(run11);
            paragraph1.Append(run12);
            paragraph1.Append(run13);
            paragraph1.Append(run14);
            paragraph1.Append(run15);

            footer1.Append(paragraph1);

            footerPart1.Footer = footer1;

        }

        public void GenerateDocumentSettingsPart1Content(DocumentSettingsPart documentSettingsPart1)
        {
            Settings settings1 = new Settings() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14" } };
            settings1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            settings1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            settings1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            settings1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            settings1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            settings1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            settings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            settings1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            settings1.AddNamespaceDeclaration("sl", "http://schemas.openxmlformats.org/schemaLibrary/2006/main");
            Zoom zoom1 = new Zoom() { Percent = "100" };

            //Previously odd section break works correctly, however after adding page number, it didn't work as expected.
            //Mirror margin so that when printing will leave empty page and that new section start on fresh paper.
            MirrorMargins mirrorMargins1 = new MirrorMargins();

            settings1.Append(zoom1);
            settings1.Append(mirrorMargins1);

            documentSettingsPart1.Settings = settings1;
        }
        // to collect user info once
        private object lockObject = new object();
        private Dictionary<Entity.Valence.OrgUnitUser, List<UserResponse>> studentResponseMap = new Dictionary<Entity.Valence.OrgUnitUser, List<UserResponse>>();
        private Dictionary<Entity.Valence.OrgUnitUser, Entity.Valence.UserData> studentUserInfoMap = new Dictionary<Entity.Valence.OrgUnitUser, Entity.Valence.UserData>();
        private Task CollectUserInfo(Entity.Valence.OrgUnitUser s, List<UserResponse> userResponses)
        {
            return Task.Run(() =>
            {

                var responseByStudent = userResponses.Where(x => x.StudentId == Convert.ToInt32(s.User.Identifier)).ToList();
                var info = valenceApi.GetUserInfo(int.Parse(s.User.Identifier));
                lock (lockObject)
                {
                    studentResponseMap[s] = responseByStudent;
                    studentUserInfoMap[s] = info;
                }
            });
        }
        public void GenerateMainDocumentPart1Content(
            MainDocumentPart mainDocumentPart1,
            List<UserResponse> userResponses,
            List<ExtractQuestion> fullQuestions,
            ExtractionInfoModel options)
        {
            Document document1 = new Document();
            Body body1 = new Body();

            var flattenedSection = new List<Entity.Valence.OrgUnitUser>();

            foreach (var section in options.Sections)
            {
                var classlist = section.Classlist;
                flattenedSection.AddRange(classlist);
            }
            try
            {
                //to check for any single tag. i.e. <b> (it doesn't have to be closed). This will determine whether HTML or non-HTML content
                Regex tagRegex = new Regex(@"<[^>]+>");
                //Regex matchingTagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
                //Regex matchingTagRegex = _tagRegex;

                int i = 0; //row count
                int j = 0; //to cater for quiz with multiple attempts

                if (!string.IsNullOrEmpty(options.GptZeroTitle))
                {
                    body1.Append(CreateGptZeroTitle(options.GptZeroTitle));
                }

                var tasks = flattenedSection.Select(s => CollectUserInfo(s, userResponses)).ToArray();
                Task.WaitAll(tasks);
                flattenedSection = flattenedSection.Where(e => !options.SectionId.HasValue || e.User.GetSectionId() == options.SectionId.Value).OrderByDescending(e =>
                {
                    var responseByStudent = studentResponseMap[e];
                    return responseByStudent.Any() ? 1 : 0;
                }).ThenBy(e => e.User.StudentSectionNameNumber).ToList();
                var keys1 = studentResponseMap.Keys.Except(studentUserInfoMap.Keys).ToList();
                foreach (var selectedStudent in flattenedSection)
                {
                    var isNonSubmittedStudent = false;
                    var responseByStudent = studentResponseMap[selectedStudent];
                    if (!responseByStudent.Any())
                    {
                        isNonSubmittedStudent = true;
                    }
                    if (responseByStudent.Count > 0)
                    {
                        foreach (var userResponse in responseByStudent)
                        {
                            var student = selectedStudent;
                            string nric = ToSafeString(student.User.OrgDefinedId);
                            string studName = student.User.DisplayName;

                            // Get UTC dates and add in the offset
                            var utcTimeStarted = userResponse.TimeStarted.UtcDateTime;
                            var utcTimeCompleted = userResponse.TimeCompleted.UtcDateTime;
                            var timeStarted = utcTimeStarted + TimeZoneInfo.Local.GetUtcOffset(userResponse.TimeStarted);
                            var timeCompleted = utcTimeCompleted + TimeZoneInfo.Local.GetUtcOffset(userResponse.TimeCompleted);

                            string tz = TZConvert.IanaToWindows(options.ClientTimezone);
                            var clientTz = TimeZoneInfo.FindSystemTimeZoneById(tz);

                            DateTime localTimeStarted = TimeZoneInfo.ConvertTime(utcTimeStarted, clientTz);
                            DateTime localTimeCompleted = TimeZoneInfo.ConvertTime(utcTimeCompleted, clientTz);

                            string timestarted = localTimeStarted.ToString("dd-MMM-yyyy hh:mm:ss tt");
                            string timecompleted = localTimeCompleted.ToString("dd-MMM-yyyy hh:mm:ss tt");

                            var duration = userResponse.TimeCompleted - userResponse.TimeStarted;
                            string timediff = $"{duration.Days} days, {duration.Hours} hours, {duration.Minutes} minutes, {duration.Seconds} secs";
                            string quizName = userResponse.QuizName;

                            if (timeCompleted.Year == 1)
                            {
                                timecompleted = "In Progress";
                                timediff = "";
                            }

                            j++;
                            GenerateFooterPart2Content(mainDocumentPart1, nric, j);

                            if (options.IsStudentNameShown)
                            {
                                var userInfo = studentUserInfoMap[selectedStudent];
                                body1.Append(CreateAnsHeader(options.CourseCode, quizName.Trim(), nric, timestarted + " – " + timecompleted, timediff, studName, student.User.GetSectionName(), userInfo.UserName));
                            }
                            else
                            {
                                body1.Append(CreateAnsHeader(options.CourseCode, quizName.Trim(), nric, timestarted + " – " + timecompleted, timediff, student.User.GetSectionName()));
                            }

                            var responses = userResponse.Responses.Where(x => options.QuestionTypes.Contains(x.QuestionType)).ToList();


                            foreach (var question in fullQuestions)
                            {
                                var questionType = question.QuestionType;
                                var questionNumber = question.QuestionNumber;
                                var qnText = question.QuestionText;

                                var lineSpacing = (decimal.Parse(options.LineSpacing) * 240).ToString();
                                var fontName = "arial";
                                var font = fontName;
                                var fontSize = options.FontSize;

                                if (options.IsQuestionShown)
                                {
                                    body1.Append(CreateParagraph("Question:", fontName, fontSize, lineSpacing));
                                    body1.Append(CreateAltChunk(mainDocumentPart1, qnText));
                                }
                                else
                                {
                                    body1.Append(CreateParagraph($"Question {questionNumber}:", fontName, fontSize, lineSpacing));
                                }
                                // href attribute is generated differently in 2 sources so we need a special comparison rule here
                                var response = responses.Where(e => OpenXmlExtensions.TextCompareWithHrefRule(e.QuestionText, question.QuestionText)
                                       && string.Equals(e.QuestionType, question.QuestionType, StringComparison.OrdinalIgnoreCase)).ToList();
                                // show answer number only when there are more than 1 answer per question (FIB answer)
                                var showAnswerNumber = response.Count > 1 ? true : false;
                                int answerIdx = 1;
                                if (response.Count > 0)
                                {
                                    foreach (var qnAns in response)
                                    {
                                        if (showAnswerNumber)
                                            body1.Append(CreateAltChunk(mainDocumentPart1, $"Answer {answerIdx}:"));
                                        else body1.Append(CreateAltChunk(mainDocumentPart1, $"Answer:"));

                                        if (OpenXmlExtensions.HighlightRegex.IsMatch(qnAns.TextResponse))
                                            body1.Append(CreateResponseAltChunk(mainDocumentPart1, qnAns.TextResponse));
                                        else
                                            body1.Append(CreateParagraph(qnAns.TextResponse, fontName, fontSize, lineSpacing));

                                        // increment answer index
                                        answerIdx += 1;
                                    }
                                }
                                else
                                {
                                    body1.Append(CreateAltChunk(mainDocumentPart1, $"Answer:"));
                                    body1.Append(CreateAltChunk(mainDocumentPart1, $"[{_configuration.GetValue<string>("NoAnswerSysText")}]"));
                                }
                            }

                            i++;
                            if (userResponse == responseByStudent.Last() && selectedStudent == flattenedSection.Last())
                            {
                                body1.Append(CreateEndSectPropertiesForNRICFooter(nric, j));
                            }
                            else
                                body1.Append(CreateSectionBreak(nric, j)); //section break after every student.
                        }

                    }
                    else
                    {
                        if (isNonSubmittedStudent)
                        {
                            j++;
                            body1.Append(CreateStudentHeader(selectedStudent, options.CourseCode, options.QuizName, options.IsStudentNameShown));
                            body1.Append(CreateAltChunk(mainDocumentPart1, $"[{_configuration.GetValue<string>("NonSubmittedSysText")}]"));
                            var nric = string.IsNullOrEmpty(selectedStudent.User.OrgDefinedId) ? selectedStudent.User.DisplayName.Replace(" ", "") : ToSafeString(selectedStudent.User.OrgDefinedId);
                            GenerateFooterPart2Content(mainDocumentPart1, nric, j);

                            if (selectedStudent != flattenedSection.Last())
                            {
                                body1.Append(CreateSectionBreak(nric, j));
                            }
                            else
                            {
                                body1.Append(CreateEndSectPropertiesForNRICFooter(nric, j));
                            }
                        }
                        else
                        {
                            var message = options.SectionId == null ? $"[No student submitted for Quiz {options.QuizName} in all sections.]" : $"[No student submitted for Quiz {options.QuizName} in section {options.SectionName}.]";
                            body1.Append(CreateAltChunk(mainDocumentPart1, message));
                        }
                    }
                }

                document1.Append(body1);
                mainDocumentPart1.Document = document1;
            }
            catch (Exception ex)
            {
                log.Error("Encounter errors", ex);
                // Gazza: Find out how to add to errorlog in elearnApps
                //LmsLogManager.AddErrorLog(ex.Message, ex.StackTrace);
            }
        }

        public Paragraph CreatePgBreak(int isLastPgBreakParagraph)
        {
            Paragraph paragraph = new Paragraph() { };
            ParagraphProperties paragraphProperties = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            RunFonts runFonts = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" };
            DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize = new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "8" };
            FontSizeComplexScript fontSizeComplexScript = new FontSizeComplexScript() { Val = "8" };

            paragraphMarkRunProperties.Append(runFonts);
            paragraphMarkRunProperties.Append(fontSize);
            paragraphMarkRunProperties.Append(fontSizeComplexScript);

            paragraphProperties.Append(paragraphMarkRunProperties);

            Run run = new Run();

            RunProperties runProperties = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" };
            DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize1 = new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "8" };
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "8" };

            runProperties.Append(runFonts1);
            runProperties.Append(fontSize1);
            runProperties.Append(fontSizeComplexScript1);
            Break breakPg = new Break() { Type = BreakValues.Page };
            LastRenderedPageBreak lastRenderedPageBreak1 = new LastRenderedPageBreak();

            run.Append(runProperties);
            if (isLastPgBreakParagraph == (int)OpenXmlHelper.PageBreak.LastParagraph)
                run.Append(lastRenderedPageBreak1);
            else
                run.Append(breakPg);

            paragraph.Append(paragraphProperties);
            paragraph.Append(run);

            if (isLastPgBreakParagraph == (int)OpenXmlHelper.PageBreak.LastParagraph)
            {
                BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_GoBack", Id = "0" };
                BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "0" };
                paragraph.Append(bookmarkStart1);
                paragraph.Append(bookmarkEnd1);
            }

            return paragraph;
        }

        public Paragraph CreateSectionBreak(String refNRIC, int runningNum)
        {
            Paragraph paragraph = new Paragraph() { };
            ParagraphProperties paragraphProperties = new ParagraphProperties();

            SectionProperties sectionProperties1 = new SectionProperties() { };
            FooterReference footerReference1 = new FooterReference() { Type = HeaderFooterValues.Default, Id = "rID" + String.Concat(refNRIC, runningNum) };
            SectionType sectionType1 = new SectionType() { Val = SectionMarkValues.OddPage };
            sectionProperties1.Append(footerReference1);
            sectionProperties1.Append(sectionType1);
            paragraphProperties.Append(sectionProperties1);

            paragraph.Append(paragraphProperties);

            return paragraph;
        }

        public SectionProperties CreateEndSectPropertiesForNRICFooter(String refNRIC, int runningNum)
        {
            SectionProperties sectionProperties1 = new SectionProperties();
            FooterReference footerReference1 = new FooterReference() { Type = HeaderFooterValues.Default, Id = "rID" + String.Concat(refNRIC, runningNum) };
            SectionType sectionType1 = new SectionType() { Val = SectionMarkValues.OddPage };

            sectionProperties1.Append(footerReference1);
            sectionProperties1.Append(sectionType1);

            return sectionProperties1;
        }

        /// <summary>
        /// Create ending section properties.
        /// </summary>
        /// <returns></returns>
        public SectionProperties CreateEndSectProperties()
        {
            var footerId = GetRID();
            SectionProperties sectionProperties1 = new SectionProperties();
            FooterReference footerReference1 = new FooterReference() { Type = HeaderFooterValues.Default, Id = footerId };
            SectionType sectionType1 = new SectionType() { Val = SectionMarkValues.OddPage };

            sectionProperties1.Append(footerReference1);
            sectionProperties1.Append(sectionType1);

            return sectionProperties1;
        }

        /// <summary>
        /// Create Answer Header without name.
        /// </summary>
        /// <param name="CourseCode"></param>
        /// <param name="QuizName"></param>
        /// <param name="NRIC"></param>
        /// <param name="DateRange"></param>
        /// <param name="TimeSpent"></param>
        /// <returns></returns>
        public Paragraph CreateAnsHeader(String CourseCode, String QuizName, String NRIC, String DateRange, String TimeSpent, string sectionName)
        {
            Paragraph paragraph = new Paragraph() { };
            ParagraphProperties paragraphProperties = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            RunFonts runFonts = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            paragraphMarkRunProperties.Append(runFonts);

            paragraphProperties.Append(paragraphMarkRunProperties);


            Run run = new Run() { };

            RunProperties runProperties = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            runProperties.Append(runFonts1);

            run.Append(runProperties);

            var kvp = new Dictionary<string, string>
            {
                { "Course Code:", CourseCode },
                { "Section:", sectionName },
                { "Quiz Name:", QuizName },
                { "Campus ID:", NRIC },
                { "Date Range:", DateRange },
                { "Total Time Spent:", TimeSpent }
            };

            paragraph.Append(paragraphProperties);

            // Create an empty table.
            Table table = new Table();

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = new TableProperties();

            // Append the TableProperties object to the empty table.
            table.AppendChild<TableProperties>(tblProp);

            foreach (var key in kvp.Keys)
            {
                var row = new TableRow();

                var label = new TableCell();
                label.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2000" }));
                label.Append(new Paragraph(new Run(new Text(key))));
                row.Append(label);

                var value = new TableCell();
                value.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5000" }));
                value.Append(new Paragraph(new Run(new Text($"     {kvp[key]}"))));
                row.Append(value);

                //var trh = row.TableRowProperties.OfType<TableRowHeight>().FirstOrDefault();
                //trh.Val = 1;

                table.Append(row);
            }
            run.AppendChild(table);

            paragraph.Append(run);

            return paragraph;
        }

        /// <summary>
        /// Create Answer Header with name.
        /// Header consist of Course Code, Quiz Name, NRIC, Date Range, Time Spent, Student Name
        /// </summary>
        /// <param name="CourseCode"></param>
        /// <param name="QuizName"></param>
        /// <param name="NRIC"></param>
        /// <param name="DateRange"></param>
        /// <param name="TimeSpent"></param>
        /// <param name="StudentName"></param>
        /// <returns>Paragraph created</returns>
        public Paragraph CreateAnsHeader(String CourseCode, String QuizName, String NRIC, String DateRange, String TimeSpent, String StudentName, string sectionName, string Username)
        {
            Paragraph paragraph = new Paragraph() { };
            ParagraphProperties paragraphProperties = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            RunFonts runFonts = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            paragraphMarkRunProperties.Append(runFonts);

            paragraphProperties.Append(paragraphMarkRunProperties);


            Run run = new Run() { };

            RunProperties runProperties = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            runProperties.Append(runFonts1);

            run.Append(runProperties);

            var kvp = new Dictionary<string, string>
            {
                { "Course Code:", CourseCode },
                { "Section:", sectionName },
                { "Quiz Name:", QuizName },
                { "Name:", StudentName },
                { "Username:", Username },
                { "Campus ID:", NRIC },
                { "Date Range:", DateRange },
                { "Total Time Spent:", TimeSpent }
            };

            paragraph.Append(paragraphProperties);

            // Create an empty table.
            Table table = new Table();

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = new TableProperties();

            // Append the TableProperties object to the empty table.
            table.AppendChild<TableProperties>(tblProp);

            foreach (var key in kvp.Keys)
            {
                var row = new TableRow();

                var label = new TableCell();
                label.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2000" }));
                label.Append(new Paragraph(new Run(new Text(key))));
                row.Append(label);

                var value = new TableCell();
                value.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5000" }));
                value.Append(new Paragraph(new Run(new Text($"     {kvp[key]}"))));
                row.Append(value);

                //var trh = row.TableRowProperties.OfType<TableRowHeight>().FirstOrDefault();
                //trh.Val = 1;

                table.Append(row);
            }

            run.AppendChild(table);

            paragraph.Append(run);

            return paragraph;
        }
        private Paragraph CreateStudentHeader(Entity.Valence.OrgUnitUser student, string courseCode, string quizName, bool isShowStudentName)
        {
            var userInfo = studentUserInfoMap[student];
            Paragraph paragraph = new Paragraph() { };
            ParagraphProperties paragraphProperties = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            RunFonts runFonts = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            paragraphMarkRunProperties.Append(runFonts);

            paragraphProperties.Append(paragraphMarkRunProperties);


            Run run = new Run() { };

            RunProperties runProperties = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            runProperties.Append(runFonts1);

            run.Append(runProperties);

            Dictionary<string, string> kvp = null;

            if (isShowStudentName)
            {
                kvp = new Dictionary<string, string>
                {
                    { "Course Code:", courseCode },
                    { "Section:", student.User.GetSectionName() },
                    { "Quiz Name:", quizName },
                    { "Name:", student.User.DisplayName },
                    { "Username:", userInfo.UserName },
                    { "Campus ID:", ToSafeString(student.User.OrgDefinedId) },
                };
            }
            else
            {
                kvp = new Dictionary<string, string>
                {
                    { "Course Code:", courseCode },
                    { "Section:", student.User.GetSectionName() },
                    { "Quiz Name:", quizName },
                    { "Campus ID:", ToSafeString(student.User.OrgDefinedId) },
                };
            }

            paragraph.Append(paragraphProperties);

            // Create an empty table.
            Table table = new Table();

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = new TableProperties();

            // Append the TableProperties object to the empty table.
            table.AppendChild<TableProperties>(tblProp);

            foreach (var key in kvp.Keys)
            {
                var row = new TableRow();

                var label = new TableCell();
                label.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2000" }));
                label.Append(new Paragraph(new Run(new Text(key))));
                row.Append(label);

                var value = new TableCell();
                value.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5000" }));
                value.Append(new Paragraph(new Run(new Text($"     {kvp[key]}"))));
                row.Append(value);

                //var trh = row.TableRowProperties.OfType<TableRowHeight>().FirstOrDefault();
                //trh.Val = 1;

                table.Append(row);
            }

            run.AppendChild(table);

            paragraph.Append(run);

            return paragraph;
        }
        /// <summary>
        /// Create AltChunk for html input.
        /// </summary>
        /// <param name="mainDocumentPart1"></param>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public AltChunk CreateAltChunk(MainDocumentPart mainDocumentPart1, String htmlText)
        {
            htmlText = OpenXmlExtensions.EncodeWordCharacter(htmlText);
            AlternativeFormatImportPart formatImportPart = mainDocumentPart1.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html);

            MemoryStream ms1 = new MemoryStream(Encoding.UTF8.GetBytes("<html><head></head><body>" + htmlText + "</body></html>"));

            formatImportPart.FeedData(ms1);

            AltChunk altChunk = new AltChunk();
            altChunk.Id = mainDocumentPart1.GetIdOfPart(formatImportPart);

            return altChunk;
        }
        public AltChunk CreateResponseAltChunk(MainDocumentPart mainDocumentPart1, String htmlText)
        {

            //htmlText = OpenXmlExtensions.ConvertHtmlText(htmlText);

            AlternativeFormatImportPart formatImportPart = mainDocumentPart1.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html);

            MemoryStream ms1 = new MemoryStream(Encoding.UTF8.GetBytes("<html><head></head><body>" + htmlText + "</body></html>"));

            formatImportPart.FeedData(ms1);

            AltChunk altChunk = new AltChunk();
            altChunk.Id = mainDocumentPart1.GetIdOfPart(formatImportPart);

            return altChunk;
        }


        /// <summary>
        /// Create paragraph for normal text. Uses parseTextForOpenXML() which replace \r\n accordingly.
        /// </summary>
        /// <param name="mainDocumentPart1"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <param name="LineSpacing"></param>
        /// <returns></returns>
        public Paragraph CreateParagraph(String text, String font, String fontSize, String LineSpacing)
        {
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { Line = LineSpacing, LineRule = LineSpacingRuleValues.Auto };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = font, HighAnsi = font, ComplexScript = font };
            DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize1 = new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = fontSize };
            FontSizeComplexScript fontSizeComplexScript3 = new FontSizeComplexScript() { Val = fontSize };

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript3);

            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run_paragraph = new Run();

            paragraph.Append(paragraphProperties1);
            paragraph.Append(parseTextForOpenXML(run_paragraph, text, font, fontSize));

            return paragraph;
        }

        /// <summary>
        /// For those text that has
        /// </summary>
        /// <param name="run"></param>
        /// <param name="textualData"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public Run parseTextForOpenXML(Run run, string textualData, String font, String fontSize)
        {
            string[] newLineArray = { Environment.NewLine };
            string[] textArray = textualData.Split(newLineArray, StringSplitOptions.None);

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = font, HighAnsi = font, ComplexScript = font };

            DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize1 = new DocumentFormat.OpenXml.Wordprocessing.FontSize()
            {
                Val = (decimal.Parse(fontSize) * 2).ToString()
            };


            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = fontSize };

            runProperties1.Append(runFonts1);
            runProperties1.Append(fontSize1);
            runProperties1.Append(fontSizeComplexScript1);

            run.Append(runProperties1);

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
            return run;
        }

        /// <summary>
        /// For footer with NRIC on left and page number on right.
        /// </summary>
        /// <param name="mainDocumentPart1"></param>
        /// <param name="refNRIC"></param>
        /// <param name="runningNum"></param>
        public void GenerateFooterPart2Content(MainDocumentPart mainDocumentPart1, String refNRIC, int runningNum)
        {
            FooterPart footerPart1 = mainDocumentPart1.AddNewPart<FooterPart>("rID" + String.Concat(refNRIC, runningNum));

            Footer footer1 = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            footer1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00254DA7", RsidParagraphProperties = "00FA265D", RsidRunAdditionDefault = "00FA265D" };

            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = refNRIC;

            run1.Append(text1);

            Run run2 = new Run();
            TabChar tabChar1 = new TabChar();

            run2.Append(tabChar1);

            Run run3 = new Run();
            TabChar tabChar2 = new TabChar();

            run3.Append(tabChar2);

            Run run4 = new Run();
            TabChar tabChar3 = new TabChar();

            run4.Append(tabChar3);

            Run run5 = new Run();
            TabChar tabChar4 = new TabChar();

            run5.Append(tabChar4);

            Run run6 = new Run();
            TabChar tabChar5 = new TabChar();

            run6.Append(tabChar5);

            Run run7 = new Run();
            TabChar tabChar6 = new TabChar();

            run7.Append(tabChar6);

            Run run8 = new Run();
            TabChar tabChar7 = new TabChar();

            run8.Append(tabChar7);

            Run run9 = new Run();
            TabChar tabChar8 = new TabChar();

            run9.Append(tabChar8);

            Run run10 = new Run();
            TabChar tabChar9 = new TabChar();

            run10.Append(tabChar9);

            Run run11 = new Run();
            TabChar tabChar10 = new TabChar();
            //Text text2 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            //text2.Text = " ";

            run11.Append(tabChar10);
            //run11.Append(text2);

            Run run12 = new Run() { RsidRunAddition = "00252814" };
            Text text3 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text3.Text = "Page ";

            run12.Append(text3);

            Run run13 = new Run() { RsidRunAddition = "00252814" };
            FieldChar fieldChar1 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

            run13.Append(fieldChar1);

            Run run14 = new Run() { RsidRunAddition = "00252814" };
            FieldCode fieldCode1 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            fieldCode1.Text = "PAGE ";

            run14.Append(fieldCode1);

            Run run15 = new Run() { RsidRunAddition = "00252814" };
            FieldChar fieldChar2 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

            run15.Append(fieldChar2);

            Run run16 = new Run();

            RunProperties runProperties1 = new RunProperties();
            NoProof noProof1 = new NoProof();

            runProperties1.Append(noProof1);
            Text text4 = new Text();
            text4.Text = "2";

            run16.Append(runProperties1);
            run16.Append(text4);

            Run run17 = new Run() { RsidRunAddition = "00252814" };
            FieldChar fieldChar3 = new FieldChar() { FieldCharType = FieldCharValues.End };

            run17.Append(fieldChar3);

            Run run18 = new Run() { RsidRunAddition = "00252814" };
            Text text5 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text5.Text = " of ";

            run18.Append(text5);

            Run run19 = new Run() { RsidRunAddition = "00252814" };
            FieldChar fieldChar4 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

            run19.Append(fieldChar4);

            Run run20 = new Run() { RsidRunAddition = "00252814" };
            FieldCode fieldCode2 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            fieldCode2.Text = "NUMPAGES  ";

            run20.Append(fieldCode2);

            Run run21 = new Run() { RsidRunAddition = "00252814" };
            FieldChar fieldChar5 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

            run21.Append(fieldChar5);

            Run run22 = new Run();

            RunProperties runProperties2 = new RunProperties();
            NoProof noProof2 = new NoProof();

            runProperties2.Append(noProof2);
            Text text6 = new Text();
            text6.Text = "6";

            run22.Append(runProperties2);
            run22.Append(text6);

            Run run23 = new Run() { RsidRunAddition = "00252814" };
            FieldChar fieldChar6 = new FieldChar() { FieldCharType = FieldCharValues.End };

            run23.Append(fieldChar6);

            paragraph1.Append(run1);
            paragraph1.Append(run2);
            paragraph1.Append(run3);
            paragraph1.Append(run4);
            paragraph1.Append(run5);
            paragraph1.Append(run6);
            paragraph1.Append(run7);
            paragraph1.Append(run8);
            paragraph1.Append(run9);
            paragraph1.Append(run10);
            paragraph1.Append(run11);
            paragraph1.Append(run12);
            paragraph1.Append(run13);
            paragraph1.Append(run14);
            paragraph1.Append(run15);
            paragraph1.Append(run16);
            paragraph1.Append(run17);
            paragraph1.Append(run18);
            paragraph1.Append(run19);
            paragraph1.Append(run20);
            paragraph1.Append(run21);
            paragraph1.Append(run22);
            paragraph1.Append(run23);

            footer1.Append(paragraph1);

            footerPart1.Footer = footer1;

        }

        public void SetContentType(System.Web.HttpRequest request, System.Web.HttpResponse resp, string docName)
        {
            if (docName.Length > 128)
                docName = docName.Substring(0, 128);
            StringBuilder sb = new StringBuilder(docName);

            string encodedDocName =
                sb
                  .Replace(';', ' ')
                  .Replace('"', ' ')
                  .Replace('/', ' ')
                  .ToString();

            if (request.Browser.Browser.Contains("IE"))
            {
                encodedDocName = Uri.EscapeDataString(Path.GetFileNameWithoutExtension(encodedDocName)).Replace("%20", " ") +
                    Path.GetExtension(encodedDocName);
            }

            resp.AppendHeader("Content-Disposition", "attachment;filename=\"" + encodedDocName + "\"");
            resp.ContentType = "application/vnd.ms-word.document";

            if (!(request.IsSecureConnection && request.Browser.Browser.Contains("IE")))
                resp.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        }

        public string StripPTag(string input)
        {
            return input
                    .Replace("<p>", string.Empty)
                    .Replace("</p>", string.Empty);
        }

        public enum BlindMode
        {
            Blind = 0,
            NonBlind = 1 //print student name
        }

        public enum DisplayQnTxt
        {
            Yes = 0,
            No = 1
        }

        /* For reference
        public enum FontSize
        {
            //OpenXML values
            Font10 = 20,
            Font11 = 22,
            Font12 = 24,
            Font14 = 28
        }

        public enum LineSpacing
        {
            //OpenXML values
            Line1_00 = 240,
            Line1_15 = 276,
            Line1_5 = 360,
            Line2_00 = 480
        }
        */

        public enum PageBreak
        {
            NotLastParagraph = 0,
            LastParagraph = 1
        }
        private string ToSafeString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            // remove invisible unicode character. it will cause breakage
            var regex = new Regex(@"[\p{Cc}\p{Cf}\p{Mn}\p{Me}\p{Zl}\p{Zp}]");
            text = regex.Replace(text, "");

            // max filename lenght is 128
            if (text.Length > 128)
                text = text.Substring(0, 128);
            StringBuilder sb = new StringBuilder(text);

            // replace unsave char with underscore
            string encodedDocName =
                sb
                  .Replace(';', '_')
                  .Replace('"', '_')
                  .Replace('/', '_')
                  .Replace(' ', '_')
                  .Replace(',', '_')
                  .ToString();

            return encodedDocName;
        }
        public Paragraph CreateGptZeroTitle(string title)
        {
            Paragraph paragraph = new Paragraph() { RsidParagraphAddition = "007A584A", RsidRunAdditionDefault = "007A584A" };

            ParagraphProperties paragraphProperties = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
            RunFonts runFonts = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };

            paragraphMarkRunProperties.Append(runFonts);

            paragraphProperties.Append(paragraphMarkRunProperties);

            paragraph.Append(paragraphProperties);

            Paragraph paragraphGtpTitle = new Paragraph() { RsidParagraphAddition = "00C36651", RsidParagraphProperties = "00C36651", RsidRunAdditionDefault = "00953993" };

            ParagraphProperties paragraphPropertiesCenter = new ParagraphProperties();
            Justification justificationCenter = new Justification() { Val = JustificationValues.Both };

            ParagraphMarkRunProperties paragraphMarkRunProperties10 = new ParagraphMarkRunProperties();
            RunFonts runFonts60 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
            FontSize fontSize6 = new FontSize() { Val = "30" };
            FontSizeComplexScript fontSizeComplexScript7 = new FontSizeComplexScript() { Val = "30" };

            paragraphMarkRunProperties10.Append(runFonts60);
            paragraphMarkRunProperties10.Append(fontSize6);
            paragraphMarkRunProperties10.Append(fontSizeComplexScript7);

            paragraphPropertiesCenter.Append(justificationCenter);
            paragraphPropertiesCenter.Append(paragraphMarkRunProperties10);

            Run run76 = new Run() { RsidRunProperties = "00C36651" };

            RunProperties runProperties51 = new RunProperties();
            RunFonts runFonts61 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
            FontSize fontSize7 = new FontSize() { Val = "30" };
            FontSizeComplexScript fontSizeComplexScript8 = new FontSizeComplexScript() { Val = "30" };
            Highlight highlight3 = new Highlight() { Val = HighlightColorValues.Yellow };

            runProperties51.Append(runFonts61);
            runProperties51.Append(fontSize7);
            runProperties51.Append(fontSizeComplexScript8);
            runProperties51.Append(highlight3);
            Text text64 = new Text();
            text64.Text = title;

            run76.Append(runProperties51);
            run76.Append(text64);
            run76.AppendChild(new Break());
            paragraphGtpTitle.Append(paragraphPropertiesCenter);
            paragraphGtpTitle.Append(run76);

            return paragraphGtpTitle;
        }
    }
}