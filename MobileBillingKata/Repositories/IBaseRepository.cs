using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Repositories
{
    public interface IBaseRepository<T>
    {
        void Add(List<T> list);
        List<T> GetAll();
        List<T> GetAllById(Predicate<T> predicate);
    }
}
