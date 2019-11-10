using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Opinion;
using TODOIT.ViewModel.Order;

namespace TODOIT.Controller.Order
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class OpinionController : Microsoft.AspNetCore.Mvc.Controller
    {
        public OpinionController(IOpinionRepository opinionRepository, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _opinionRepository = opinionRepository;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }

        private readonly IOpinionRepository _opinionRepository;


        #endregion

        /// <summary>
        /// Return all availables rates
        /// </summary>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpGet(nameof(AvailableRates))]
        public Dictionary<string, int> AvailableRates()
        {
            return Enum.GetValues(typeof(Rate)).Cast<Rate>().ToDictionary(x => x.ToString(), x => (int)x);
        }

        /// <summary>
        /// edit existed Order
        /// </summary>
        /// <param name="opinionId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Update), "{opinionId}")]
        public async Task Update(Guid opinionId, OpinionViewModel model)
        {
            var userId = UserManager.GetUserId(User);

            if (await _opinionRepository.IAmAuthor(userId, opinionId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            await _opinionRepository.Update(opinionId, model);
        }

        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Remove), "{opinionId}")]
        public async Task Remove(Guid opinionId)
        {
            var userId = UserManager.GetUserId(User);

            if (await _opinionRepository.IAmAuthor(userId, opinionId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            _opinionRepository.Delete(opinionId, true);
        }
    }
}
