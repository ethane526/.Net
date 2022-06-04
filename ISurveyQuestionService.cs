using Sabio.Models;
using Sabio.Models.Domain.Survey;
using Sabio.Models.Requests.Surveys;

namespace Sabio.Services
{
    public interface ISurveyQuestionService
    {
        int Add(SurveyQuestionAddRequest model, int userId);
        void Delete(int id);
        Paged<SurveyQuestion> GetByCreatedBy(int pageIndex, int pageSize, int createdBy);
        SurveyQuestion GetById(int id);
        Paged<SurveyQuestion> Pagination(int pageIndex, int pageSize);
        void Update(SurveyQuestionUpdateRequest model, int userId);
    }
}