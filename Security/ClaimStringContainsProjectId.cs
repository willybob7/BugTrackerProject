using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Security
{
    public class ClaimStringContainsProjectId
    {

        public static bool Check (string projectId, List<string> projectList)
        {
            var result = false;
            for (int i = 0; i < projectList.Count; i++)
            {
                if (projectList[i] == projectId )
                {
                    result = true;
                    return result;
                }
            }
            return result;
        }

        public static List<string> RemoveProjectIdFromList (string projectId, List<string> projectList)
        {

            for (int i = 0; i < projectList.Count; i++)
            {
                if (projectList[i] == projectId)
                {
                    projectList.RemoveAt(i);
                    return projectList;
                }
            }
            return projectList;
        }


    }
}
