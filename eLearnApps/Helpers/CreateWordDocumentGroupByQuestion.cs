using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using eLearnApps.ViewModel.Valence;
using eLearnApps.Models;
using eLearnApps.Valence;
using OpenXmlPowerTools;
using TimeZoneConverter;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using HtmlAgilityPack;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using eLearnApps.Core;
using SectionType = DocumentFormat.OpenXml.Wordprocessing.SectionType;
using eLearnApps.ViewModel.EE;
using System.Text;
using ExtendedProperties = DocumentFormat.OpenXml.ExtendedProperties;
using eLearnApps.Entity.LmsTools;
using PagedList;
using System.Threading.Tasks;

namespace eLearnApps.Helpers
{
    public class CreateWordDocumentGroupByQuestion
    {
        /// <summary>
        /// Unique Identifier prefix. openxml uses unique identifier to reference component
        /// </summary>
        private const string PrefixRid = "rId";

        /// <summary>
        /// Unique Identifier auto incrementid. openxml uses unique identifier to reference component
        /// </summary>
        private int autorId;

        private List<Entity.Valence.OrgUnitUser> _classLists;
        private readonly string _fontSize;
        private readonly string _lineSpace;
        private readonly Regex _matchingTagRegex;

        private readonly ExtractionInfoModel _options;

        /// <summary>
        /// to check for any single tag. i.e. <b> (it doesn't have to be closed). 
        /// This will determine whether HTML or non-HTML content
        /// </summary>
        private readonly Regex _tagRegex = OpenXmlExtensions.TagRegex;
        private readonly List<UserResponse> _userResponses;
        private Body _body;
        private Document _document;
        private MainDocumentPart _mainPart;
        private Dictionary<int, string> _dicStudent;
        private readonly ValenceApi _valenceApi;
        private List<ExtractQuestion> _fullQuestions;
        private readonly IConfiguration _configuration;
        private readonly Constants _constants;
        private string _footerrId;

        public CreateWordDocumentGroupByQuestion(List<UserResponse> userResponses, ExtractionInfoModel options, List<ExtractQuestion> fullQuestions, IConfiguration configuration)
        {
            _classLists = new List<Entity.Valence.OrgUnitUser>();
            _document = new Document();
            _body = new Body();
            _userResponses = userResponses;
            _options = options;
            _fontSize = _options.FontSize;
            _lineSpace = (decimal.Parse(_options.LineSpacing) * 240).ToString();
            _matchingTagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            _dicStudent = new Dictionary<int, string>();
            _valenceApi = new ValenceApi(configuration);
            _configuration = configuration;
            _constants = new Constants(configuration);
            autorId = 0;
            _fullQuestions = fullQuestions;
        }

        private string GetRID()
        {
            autorId += 1;
            var rid = $"{PrefixRid}{autorId}";
            return rid;
        }
        public void CreatePackage(MemoryStream ms)
        {
            try
            {
                using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    var rid = GetRID();
                    ExtendedFilePropertiesPart extendedFilePropertiesPart1 = doc.AddNewPart<ExtendedFilePropertiesPart>(rid);
                    GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

                    _mainPart = doc.AddMainDocumentPart();

                    // whole document uses 1 same footer that contains page number
                    // generated footer can be referenced using _footerrId
                    GenerateFooter();

                    GenerateExamSummaryPage(_options.GptZeroTitle);

                    GenerateQuestionAnswerPage();

                    _document.Append(_body);
                    _mainPart.Document = _document;
                    doc.MainDocumentPart.Document.Save();
                    doc.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _body = null;
                _mainPart = null;
                _document = null;
            }
        }

