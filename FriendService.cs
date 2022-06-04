using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class FriendService : IFriendService
    {
        IDataProvider _data = null;

        public FriendService(IDataProvider data)
        {
            _data = data;
        }

        public FriendV3 GetV3(int id)
        {
            string procName = "[dbo].[Friends_SelectByIdV3]";
            FriendV3 friendV3 = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                friendV3 = MapSingleFriendV3(reader, ref startingIndex);
            }
            );
            return friendV3;
        }

        public List<FriendV3> GetAllV3()
        {
            List<FriendV3> list = null;
            string procName = "[dbo].[Friends_SelectAllV3]";
            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    FriendV3 friend = MapSingleFriendV3(reader, ref startingIndex);
                    if (list == null)
                    {
                        list = new List<FriendV3>();
                    }
                    list.Add(friend);
                }
                );
            return list;
        }

        public int AddV3(FriendAddRequestV3 friendModel, int currentUserId)
        {
            int id = 0;
            string procName = "[dbo].[Friends_InsertV3]";
            DataTable dt = MapSkillsToTable(friendModel.Skills);
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParamsV3(friendModel, col, dt);
                col.AddWithValue("@UserId", currentUserId);

                SqlParameter idOut = new SqlParameter("@FriendId", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);

            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@FriendId"].Value;
                int.TryParse(oId.ToString(), out id);
            }
            );
            return id;
        }

        public void UpdateV3(FriendUpdateRequestV3 friendModel, int currentUserId)
        {
            string procName = "[dbo].[Friends_UpdateV3]";
            DataTable dt = MapSkillsToTable(friendModel.Skills);
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParamsV3(friendModel, col, dt);
                col.AddWithValue("@UserId", currentUserId);
                col.AddWithValue("@FriendId", friendModel.Id);

            }, returnParameters: null);

        }

        public void DeleteV3(int Id)
        {
            string procName = "[dbo].[Friends_DeleteV3]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", Id);

            }, returnParameters: null);
        }

        public Paged<FriendV3> PaginationV3(int PageIndex, int PageSize)
        {
            Paged<FriendV3> pagedResult = null;
            List<FriendV3> listResult = null;
            int totalCount = 0;

            string procName = "[dbo].[Friends_PaginationV3]";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", PageIndex);
                parameterCollection.AddWithValue("@PageSize", PageSize);

            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                FriendV3 friendV3 = MapSingleFriendV3(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                };

                if (listResult == null)
                {
                    listResult = new List<FriendV3>();
                }
                listResult.Add(friendV3);
            });
            if (listResult != null)
            {
                pagedResult = new Paged<FriendV3>(listResult, PageIndex, PageSize, totalCount);
            };
            return pagedResult;
        }

        public Paged<FriendV3> SearchV3(int PageIndex, int PageSize, string Query)
        {
            Paged<FriendV3> pagedResult = null;
            List<FriendV3> listResult = null;
            int totalCount = 0;

            string procName = "[dbo].[Friends_Search_PaginationV3]";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", PageIndex);
                parameterCollection.AddWithValue("@PageSize", PageSize);
                parameterCollection.AddWithValue("@Query", Query);

            }, delegate (IDataReader reader, short set)
            {
                
                int startingIndex = 0;
                FriendV3 friendV3 = MapSingleFriendV3(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                };

                if (listResult == null)
                {
                    listResult = new List<FriendV3>();
                }
                listResult.Add(friendV3);
            });
            if (listResult != null)
            {
                pagedResult = new Paged<FriendV3>(listResult, PageIndex, PageSize, totalCount);
            };
            return pagedResult;
        }

        private static FriendV3 MapSingleFriendV3(IDataReader reader, ref int startingIndex)
        {
            FriendV3 friendV3 = new FriendV3();
            friendV3.PrimaryImage = new Image();

            friendV3.Id = reader.GetSafeInt32(startingIndex++);
            friendV3.PrimaryImage.Url = reader.GetSafeString(startingIndex++);
            friendV3.PrimaryImage.Id = reader.GetSafeInt32(startingIndex++);
            friendV3.PrimaryImage.TypeId = reader.GetSafeInt32(startingIndex++);
            friendV3.Title = reader.GetSafeString(startingIndex++);
            friendV3.Bio = reader.GetSafeString(startingIndex++);
            friendV3.Summary = reader.GetSafeString(startingIndex++);
            friendV3.Headline = reader.GetSafeString(startingIndex++);
            friendV3.Slug = reader.GetSafeString(startingIndex++);
            friendV3.StatusId = reader.GetSafeInt32(startingIndex++);
            friendV3.DateCreated = reader.GetSafeDateTime(startingIndex++);
            friendV3.DateModified = reader.GetSafeDateTime(startingIndex++);
            friendV3.UserId = reader.GetSafeInt32(startingIndex++);
            friendV3.Skills = reader.DeserializeObject<List<Skill>>(startingIndex++);

            return friendV3;

        }

        private static void AddCommonParamsV3(FriendAddRequestV3 friendModel, SqlParameterCollection col, DataTable dt)
        {
            col.AddWithValue("@Title", friendModel.Title);
            col.AddWithValue("@Bio", friendModel.Bio);
            col.AddWithValue("@Summary", friendModel.Summary);
            col.AddWithValue("@Headline", friendModel.Headline);
            col.AddWithValue("@Slug", friendModel.Slug);
            col.AddWithValue("@StatusId", friendModel.StatusId);
            col.AddWithValue("@PrimaryImageUrl", friendModel.PrimaryImage.Url);
            col.AddWithValue("@TypeId", friendModel.PrimaryImage.TypeId);
            col.AddWithValue("@newSkills", dt);

        }
        private DataTable MapSkillsToTable(List<string> skillsToMap)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));

            if (skillsToMap != null)
            {
                foreach (string singleSkill in skillsToMap)
                {
                    DataRow dr = dt.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleSkill);
                    dt.Rows.Add(dr);

                }

            }
            return dt;
        }

    }
}
