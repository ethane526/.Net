using Sabio.Models.Domain.TechCompanies;
using Sabio.Models.Requests.TechCompanies;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface ITechCompanyService
    {
        TechCompany Get(int id);
        List<TechCompany> GetAll();
        int Add(TechCompanyAddRequest companyModel);
        void Update(TechCompanyUpdateRequest companyModel);
        void Delete(int id);
    }
}