using System.Collections.Generic;
using System.Linq;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace UCDArch.Testing.Fakes
{
    public abstract class ControllerRecordFakes<T> where T : DomainObject
    {
        public void Records(int count, IRepository<T> repository)
        {
            var records = new List<T>();
            Records(count, repository, records);
        }


        public void Records(int count, IRepository<T> repository, List<T> specificRecords)
        {
            var records = new List<T>();
            var specificRecordsCount = 0;
            if (specificRecords != null)
            {
                specificRecordsCount = specificRecords.Count;
                for (int i = 0; i < specificRecordsCount; i++)
                {
                    records.Add(specificRecords[i]);
                }
            }

            for (int i = 0; i < count; i++)
            {
                records.Add(CreateValid(i + specificRecordsCount + 1));
            }

            var totalCount = records.Count;
            for (int i = 0; i < totalCount; i++)
            {
                records[i].Id = i + 1;
                int i1 = i;
                Moq.Mock.Get(repository)
                    .Setup(a => a.GetNullableById(i1 + 1))
                    .Returns(records[i]);
            }
            Moq.Mock.Get(repository).Setup(a => a.GetNullableById(totalCount + 1)).Returns<IRepositoryWithTypedId<T, int>>(null);
            Moq.Mock.Get(repository).Setup(a => a.GetAll()).Returns(records);
        }

        protected abstract T CreateValid(int i);
    }
}
