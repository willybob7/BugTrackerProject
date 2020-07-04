using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Storage
{
    public interface IFirebaseFileStorage
    {
        public Task<string> Upload(byte[] fileBytes, string fileName);
        public void Delete(string fileName);
    }
}
