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
            var item = _context.Criterias.Include(c => c.Keywords).Include(c => c.DurationCriterias).FirstOrDefault();

            if (item == null)
            {
                _context.Criterias.Add(criteria);
            }
            else
            {
                //Uppdatera Keywords
                var keywordsToDelete = item.Keywords.Where(existing => !criteria.Keywords.Where(x => x.Text == existing.Text).Any());
                if (keywordsToDelete.Any())
                {
                    item.Keywords.RemoveAll(x => keywordsToDelete.Contains(x));
                }
                var keywordsToAdd = criteria.Keywords.Where(x => !item.Keywords.Where(existing => existing.Text == x.Text).Any());
                if (keywordsToAdd.Any())
                {
                    item.Keywords.AddRange(keywordsToAdd);
                }

                //Uppdatera DurationCriterias
                var durationsToDelete = item.DurationCriterias.Where(existing => !criteria.DurationCriterias.Where(x => x.Type == existing.Type && x.Target == existing.Target).Any());
                if (durationsToDelete.Any())
                {
                    item.DurationCriterias.RemoveAll(x => durationsToDelete.Contains(x));
                }
                var durationsToAdd = criteria.DurationCriterias.Where(x => !item.DurationCriterias.Where(existing => existing.Type == x.Type && existing.Target == x.Target).Any());
                if (durationsToAdd.Any())
                {
                    item.DurationCriterias.AddRange(durationsToAdd);
                }
            }

            _context.SaveChanges();
        }
    }
}