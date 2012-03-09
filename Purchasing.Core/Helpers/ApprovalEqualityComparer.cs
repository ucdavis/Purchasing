using System.Collections.Generic;
using Purchasing.Core.Domain;

namespace Purchasing.Core.Helpers
{
    public class ApprovalEqualityComparer : IEqualityComparer<Approval>
    {
        public bool Equals(Approval x, Approval y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x.StatusCode.Level != y.StatusCode.Level) return false;

            return x.User == y.User && x.SecondaryUser == y.SecondaryUser;
        }

        public int GetHashCode(Approval approval)
        {
            var level = approval.StatusCode.Level.GetHashCode();

            var userHash = approval.User == null ? 1 : approval.User.GetHashCode();
            var secondaryUserHash = approval.SecondaryUser == null ? 1 : approval.SecondaryUser.GetHashCode();

            return level ^ (userHash*secondaryUserHash);
        }
    }
}