        private void GenerateExtendedFilePropertiesPart1Content(ExtendedFilePropertiesPart extendedFilePropertiesPart1)
        {
            ExtendedProperties.Properties properties1 = new ExtendedProperties.Properties();
            properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            ExtendedProperties.Template template1 = new ExtendedProperties.Template();
            template1.Text = "Normal.dotm";
            ExtendedProperties.TotalTime totalTime1 = new ExtendedProperties.TotalTime();
            totalTime1.Text = "1";
            ExtendedProperties.Pages pages1 = new ExtendedProperties.Pages();
            pages1.Text = "1";
            ExtendedProperties.Words words1 = new ExtendedProperties.Words();
            words1.Text = "0";
            ExtendedProperties.Characters characters1 = new ExtendedProperties.Characters();
            characters1.Text = "0";
            ExtendedProperties.Application application1 = new ExtendedProperties.Application();
            application1.Text = "Microsoft Office Word";
            ExtendedProperties.DocumentSecurity documentSecurity1 = new ExtendedProperties.DocumentSecurity();
            documentSecurity1.Text = "0";
            ExtendedProperties.Lines lines1 = new ExtendedProperties.Lines();
            lines1.Text = "0";
            ExtendedProperties.Paragraphs paragraphs1 = new ExtendedProperties.Paragraphs();
            paragraphs1.Text = "0";
            ExtendedProperties.ScaleCrop scaleCrop1 = new ExtendedProperties.ScaleCrop();
            scaleCrop1.Text = "false";
            ExtendedProperties.Company company1 = new ExtendedProperties.Company();
            company1.Text = "";
            ExtendedProperties.LinksUpToDate linksUpToDate1 = new ExtendedProperties.LinksUpToDate();
            linksUpToDate1.Text = "false";
            ExtendedProperties.CharactersWithSpaces charactersWithSpaces1 = new ExtendedProperties.CharactersWithSpaces();
            charactersWithSpaces1.Text = "0";
            ExtendedProperties.SharedDocument sharedDocument1 = new ExtendedProperties.SharedDocument();
            sharedDocument1.Text = "false";
            ExtendedProperties.HyperlinksChanged hyperlinksChanged1 = new ExtendedProperties.HyperlinksChanged();
            hyperlinksChanged1.Text = "false";
            ExtendedProperties.ApplicationVersion applicationVersion1 = new ExtendedProperties.ApplicationVersion();
            applicationVersion1.Text = "16.0000";

            properties1.Append(template1);
            properties1.Append(totalTime1);
            properties1.Append(pages1);
            properties1.Append(words1);
            properties1.Append(characters1);
            properties1.Append(application1);
            properties1.Append(documentSecurity1);
            properties1.Append(lines1);
            properties1.Append(paragraphs1);
            properties1.Append(scaleCrop1);
            properties1.Append(company1);
            properties1.Append(linksUpToDate1);
            properties1.Append(charactersWithSpaces1);
            properties1.Append(sharedDocument1);
            properties1.Append(hyperlinksChanged1);
            properties1.Append(applicationVersion1);

            extendedFilePropertiesPart1.Properties = properties1;
        }

