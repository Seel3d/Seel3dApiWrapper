using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Seel3d.ApiWrapper.Model;

namespace Seel3d.ApiWrapper
{
    public class Seel3dClient
    {
        protected HttpClient WebApi { get; set; }

        public Seel3dClient(string baseAddress = "http://seel3dwebservice.azurewebsites.net/api/")
        {
            WebApi = new HttpClient
            {
                //BaseAddress = new Uri("http://localhost:41148/api/")
                BaseAddress = new Uri(baseAddress)
            };
            WebApi.DefaultRequestHeaders.Accept.Clear();
            WebApi.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            WebApi.Timeout = TimeSpan.FromMinutes(10);
        }

        public async Task<ModelDto> SendPicturesAsync(string modelName, ObservableCollection<byte[]> picCollection,
            string sex, string age, string height, string facebookId)
        {
            try
            {
                var content = new MultipartFormDataContent();

                for (var i = 0; i < picCollection.Count; i++)
                {
                    var fileContent = new ByteArrayContent(picCollection[i]);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = String.Format("image{0}.jpg", i)
                    };
                    content.Add(fileContent);
                }

                HttpResponseMessage response =
                    await
                        WebApi.PostAsync(
                            String.Format("CreateModel/{0}/{1}/{2}/{3}/{4}", sex, height, age, modelName, facebookId),
                            content);
                if (!response.IsSuccessStatusCode)
                {
                    return null;                   
                }
                var modelResult = await response.Content.ReadAsAsync<ModelDto>();

                return modelResult;


                //// Get local model save temporaly on server
                //HttpResponseMessage responseGetModel =
                //    await WebApi.GetAsync(String.Format(@"Model\GetLocalModel\{0}", modelResult.Id));
                //if (response.IsSuccessStatusCode)
                //{
                //    var modelBytes = await responseGetModel.Content.ReadAsByteArrayAsync();
                //    if (modelBytes != null)
                //    {
                //        StorageManager.SaveFileAsync(String.Format("\\Seel3d\\Model\\Local\\{0}.obj", modelResult.Id), modelBytes);
                //    }
                //}

                //// Save local model parameters as XML file
                //StorageManager.SerializeToFileAsync(String.Format("\\Seel3d\\Model\\Local\\{0}.xml", modelResult.Id), modelResult);

                //return true;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> SendModelAsync(string modelName, string modelString, string sex, string age,
            string height, string facebookId)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StringContent(modelString);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "model.obj"
                };
                content.Add(fileContent);

                var response = await WebApi.PostAsync(
                    String.Format("SendModel/{0}/{1}/{2}/{3}/{4}", sex, height, age, modelName, facebookId),
                    content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteModelAsync(int id)
        {
            try
            {
                await WebApi.DeleteAsync(String.Format("Model/Delete/{0}", id));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RenameModelAsync(int id, string newName)
        {
            try
            {
                await WebApi.PutAsync(String.Format("Model/Rename/{0}", id), new StringContent(newName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CollectionDto>> GetCollectionsAsync()
        {
            HttpResponseMessage response = await WebApi.GetAsync("Collection/Get/");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<CollectionDto>>();
            }
            return new List<CollectionDto>();
        }

        public async Task<List<ClotheDto>> GetClothesAsync()
        {
            HttpResponseMessage response = await WebApi.GetAsync("Clothe/Get");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<ClotheDto>>();
            }
            return new List<ClotheDto>();
        }

        public async Task<byte[]> GetPictureAsync(int id)
        {
            HttpResponseMessage response =
                await
                    WebApi.GetAsync(String.Format(@"Clothe\GetPicture\{0}",
                        id));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<byte[]> GetModeleAsync(int id)
        {
            HttpResponseMessage response =
                await
                    WebApi.GetAsync(String.Format(@"Clothe\GetModele\{0}",
                        id));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<byte[]> GetMaterialAsync(int id)
        {
            HttpResponseMessage response =
                await
                    WebApi.GetAsync(String.Format(@"Clothe\GetMaterial\{0}",
                        id));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<byte[]> GetTextureAsync(int id)
        {
            HttpResponseMessage response =
                await
                    WebApi.GetAsync(String.Format(@"Clothe\GetTexture\{0}",
                        id));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<byte[]> GetModelAsync(int id)
        {
            HttpResponseMessage responseClotheMessage =
                await
                    WebApi.GetAsync(String.Format(@"Model/Get/{0}", id));
            if (responseClotheMessage.IsSuccessStatusCode)
            {
                var clothePic = await responseClotheMessage.Content.ReadAsByteArrayAsync();
                return clothePic;
            }
            return null;
        }

        public async Task<List<ModelDto>> GetModelsAsync(string facebookId)
        {
            var modelList = new List<ModelDto>();
            HttpResponseMessage response =
                await
                    WebApi.GetAsync(String.Format("GetSavedModel/{0}", facebookId));
            if (response.IsSuccessStatusCode)
            {
                modelList = await response.Content.ReadAsAsync<List<ModelDto>>();
            }
            return modelList;
        }

        public async Task<Byte[]> GetClotheModeleAsync(int id)
        {
            HttpResponseMessage responseClotheMessage =
                await
                    WebApi.GetAsync(String.Format(@"Clothe/GetModele/{0}",
                        id));
            if (responseClotheMessage.IsSuccessStatusCode)
            {
                return await responseClotheMessage.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<bool> MakeHandShakeAsync()
        {
            try
            {
                HttpResponseMessage responseClotheMessage =
                    await
                        WebApi.GetAsync("HandShake");
                if (responseClotheMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendTryAsync(int clotheId)
        {
            HttpResponseMessage response = await WebApi.PostAsJsonAsync("Try/Post", clotheId);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        // todo
        //public async Task<bool> SendPresenceAsync(byte[] img)
        //{
        //    //return App.Client.DetectionAsync(img, 400).Result;
        //    return true;
        //}
    }
}
