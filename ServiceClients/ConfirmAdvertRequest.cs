using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{

    // 26  Confirm method AdverApi
    public class ConfirmAdvertRequest
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public AdvertStatus Status { get; set; }
    }
}
