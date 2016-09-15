using Flyttaihop.Framework.Models;

namespace Flyttaihop.Framework.Interfaces
{
    public interface ICriteriaRepository
    {
        Criteria GetSavedCriteria();

        void SetSavedCriteria(Criteria criteria);
    }
}