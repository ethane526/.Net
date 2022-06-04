using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Survey;
using Sabio.Models.Requests.Surveys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class SurveyQuestionService : ISurveyQuestionService
    {
        IDataProvider _data = null;
        public SurveyQuestionService(IDataProvider data)
        {
            _data = data;
        }

        public int Add(SurveyQuestionAddRequest model, int userId)
        {
            string procName = "[dbo].[SurveyQuestions_Insert]";
            int id = 0;

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col, userId);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);
            });
            return id;

        }

        public void Update(SurveyQuestionUpdateRequest model, int userId)
        {
            string procName = "[dbo].[SurveyQuestions_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col, userId);
                col.AddWithValue("@Id", model.Id);
            }, returnParameters: null);
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[SurveyQuestions_Delete_ById]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            });
        }

        public SurveyQuestion GetById(int id)
        {
            string procName = "[dbo].[SurveyQuestions_Select_ById]";
            SurveyQuestion question = null;
            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                question = MapSurveyQuestions(reader, ref startingIndex);
            });
            return question;
        }

        public Paged<SurveyQuestion> Pagination(int pageIndex, int pageSize)
        {
            Paged<SurveyQuestion> pagedList = null;
            List<SurveyQuestion> list = null;
            int totalCount = 0;
            string procName = "[dbo].[SurveyQuestions_SelectAll]";
            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@pageIndex", pageIndex);
                col.AddWithValue("@pageSize", pageSize);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                SurveyQuestion sQuestion = MapSurveyQuestions(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<SurveyQuestion>();
                }
                list.Add(sQuestion);
            });
            if (list != null)
            {
                pagedList = new Paged<SurveyQuestion>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<SurveyQuestion> GetByCreatedBy(int pageIndex, int pageSize, int createdBy)
        {
            Paged<SurveyQuestion> pagedList = null;
            List<SurveyQuestion> list = null;
            int totalCount = 0;
            string procName = "[dbo].[SurveyQuestions_Select_ByCreatedBy]";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@pageIndex", pageIndex);
                col.AddWithValue("@pageSize", pageSize);
                col.AddWithValue("@UserId", createdBy);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                SurveyQuestion sQuestion = MapSurveyQuestions(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<SurveyQuestion>();
                }
                list.Add(sQuestion);
            });
            if (list != null)
            {
                pagedList = new Paged<SurveyQuestion>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        private static void AddCommonParams(SurveyQuestionAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@UserId", userId);
            col.AddWithValue("@Question", model.Question);
            col.AddWithValue("@HelpText", model.HelpText);
            col.AddWithValue("@IsRequired", model.IsRequired);
            col.AddWithValue("@IsMultipleAllowed", model.IsMultipleAllowed);
            col.AddWithValue("@QuestionTypeId", model.QuestionTypeId);
            col.AddWithValue("@SurveyId", model.SurveyId);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@SortOrder", model.SortOrder);
        }

        private SurveyQuestion MapSurveyQuestions(IDataReader reader, ref int startingIndex)
        {
            SurveyQuestion sQuestion = new SurveyQuestion();
            sQuestion.Id = reader.GetSafeInt32(startingIndex++);
            sQuestion.UserId = reader.GetSafeInt32(startingIndex++);
            sQuestion.Question = reader.GetSafeString(startingIndex++);
            sQuestion.HelpText = reader.GetSafeString(startingIndex++);
            sQuestion.IsRequired = reader.GetSafeBool(startingIndex++);
            sQuestion.IsMultipleAllowed = reader.GetSafeBool(startingIndex++);
            sQuestion.QuestionType = reader.GetSafeString(startingIndex++);
            sQuestion.SurveyId = reader.GetSafeInt32(startingIndex++);
            sQuestion.Status = reader.GetSafeString(startingIndex++);
            sQuestion.SortOrder = reader.GetSafeInt32(startingIndex++);
            sQuestion.DateCreated = reader.GetSafeDateTime(startingIndex++);
            sQuestion.DateModified = reader.GetSafeDateTime(startingIndex++);
            return sQuestion;
        }
    }
}
