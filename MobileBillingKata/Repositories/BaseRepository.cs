using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBillingKata.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
    {
        public readonly List<T> objectList = new List<T>();

        public void Add(List<T> list)
        {
            objectList.AddRange(list);
        }

        public List<T> GetAll()
        {
            return objectList;
        }

        public List<T> GetAllById(Predicate<T> predicate)
        {
            return objectList.FindAll(predicate);
        }
    }
}
