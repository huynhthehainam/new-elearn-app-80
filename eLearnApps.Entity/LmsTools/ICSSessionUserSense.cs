using System;
using eLearnApps.Core;

namespace eLearnApps.Entity.LmsTools
{
    public class ICSSessionUserSense : BaseEntity
    {
        public int Id { get; set; }
        public int ICSSessionId { get; set; }
        public int UserId { get; set; }

        /// <summary>
        ///     Value is in Constants.Senses
        /// </summary>
        public Senses Sense { get; set; }

        /// <summary>
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}