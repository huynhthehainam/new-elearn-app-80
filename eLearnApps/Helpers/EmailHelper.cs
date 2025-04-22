using System.Net.Mail;
using eLearnApps.Business.Interface;
using eLearnApps.Core;
using eLearnApps.Core.Caching;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Extension;
using eLearnApps.Models;
using eLearnApps.ViewModel.FFTS;
using eLearnApps.ViewModel.Logging;
using Encoding = System.Text.Encoding;

namespace eLearnApps.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly SmtpClient _client;
        private readonly ILoggingService _loggingService;
        private readonly IUserService _userService;
        private readonly UserModel _userInfo;
        private LoggingModel? _loggingModel;
        private readonly int _courseId;
        private readonly Constants _constants;

        public EmailHelper(IServiceProvider serviceProvider,
            int userId, int courseId)
        {
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _loggingService = serviceProvider.GetRequiredService<ILoggingService>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _constants = new Constants(configuration);
            var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
            _client = new SmtpClient();
            _userInfo = cacheManager.Get<UserModel>(string.Format(_constants.KeyUserInfo, userId)) ?? new UserModel();
            _courseId = courseId;
        }

        /// <summary>
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public bool SendMail(string toEmail, string toName, string fromAddress, string fromName, string subject,
            string body, bool isBodyHtml = true)
        {
            var isSent = false;
            try
            {
                var mMailMessage = SettingMailMessage(toEmail, toName, fromAddress, fromName, subject, body, isBodyHtml);
                _client.Send(mMailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {
                _loggingModel = new LoggingModel
                {
                    UserId = _userInfo.UserId,
                    OrgUnitId = _courseId,
                    IpAddress = Constants.LocalIpAddress,
                    FullMessage = ex.StackTrace,
                    ShortMessage = ex.Message,
                    ToolId = nameof(EmailHelper)
                };
                _loggingService.Log(LogType.Error, _loggingModel);
            }
            finally
            {
                _client.Dispose();
            }

            return isSent;
        }

        /// <summary>
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public bool SendMailMultiple(List<string> toEmails, string fromAddress, string fromName, string subject,
            string body, bool isBodyHtml = true)
        {
            var isSent = false;
            try
            {
                var mMailMessage = SettingMailMessageMultiple(toEmails, fromAddress, fromName, subject, body, isBodyHtml);
                _client.Send(mMailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {
                _loggingModel = new LoggingModel
                {
                    UserId = _userInfo.UserId,
                    OrgUnitId = _courseId,
                    IpAddress = Constants.LocalIpAddress,
                    FullMessage = ex.StackTrace,
                    ShortMessage = ex.Message,
                    ToolId = nameof(EmailHelper)
                };
                _loggingService.Log(LogType.Error, _loggingModel);
            }
            finally
            {
                _client.Dispose();
            }

            return isSent;
        }

        /// <summary>
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public async Task<bool> SendMailAsync(string toEmail, string toName, string fromAddress, string fromName,
            string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                // NOTE: to reenable once we move back from cloud, to use self hosted solution
                var mMailMessage =
                    SettingMailMessage(toEmail, toName, fromAddress, fromName, subject, body, isBodyHtml);
                _client.SendCompleted += (sender, e) =>
                {
                    _loggingModel = new LoggingModel();
                    if (e.Cancelled)
                    {
                        _loggingModel.ShortMessage = $"send mail to [{toEmail}] cancelled.\n{e.ToString()}";
                        _loggingModel.FullMessage = _loggingModel.ShortMessage;
                        _loggingModel.Page = nameof(EmailHelper);
                        _loggingModel.UserId = _userInfo.UserId;
                        _loggingModel.OrgUnitId = _courseId;
                        _loggingModel.ToolId = nameof(ToolName.Ffts);
                        _loggingService.Log(LogType.Error, _loggingModel);
                    }
                    else
                    {
                        if (e.Error != null)
                        {
                            _loggingModel.ShortMessage = e.Error.Message;
                            _loggingModel.FullMessage = e.Error.StackTrace;
                            _loggingModel.Page = nameof(EmailHelper);
                            _loggingModel.UserId = _userInfo.UserId;
                            _loggingModel.OrgUnitId = _courseId;
                            _loggingModel.ToolId = nameof(ToolName.Ffts);
                            _loggingService.Log(LogType.Error, _loggingModel);

                        }
                    }
                };
                await _client.SendMailAsync(mMailMessage);

                return true;
            }
            catch (Exception ex)
            {
                _loggingModel = new LoggingModel
                {
                    ShortMessage = ex.Message,
                    FullMessage = ex.StackTrace,
                    Page = nameof(EmailHelper),
                    UserId = _userInfo.UserId,
                    OrgUnitId = _courseId,
                    ToolId = nameof(ToolName.Ffts)
                };

                _loggingService.Log(LogType.Error, _loggingModel);
                return false;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public bool SendMailWithAttachment(ToolName toolName, string toEmail, string toName, string fromAddress, string fromName,
            string subject, string body, Attachment attachments, bool isBodyHtml = true)
        {
            try
            {
                // NOTE: to reenable once we move back from cloud, to use self hosted solution
                var mMailMessage =
                    SettingMailMessage(toEmail, toName, fromAddress, fromName, subject, body, isBodyHtml);
                mMailMessage.Attachments.Add(attachments);

                _client.Send(mMailMessage);


                return true;
            }
            catch (Exception ex)
            {
                _loggingModel = new LoggingModel
                {
                    ShortMessage = ex.Message,
                    FullMessage = ex.StackTrace,
                    Page = nameof(EmailHelper),
                    UserId = _userInfo.UserId,
                    OrgUnitId = _courseId,
                    ToolId = toolName.ToString()
                };

                _loggingService.Log(LogType.Error, _loggingModel);
                return false;
            }
            finally
            {
                _client.Dispose();
            }
        }


        public async void SendMailInviteRespondedAlert(Meeting meeting, int attendeeId,
            List<MeetingAttendee> lstAttendees,
            InviteResponseType responseType, string rootPath)
        {
            var fullFileName = $"{rootPath}/Views/Ffts/_MailInviteResponseAlertTemplate.cshtml";
            var template = File.ReadAllText(fullFileName);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_constants.SingaporeStandardTime);
            var fromEmail = _constants.SystemMailAddress;
            var fromName = _constants.SystemMailName;
            var owner = _userService.GetById(meeting.OwnerId);
            var attendee = _userService.GetById(attendeeId);
            var lstInvitee = lstAttendees.Select(x => x.AttendeeId).ToList();
            var lstUserInvitee = _userService.GetListAttendeesById(lstInvitee);
            var userJoin = lstUserInvitee.Select(x => x.DisplayName).OrderBy(name => name).ToList();

            var start = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartDate, timeZoneInfo);
            var end = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndDate, timeZoneInfo);
            var starttime = $"{start:dd MMM yyyy HH:mm}";
            var endtime = $"{end:dd MMM yyyy HH:mm}";

            string emailDescription;
            switch (responseType)
            {
                case InviteResponseType.Accept:
                    emailDescription = $"{attendee.DisplayName} has accepted your meeting invite.";
                    break;
                case InviteResponseType.Reject:
                    emailDescription = $"{attendee.DisplayName} has rejected your meeting invite.";
                    break;
                case InviteResponseType.Expire:
                    emailDescription = "Meeting described below has expired.";
                    break;
                default:
                    emailDescription = "";
                    break;
            }

            var model = new TemplateViewModel
            {
                MeetingTitle = meeting.Title,
                OwnerName = owner.DisplayName,
                AttendeeName = attendee.DisplayName,
                Description = meeting.Description,
                Start = meeting.StartDate,
                End = meeting.EndDate,
                StartTimeInSGTimezone = starttime,
                EndTimeInSGTimezone = endtime,
                AttendeesName = userJoin,
                InviteUserStatus = (int)responseType,
                Location = meeting.Location,
                HeadingDescription = emailDescription
            };

            string subject;
            var title = $"{meeting.Title} @ {starttime} ({_constants.Gmt8})";
            switch (responseType)
            {
                case InviteResponseType.Accept:
                    subject = string.Format(_constants.SubjectInformInvitationAccept, attendee.DisplayName, title);
                    break;
                case InviteResponseType.Reject:
                    subject = string.Format(_constants.SubjectInformInvitationDecline, attendee.DisplayName, title);
                    break;
                case InviteResponseType.Expire:
                    subject = string.Format(_constants.SubjectInformInvitationExpire, attendee.DisplayName, title);
                    break;
                default:
                    subject = "";
                    break;
            }

            var mailContent = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), null, model);
            await SendMailAsync(owner.EmailAddress, owner.DisplayName, fromEmail, fromName, subject, mailContent);
        }

        public async void SendMailInvite(Meeting meeting,
            List<MeetingAttendee> lstAttendees, string rootPath)
        {
            var fullFileName = $"{rootPath}/Views/Ffts/_MailInviteeTemplate.cshtml";
            var template = File.ReadAllText(fullFileName);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_constants.SingaporeStandardTime);
            var fromEmail = _constants.SystemMailAddress;
            var fromName = _constants.SystemMailName;
            var owner = _userService.GetById(meeting.OwnerId);

            var count = lstAttendees?.Count;
            if (count > 0)
            {
                var lstInvitee = lstAttendees.Select(x => x.AttendeeId).ToList();
                var lstUserInvitee = _userService.GetListAttendeesById(lstInvitee);
                var userJoin = lstUserInvitee.Select(x => x.DisplayName).OrderBy(name => name).ToList();
                for (var i = 0; i < count; i++)
                {
                    var item = lstAttendees[i];
                    var hyperLink = $"{_constants.LinkInvite}?SecretKey={item.SecretKey}";
                    var user = lstUserInvitee.FirstOrDefault(x => x.Id == item.AttendeeId);

                    var start = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartDate, timeZoneInfo);
                    var end = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndDate, timeZoneInfo);
                    var starttime = $"{start:dd MMM yyyy HH:mm}";
                    var endtime = $"{end:dd MMM yyyy HH:mm}";
                    var model = new TemplateViewModel
                    {
                        MeetingTitle = meeting.Title,
                        OwnerName = owner.DisplayName,
                        Link = hyperLink,
                        AttendeeName = user?.DisplayName,
                        Description = meeting.Description,
                        Start = meeting.StartDate,
                        End = meeting.EndDate,
                        StartTimeInSGTimezone = starttime,
                        EndTimeInSGTimezone = endtime,
                        AttendeesName = userJoin,
                        InviteUserStatus = (int)InviteUserStatus.New,
                        Location = meeting.Location
                    };
                    var mailContent = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), null, model);
                    var subject = $"Invitation: {meeting.Title} @ {starttime}({Constants.Gmt8}) ({fromEmail})";
                    await SendMailAsync(user?.EmailAddress, user?.DisplayName, fromEmail, fromName, subject,
                        mailContent);
                }
            }
        }

        /// <summary>
        ///     Notify meeting attendee that meeting has been cancelled by host
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="attendee"></param>
        /// <param name="rootPath"></param>
        public async void SendMailCancel(Meeting meeting, List<MeetingAttendee> attendee, string rootPath)
        {
            var lstTemplate = new List<TemplateViewModel>();
            var fullFileName = $"{rootPath}/Views/Ffts/_MailInviteeTemplateChange.cshtml";
            var templateMail = File.ReadAllText(fullFileName);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_constants.SingaporeStandardTime);
            var fromEmail = _constants.SystemMailAddress;
            var fromName = _constants.SystemMailName;
            var owner = _userService.GetById(meeting.OwnerId);
            var count = attendee?.Count;
            for (var i = 0; i < count; i++)
            {
                var item = attendee[i];
                var start = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartDate, timeZoneInfo);
                var end = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndDate, timeZoneInfo);
                var starttime = $"{start:dd MMM yyyy HH:mm}";
                var endtime = $"{end:dd MMM yyyy HH:mm}";
                var user = _userService.GetById(item.AttendeeId);
                var model = new TemplateViewModel
                {
                    ToMailAddress = user.EmailAddress,
                    ToName = user.DisplayName,
                    MeetingTitle = meeting.Title,
                    OwnerName = owner.DisplayName,
                    AttendeeName = user?.DisplayName,
                    Description = meeting.Description,
                    Start = meeting.StartDate,
                    End = meeting.EndDate,
                    StartTimeInSGTimezone = starttime,
                    EndTimeInSGTimezone = endtime,
                    InviteUserStatus = (int)InviteUserStatus.Remove,
                    Location = meeting.Location,
                    AttendeesName = new List<string>()
                };

                var subject =
                    $"Meeting Canceled: {model.MeetingTitle} @ {starttime}({Constants.Gmt8}) ({owner.DisplayName})";
                model.Subject = subject;
                model.HeadingDescription = $"{owner.DisplayName} has canceled this meeting.";
                lstTemplate.Add(model);
            }

            count = lstTemplate.Count;
            for (var i = 0; i < count; i++)
            {
                var item = lstTemplate[i];
                var mailContent = Engine.Razor.RunCompile(templateMail, Guid.NewGuid().ToString(), null, item);
                await SendMailAsync(item.ToMailAddress, item.ToName, fromEmail, fromName, item.Subject,
                    mailContent);
            }
        }

        /// <summary>
        /// Notify attendee that owner has cancel this meeting
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="attendee"></param>
        /// <param name="rootPath"></param>
        public async void SendMailCancel(Meeting meeting, MeetingAttendee attendee, string rootPath)
        {
            var fullFileName = $"{rootPath}/Views/Ffts/_MailInviteeTemplateChange.cshtml";
            var template = File.ReadAllText(fullFileName);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_constants.SingaporeStandardTime);
            var fromEmail = _constants.SystemMailAddress;
            var fromName = _constants.SystemMailName;
            var owner = _userService.GetById(meeting.OwnerId);
            if (attendee != null)
            {
                var start = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartDate, timeZoneInfo);
                var end = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndDate, timeZoneInfo);
                var starttime = $"{start:dd MMM yyyy HH:mm}";
                var endtime = $"{end:dd MMM yyyy HH:mm}";
                var user = _userService.GetById(attendee.AttendeeId);
                var model = new TemplateViewModel
                {
                    MeetingTitle = meeting.Title,
                    OwnerName = owner.DisplayName,
                    Description = meeting.Description,
                    Start = meeting.StartDate,
                    End = meeting.EndDate,
                    StartTimeInSGTimezone = starttime,
                    EndTimeInSGTimezone = endtime,
                    InviteUserStatus = (int)InviteUserStatus.Remove,
                    Location = meeting.Location,
                    AttendeeName = user.DisplayName,
                    AttendeesName = new List<string>()
                };

                var subject = $"Canceled: {model.MeetingTitle} @ {starttime}({Constants.Gmt8}) ({owner.DisplayName})";
                model.Subject = subject;
                model.HeadingDescription = $"{user.DisplayName} has Canceled this meeting";
                var mailContent = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), null, model);
                await SendMailAsync(owner.EmailAddress, owner.DisplayName, fromEmail, fromName, subject, mailContent);
            }
        }

        /// <summary>
        ///     Modify meeting, send mail
        /// </summary>
        /// <param name="originalMeeting"></param>
        /// <param name="lstOriginalAttendees"></param>
        /// <param name="newMeeting"></param>
        /// <param name="lstNewAttendees"></param>
        /// <param name="rootPath"></param>
        public async void SendMailInvite(Meeting originalMeeting, List<MeetingAttendee> lstOriginalAttendees,
            Meeting newMeeting, List<MeetingAttendee> lstNewAttendees, string rootPath)
        {
            try
            {
                var countOriginal = lstOriginalAttendees?.Count;
                var countNew = lstNewAttendees?.Count;
                var lstTemplate = new List<TemplateViewModel>();
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_constants.SingaporeStandardTime);
                var fromEmail = _constants.SystemMailAddress;
                var fromName = _constants.SystemMailName;
                var fullFileName = $"{rootPath}/Views/Ffts/_MailInviteeTemplateChange.cshtml";
                var templateMail = File.ReadAllText(fullFileName);
                var owner = _userService.GetById(originalMeeting.OwnerId);
                var lstUserAttendees = new List<int>();
                if (countOriginal > 0)
                {
                    var lstInvitee = lstOriginalAttendees.Select(x => x.AttendeeId).ToList();
                    var lstUserInvitee = _userService.GetListAttendeesById(lstInvitee);
                    var userJoin = lstUserInvitee.Select(x => x.DisplayName).OrderBy(name => name).ToList();

                    var updatedUserInvitee =
                        _userService.GetListAttendeesById(lstNewAttendees.Select(usr => usr.AttendeeId).ToList());
                    var updatedUserJoin = updatedUserInvitee.Select(x => x.DisplayName).OrderBy(name => name).ToList();

                    for (var i = 0; i < countOriginal; i++)
                    {
                        var item = lstOriginalAttendees[i];
                        var newItem = lstNewAttendees?.FirstOrDefault(x => x.AttendeeId == item.AttendeeId);
                        var user = lstUserInvitee.FirstOrDefault(x => x.Id == item.AttendeeId);
                        var start = TimeZoneInfo.ConvertTimeFromUtc(originalMeeting.StartDate, timeZoneInfo);
                        var end = TimeZoneInfo.ConvertTimeFromUtc(originalMeeting.EndDate, timeZoneInfo);
                        var starttime = $"{start:dd MMM yyyy HH:mm}";
                        var endtime = $"{end:dd MMM yyyy HH:mm}";

                        var template = new TemplateViewModel
                        {
                            Description = originalMeeting.Description,
                            End = originalMeeting.EndDate,
                            Start = originalMeeting.StartDate,
                            InviteUserStatus = (int)InviteUserStatus.Modified,
                            MeetingTitle = originalMeeting.Title,
                            AttendeeName = user?.DisplayName,
                            AttendeesName = userJoin,
                            OwnerName = owner.DisplayName,
                            StartTimeInSGTimezone = starttime,
                            EndTimeInSGTimezone = endtime,
                            AttendeeId = item.AttendeeId,
                            ToMailAddress = user?.EmailAddress,
                            ToName = user?.DisplayName,
                            Location = originalMeeting.Location
                        };
                        if (newItem == null)
                        {
                            // for user who was removed from meeting
                            var subject =
                                $"Canceled: {template.MeetingTitle} @ {starttime}({Constants.Gmt8}) ({owner.DisplayName})";
                            template.InviteUserStatus = (int)InviteUserStatus.Remove;
                            template.Subject = subject;
                            template.HeadingDescription = $"{owner.DisplayName} has Canceled this meeting";
                        }
                        else
                        {
                            var subject =
                                $"Updated invitation: {template.MeetingTitle} @ {starttime}({Constants.Gmt8}) ({owner.DisplayName})";
                            template.InviteUserStatus = (int)InviteUserStatus.Modified;
                            template.Subject = subject;
                            var hyperLink = $"{_constants.LinkInvite}?SecretKey={newItem.SecretKey}";
                            template.Link = hyperLink;
                            template.AttendeesName = updatedUserJoin;

                            lstUserAttendees.Add(item.AttendeeId);
                        }

                        lstTemplate.Add(template);
                    }
                }

                if (countNew > 0)
                {
                    var lstInvitee = lstNewAttendees.Select(x => x.AttendeeId).ToList();
                    lstInvitee.AddRange(lstUserAttendees);
                    var lstUserInvitee = _userService.GetListAttendeesById(lstInvitee);
                    var userJoin = lstUserInvitee.Select(x => x.DisplayName).OrderBy(name => name).ToList();
                    for (var i = 0; i < countNew; i++)
                    {
                        var item = lstNewAttendees[i];
                        var originalItem = lstOriginalAttendees?.FirstOrDefault(x => x.AttendeeId == item.AttendeeId);
                        if (originalItem == null)
                        {
                            var hyperLink =
                                $"{_constants.LinkInvite}?SecretKey={item.SecretKey}";
                            var user = lstUserInvitee.FirstOrDefault(x => x.Id == item.AttendeeId);
                            var start = TimeZoneInfo.ConvertTimeFromUtc(newMeeting.StartDate, timeZoneInfo);
                            var end = TimeZoneInfo.ConvertTimeFromUtc(newMeeting.EndDate, timeZoneInfo);
                            var starttime = $"{start:dd MMM yyyy HH:mm}";
                            var endtime = $"{end:dd MMM yyyy HH:mm}";

                            var template = new TemplateViewModel
                            {
                                Description = newMeeting.Description,
                                End = newMeeting.EndDate,
                                Start = newMeeting.StartDate,
                                InviteUserStatus = (int)InviteUserStatus.Modified,
                                MeetingTitle = newMeeting.Title,
                                AttendeeName = user?.DisplayName,
                                AttendeesName = userJoin,
                                Link = hyperLink,
                                OwnerName = owner.DisplayName,
                                StartTimeInSGTimezone = starttime,
                                EndTimeInSGTimezone = endtime,
                                AttendeeId = item.AttendeeId,
                                ToMailAddress = user?.EmailAddress,
                                ToName = user?.DisplayName,
                                Location = newMeeting.Location
                            };
                            var subject =
                                $"Invitation: {template.MeetingTitle} @ {starttime}({_constants.Gmt8}) ({owner.DisplayName})";
                            template.InviteUserStatus = (int)InviteUserStatus.New;
                            template.Subject = subject;
                            lstTemplate.Add(template);
                        }
                    }
                }

                var count = lstTemplate.Count;
                for (var i = 0; i < count; i++)
                {
                    var item = lstTemplate[i];
                    var mailContent = Engine.Razor.RunCompile(templateMail, Guid.NewGuid().ToString(), null, item);
                    await SendMailAsync(item.ToMailAddress, item.ToName, fromEmail, fromName, item.Subject,
                        mailContent);
                }
            }
            catch (Exception e)
            {
                _loggingModel = new LoggingModel
                {
                    ShortMessage = e.Message,
                    FullMessage = e.StackTrace,
                    Page = nameof(EmailHelper),
                    UserId = _userInfo.UserId,
                    OrgUnitId = _courseId,
                    ToolId = nameof(ToolName.Ffts)
                };
                _loggingService.Log(LogType.Error, _loggingModel);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        private MailMessage SettingMailMessage(string toEmail, string toName, string fromAddress, string fromName,
            string subject, string body, bool isBodyHtml = true)
        {
            if (string.IsNullOrEmpty(fromAddress) || string.IsNullOrWhiteSpace(fromAddress))
                throw new ArgumentException("Email is null. Cannot send an email from " + fromName);
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Email is null. Cannot send an email to " + toName);
            var mMailMessage = new MailMessage { From = new MailAddress(fromAddress, fromName) };
            mMailMessage.To.Add(new MailAddress(toEmail, toName));
            mMailMessage.SubjectEncoding = Encoding.UTF8;
            mMailMessage.BodyEncoding = Encoding.UTF8;
            mMailMessage.Subject = subject;
            mMailMessage.Body = body;
            mMailMessage.IsBodyHtml = isBodyHtml;
            return mMailMessage;
        }

        /// <summary>
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        private MailMessage SettingMailMessageMultiple(List<string> toEmails, string fromAddress, string fromName,
            string subject, string body, bool isBodyHtml = true)
        {
            if (string.IsNullOrEmpty(fromAddress) || string.IsNullOrWhiteSpace(fromAddress))
                throw new ArgumentException("Email is null. Cannot send an email from " + fromName);
            if (toEmails == null || toEmails.Count == 0)
                throw new ArgumentException("Email is null. Cannot send an email to empty recipient");
            var mMailMessage = new MailMessage { From = new MailAddress(fromAddress, fromName) };

            foreach (var toEmail in toEmails)
                mMailMessage.To.Add(new MailAddress(toEmail, toEmail));

            mMailMessage.SubjectEncoding = Encoding.UTF8;
            mMailMessage.BodyEncoding = Encoding.UTF8;
            mMailMessage.Subject = subject;
            mMailMessage.Body = body;
            mMailMessage.IsBodyHtml = isBodyHtml;
            return mMailMessage;
        }
        public async void SendMailNotifyGradeReleaseUpdate(string content,
            List<UserModel> users)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_constants.SingaporeStandardTime);
            var dateTimeNow = $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo):dd MMM yyyy HH:mm}";
            var fromEmail = _constants.SystemMailAddress;
            var fromName = _constants.SystemMailName;
            foreach (var user in users)
            {
                var subject = $"Grade Release Update - {dateTimeNow}({_constants.Gmt8})";
                await SendMailAsync(user?.EmailAddress, user?.DisplayName, fromEmail, fromName, subject,
                    string.Format(content, user?.DisplayName));

            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string GetEmailTemplate(string templateFileName)
        {

            var path = $"{CommonHelper.MapPath(_constants.StaticFilesFolder)}/Templates/Email";

            var fullFilePath = $"{path}" + Path.DirectorySeparatorChar + $"{templateFileName}";
            if (File.Exists(fullFilePath))
            {
                var content = File.ReadAllText(fullFilePath);
                return content;
            }
            return string.Empty;
        }
    }
}