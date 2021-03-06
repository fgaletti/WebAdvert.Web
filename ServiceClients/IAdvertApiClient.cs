﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
namespace WebAdvert.Web.ServiceClients
{
    // 24
    public interface IAdvertApiClient
    {
        Task<AdvertResponse> Create(CreateAdvertModel model);

        Task<bool> Confirm(ConfirmAdvertRequest model); // 26
    }
}
