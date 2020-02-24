using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class GlobalVar
    {

        static int _globalValue;
        static ProjectAttributes GlobalProject;

        public static int ProjectId
        {
            get
            {
                return _globalValue;
            }
            set
            {
                _globalValue = value;
            }
        }

        public static ProjectAttributes Project
        {
            get
            {
                return GlobalProject;
            }
            set
            {
                GlobalProject = value;
            }
        }
    }
}
