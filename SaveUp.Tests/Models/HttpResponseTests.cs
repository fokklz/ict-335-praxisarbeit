using Newtonsoft.Json;
using SaveUp.Common.Models;
using SaveUpModels.DTOs;
using SaveUpModels.DTOs.Responses;
using System.Net;
using System.Text;

namespace SaveUp.Tests.Models
{
    public class HttpResponseTests
    {
        [Fact]
        public async Task HttpResponse_Success()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var httpResponse = new HTTPResponse<string>(response);

            Assert.True(httpResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task HttpResponse_Failure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var httpResponse = new HTTPResponse<string>(response);

            Assert.False(httpResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, httpResponse.StatusCode);
        }

        [Fact]
        public async Task HttpResponse_ParseSuccess()
        {
            var expectedResponse = new ItemResponse
            {
                Id = "1",
                Name = "test",
                Description = "test",
                Price = 1,
                CreatedAt = DateTime.Now,
                TimeSpan = DateTime.Now.ToString("dd-MM-yyyy.HH-mm-ss"),
            };
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(expectedResponse), Encoding.UTF8, "application/json");
            var httpResponse = new HTTPResponse<ItemResponse>(response);

            var parsed = await httpResponse.ParseSuccess();

            Assert.True(httpResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            Assert.Equal(expectedResponse.Name, parsed.Name);
            Assert.Equal(expectedResponse.Description, parsed.Description);
            Assert.Equal(expectedResponse.Price, parsed.Price);
            Assert.Equal(expectedResponse.CreatedAt, parsed.CreatedAt);
            Assert.Equal(expectedResponse.TimeSpan, parsed.TimeSpan);
        }

        [Fact]
        public async Task HttpResponse_ParseError()
        {
            var expectedError = new ErrorData
            {
                Code = "500",
                Message = "test",
            };
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            response.Content = new StringContent(JsonConvert.SerializeObject(expectedError), Encoding.UTF8, "application/json");
            var httpResponse = new HTTPResponse<UserResponse>(response);

            var parsed = await httpResponse.ParseError();

            Assert.False(httpResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, httpResponse.StatusCode);
            Assert.Equal(expectedError.Code, parsed.Code) ;
        }

    }
}
