using System.Collections.Generic;
using System.Linq;
using Flyttaihop.Framework.Database;
using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Microsoft.EntityFrameworkCore;

namespace Flyttaihop.Framework.Implementations
{
    public class EFCriteriaRepository : ICriteriaRepository
    {
        private readonly CriteriaContext _context;

        public EFCriteriaRepository(CriteriaContext context)
        {
            _context = context;
        }

        public Criteria GetSavedCriteria()
        {
            //TODO: Debug: Utan användarhantering använder nu alla användare samma objekt 
            var item = _context.Criterias.Include(c => c.Keywords).Include(c => c.DurationCriterias).FirstOrDefault();

            if (item == null)
            {
                return new Criteria
                {
                    Keywords = new List<Keyword>(),
                    DurationCriterias = new List<Duration>()
                };
            }

            return item;
        }

        public void SetSavedCriteria(Criteria criteria)
        {
            //TODO: Uppdatera om redan existerar
            //TODO: Ta bort alla Keyword- och Duration-rader som inte längre är kopplade till någon Criteria
            _context.Criterias.Add(criteria);
            _context.SaveChanges();
        }
    }
}