        private void GenerateQuestionAnswerPage()
        {
            //var allQuestions = _fullQuestions.SelectMany(ur => ur.Responses)
            //    .OrderBy(r => r.QuestionNumber)
            //    .ThenBy(r => r.QuestionText)
            //    .Select(r => new QuestionViewModel(r.QuestionText, r.QuestionType))
            //    .Distinct()
            //    .ToList();
            var allQuestions = _fullQuestions
              .OrderBy(r => r.QuestionNumber)
              .ThenBy(r => r.QuestionText)
              .Select(r => new QuestionViewModel(r.QuestionText, r.QuestionType))
              .Distinct()
              .ToList();
            //var allQuestions = _fullQuestions;

            // we only export these 4 type of Text Questions
            if (_options.QuestionTypes != null)
            {
                allQuestions = FilterQuestion(allQuestions);
            }
            if (_userResponses.Count == 0)
            {
                var message = _options.SectionId == null ? $"[No student submitted for Quiz {_options.QuizName} in all sections.]" : $"[No student submitted for Quiz {_options.QuizName} in section {_options.SectionName}.]";
                Paragraph noAnswerLabelParagraph = new Paragraph();
                var noAnswerContainer = new Run();
                noAnswerContainer.SetFontSize(_fontSize);
                noAnswerContainer.Append(new Text
                {
                    Text = message,
                });
                noAnswerLabelParagraph.Append(noAnswerContainer);
                _body.Append(noAnswerLabelParagraph);
                return;
            }
            if (allQuestions != null && allQuestions.Count() != 0)
            {
                var questionNumber = 0;
                foreach (var question in allQuestions)
                {
                    questionNumber += 1;
                    var questionNumberLabel = $"Question {questionNumber}";
                    if (questionNumber == 1)
                    {
                        Paragraph contentParagraph = GenerateSection(headerText: "Exam summary");
                        _body.Append(contentParagraph);
                    }

                    if (_options.IsQuestionShown)
                    {
                        Paragraph questionLabelParagraph = new Paragraph();
                        Run questionLabelContainer = new Run();
                        questionLabelContainer.SetFontSize(_fontSize, bold: true);
                        questionLabelContainer.Append(new Text
                        {
                            Text = "Question:"
                        });
                        questionLabelParagraph.Append(questionLabelContainer);
                        _body.Append(questionLabelParagraph);
                        _body.Append(CreateAltChunk(question.QuestionText));
                    }

                    Paragraph answerLabelParagraph = new Paragraph();
                    Run answerLabelContainer = new Run();
                    answerLabelContainer.SetFontSize(_fontSize, bold: true);
                    answerLabelContainer.Append(new Text
                    {
                        Text = $"Student's answer for {questionNumberLabel}:"
                    });
                    answerLabelParagraph.Append(answerLabelContainer);
                    _body.Append(answerLabelParagraph);


                    if (_classLists.Count > 0)
                    {
                        bool addHR = false;
                        foreach (var student in _classLists)
                        {
                            var responses = studentResponseMap[student];
                            if (responses.Count == 0)
                            {
                                responses = new List<UserResponse> { null }; // to simulate student did not submit
                            }
                            ;

                            foreach (var user in responses)
                            {

                                // get answers for selected question for selected student


                                // do not continue if no responses found (student only answers non text question)
                                //if (responses == null || responses.Count() == 0) continue;

                                var nRic = student.User.OrgDefinedId;
                                var studentName = student.User.DisplayName;
                                var section = student.User.GetSectionName();
                                var username = _dicStudent[int.Parse(student.User.Identifier)];
                                var campusId = nRic;

                                if (addHR)
                                {
                                    Paragraph contentParagraph = GenerateSection(headerText: questionNumberLabel);
                                    _body.Append(contentParagraph);
                                }
                                var tblStudentResponse = CreateStudentResponseTable(studentName, username, campusId, section, user, question, addHR);
                                _body.AppendChild(tblStudentResponse);

                                // horizontal line is applied to top, 1st student does not need any HR
                                addHR = true;
                            }
                        }
                    }
                    else
                    {
                        var message = _options.SectionId == null ? $"[No student submitted for Quiz {_options.QuizName} in all sections.]" : $"[No student submitted for Quiz {_options.QuizName} in section {_options.SectionName}.]";
                        Paragraph noAnswerLabelParagraph = new Paragraph();
                        var noAnswerContainer = new Run();
                        noAnswerContainer.SetFontSize(_fontSize);
                        noAnswerContainer.Append(new Text
                        {
                            Text = message,
                        });
                        noAnswerLabelParagraph.Append(noAnswerContainer);
                        _body.Append(noAnswerLabelParagraph);
                    }
                    // only add section breaker when NOT last page
                    if (questionNumber < allQuestions.Count())
                    {
                        // section break
                        Paragraph contentParagraph = GenerateSection(headerText: questionNumberLabel);
                        _body.Append(contentParagraph);
                    }
                    else
                    {
                        // no section break
                        var headerId = GenerateHeader(text: questionNumberLabel);
                        _body.Append(CreateSectionProperties(headerId));
                    }
                }
            }
        }

