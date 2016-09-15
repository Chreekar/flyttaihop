using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class CriteriasController : Controller
    {
        private readonly ICriteriaRepository _criteriaRepository;

        public CriteriasController(ICriteriaRepository criteriaRepository)
        {
            _criteriaRepository = criteriaRepository;
        }
        
        [HttpGet()]
        ///<summary>Hämta tidigare sparad criteria</summary>
        public Criteria Load()
        {
            return _criteriaRepository.GetSavedCriteria();
        }

        [HttpPost()]
        ///<summary>Spara ny criteria (skriver över existerande om redan finns)</summary>
        public Criteria Save([FromBody]Criteria request)
        {
            _criteriaRepository.SetSavedCriteria(request);
            return _criteriaRepository.GetSavedCriteria();
        }
    }
}