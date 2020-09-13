using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    //24
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        private readonly string _baseAddress;

        public AdvertApiClient (IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            this._configuration = configuration;
            this._client = client;
            this._mapper = mapper;
            var createUrl = _configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");
            _client.BaseAddress = new Uri(createUrl);
            //_client.DefaultRequestHeaders.Add("Content-type", "application/json");

           // _client.Content.Headers.Add("Content-Type", "application/json;charset=UTF-8");

            _client.DefaultRequestHeaders
      .Accept
      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

             _baseAddress = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");  // cchange to BaseUrl

        }

        //24
        public async  Task<AdvertResponse> Create(CreateAdvertModel model)
        {
            //var advertApiModel = new AdvertModel(); // mapper
            var advertApiModel = _mapper.Map<AdvertModel>(model); // 26

            var jsonModel = JsonConvert.SerializeObject(advertApiModel); // shoul use mapper  (model)
            //_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            ////_client.setRequestHeader("accept-charset", "UTF-8");
            //_client.DefaultRequestHeaders.Add("accept-charset", "UTF-8");
            //string _ContentType = "application/json";

            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));

           // var response = await _client.PostAsync(_client.BaseAddress, new StringContent(jsonModel)).ConfigureAwait(false);
            var response = await _client.PostAsync(new Uri($"{_baseAddress}/create"),
            new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false);


            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
            //var advertResponse = new AdvertResponse(); // Automapper  

            var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse); // 26  

            return advertResponse;
        }

        // 26
        public async Task<bool> Confirm(ConfirmAdvertRequest model)
        {
            var advertModel = _mapper.Map<ConfirmAdvertModel>(model); //convert website model to Api Model
            var jsonModel = JsonConvert.SerializeObject(advertModel);
            //var response = await _client.PutAsync(new Uri($"{_client.BaseAddress}/confirm"), new StringContent(jsonModel))
            //                    .ConfigureAwait(false);

            var response = await _client
                .PutAsync(new Uri($"{_baseAddress}/confirm"),
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
