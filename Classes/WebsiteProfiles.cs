using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Classes
{
    // CREATE MAPP  26
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            // 26 we need to pass CreateAdvertViewModel to AdverApi
            CreateMap<CreateAdvertViewModel, CreateAdvertModel>().ReverseMap();
        }
    }
}
