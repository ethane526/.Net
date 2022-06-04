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
    public class SurveysInstancesService : ISurveysInstancesService
    {
        IDataProvider _data = null;
        public SurveysInstancesService(IDataProvider data)
        {
            _data = data;
        }

        public int Add(int surveyId, int userId)
        {
            string procName = "[dbo].[SurveysInstances_Insert]";
            int id = 0;
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(surveyId, userId, col);
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

        public void Update(int id, int surveyId, int userId)
        {
            string procName = "[dbo].[SurveysInstances_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(surveyId, userId, col);
                col.AddWithValue("@Id", id);
            }, returnParameters: null);
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[SurveysInstances_Delete_ById]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            });
        }

        public SurveyInstance GetById(int id)
        {
            string procName = "[dbo].[SurveysInstances_Select_ById]";
            SurveyInstance instance = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                instance = MapSurveyInstance(reader, ref startingIndex);
            });
            return instance;
        }

        public Paged<SurveyInstance> Pagination(int pageIdex, int pageSize)
        {
            Paged<SurveyInstance> pagedList = null;
            List<SurveyInstance> list = null;
            int totalCount = 0;

            string procName = "[dbo].[SurveysInstances_SelectAll]";

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
           {
               col.AddWithValue("@PageIndex", pageIdex);
               col.AddWithValue("@PageSize", pageSize);
           }, delegate (IDataReader reader, short set)
           {
               int startingIndex = 0;

               SurveyInstance instance = MapSurveyInstance(reader, ref startingIndex);

               if (totalCount == 0)
               {
                   totalCount = reader.GetSafeInt32(startingIndex++);
               };
               if (list == null)
               {
                   list = new List<SurveyInstance>();
               }
               list.Add(instance);
           });
            if (list != null)
            {
                pagedList = new Paged<SurveyInstance>(list, pageIdex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<SurveyInstance> GetByCreatedBy(int pageIndex, int pageSize, int createdBy)
        {
            Paged<SurveyInstance> pagedList = null;
            List<SurveyInstance> list = null;
            int totalCount = 0;

            string procName = "[dbo].[SurveysInstances_Select_ByCreatedBy]";

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@UserId", createdBy);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;

                SurveyInstance instance = MapSurveyInstance(reader, ref startingIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<SurveyInstance>();
                }
                list.Add(instance);
            });
            if (list != null)
            {
                pagedList = new Paged<SurveyInstance>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public List<SurveyInstance> GetAll()
        {
            List<SurveyInstance> list = null;
            string procName = "[dbo].[SurveysInstances_SelectAllV2]";
            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    SurveyInstance instance = MapSurveyInstance(reader, ref startingIndex);
                    if (list == null)
                    {
                        list = new List<SurveyInstance>();
                    }
                    list.Add(instance);
                });
            return list;
        }

        private static void AddCommonParams(int surveyId, int userId,SqlParameterCollection col)
        {
            col.AddWithValue("@SurveyId", surveyId);
            col.AddWithValue("@UserId", userId);
        }

        private SurveyInstance MapSurveyInstance(IDataReader reader, ref int startingIndex)
        {
            SurveyInstance surveyInstance = new SurveyInstance();

            surveyInstance.Id = reader.GetSafeInt32(startingIndex++);
            surveyInstance.Survey = reader.GetSafeString(startingIndex++);
            surveyInstance.User = reader.GetSafeString(startingIndex++);
            surveyInstance.SurveyAnswer = reader.GetSafeString(startingIndex++);
            surveyInstance.DateCreated = reader.GetSafeDateTime(startingIndex++);
            surveyInstance.DateModified = reader.GetSafeDateTime(startingIndex++);

            return surveyInstance;

        }
    }
}
