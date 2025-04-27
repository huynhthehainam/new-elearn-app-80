using System.Collections.Generic;
using System.Threading.Tasks;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Core;
using eLearnApps.Models;
using System.Net.Mail;

namespace eLearnApps.Helpers
{
    public interface IEmailHelper
    {
        bool SendMail(string toEmail, string toName, string fromAddress, string fromName, string subject, string body, bool isBodyHtml = true);
        Task<bool> SendMailAsync(string toEmail, string toName, string fromAddress, string fromName, string subject, string body, bool isBodyHtml = true);
        bool SendMailWithAttachment(ToolName toolName, string toEmail, string toName, string fromAddress, string fromName, string subject, string body, Attachment attachments, bool isBodyHtml = true);
        Task SendMailInvite(Meeting meeting, List<MeetingAttendee> lstAttendees, string rootPath);
        Task SendMailCancel(Meeting meeting, List<MeetingAttendee> attendee, string rootPath);
        Task SendMailCancel(Meeting meeting, MeetingAttendee attendee, string rootPath);
        Task SendMailInviteRespondedAlert(Meeting meeting, int attendeeId, List<MeetingAttendee> lstAttendees, InviteResponseType responseType, string rootPath);
        Task SendMailInvite(Meeting originalMeeting, List<MeetingAttendee> lstOriginalAttendees, Meeting newMeeting, List<MeetingAttendee> lstNewAttendees, string rootPath);
        Task SendMailNotifyGradeReleaseUpdate(string content, List<UserModel> users);
    }
}