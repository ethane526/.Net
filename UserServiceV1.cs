using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class UserServiceV1 : IUserServiceV1
    {
        IDataProvider _data = null;
        public UserServiceV1(IDataProvider data)
        {
            _data = data;
        }

        public User Get(int id)
        {
            string procName = "[dbo].[Users_SelectById]";

            User user = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {
                user = MapSingleUser(reader);
            }
            );

            return user;

        }

        public List<User> GetAll()
        {
            List<User> list = null;

            string procName = "[dbo].[Users_SelectAll]";

            _data.ExecuteCmd(procName, inputParamMapper: null
            , singleRecordMapper: delegate (IDataReader reader, short set)
             {
                 User user = MapSingleUser(reader);

                 if (list == null)
                 {
                     list = new List<User>();
                 }
                 list.Add(user);
             }
            );

            return list;
        }

        public int Add(UserAddRequest userModel)
        {
            int id = 0;
            string procName = "[dbo].[Users_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(userModel, col);

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



        public void Update(UserUpdateRequest userModel)
        {
            string procName = "[dbo].[Users_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(userModel, col);

                col.AddWithValue("@Id", userModel.Id);

            }, returnParameters: null);
        }

        public void Delete(int Id)
        {
            string procName = "[dbo].[Users_Delete]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", Id);

            }, returnParameters: null);

        }

        private static User MapSingleUser(IDataReader reader)
        {
            User user = new User();
            int startingIndex = 0;
            user.Id = reader.GetSafeInt32(startingIndex++);
            user.FirstName = reader.GetSafeString(startingIndex++);
            user.LastName = reader.GetSafeString(startingIndex++);
            user.Email = reader.GetSafeString(startingIndex++);
            user.AvatarUrl = reader.GetSafeString(startingIndex++);
            user.TenantId = reader.GetSafeString(startingIndex++);
            user.DateCreated = reader.GetSafeDateTime(startingIndex++);
            user.DateModified = reader.GetSafeDateTime(startingIndex++);
            return user;
        }
        private static void AddCommonParams(UserAddRequest userModel, SqlParameterCollection col)
        {
            col.AddWithValue("@FirstName", userModel.FirstName);
            col.AddWithValue("@LastName", userModel.LastName);
            col.AddWithValue("@Email", userModel.Email);
            col.AddWithValue("@AvatarUrl", userModel.AvatarUrl);
            col.AddWithValue("@TenantId", userModel.TenantId);
            col.AddWithValue("@Password", userModel.Password);
            col.AddWithValue("@PasswordConfirm", userModel.PasswordConfirm);
        }
    }
}