        private Paragraph GenerateSection(string headerText)
        {
            var headerId = GenerateHeader(headerText);
            var paragraphProperties = CreateSection(headerId);
            paragraphProperties.SetLineSpace(_lineSpace);
            Paragraph paragraph1 = new Paragraph();
            paragraph1.Append(paragraphProperties);
            return paragraph1;
        }

        private ParagraphProperties CreateSection(string headerId)
        {
            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            paragraphProperties1.Append(CreateSectionProperties(headerId));
            return paragraphProperties1;
        }

        private SectionProperties CreateSectionProperties(string headerId)
        {
            SectionProperties sectionProperties1 = new SectionProperties();
            HeaderReference headerReference2 = new HeaderReference() { Type = HeaderFooterValues.Default, Id = headerId };
            FooterReference footerRef = new FooterReference() { Type = HeaderFooterValues.Default, Id = _footerrId };
            SectionType sectionType1 = new SectionType() { Val = SectionMarkValues.OddPage };
            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)11906U, Height = (UInt32Value)16838U };
            PageMargin pageMargin1 = new PageMargin() { Top = 1440, Right = (UInt32Value)1440U, Bottom = 1440, Left = (UInt32Value)1440U, Header = (UInt32Value)708U, Footer = (UInt32Value)708U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "708" };
            DocGrid docGrid1 = new DocGrid() { LinePitch = 360 };

            sectionProperties1.Append(headerReference2);
            sectionProperties1.Append(footerRef);
            sectionProperties1.Append(sectionType1);
            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(docGrid1);

