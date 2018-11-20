using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EPES.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the EPESUser class
    public class EPESUser : IdentityUser
    {
        [PersonalData]
        public string Title { get; set; }
        [PersonalData]
        public string FName { get; set; }
        [PersonalData]
        public string LName { get; set; }
        [PersonalData]
        public string PosName { get; set; }
        [PersonalData]
        public string OfficeId { get; set; }
        [PersonalData]
        public string OfficeName { get; set; }
        [PersonalData]
        public string PIN { get; set; }
        [PersonalData]
        public string Class { get; set; }
        [PersonalData]
        public string GroupName { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }
    }
}
