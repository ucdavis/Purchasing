using System.Collections.Generic;
using System.Linq;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.Core
{
    public abstract class AbstractControllerRecordFakesStrings<T> where T : DomainObjectWithTypedId<string>
    {
        public void Records(int count, IRepositoryWithTypedId<T, string> repository, bool bypassSetIdTo)
        {
            var records = new List<T>();
            Records(count, repository, records, bypassSetIdTo);
        }


        public void Records(int count, IRepositoryWithTypedId<T, string> repository, List<T> specificRecords, bool bypassSetIdTo)
        {
            var records = new List<T>();
            var specificRecordsCount = 0;
            if(specificRecords != null)
            {
                specificRecordsCount = specificRecords.Count;
                for(int i = 0; i < specificRecordsCount; i++)
                {
                    records.Add(specificRecords[i]);
                }
            }

            for(int i = 0; i < count; i++)
            {
                records.Add(CreateValid(i + specificRecordsCount + 1));
            }

            var totalCount = records.Count;
            for(int i = 0; i < totalCount; i++)
            {
                if(!bypassSetIdTo)
                {
                    var stringId = (i + 1).ToString();
                    records[i].Id = stringId;
                    Moq.Mock.Get(repository).Setup(a => a.GetNullableById(stringId))
                        .Returns(records[i]);
                    Moq.Mock.Get(repository).Setup(a => a.GetById(stringId))
                        .Returns(records[i]);
                }
                else
                {
                    var i1 = i;
                    Moq.Mock.Get(repository).Setup(a => a.GetNullableById(records[i1].Id))
                        .Returns(records[i]);
                    Moq.Mock.Get(repository).Setup(a => a.GetById(records[i1].Id))
                        .Returns(records[i]);
                }
            }
            Moq.Mock.Get(repository).Setup(a => a.GetNullableById((totalCount + 1).ToString())).Returns<IRepositoryWithTypedId<T, string>>(null);
            Moq.Mock.Get(repository).Setup(a => a.GetById((totalCount + 1).ToString())).Returns<IRepositoryWithTypedId<T, string>>(null);
            Moq.Mock.Get(repository).SetupGet(a => a.Queryable).Returns(records.AsQueryable());
            Moq.Mock.Get(repository).Setup(a => a.GetAll()).Returns(records);
        }

        protected abstract T CreateValid(int i);
    }
}
