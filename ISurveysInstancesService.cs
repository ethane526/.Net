using Sabio.Models;
using Sabio.Models.Domain.Survey;
using Sabio.Models.Requests.Surveys;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface ISurveysInstancesService
    {
        int Add(int surveyId, int userId);
        void Delete(int id);
        Paged<SurveyInstance> GetByCreatedBy(int pageIndex, int pageSize, int createdBy);
        SurveyInstance GetById(int id);
        Paged<SurveyInstance> Pagination(int pageIdex, int pageSize);
        void Update(int id, int surveyId, int userId);
        List<SurveyInstance> GetAll();
    }
}