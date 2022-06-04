using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Survey
{
    public class SurveyInstance
    {
        public int Id { get; set; }
        public string Survey { get; set; }
        public string User { get; set; }
        public string SurveyAnswer { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
