using ContractLoader.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace ContractLoader.FileLoader
{
    public class FileUploadManager(HttpClient client, string token, string host)
    {
        private const string questionMark = "?";

        private readonly HttpClient _httpClient = client;

        private string uploadEndpoint = "";

        private string FileId = "";

        private readonly string _token = token;

        private readonly string _host = host;

        private FileCreationModel _fileCreationModel = new();

        async public Task<(FileAttachment?, string)> GetFileBodyAndUpload(string pathToFile)
        {
            string fileName = Path.GetFileName(pathToFile);
            byte[] fileBody = File.ReadAllBytes(pathToFile);
            var (fileRecord, fileUploadError) = await UploadFile(fileBody, fileName);
            if (fileRecord is not null)
            {
                return (new FileAttachment(fileRecord), "success");
            }
            else if (!string.IsNullOrEmpty(fileUploadError))
            {
                return (null, fileUploadError);
            }
            else
            {
                return (null, "unknown error while uploading file");
            }
        }

        private void SendPutLinkRequest(byte[] fileBody, string fileName)
        {
            string putLinkEndpoint = $"{_host}api/disk/files/putlink?size={fileBody.Length}";            
            CalloutManager cm = new(_httpClient, putLinkEndpoint, null, _token, HttpMethod.Get);
            var putLinkRequestResult = cm.SendRequest(out HttpStatusCode statusCode);
            if (statusCode == HttpStatusCode.OK)
            {
                try
                {
                    FileLinkResponse fileLinkResponse = JsonConvert.DeserializeObject<FileLinkResponse>(putLinkRequestResult);
                    if (fileLinkResponse != null)
                    {
                        _fileCreationModel.size = fileBody.Length;
                        _fileCreationModel.hash = fileLinkResponse.Hash;
                        _fileCreationModel.originalName = fileName;
                        _fileCreationModel.version = 1;
                        _fileCreationModel.__createdAt = DateTime.Now;
                        _fileCreationModel.__currentUserPermissions = [];
                        _fileCreationModel.__id = fileLinkResponse.Hash;
                        _fileCreationModel.__subscribers = [];
                        _fileCreationModel.__updatedAt = DateTime.Now;
                        string fileId = fileLinkResponse.Hash;
                        FileId = fileId;
                        string link = fileLinkResponse.Link;
                        uploadEndpoint = link;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка десериализации ответа на SetPutLink-запрос: " + ex.ToString());
                }
            }
            else
            {
                throw new Exception("Ошибка запроса SetPutLink - статус ответа: " + statusCode);
            }
        }

        public async Task<(FileCreationModel?, string)> UploadFile(byte[] fileBody, string fileName)
        {
            var error = "";
            try
            {
                SendPutLinkRequest(fileBody, fileName);
            }
            catch (Exception e)
            {
                error += e.Message;
                return (null, error);
            }
            using var request = new HttpRequestMessage(HttpMethod.Put, uploadEndpoint);
            ByteArrayContent content = new(fileBody);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content = content;
            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                return (_fileCreationModel, error);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public class FileLinkResponse
        {
            [JsonProperty("hash")]
            public string Hash { get; set; }

            [JsonProperty("link")]
            public string Link { get; set; }
        }
    }

}
