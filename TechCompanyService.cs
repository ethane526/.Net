using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Domain.TechCompanies;
using Sabio.Models.Requests.Friends;
using Sabio.Models.Requests.TechCompanies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class TechCompanyService : ITechCompanyService
    {
        IDataProvider _data = null;
        public TechCompanyService(IDataProvider data)
        {
            _data = data;
        }

        public TechCompany Get(int id)
        {
            string procName = "[dbo].[TechComp_SelectById]";
            TechCompany company = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                company = MapSingleCompany(reader, ref startingIndex);

            });
            return company;

        }

        public List<TechCompany> GetAll()
        {
            List<TechCompany> list = null;
            string procName = "[dbo].[TechComp_SelectAll]";
            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    TechCompany company = MapSingleCompany(reader, ref startingIndex);
                    if (list == null)
                    {
                        list = new List<TechCompany>();
                    }
                    list.Add(company);
                });
            return list;
        }

        public int Add(TechCompanyAddRequest companyModel)
        {
            int id = 0;
            string procName = "[dbo].[TechComp_Insert]";
            DataTable dtTags = MapTagsToTable(companyModel.Tags);
            DataTable dtUrls = MapUrlsToTable(companyModel.Urls);
            DataTable dtImages = MapImagesToTable(companyModel.Images);
            DataTable dtFriendIds = MapFriendIdsToTable(companyModel.FriendIds);
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(col, companyModel, dtTags, dtUrls, dtImages, dtFriendIds);
                SqlParameter idOut = new SqlParameter("@TechCompId", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@TechCompId"].Value;
                int.TryParse(oId.ToString(), out id);
            });
            return id;
        }

        public void Update(TechCompanyUpdateRequest companyModel)
        {
            string procName = "[dbo].[TechComp_Update]";
            DataTable dtTags = MapTagsToTable(companyModel.Tags);
            DataTable dtUrls = MapUrlsToTable(companyModel.Urls);
            DataTable dtImages = MapImagesToTable(companyModel.Images);
            DataTable dtFriendIds = MapFriendIdsToTable(companyModel.FriendIds);
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@TechCompId", companyModel.Id);
                AddCommonParams(col, companyModel,  dtTags, dtUrls, dtImages, dtFriendIds);
               

            }, returnParameters: null);

        }

        public void Delete(int id)
        {
            string procName = "[dbo].[TechComp_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@TechCompId", id);
            }, returnParameters: null);
        }

        //public Paged<TechCompany> Pagination(int PageIndex, int PageSize)
        //{
        //    Paged<TechCompany> pagedResult = null;
        //    List<TechCompany> listResult = null;
        //    int totalCount = 0;
        //    string procName = "[dbo].[TechComp_Pagination]";
        //    _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
        //    {
        //        col.AddWithValue("@PageIndex", PageIndex);
        //        col.AddWithValue("@PageSize", PageSize);

        //    }, delegate(IDataReader reader, short set)
        //    {
        //        int startingIndex= 0;
        //        TechCompany company = MapSingleCompany(reader, ref startingIndex);

        //    })

        //}

        private static TechCompany MapSingleCompany(IDataReader reader, ref int startingIdex)
        {
            TechCompany company = new TechCompany();
            company.Id = reader.GetSafeInt32(startingIdex++);
            company.Name = reader.GetSafeString(startingIdex++);
            company.Profile = reader.GetSafeString(startingIdex++);
            company.Summary = reader.GetSafeString(startingIdex++);
            company.Headline = reader.GetSafeString(startingIdex++);
            company.ContactInformation = reader.GetSafeString(startingIdex++);
            company.Slug = reader.GetSafeString(startingIdex++);
            company.StatusId = reader.GetSafeInt32(startingIdex++);
            company.DateCreated = reader.GetSafeDateTime(startingIdex++);
            company.DateModified = reader.GetSafeDateTime(startingIdex++);
            company.Images = reader.DeserializeObject<List<Image>>(startingIdex++);
            company.Urls = reader.DeserializeObject<List<Urls>>(startingIdex++);
            company.Tags = reader.DeserializeObject<List<Tag>>(startingIdex++);
            company.FriendIds = reader.DeserializeObject<List<FriendId>>(startingIdex++);

            return company;
        }

        private static void AddCommonParams(SqlParameterCollection col, TechCompanyAddRequest companyModel,  DataTable dtTags, DataTable dtUrls, DataTable dtImages, DataTable dtFriendIds)
        {
            col.AddWithValue("@Name", companyModel.Name);
            col.AddWithValue("@Profile", companyModel.Profile);
            col.AddWithValue("@Summary", companyModel.Summary);
            col.AddWithValue("@Headline", companyModel.Headline);
            col.AddWithValue("@ContactInformation", companyModel.ContactInformation);
            col.AddWithValue("@Slug", companyModel.Slug);
            col.AddWithValue("@StatusId", companyModel.StatusId);
            col.AddWithValue("@newTags", dtTags);
            col.AddWithValue("@newUrls",dtUrls);
            col.AddWithValue("@newImages", dtImages);
            col.AddWithValue("@newFriendIds", dtFriendIds);
        }

        private DataTable MapUrlsToTable(List<string> urlsToMap)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Url", typeof(string));

            if (urlsToMap != null)
            {
                foreach (string singleUrl in urlsToMap)
                {
                    DataRow dr = dt.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleUrl);
                    dt.Rows.Add(dr);
                }
            }
            return dt;            
        }
        private DataTable MapTagsToTable(List<string> tagsToMap)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Tags", typeof (string));

            if (tagsToMap != null)
            {
                foreach (string singleTag in tagsToMap)
                {
                    DataRow dr = dt.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleTag);
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private DataTable MapFriendIdsToTable(List<int> friendIdsToMap)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            if (friendIdsToMap != null)
            {
                foreach(int singleId in friendIdsToMap)
                {
                    DataRow dr = dt.NewRow();
                    int index = 0;
                    dr.SetField(index++, singleId);
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        private DataTable MapImagesToTable(List<ImageAddRequest> imagesToMap)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TypeId", typeof(int));
            dt.Columns.Add("Url", typeof(string));
            if (imagesToMap != null)
            {
                foreach (ImageAddRequest imageItem in imagesToMap)
                {
                    DataRow dr = dt.NewRow();
                    int index = 0;
                    dr.SetField(index++, imageItem.TypeId);
                    dr.SetField(index++, imageItem.Url);
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }


}
