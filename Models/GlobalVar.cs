﻿using BugTrackerProject.Models.SubModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class GlobalVar
    {

        public static int _globalValue;
        public static ProjectAttributes GlobalProject;
        public static bool globalInitialScreenShots;
        public static List<Claim> globalCurrentUserClaims;




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

        public static bool InitialScreenShots
        {
            get
            {
                return globalInitialScreenShots;
            }
            set
            {
                globalInitialScreenShots = value;
            }
        }

        public static List<Claim> UserClaims
        {
            get
            {
                return globalCurrentUserClaims;
            }
            set
            {
                globalCurrentUserClaims = value;
            }
        }
    }
}
