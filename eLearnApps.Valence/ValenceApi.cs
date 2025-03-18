using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.Restsharp;
using eLearnApps.Entity.Valence;
using RestSharp;
using System.Threading.Tasks;
using eLearnApps.ViewModel.Valence.Grade;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Valence
{

    public class ValenceApi
    {
        private readonly string lp_version;
        private readonly string le_version;
        private readonly ID2LUserContext _mValenceUserContext;
        private readonly IConfiguration _configuration;
        private readonly Constants _constants;
        public ValenceApi(IConfiguration configuration)
        {
            _configuration = configuration;
            _constants = new Constants(_configuration);
            var appFactory = new D2LAppContextFactory();
            var mValenceAppContext = appFactory.Create(_constants.ValenceApiAppId, _constants.ValenceApiAppKey);
            var mValenceHost = new HostSpec(_constants.Https, _constants.LsmHost, _constants.DefaultHttpsPost);
            _mValenceUserContext = mValenceAppContext.CreateUserContext(_constants.ValenceApiUserId,
                _constants.ValenceApiUserKey, mValenceHost);

            var productVersions = getVersions();
            var ignoreCase = true;
            var productLP = productVersions.Where(s => string.Compare(s.ProductCode, "lp", ignoreCase) == 0).FirstOrDefault();
            var productLE = productVersions.Where(s => string.Compare(s.ProductCode, "le", ignoreCase) == 0).FirstOrDefault();
            lp_version = productLP?.LatestVersion ?? _constants.LpVersion;
            le_version = productLE?.LatestVersion ?? _constants.VALENCE_LE_VERSION;
        }

        public T Execute<T>(string route) where T : new()
        {
            try
            {
                var client = new RestClient(_constants.BaseHost);
                var authenticator = new ValenceAuthenticator(_mValenceUserContext);
                var request = new RestRequest(route);
                //authenticator.Authenticate(client, request);
                var response = client.Execute<T>(request);

                // return null when there is no data
                if (response.StatusCode == HttpStatusCode.OK) return response.Data;
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new Exception($"Unable to consume web API: {route}. Http Status Code: {response.StatusCode}. ");
                }
                else
                {
                    return default;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public HttpStatusCode Put<T>(string route, T param)
        {
            try
            {
                var client = new RestClient(_constants.BaseHost);
                var authenticator = new ValenceAuthenticator(_mValenceUserContext);
                var request = new RestRequest(route, Method.Put) { RequestFormat = DataFormat.Json };
                request.AddBody(param);
                //authenticator.Authenticate(client, request);
                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public RestResponse? Post<T>(string route, T param)
        {
            try
            {
                var client = new RestClient(_constants.BaseHost);
                var authenticator = new ValenceAuthenticator(_mValenceUserContext);
                var request = new RestRequest(route, Method.Post) { RequestFormat = DataFormat.Json };
                request.AddBody(param);
                //authenticator.Authenticate(client, request);
                var response = client.Execute(request);
                return response;
            }
            catch
            {
                return null;
            }
        }

        private List<ProductVersion> getVersions()
        {
            var apiUrl = $"/d2l/api/versions/";
            var productVersions = Execute<List<ProductVersion>>(apiUrl);
            return productVersions;
        }

        public List<QuestionData> getQuizQustions(int orgUnitId, int quizId)
        {
            var route = $"/d2l/api/le/{le_version}/{orgUnitId}/quizzes/{quizId}/questions/";
            //List<QuestionData> result = new List<QuestionData>();
            //bool keeplooping = true;
            //do
            //{
            //    var temp = Execute<ViewModel.Valence.ObjectListPage<QuestionData>>(route);
            //    if (temp == null)
            //    {
            //        break;
            //    }
            //    result.AddRange(temp.Objects);


            //    // When request is to pull all items; set next route; stop when NEXT is null
            //    if (keeplooping)
            //    {
            //        route = temp.Next;
            //        keeplooping = !string.IsNullOrEmpty(temp.Next);
            //    }
            //}
            //while (keeplooping);

            //return result;

            var result = new List<QuestionData>();

            while (true)
            {
                var temp = Execute<ViewModel.Valence.ObjectListPage<QuestionData>>(route);
                if (temp == null || temp.Objects == null)
                {
                    break;
                }
                result.AddRange(temp.Objects);

                if (string.IsNullOrEmpty(temp.Next))
                {
                    break;
                }

                route = new Uri(temp.Next).PathAndQuery;
            }

            return result;
        }

        public List<UserData> FindUserByEmail(string email)
        {

            var apiUrl = $"{_constants.BasePath}{_constants.LpVersion}/users/?externalEmail=" + email;
            var enrollmentData = Execute<List<UserData>>(apiUrl);

            return enrollmentData;
        }

        public ViewModel.Valence.EnrollmentData GetEnrollmentFor(int userId, int courseId)
        {
            var apiUrl = $"{_constants.BasePath}{_constants.LpVersion}/enrollments/orgUnits/{courseId}/users/{userId}";
            var enrollmentData = Execute<ViewModel.Valence.EnrollmentData>(apiUrl);
            return enrollmentData;
        }

        public PagedResultSetDynamic<MyOrgUnitInfo> GetAllCourses(string bookmark)
        {
            // utilizing  main login's enrolled course, we get all courses
            // on assumption that main login will have enrolled in ALL courses
            var apiUrl =
                $"{_constants.BasePath}{_constants.LpVersion}/enrollments/myenrollments/?orgUnitTypeId={_constants.CourseIndicator}{(string.IsNullOrEmpty(bookmark) ? string.Empty : $"&bookmark={bookmark}")}";
            var lstEnroll = Execute<PagedResultSetDynamic<MyOrgUnitInfo>>(apiUrl);
            return lstEnroll;
        }

        public List<Role> GetRoles()
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/roles/";
            var roles = Execute<List<Role>>(route);
            return roles;
        }

        public GradeSetupInfo GetGradeSetupInfo(int orgUnitId)
        {
            var route = $"{_constants.LEBasePath}{_constants.LpVersion}/{orgUnitId}/grades/setup/";
            var gradeSetupinfo = Execute<GradeSetupInfo>(route);
            return gradeSetupinfo;
        }

        public List<OrgUnitUser> GetEnrolledUser(int orgUnitId)
        {
            var baseroute = $"{_constants.BasePath}{_constants.LpVersion}/enrollments/orgUnits/{orgUnitId}/users/";
            var bookmark = string.Empty;
            var keepreading = true;
            var route = string.Empty;
            var allEnrolledUsers = new List<OrgUnitUser>();
            while (keepreading)
            {
                route = baseroute + bookmark;
                var enrolledUser = Execute<PagedResultSetDynamic<OrgUnitUser>>(route);
                if (enrolledUser == null)
                {
                    keepreading = false;
                }
                else
                {
                    allEnrolledUsers.AddRange(enrolledUser.Items);
                    if (enrolledUser.PagingInfo.HasMoreItems)
                        bookmark = $"?bookmark={enrolledUser.PagingInfo.Bookmark}";
                    else
                        keepreading = false;
                }
            }

            return allEnrolledUsers;
        }

        public CourseOffering GetCourceDetailById(int courseId)
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/courses/{courseId}";
            var courseOfferingInfo = Execute<CourseOffering>(route);
            return courseOfferingInfo;
        }

        public List<SectionData> GetSectionsByCourseId(int courseId)
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/{courseId}/sections/";
            var courseOfferingInfo = Execute<List<SectionData>>(route);
            return courseOfferingInfo;
        }

        public List<GroupCategoryData> GetCategory(int courseId)
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/{courseId}/groupcategories/";
            var categories = Execute<List<GroupCategoryData>>(route);
            return categories;
        }

        public List<ClasslistUser> GetClasslistUser(int orgUnitId)
        {
            var route = $"{_constants.LEBasePath}{_constants.LpVersion}/{orgUnitId}/classlist/";
            var classListUser = Execute<List<ClasslistUser>>(route);
            return classListUser;
        }

        public List<GroupData> GetGroupAndStudentEnrollmentsByCategoryId(int courseId, int categoryId)
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/{courseId}/groupcategories/{categoryId}/groups/";
            var groupAndStudentEnrollments = Execute<List<GroupData>>(route);
            return groupAndStudentEnrollments;
        }

        public List<Section> GetSectionByCourseId(int courseId)
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/{courseId}/sections/";
            var lstSections = Execute<List<Section>>(route);
            return lstSections;
        }

        public List<GradeObject> GetGradeObjectByCourseId(int courseId)
        {
            var route = $"{_constants.LEBasePath}{_constants.VALENCE_LE_VERSION}/{courseId}/grades/";
            var gradeObjects = Execute<List<GradeObject>>(route);

            // need to massage data alittle 
            foreach (var go in gradeObjects)
            {
                // when setup as default value in D2L, gradeschemeid became null and gradeschemeUrl contains the url with default id
                // we need to extract value and put into id
                if (!string.IsNullOrWhiteSpace(go.GradeSchemeUrl) && go.GradeSchemeId == null)
                {
                    var idx = go.GradeSchemeUrl.LastIndexOf("/");
                    if (idx > 0 && idx < (go.GradeSchemeUrl.Length - 1))
                    {
                        var temp = go.GradeSchemeUrl.Substring(idx + 1);
                        int gradeSchemeId;
                        if (int.TryParse(temp, out gradeSchemeId))
                            go.GradeSchemeId = gradeSchemeId;
                    }
                }
            }

            return gradeObjects;
        }

        public List<GradeObjectCategory> GetGradeObjectCategoryByCourseId(int courseId)
        {
            var route = $"{_constants.LEBasePath}{_constants.LpVersion}/{courseId}/grades/categories/";
            var gradeObjectCategories = Execute<List<GradeObjectCategory>>(route);
            return gradeObjectCategories;
        }

        public List<UserGradeObject> GetUserGradeObjectByCourseId(int courseId, int numberOfExpectedRecords)
        {
            var baseroute = $"{_constants.CustomizationBasePath}gradevalues/{courseId}/";

            var numOfLoop = (numberOfExpectedRecords / 100) + 1;
            IEnumerable<int> bookmarks = Enumerable.Range(0, numOfLoop).Select(x => x * 100).ToList();
            var allUserGrades = new List<UserGradeObject>();

            Object allUserGradesLock = new Object();

            Parallel.ForEach(bookmarks, bookmark =>
            {
                if (bookmark == bookmarks.Last())
                {
                    System.Diagnostics.Debug.WriteLine($"{bookmark} - {DateTime.Now}");
                    var usergrades = Execute<CustomizedPagedResult<UserGradeObject>>(baseroute + $"?bookmark={bookmark}");
                    lock (allUserGradesLock)
                    {
                        allUserGrades.AddRange(usergrades.Items);
                    }

                    // last bookmark need to handle any trailing or missing bookmark (just in case)
                    while (usergrades.HasMoreItems)
                    {
                        System.Diagnostics.Debug.WriteLine($"end {usergrades.Bookmark} - {DateTime.Now}");
                        usergrades = Execute<CustomizedPagedResult<UserGradeObject>>(baseroute + $"?bookmark={usergrades.Bookmark}");
                        lock (allUserGradesLock)
                        {
                            allUserGrades.AddRange(usergrades.Items);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"{bookmark} - {DateTime.Now}");
                    var usergrades = Execute<CustomizedPagedResult<UserGradeObject>>(baseroute + $"?bookmark={bookmark}");
                    lock (allUserGradesLock)
                    {
                        allUserGrades.AddRange(usergrades.Items);
                    }
                }
            });

            return allUserGrades;
        }

        public List<GradeValue> GetUserGrade(int courseId, IEnumerable<int> userIds)
        {
            var route = $"{_constants.LEBasePath}{_constants.LpVersion}/{courseId}/grades/values";

            Object lockAllUserGrades = new Object();
            var allUserGrades = new List<GradeValue>();
            Parallel.ForEach(userIds, userId =>
            {
                var studentRoute = $"{route}/{userId}/";
                var usergrades = Execute<List<GradeValue>>(studentRoute);
                //.Select(g => new ViewModel.RPT.UserGradeObjectViewModel() 
                //{ 
                //    UserId = g.UserId,
                //    OrgDefinedId = string.Empty,
                //    CourseId = g.OrgUnitId,
                //    GradeObjectId = g.GradeObjectIdentifier,
                //    PointsNumerator = g.PointsNumerator,
                //    PointsDenominator = g.PointsDenominator,
                //    WeightedNumerator = g.WeightedNumerator,
                //    WeightedDenominator = g.WeightedDenominator,
                //    CommentsForUser = g.Comments.Text,
                //    DisplayedGrade = g.DisplayedGrade,
                //    GradeObjectTypeId = g.GradeObjectTypeName
                //});

                //public int UserId { get; set; }
                //public string OrgDefinedId { get; set; }
                //public int GradeObjectId { get; set; }
                //public double PointsNumerator { get; set; }
                //public double PointsDenominator { get; set; }
                //public double WeightedDenominator { get; set; }
                //public double WeightedNumerator { get; set; }
                //public string CommentsForUser { get; set; }
                //public string DisplayedGrade { get; set; }
                //public int GradeObjectTypeId { get; set; }
                //public int CourseId { get; set; }

                //In the function
                lock (lockAllUserGrades)
                {
                    allUserGrades.AddRange(usergrades);
                }
            });
            return allUserGrades;
        }

        public QuizzesObject GetQuizObjectsByCourseId(int courseId)
        {
            var baseroute = $"{_constants.LEBasePath}1.28/{courseId}/quizzes/";
            string route = baseroute;
            var quizObjects = new QuizzesObject();
            var haveDataToRead = true;
            string next;
            try
            {
                while (haveDataToRead)
                {
                    var tempQuizObjects = Execute<QuizzesObject>(route);
                    quizObjects.Objects.AddRange(tempQuizObjects.Objects);

                    // check if there is any next page to read
                    if (tempQuizObjects.Next != null && tempQuizObjects != default(object))
                    {
                        next = tempQuizObjects.Next.ToString();
                        var idx = next.IndexOf(baseroute);
                        route = next.Substring(idx);
                    }
                    else haveDataToRead = false;
                }

            }
            catch (Exception e)
            {
                var delme = e.ToString();
            }

            return quizObjects ?? new QuizzesObject();
        }

        public QuizObject GetQuizObjectByQuizId(int courseId, int quizId)
        {
            var route = $"{_constants.LEBasePath}1.28/{courseId}/quizzes/{quizId}";
            var quizObjects = Execute<QuizObject>(route);
            return quizObjects ?? new QuizObject();
        }

        public eLearnApps.ViewModel.Valence.UserResponse GetQuizResponseByUserId(int courseId, int quizId, int userId)
        {
            var route = $"{_constants.CustomizationBasePath}quizzes/{courseId}/{quizId}/lastquizattempt/{userId}";
            var userResponse = Execute<eLearnApps.ViewModel.Valence.UserResponse>(route);
            if (userResponse == null) // || userResponse.Responses.Count() == 0)
            {
                userResponse = new eLearnApps.ViewModel.Valence.UserResponse();
                userResponse.StudentId = userId;
                userResponse.HasResponse = false;
                userResponse.QuizId = quizId;
                userResponse.Responses = new List<eLearnApps.ViewModel.Valence.Response>()
                {
                    new eLearnApps.ViewModel.Valence.Response
                    {
                        QuestionText = "No attempts yet.",
                        TextResponse = "No attempts yet.",
                        QuestionType = "Multi-Select"
                    }
                };
                return userResponse;
            }
            else
            {
            }

            userResponse.StudentId = userId;
            return userResponse;
        }

        public UserResponse GetQuizResponseByUserIdEntity(int courseId, int quizId, int userId)
        {
            var route = $"{_constants.CustomizationBasePath}quizzes/{courseId}/{quizId}/lastquizattempt/{userId}";
            var userResponse = Execute<UserResponse>(route);
            if (userResponse == null || userResponse.Responses.Count() == 0)
            {
                userResponse = new UserResponse();
                userResponse.StudentId = userId;
                userResponse.UserId = userId;
                userResponse.HasResponse = false;
                userResponse.QuizId = quizId;
                userResponse.Responses = new List<Response>()
                {
                    new Response
                    {
                        QuestionText = "No attempts yet.",
                        TextResponse = "No attempts yet.",
                        QuestionType = "Multi-Select"
                    }

                };
                return userResponse;
            }

            userResponse.StudentId = userId;
            return userResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="gradeObjectId"></param>
        /// <param name="userId"></param>
        /// <param name="gradeObjectType"></param>
        /// <param name="pointsNumerator"></param>
        /// <returns></returns>
        public HttpStatusCode CreateUserGradeObject(int courseId, int gradeObjectId, int userId, int gradeObjectType, decimal pointsNumerator)
        {
            var route = $"{_constants.LEBasePath}{_constants.LpVersion}/{courseId}/grades/{gradeObjectId}/values/{userId}";
            var gradeIncomingGradeValue = new GradeIncomingGradeValue
            {
                Comments = new Comments
                {
                    Content = string.Empty,
                    Type = "Text"
                },
                PrivateComments = new Privatecomments
                {
                    Content = string.Empty,
                    Type = "Text"
                },
                GradeObjectType = gradeObjectType,
                PointsNumerator = pointsNumerator
            };
            return Put(route, gradeIncomingGradeValue);
        }

        public string? CreateGradeObject(int courseId, decimal maxPoints, string name, decimal weight)
        {
            var route = $"{_constants.LEBasePath}{_constants.LpVersion}/{courseId}/grades/";
            var response = Post(route, new
            {
                MaxPoints = maxPoints,
                CanExceedMaxPoints = false,
                IsBonus = false,
                ExcludeFromFinalGradeCalculation = false,
                GradeSchemeId = (string?)null,
                Name = name,
                ShortName = (string?)null,
                GradeType = "Numeric",
                CategoryId = 0,
                Description = (string?)null,
                GradeSchemeUrl = (string?)null,
                Weight = weight,
                ActivityId = (string?)null,
                AssociatedTool = (string?)null

            });
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Content;
            }
            return "";
        }

        public eLearnApps.ViewModel.Valence.GradeScheme? GetGradeSchemeByRoute(string route)
        {
            try
            {
                var gradeScheme = Execute<eLearnApps.ViewModel.Valence.GradeScheme>(route);
                return gradeScheme;
            }
            catch
            {
                return null;
            }
        }

        public string GetGradeSchemeUrl(int orgUnitId, int orgSchemeId)
        {
            var route = $"{_constants.LEBasePath}{_constants.VALENCE_LE_VERSION}/{orgUnitId}/grades/schemes/{orgSchemeId}";
            return route;
        }

        public eLearnApps.ViewModel.Valence.GradeScheme? GetGradeScheme(int orgUnitId, int orgSchemeId)
        {
            var route = GetGradeSchemeUrl(orgUnitId, orgSchemeId);
            var gradeScheme = Execute<eLearnApps.ViewModel.Valence.GradeScheme>(route);
            if (gradeScheme != null)
            {
                return gradeScheme;
            }

            return null;
        }

        public HttpStatusCode UpdateFinalGrade(int courseId,
            List<ViewModel.Valence.GradeUpdate> gradesToUpdate)
        {
            HttpStatusCode statusCode = HttpStatusCode.OK;
            foreach (var grade in gradesToUpdate)
            {
                // try to update gradebook via valence
                statusCode = UpdateUserGradeFinalValue(courseId, grade.UserId,
                    grade.Numerator, grade.Denumerator);

                if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.NotFound) break;
            }

            return statusCode;
        }

        /// <summary>
        ///     Update grade final value for user
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <param name="pointsNumerator"></param>
        /// <param name="pointsDenominator"></param>
        public HttpStatusCode UpdateUserGradeFinalValue(int courseId, int userId, double? pointsNumerator,
            double? pointsDenominator)
        {
            var route = $"{_constants.LEBasePath}{_constants.VALENCE_LE_VERSION}/{courseId}/grades/final/values/{userId}";
            return Put(route, new { PointsNumerator = pointsNumerator, PointsDenominator = pointsDenominator });
        }

        public List<ViewModel.Valence.UserGradeValue> GetCourseFinalGrade(int courseId)
        {
            var route = $"{_constants.LEBasePath}{_constants.VALENCE_LE_VERSION}/{courseId}/grades/final/values/";

            List<ViewModel.Valence.UserGradeValue> result = new List<ViewModel.Valence.UserGradeValue>();
            bool keeplooping = true;
            do
            {
                var temp = Execute<ViewModel.Valence.ObjectListPage<ViewModel.Valence.UserGradeValue>>(route);
                if (temp == null)
                {
                    break;
                }
                result.AddRange(temp.Objects);


                // When request is to pull all items; set next route; stop when NEXT is null
                if (keeplooping)
                {
                    route = temp.Next;
                    keeplooping = !string.IsNullOrEmpty(temp.Next);
                }
            }
            while (keeplooping);

            return result;
        }

        public UserData GetUserInfo(int userId)
        {
            var route = $"{_constants.BasePath}{_constants.LpVersion}/users/{userId}";
            var user = Execute<UserData>(route);
            return user;
        }

        public List<GradeValue> GetStudentGrade(int orgUnitId, int userId)
        {
            var route = $"/d2l/api/le/{le_version}/{orgUnitId}/grades/values/{userId}/";
            var grades = Execute<List<GradeValue>>(route);
            return grades;
        }

        public GradeValue GetStudentFinalCalculatedGrade(int orgUnitId, int userId)
        {

            var route = $"/d2l/api/le/{le_version}/{orgUnitId}/grades/final/values/{userId}?gradeType=calculated";
            var finalCalculated = Execute<GradeValue>(route);
            return finalCalculated;
        }

        public GradeValue GetStudentFinalAdjustedGrade(int orgUnitId, int userId)
        {
            var route = $"/d2l/api/le/{le_version}/{orgUnitId}/grades/final/values/{userId}?gradeType=adjusted";
            var finalAdjusted = Execute<GradeValue>(route);
            return finalAdjusted;
        }

        public GradeStatisticsInfo GetGradeObjectStatistic(int orgUnitId, int gradeObjectId)
        {
            var route = $"/d2l/api/le/{le_version}/{orgUnitId}/grades/{gradeObjectId}/statistics";
            var gradeObjectStatistic = Execute<GradeStatisticsInfo>(route);
            return gradeObjectStatistic;
        }

        public void GetClassFinalGrades(int orgUnitId)
        {
            var apiUrl = $"/d2l/api/le/{le_version}/{orgUnitId}/grades/final/values/";
            var bookmark = string.Empty;
            var keepreading = true;
            var route = string.Empty;
            var allEnrolledUsers = new List<ViewModel.Valence.UserGradeValue>();
            while (keepreading)
            {
                route = apiUrl;
                var enrolledUser = Execute<ViewModel.Valence.ObjectListPage<ViewModel.Valence.UserGradeValue>>(route);
                if (enrolledUser == null)
                {
                    keepreading = false;
                }
                else
                {
                    allEnrolledUsers.AddRange(enrolledUser.Objects);
                    if (!string.IsNullOrWhiteSpace(enrolledUser.Next))
                        apiUrl = enrolledUser.Next;
                    else
                        keepreading = false;
                }
            }
        }

        public List<ClasslistUser> GetClasslistUsers(int orgUnitId)
        {
            var route = $"/d2l/api/le/{le_version}/{orgUnitId}/classlist/";
            var classlistusers = Execute<List<ClasslistUser>>(route);
            return classlistusers;
        }
    }
}