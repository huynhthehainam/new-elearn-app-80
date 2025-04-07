using System;
using System.Collections.Generic;

namespace eLearnApps.ViewModel.ICS
{
    public class SessionDetailViewModel
    {
        public SessionViewModel Session { get; set; }
        public List<LearningPointViewModel> LearningPoints { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public List<UserSenseViewModel> UserSenses { get; set; }
        public List<LearningPointCheckViewModel> LearningPointChecks { get; set; }
        public bool IsActiveSession { get; set; }
        public bool IsInstructorPreviewAsStudent { get; set; }

        /// <summary>
        /// Do not show UI when preview with Datetime > session datetime
        /// </summary>
        public bool ShowUI { get; set; }
    }
}