using System.Threading;
using Microsoft.Owin.Hosting;

namespace Host_Service
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:8080/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                while (true)
                {
                    ///減少Client loading
                    Thread.Sleep(10);
                };
            }

        }
    }
}
