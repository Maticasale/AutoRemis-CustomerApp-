using AutoRemis.Models.Services;
using System.Threading.Tasks;

namespace AutoRemis.Interfaces
{
    public interface IFirebaseManager
    {
        Task<string> GetFirebaseToken();
    }

}
