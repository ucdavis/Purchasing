using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purchasing.Tests.Core
{
    /// <summary>
    /// A list of 20 specific Guids that do not change
    /// </summary>
    public static class SpecificGuid
    {
        private static readonly List<Guid> SpecificGuidValues;
        static SpecificGuid()
        {
            SpecificGuidValues = new List<Guid>(20);
            SpecificGuidValues.Add(new Guid("730ce6ea-d76d-482f-84da-915f1d3b7562"));
            SpecificGuidValues.Add(new Guid("9e577f40-6c1a-448f-95f7-67de41265333"));
            SpecificGuidValues.Add(new Guid("3a54ec03-20b6-46a0-a1de-726f352f14ee"));
            SpecificGuidValues.Add(new Guid("045b6218-5bfb-47ca-9c5e-449b9e031197"));
            SpecificGuidValues.Add(new Guid("d7e66467-bffe-45e5-8e6d-7c6f6da5a072"));
            SpecificGuidValues.Add(new Guid("d649c32c-6a64-4168-b990-f513876dff3d"));
            SpecificGuidValues.Add(new Guid("e336213c-4d9b-4bea-aa25-020beb5368ab"));
            SpecificGuidValues.Add(new Guid("49ff29e5-d30c-41d6-9f02-09ff63c2d968"));
            SpecificGuidValues.Add(new Guid("68e492f1-5dc1-49f9-b3c7-39e733023505"));
            SpecificGuidValues.Add(new Guid("143531fc-31e4-4754-b4f0-26d3214079d8"));

            SpecificGuidValues.Add(new Guid("3cda062d-8583-4d10-80c3-58a97c830951"));
            SpecificGuidValues.Add(new Guid("286779e5-6f69-4ec5-8a2a-49bca848af0d"));
            SpecificGuidValues.Add(new Guid("2d8486fe-2019-4ad3-b415-698e70aaee5b"));
            SpecificGuidValues.Add(new Guid("564e5dc4-1679-4eaa-9da9-21cf460fda58"));
            SpecificGuidValues.Add(new Guid("f8d65475-b3a0-4b7b-8537-72f2392a850e"));
            SpecificGuidValues.Add(new Guid("65be3e47-83c7-4a0b-96c3-c8b0009dc3f1"));
            SpecificGuidValues.Add(new Guid("8ff18565-c677-42f7-b293-9b58e9bb83f0"));
            SpecificGuidValues.Add(new Guid("1845366b-c07b-46f1-bf98-a3a251c02824"));
            SpecificGuidValues.Add(new Guid("0d2d6c45-8777-445a-9750-e1c11a297181"));
            SpecificGuidValues.Add(new Guid("9663a0e0-6f0f-4c5d-8798-3a74dc189504"));
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="id">1 based index</param>
        /// <returns></returns>
        public static Guid GetGuid(int id)
        {
            if (id == 0 || id > SpecificGuidValues.Count)
            {
                return Guid.NewGuid();
                //throw new ApplicationException(string.Format("id must be 1 to {0} only", SpecificGuidValues.Count));
            }
            return SpecificGuidValues[id - 1];
        }
    }
}
