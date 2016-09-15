using System.Collections.Generic;
using Flyttaihop.Framework.Extensions;
using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Microsoft.AspNetCore.Http;

namespace Flyttaihop.Framework.Implementations
{
    public class SessionCriteriaRepository : ICriteriaRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session; //Snabbmetod, som i TypeScript

        public SessionCriteriaRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Criteria GetSavedCriteria()
        {
            var item = _session.GetObjectFromJson<Criteria>("criteria");

            if (item == null)
            {
                return new Criteria
                {
                    Keywords = new List<string>(),
                    DistanceCriterias = new List<Criteria.DistanceCriteria>()
                };
            }

            return item;
        }

        public void SetSavedCriteria(Criteria criteria)
        {
            _session.SetObjectAsJson("criteria", criteria);
        }
    }
}