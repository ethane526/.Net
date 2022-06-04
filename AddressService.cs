using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Addresses;
using Sabio.Models.Requests.Addresses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class AddressService
    {
        IDataProvider _data = null;
        public AddressService(IDataProvider data)
        {
            _data = data;
        }

        public void Update(AddressUpdateRequest model)
        {
            string procName = "[dbo].[Sabio_Addresses_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@Id", model.Id);

            },
            returnParameters: null);
        }

        public int Add(AddressAddRequest model)
        {
            int id = 0;
            string procName = "[dbo].[Sabio_Addresses_Insert]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                col.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;

                Int32.TryParse(oId.ToString(), out id);

            });

            return id;
        }
        private static void AddCommonParams(AddressAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@LineOne", model.LineOne);
            col.AddWithValue("@SuiteNumber", model.SuiteNumber);
            col.AddWithValue("@City", model.City);
            col.AddWithValue("@State", model.State);
            col.AddWithValue("@PostalCode", model.PostalCode);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@Lat", model.Lat);
            col.AddWithValue("@Long", model.Long);
        }
        public Address Get(int id)
        {
            string procName = "[dbo].[Sabio_Addresses_SelectById]";
            Address address = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                address = MapSingleAddress(reader);
            }
            );
            return address;
        }

        public List<Address> GetRandomAddresses()
        {
            List<Address> list = null;
            string procName = "[dbo].[Sabio_Addresses_SelectRandom50]";
            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    Address address = MapSingleAddress(reader);
                    if (list == null)
                    {
                        list = new List<Address>();
                    }
                    list.Add(address);
                });
            return list;
        }

        private static Address MapSingleAddress(IDataReader reader)
        {
            Address address = new Address();
            int startingIndex = 0;
            address.Id = reader.GetSafeInt32(startingIndex++);
            address.LineOne = reader.GetSafeString(startingIndex++);
            address.SuiteNumber = reader.GetSafeInt32(startingIndex++);
            address.City = reader.GetSafeString(startingIndex++);
            address.State = reader.GetSafeString(startingIndex++);
            address.PostalCode = reader.GetSafeString(startingIndex++);
            address.IsActive = reader.GetSafeBool(startingIndex++);
            address.Lat = reader.GetSafeDouble(startingIndex++);
            address.Long = reader.GetSafeDouble(startingIndex++);
            return address;
        }

    }
}
