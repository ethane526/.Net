using Sabio.Models.Domain.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.TechCompanies
{
    public class TechCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public string Summary { get; set; }
        public string Headline { get; set; }
        public string ContactInformation { get; set; }
        public string Slug { get; set; }
        public int StatusId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public List<Image> Images { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Urls> Urls { get; set; }
        public List<FriendId> FriendIds { get; set; }



    }
}
