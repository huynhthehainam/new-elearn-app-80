using eLearnApps.Entity.LmsTools.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eLearnApps.Helpers
{
    public static class SeedDataHelper
    {
        public static string GenerateSeedQuery(SeedData seedData)
        {
            if(seedData == null) return null;

            StringBuilder sbSeed = new StringBuilder();
            
            sbSeed.Append($"-- =============================================\r\n-- Author: Uy\r\n-- Create Date: {DateTime.Now.ToString("dd MMM yyyy")}\r\n-- Description:\r\n-- Initial data injection to create sample evaluation data.\r\n-- This script is only for simplicity of development.\r\n-- =============================================\r\n").AppendLine().AppendLine();

            //Seed for PeerFeedbackQuestion
            sbSeed.Append("--Seed for PeerFeedbackQuestion").AppendLine();
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackQuestion] ON").AppendLine();
            sbSeed.Append("GO").AppendLine();
            foreach (var item in seedData.PeerFeedbackQuestions)
            {
                var s1 = $"IF NOT EXISTS (SELECT * FROM PeerFeedbackQuestion WHERE Id = '{item.Id}')";
                sbSeed.Append(s1).AppendLine();
                sbSeed.Append("BEGIN").AppendLine();
                sbSeed.Append($"    INSERT [dbo].[PeerFeedbackQuestion] ([Id], [Title], [Description], [Deleted]) VALUES ({item.Id}, N'{item.Title.Replace("'", "''")}', N'{item.Description.Replace("'", "''")}', {(item.Deleted ? 1 : 0)})").AppendLine();
                sbSeed.Append("END").AppendLine();
            }
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackQuestion] OFF").AppendLine();
            sbSeed.Append("GO").AppendLine().AppendLine().AppendLine();

            //Seed for PeerFeedbackRatingQuestion
            sbSeed.Append("--Seed for PeerFeedbackRatingQuestion").AppendLine();
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackRatingQuestion] ON").AppendLine();
            sbSeed.Append("GO").AppendLine();
            foreach (var item in seedData.PeerFeedbackRatingQuestions)
            {
                var s1 = $"IF NOT EXISTS (SELECT * FROM PeerFeedbackRatingQuestion WHERE Id = '{item.Id}')";
                sbSeed.Append(s1).AppendLine();
                sbSeed.Append("BEGIN").AppendLine();
                sbSeed.Append($"    INSERT [dbo].[PeerFeedbackRatingQuestion] ([Id], [Name], [Deleted], [DisplayOrder]) VALUES ({item.Id}, N'{item.Name.Replace("'", "''")}', {(item.Deleted ? 1 : 0)}, {item.DisplayOrder})").AppendLine();
                sbSeed.Append("END").AppendLine();
            }
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackRatingQuestion] OFF").AppendLine();
            sbSeed.Append("GO").AppendLine().AppendLine().AppendLine();

            //Seed for PeerFeedbackRatingOption
            sbSeed.Append("--Seed for PeerFeedbackRatingOption").AppendLine();
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackRatingOption] ON").AppendLine();
            sbSeed.Append("GO").AppendLine();
            foreach (var item in seedData.PeerFeedbackRatingOptions)
            {
                var s1 = $"IF NOT EXISTS (SELECT * FROM PeerFeedbackRatingOption WHERE Id = '{item.Id}')";
                sbSeed.Append(s1).AppendLine();
                sbSeed.Append("BEGIN").AppendLine();
                sbSeed.Append($" INSERT [dbo].[PeerFeedbackRatingOption] ([Id], [Name], [Deleted]) VALUES ({item.Id}, N'{item.Name.Replace("'", "''")}', {(item.Deleted ? 1 : 0)}) ").AppendLine();
                sbSeed.Append("END").AppendLine();
            }
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackRatingOption] OFF").AppendLine();
            sbSeed.Append("GO").AppendLine().AppendLine().AppendLine();

            //Seed for PeerFeedbackQuestionRatingMap
            sbSeed.Append("--Seed for PeerFeedbackQuestionRatingMap").AppendLine();
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackQuestionRatingMap] ON").AppendLine();
            sbSeed.Append("GO").AppendLine();
            foreach (var item in seedData.PeerFeedbackQuestionRatingMaps)
            {
                var s1 = $"IF NOT EXISTS (SELECT * FROM PeerFeedbackQuestionRatingMap WHERE Id = '{item.Id}')";
                sbSeed.Append(s1).AppendLine();
                sbSeed.Append("BEGIN").AppendLine();
                sbSeed.Append($" INSERT [dbo].[PeerFeedbackQuestionRatingMap] ([Id], [QuestionId], [RatingOptionId], [RatingQuestionId]) VALUES ({item.Id}, {item.QuestionId}, {item.RatingOptionId}, {item.RatingQuestionId})").AppendLine();
                sbSeed.Append("END").AppendLine();
            }
            sbSeed.Append("SET IDENTITY_INSERT [dbo].[PeerFeedbackQuestionRatingMap] OFF").AppendLine();
            sbSeed.Append("GO").AppendLine().AppendLine().AppendLine();

            return sbSeed.ToString();
        }
    }
}