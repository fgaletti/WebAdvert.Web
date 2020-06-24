using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient; // 26
        private readonly IMapper _mapper; // 26

       // public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
         public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient; // 26
            _mapper = mapper;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                //var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                //createAdvertModel.UserName = User.Identity.Name;

                //var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);

                // you must make a call to AdveraPI, create the advertisement in the datatbse and return ID
                var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                var apiCallResponse = await _advertApiClient.Create(createAdvertModel).ConfigureAwait(false);
                //var id = "11111"; // apiCallResponse.Id;
                var id = apiCallResponse.Id;
                //bool isOkToConfirmAd = true;
                string filePath = string.Empty;
                if (imageFile != null)
                {
                    var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    filePath = $"{id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                .ConfigureAwait(false);
                            if (!result)
                                throw new Exception(
                                    "Could not upload the image to file repository. Please see the logs for details.");
                        }

                        //CALL ADVERT API
                        var confirmModel = new ConfirmAdvertRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Active
                        };
                        var canConfirm = await _advertApiClient.Confirm(confirmModel).ConfigureAwait(false);
                        if (!canConfirm)
                        {
                            throw new Exception($"Cannot confirm advert of id={id}");
                        }
                        return RedirectToAction("Index", "Home");

                    }
                    catch (Exception e)
                    {
                        // CALL TO ADVERT API COMMNETED OUT

                        //isOkToConfirmAd = false;

                        // 26
                        // if we cannot upload the file we need to delete everythig
                        var confirmModel = new ConfirmAdvertRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending // now is pending because is not confirmed, something was wrong
                        };
                        await _advertApiClient.Confirm(confirmModel).ConfigureAwait(false); // 26 update to PENDING
                        Console.WriteLine(e); // we need add loggin
                    }


                }

                //if (isOkToConfirmAd)
                //{
                //    var confirmModel = new ConfirmAdvertRequest()
                //    {
                //        Id = id,
                //        FilePath = filePath,
                //        Status = AdvertStatus.Active
                //    };
                //    await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                //}

                //return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
