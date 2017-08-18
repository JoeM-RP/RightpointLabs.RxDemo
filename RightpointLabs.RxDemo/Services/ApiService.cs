using System;
using System.Threading.Tasks;
using RightpointLabs.RxDemo.Models;

namespace RightpointLabs.RxDemo.Services
{
	public interface IApiService
	{
		Task<ApiResponse> Refresh(string stationId = null);
	}

    public class ApiService : IApiService
    {
        public ApiService()
        {
        }

        public Task<ApiResponse> Refresh(string stationId = null)
        {
            throw new NotImplementedException();
        }
    }
}
