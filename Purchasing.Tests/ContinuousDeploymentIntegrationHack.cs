using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purchasing.Tests
{
    public class ContinuousIntegrationDeploymentHack
    {
        public ContinuousIntegrationDeploymentHack()
        {
            //new log4net.Appender.AdoNetAppender();
            //new NHibernate.ByteCode.Spring.ProxyFactoryFactory();
            new System.Data.SQLite.SQLiteException();

            throw new Exception("This class should never be called or instantiated");
        }
    }
}
