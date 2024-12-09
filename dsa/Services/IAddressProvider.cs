using System.Net;
namespace Dsa.Service;
public interface IAddressProvider 
{
    IPEndPoint GetAddress();
}