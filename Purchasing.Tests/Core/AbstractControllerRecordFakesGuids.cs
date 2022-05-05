using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.Core
{

    public abstract class AbstractControllerRecordFakesGuids<T> where T : DomainObjectWithTypedId<Guid>
    {
        public void Records(int count, IRepositoryWithTypedId<T, Guid> repository, bool bypassSetIdTo)
        {
            var records = new List<T>();
            Records(count, repository, records, bypassSetIdTo);
        }


        public void Records(int count, IRepositoryWithTypedId<T, Guid> repository, List<T> specificRecords, bool bypassSetIdTo, bool useSpecificGuid= false)
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
                    var stringId = Guid.NewGuid();
                    if (useSpecificGuid)
                    {
                        stringId = SpecificGuid.GetGuid(i + 1);
                    }
                    records[i].Id = stringId;
                    Moq.Mock.Get(repository).Setup(a => a.GetNullableById(stringId)).Returns(records[i]);
                    Moq.Mock.Get(repository).Setup(a => a.GetById(stringId)).Returns(records[i]);
                }
                else
                {
                    var i1 = i;

                    Moq.Mock.Get(repository).Setup(a => a.GetNullableById(records[i1].Id)).Returns(records[i]);
                    Moq.Mock.Get(repository).Setup(a => a.GetById(records[i1].Id)).Returns(records[i]);
                }
            }
            //repository.Expect(a => a.GetNullableById((totalCount + 1).ToString())).Return(null);
            Moq.Mock.Get(repository).SetupGet(a => a.Queryable).Returns(records.AsQueryable());
            Moq.Mock.Get(repository).Setup(a => a.GetAll()).Returns(records);
        }

        protected abstract T CreateValid(int i);
    }
}