            return sectionProperties1;
        }

        private void GenerateFooter()
        {
            autorId += 1;
            _footerrId = $"rId{autorId}";
            FooterPart footerPart = _mainPart.AddNewPart<FooterPart>(_footerrId);

            Footer footer1 = new Footer();
            Paragraph paragraph = new Paragraph();

            // position tab to get right alignment
            Run rightAlignContainer = new Run();
            PositionalTab rightAlign = new PositionalTab() { Alignment = AbsolutePositionTabAlignmentValues.Right, RelativeTo = AbsolutePositionTabPositioningBaseValues.Margin, Leader = AbsolutePositionTabLeaderCharValues.None };
            rightAlignContainer.Append(rightAlign);
            paragraph.Append(rightAlignContainer);

            // set the word "Page"
            Run pageLabelContainer = new Run();
            Text pageLabel = new Text() { Space = SpaceProcessingModeValues.Preserve };
            pageLabel.Text = "Page ";
            pageLabelContainer.Append(pageLabel);

            // Page number field
            Run pageNumFieldStart = new Run();
            FieldChar pageNumStart = new FieldChar() { FieldCharType = FieldCharValues.Begin };
            pageNumFieldStart.Append(pageNumStart);
            paragraph.Append(pageNumFieldStart);

            Run pageNumContainer = new Run();
            RunProperties pageNumStyle = new RunProperties();
            Bold pageNumBold = new Bold();
            pageNumStyle.Append(pageNumBold);
            FieldCode pageNum = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            pageNum.Text = " PAGE  \\* Arabic  \\* MERGEFORMAT ";
            pageNumContainer.Append(pageNumStyle);
            pageNumContainer.Append(pageNum);
            paragraph.Append(pageNumContainer);

            Run pageNumFieldEnd = new Run();
            FieldChar pageNumEnd = new FieldChar() { FieldCharType = FieldCharValues.End };
            pageNumFieldEnd.Append(pageNumEnd);
            paragraph.Append(pageNumFieldEnd);

            // label " of "
            Run ofLabelContainer = new Run();
            Text ofLabel = new Text() { Space = SpaceProcessingModeValues.Preserve };
            ofLabel.Text = " of ";
            ofLabelContainer.Append(ofLabel);
            paragraph.Append(ofLabelContainer);

            // numOfPage field begin
            Run numOfPageFieldBegin = new Run();
            FieldChar numOfPageBegin = new FieldChar() { FieldCharType = FieldCharValues.Begin };
            numOfPageFieldBegin.Append(numOfPageBegin);
            paragraph.Append(numOfPageFieldBegin);

            // total number of page
            Run numOfPageContainer = new Run();
            RunProperties numOfPageStyle = new RunProperties();
            Bold bold7 = new Bold();
            numOfPageStyle.Append(bold7);
            FieldCode numOfPage = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            numOfPage.Text = " NUMPAGES  \\* Arabic  \\* MERGEFORMAT ";
            numOfPageContainer.Append(numOfPageStyle);
            numOfPageContainer.Append(numOfPage);
            paragraph.Append(numOfPageContainer);

            // end close "Field Char"
            Run numOfPageFieldEnd = new Run();
            FieldChar numOfPageEnd = new FieldChar() { FieldCharType = FieldCharValues.End };
            numOfPageFieldEnd.Append(numOfPageEnd);
            paragraph.Append(numOfPageFieldEnd);

            footer1.Append(paragraph);
            footerPart.Footer = footer1;
        }

        private string GenerateHeader(string text)
        {
            autorId += 1;
            var headerId = $"rId{autorId}";

            HeaderPart headerPart = _mainPart.AddNewPart<HeaderPart>(headerId);

            Run headerTextContainer = new Run();
            headerTextContainer.Append(new Text()
            {
                Text = text
            });

            Paragraph headerParagraph = new Paragraph();
            headerParagraph.Append(headerTextContainer);

            Header header = new Header();
            header.Append(headerParagraph);

            headerPart.Header = header;

            return headerId;
        }
        private object lockObject = new object();
        private Dictionary<Entity.Valence.OrgUnitUser, List<UserResponse>> studentResponseMap = new Dictionary<Entity.Valence.OrgUnitUser, List<UserResponse>>();
        private Dictionary<Entity.Valence.OrgUnitUser, Entity.Valence.UserData> studentUserInfoMap = new Dictionary<Entity.Valence.OrgUnitUser, Entity.Valence.UserData>();
        private Task CollectUserInfo(Entity.Valence.OrgUnitUser s, List<UserResponse> userResponses)
        {
            return Task.Run(() =>
            {

                var responseByStudent = userResponses.Where(x => x.StudentId == Convert.ToInt32(s.User.Identifier)).ToList();
                var info = _valenceApi.GetUserInfo(int.Parse(s.User.Identifier));
                lock (lockObject)
                {
                    studentResponseMap[s] = responseByStudent;
                    studentUserInfoMap[s] = info;
                }
            });
        }
        private void GenerateExamSummaryPage(string headerText)
        {
            if (!string.IsNullOrEmpty(headerText))
            {
                Paragraph contentParagraph = (new OpenXmlHelper(_configuration)).CreateGptZeroTitle(headerText);
                _body.Append(contentParagraph);
            }

            var tblInfo = new Table();
            tblInfo.SetTableAutoFit().SetTableColumn(2);

            var trCourseCode = new TableRow();
            trCourseCode.Append(new TableCell().SetText("Course Code:", _fontSize, _lineSpace),
                new TableCell().SetText(_options.CourseCode, _fontSize, _lineSpace));
            tblInfo.Append(trCourseCode);

            var trQuiz = new TableRow();
            trQuiz.Append(new TableCell().SetText("Quiz Name:", _fontSize, _lineSpace),
                new TableCell().SetText(_options.QuizName, _fontSize, _lineSpace));
            tblInfo.AppendChild(trQuiz);
            _body.Append(tblInfo);

            foreach (var section in _options.Sections)
            {
                var classList = section.Classlist;
                _classLists.AddRange(classList);
            }
            var tasks = _classLists.Select(e => CollectUserInfo(e, _userResponses)).ToArray();
            Task.WaitAll(tasks);

            _classLists = _classLists.Where(e => !_options.SectionId.HasValue || e.User.GetSectionId() == _options.SectionId.Value).OrderByDescending(e =>
            {
                var responseByStudent = studentResponseMap[e];
                return responseByStudent.Any() ? 1 : 0;
            }).ThenBy(e => e.User.StudentSectionNameNumber).ToList();
            foreach (var student in _classLists)
            {
                var user = studentResponseMap[student].FirstOrDefault();
                var studentId = int.Parse(student.User.Identifier);
                var userInfo = studentUserInfoMap[student];
                _dicStudent.Add(studentId, userInfo.UserName);

                var nRic = student.User.OrgDefinedId;
                var studentName = student.User.DisplayName;

                // Get UTC dates and add in the offset


                var tblStudent = new Table().SetTableAutoFit().SetTableColumn(2);

                if (_options.IsStudentNameShown)
                {
                    var trStudentName = new TableRow();
                    var cellNameText = new TableCell();
                    cellNameText.CreateParagraph();
                    cellNameText.SetTopBorder().SetText("Name:", _fontSize, _lineSpace).SetCellWidth(1013);

                    var cellNameValue = new TableCell();
                    cellNameValue.CreateParagraph();
                    cellNameValue.SetTopBorder().SetText(studentName, _fontSize, _lineSpace).SetCellWidth(3987);

                    trStudentName.Append(cellNameText, cellNameValue);
                    // Add row to the table.
                    tblStudent.AppendChild(trStudentName);

                    var trUserName = new TableRow();
                    trUserName.Append(new TableCell().SetText("Username:", _fontSize, _lineSpace).SetCellWidth(1013), new TableCell().SetText(_dicStudent[studentId], _fontSize, _lineSpace).SetCellWidth(3987));
                    tblStudent.AppendChild(trUserName);
                }

                var trCampus = new TableRow();

                var cellCampusText = new TableCell();

                if (!_options.IsStudentNameShown)
                {
                    cellCampusText.CreateParagraph();
                    cellCampusText.SetTopBorder();
                }
                var cellCampusValue = new TableCell();
                if (!_options.IsStudentNameShown)
                {
                    cellCampusValue.CreateParagraph();
                    cellCampusValue.SetTopBorder();
                }

                trCampus.Append(cellCampusText.SetText("Campus ID:", _fontSize, _lineSpace).SetCellWidth(1013), cellCampusValue.SetText(nRic, _fontSize, _lineSpace).SetCellWidth(3987));
                tblStudent.AppendChild(trCampus);

                var trSection = new TableRow();
                var cellSectionText = new TableCell();
                var cellSectionValue = new TableCell();
                cellSectionText.SetText("Section:", _fontSize, _lineSpace).SetCellWidth(1013);
                cellSectionValue.SetText(student.User.GetSectionName(), _fontSize, _lineSpace).SetCellWidth(3987);
                trSection.Append(cellSectionText, cellSectionValue);
                tblStudent.AppendChild(trSection);
                if (user != null)
                {
                    var utcTimeStarted = user.TimeStarted.UtcDateTime;
                    var utcTimeCompleted = user.TimeCompleted.UtcDateTime;
                    var timeCompleted = utcTimeCompleted + TimeZoneInfo.Local.GetUtcOffset(user.TimeCompleted);

                    var tz = TZConvert.IanaToWindows(_options.ClientTimezone);
                    var clientTz = TimeZoneInfo.FindSystemTimeZoneById(tz);

                    var localTimeStarted = TimeZoneInfo.ConvertTime(utcTimeStarted, clientTz);
                    var localTimeCompleted = TimeZoneInfo.ConvertTime(utcTimeCompleted, clientTz);

                    var started = localTimeStarted.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var completed = localTimeCompleted.ToString("dd-MMM-yyyy hh:mm:ss tt");

                    var duration = user.TimeCompleted - user.TimeStarted;
                    var timeDiff =
                        $"{duration.Days} days, {duration.Hours} hours, {duration.Minutes} minutes, {duration.Seconds} secs";

                    if (timeCompleted.Year == 1)
                    {
                        completed = "In Progress";
                        timeDiff = string.Empty;
                    }
                    var trDateRange = new TableRow();
                    trDateRange.Append(new TableCell().SetText("Date Range:", _fontSize, _lineSpace),
                        new TableCell().SetText($"{started} - {completed}", _fontSize, _lineSpace));
                    tblStudent.AppendChild(trDateRange);

                    var trSpentTime = new TableRow();
                    trSpentTime.Append(new TableCell().SetText("Total Spent Time:", _fontSize, _lineSpace),
                        new TableCell().SetText(timeDiff, _fontSize, _lineSpace));
                    tblStudent.AppendChild(trSpentTime);
                }


                _body.AppendChild(tblStudent);
            }
        }

        private Table CreateStudentResponseTable(string studentName, string username, string campusId, string sectionName, UserResponse user, QuestionViewModel question, bool addTopHorizontalRule)
        {
            var numberOfColumn = 2;
            // create table with 2 columns
            var tblStudent = new Table().SetTableAutoFit().SetTableColumn(numberOfColumn);

            // set table properties
            TableProperties tableProperties = new TableProperties();
            TablePositionProperties tablePositionProperties = new TablePositionProperties() { VerticalAnchor = VerticalAnchorValues.Text };
            tableProperties.AppendChild(tablePositionProperties);
            tblStudent.AppendChild(tableProperties);

            // add top row to control column width
            var trHR = new TableRow();
            var cellHR1 = new TableCell().SetCellWidth(1013);
            var cellHR2 = new TableCell().SetCellWidth(3987);
            //if (addTopHorizontalRule)
            //{
            //    cellHR1.SetTopBorder();
            //    cellHR2.SetTopBorder();
            //}
            cellHR1.CreateParagraph();
            cellHR2.CreateParagraph();
            trHR.Append(cellHR1, cellHR2);
            tblStudent.AppendChild(trHR);

            // add student name and username if option is set
            if (_options.IsStudentNameShown)
            {
                // add display name
                var trName = new TableRow();
                trName.Append(
                    new TableCell().SetText("Name:", _fontSize, _lineSpace),
                    new TableCell().SetText(studentName, _fontSize, _lineSpace));
                tblStudent.AppendChild(trName);

                // add username
                var trUserName = new TableRow();
                trUserName.Append(
                    new TableCell().SetText("Username:", _fontSize, _lineSpace),
                    new TableCell().SetText(username, _fontSize, _lineSpace));
                tblStudent.AppendChild(trUserName);
            }

            // add campus Id (previously NRIC)
            var trCampus = new TableRow();
            trCampus.Append(
                new TableCell().SetText("Campus ID:", _fontSize, _lineSpace),
                new TableCell().SetText(campusId, _fontSize, _lineSpace));
            tblStudent.AppendChild(trCampus);

            var trSection = new TableRow();
            trSection.Append(new TableCell().SetText("Section:", _fontSize, _lineSpace),
                new TableCell().SetText(sectionName, _fontSize, _lineSpace));
            tblStudent.AppendChild(trSection);

            // add response label
            var trResponseLabel = new TableRow();
            trResponseLabel.Append(
                new TableCell().SetText("Response:", _fontSize, _lineSpace),
                new TableCell().SetText("", _fontSize, _lineSpace));
            tblStudent.AppendChild(trResponseLabel);

            // add response content


            // only show answer number label when there are more than 1 answer in a question. i.e. FIB or MSA
            if (user != null)
            {
                var trResponseText = new TableRow();
                var tdResponseText = new TableCell();
                var responseCellProperties = new TableCellProperties();
                responseCellProperties.AddTableCellProperties(TableWidthUnitValues.Auto, 2);
                tdResponseText.AppendChild(responseCellProperties);
                // href attribute is generated differently in 2 sources so we need a special comparison rule here
                var responses = user.Responses
                                   .Where(userAnswer =>
                                        OpenXmlExtensions.TextCompareWithHrefRule(userAnswer.QuestionText, question.QuestionText)
                                       && string.Equals(userAnswer.QuestionType, question.QuestionType, StringComparison.OrdinalIgnoreCase)
                                   );
                var displayAnswerNumberLabel = responses.Count() > 1 ? true : false;

                var answerNumber = 1;
                if (responses.Count() > 0)
                {
                    foreach (var response in responses)
                    {
                        if (displayAnswerNumberLabel)
                            tdResponseText.Append(OpenXmlExtensions.CreateParagraph($"Answer {answerNumber}:", _fontSize, _lineSpace));


                        if (OpenXmlExtensions.HighlightRegex.IsMatch(response.TextResponse))
                            tdResponseText.Append(CreateResponseAltChunk(response.TextResponse));
                        else
                            tdResponseText.Append(OpenXmlExtensions.CreateParagraph(response.TextResponse, _fontSize, _lineSpace));

                        answerNumber += 1;
                    }
                }
                else
                {
                    if (displayAnswerNumberLabel)
                        tdResponseText.Append(OpenXmlExtensions.CreateParagraph($"Answer {answerNumber}:", _fontSize, _lineSpace));
                    tdResponseText.Append(OpenXmlExtensions.CreateParagraph($"[{_configuration.GetValue<string>("NoAnswerSysText")}]", _fontSize, _lineSpace));
                }

                trResponseText.AppendChild(tdResponseText);
                tblStudent.AppendChild(trResponseText);
            }
            else
            {
                var mergeProperties = new TableCellProperties();
                mergeProperties.AddTableCellProperties(TableWidthUnitValues.Auto, 2);
                var trNonSubmittedResponseLabel = new TableRow();
                var nonSubmittedCell = new TableCell();
                nonSubmittedCell.AppendChild(mergeProperties);
                nonSubmittedCell.SetText($"[{_configuration.GetValue<string>("NonSubmittedSysText")}]", _fontSize, _lineSpace);
                trNonSubmittedResponseLabel.AppendChild(nonSubmittedCell);
                tblStudent.AppendChild(trNonSubmittedResponseLabel);
            }


            // Add row to the table.

            return tblStudent;
        }
        public AltChunk CreateAltChunk(String htmlText)
        {
            htmlText = OpenXmlExtensions.EncodeWordCharacter(htmlText);
            var htmlTextWrapper = "<html><head></head><body style='font-family: Arial; '>" + htmlText + "</body></html>";
            var formatImportPart = _mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html);
            using (MemoryStream ms1 = new MemoryStream(Encoding.UTF8.GetBytes(htmlTextWrapper)))
            {
                formatImportPart.FeedData(ms1);
            }

            AltChunk altChunk = new AltChunk
            {
                Id = _mainPart.GetIdOfPart(formatImportPart)
            };
            return altChunk;
        }
        public AltChunk CreateResponseAltChunk(String htmlText)
        {
            //htmlText = OpenXmlExtensions.ConvertHtmlText(htmlText);

            var htmlTextWrapper = "<html><head></head><body style='font-family: Arial; '>" + htmlText + "</body></html>";
            var formatImportPart = _mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html);
            using (MemoryStream ms1 = new MemoryStream(Encoding.UTF8.GetBytes(htmlTextWrapper)))
            {
                formatImportPart.FeedData(ms1);
            }

            AltChunk altChunk = new AltChunk
            {
                Id = _mainPart.GetIdOfPart(formatImportPart)
            };
            return altChunk;
        }

        public List<QuestionViewModel> FilterQuestion(List<QuestionViewModel> Questions)
        {
            return Questions.Where(x => _options.QuestionTypes.Contains(x.QuestionType)).ToList();
        }

        public bool IsTextQuestion(string questionType)
        {
            var questionTypes = _constants.QuestionTypes.Split(',').ToList();
            return questionTypes.Contains(questionType);
        }
    }
}