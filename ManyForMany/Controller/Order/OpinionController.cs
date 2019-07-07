using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.Rate;
using ManyForMany.Models.Entity.User;
using ManyForMany.Models.File;
using ManyForMany.ViewModel.Opinion;
using ManyForMany.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Attributes;
using MvcHelper.Entity;

namespace ManyForMany.Controller.Order
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class OpinionController : Microsoft.AspNetCore.Mvc.Controller
    {
        public OpinionController(ILogger<OpinionController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger _logger;
        private readonly Context _context;
        private OrderFileManager _orderFileManager = new OrderFileManager();

        #endregion

        [MvcHelper.Attributes.HttpGet(nameof(Rates))]
        public Dictionary<string, int> Rates()
        {
            return Enum.GetValues(typeof(Rate)).Cast<Rate>().ToDictionary(x => x.ToString(), x => (int) x);
        }
    }
}
