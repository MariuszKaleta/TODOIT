using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Skill;

namespace TODOIT.Controller.Order
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class SkillController : Microsoft.AspNetCore.Mvc.Controller
    {
        public SkillController(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        #region Properties

        private readonly ISkillRepository _skillRepository;

        #endregion


        /// <summary>
        /// Create new Skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpPost(nameof(Create))]
        //[Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        public async Task Create(CreateSkillViewModel model)
        {

            await _skillRepository.Create(model);
        }

        /// <summary>
        /// edit existed Skill
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        //[Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Update), "skillName")]
        public async Task Update(string skillName, CreateSkillViewModel model)
        {
            await _skillRepository.Update(skillName, model);
        }

        //[Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Remove), "skillName")]
        public async Task Remove(string skillName)
        {
            _skillRepository.Delete(skillName, true);
        }
    }
}
