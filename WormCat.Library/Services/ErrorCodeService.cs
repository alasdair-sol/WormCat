using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WormCat.Library.Models;
using WormCat.Library.Services.Interfaces;

namespace WormCat.Library.Services
{
    public class ErrorCodeService : IErrorCodeService
    {
        private Dictionary<int, string> models = new Dictionary<int, string>()
        {
            { 100, "An unknown error occurred" },
            { 101, "An unknown error occurred, please try again" },
            { 102, "You must have at least one location" },
            { 103, "Location does not exist" },
            { 104, "This location does not belong to you" },
            { 105, "User does not exist" },
            { 106, "Please create a container, then try again" },
        };

        public string GetErrorMessage(int? errorCode)
        {
            errorCode ??= 100;

            if (models.ContainsKey((int)errorCode!))
                return models[(int)errorCode];

            return models[100];
        }
    }
}
