using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCECPortal.BaseController
{
    public class Base : Controller
    {
        protected IConfiguration Configuration { get; }
        protected ILogger Logger;
        protected Base(ILogger<Base> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = logger;
        }
    }
}
