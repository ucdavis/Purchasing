using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Tests.Extensions;

namespace Purchasing.Tests.Core
{
    public static class AttributeAndFieldValidation
    {
        /// <summary>
        /// Validates the fields and attributes.
        /// </summary>
        /// <param name="expectedFields">The expected fields. (Fields must be in ascending order, and any attributes must also be in ascending order.)</param>
        /// <param name="entityType">Type of the entity.</param>
        public static void ValidateFieldsAndAttributes(List<NameAndType> expectedFields, Type entityType)
        {
            #region Act
            // get all public static properties of MyClass type


            var propertyInfos = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // sort properties by name
            Array.Sort(propertyInfos, (propertyInfo1, propertyInfo2) => propertyInfo1.Name.CompareTo(propertyInfo2.Name));
            #endregion Act

            #region Assert
            Assert.AreEqual(expectedFields.Count, propertyInfos.Count(), "Found:" + propertyInfos.ParseList());
            for (int i = 0; i < propertyInfos.Count(); i++)
            {
                Assert.AreEqual(expectedFields[i].Name, propertyInfos[i].Name);
                Assert.AreEqual(expectedFields[i].Property, propertyInfos[i].PropertyType.ToString(), "For Field: " + propertyInfos[i].Name);
                if (expectedFields[i].Attributes != null)
                {
                    CompareOldWay(expectedFields[i], propertyInfos[i]);
                }
                else
                {
                    CompareNewWay(expectedFields[i], propertyInfos[i]);
                }

                //var foundAttributes = CustomAttributeData.GetFilteredCustomAttributes(propertyInfos[i])
                //    .AsQueryable().OrderBy(a => a.ToString()).ToList();
                //Assert.AreEqual(expectedFields[i].Attributes.Count, foundAttributes.Count(), "For Field: " + propertyInfos[i].Name);
                //if (foundAttributes.Count() > 0)
                //{                    
                //    for (int j = 0; j < foundAttributes.Count(); j++)
                //    {
                //        Assert.AreEqual(expectedFields[i].Attributes[j], foundAttributes[j].ToString(), "For Field: " + propertyInfos[i].Name);
                //    }
                //}
            }
            #endregion Assert
        }

        private static void CompareNewWay(NameAndType expectedField, PropertyInfo propertyInfo)
        {
            var foundAttributes = propertyInfo.GetFilteredCustomAttributeData()
                .AsQueryable().OrderBy(a => a.ToString()).ToList();
            Assert.AreEqual(expectedField.ParameterAttributes.Count, foundAttributes.Count(), "For Field: " + propertyInfo.Name);
            if (foundAttributes.Count() > 0)
            {
                for (int j = 0; j < foundAttributes.Count(); j++)
                {
                    //Assert.AreEqual(expectedField.Attributes[j], foundAttributes[j].ToString(), "For Field: " + propertyInfo.Name);
                    Assert.IsTrue(
                        foundAttributes[j].ToString().StartsWith(
                            expectedField.ParameterAttributes[j].AttributeNameStartsWith));
                    var namedParameters = foundAttributes[j].NamedArguments.ToList();
                    Assert.AreEqual(expectedField.ParameterAttributes[j].NamedParameters.Count, namedParameters.Count,
                                    "For Field: " + propertyInfo.Name + " For Attribute: " + foundAttributes[j]);
                    if (namedParameters.Count > 0)
                    {
                        var namedParametersAsStrings = new List<string>();
                        foreach (var namedParameter in namedParameters)
                        {
                            namedParametersAsStrings.Add(namedParameter.ToString());
                        }
                        foreach (var expectedParameter in expectedField.ParameterAttributes[j].NamedParameters)
                        {
                            Assert.IsTrue(namedParametersAsStrings.Contains(expectedParameter), "For Field: " + propertyInfo.Name + " For Attribute: " + foundAttributes[j] + "Expected Parameter: " + expectedParameter + " Found: " + namedParametersAsStrings.ParseList());
                        }
                    }
                }
            }
        }

        private static void CompareOldWay(NameAndType expectedField, PropertyInfo propertyInfo)
        {
            var foundAttributes = propertyInfo.GetFilteredCustomAttributeData()
                .AsQueryable().OrderBy(a => a.ToString()).ToList();
            Assert.AreEqual(expectedField.Attributes.Count, foundAttributes.Count(), "For Field: " + propertyInfo.Name);
            if (foundAttributes.Count() > 0)
            {
                for (int j = 0; j < foundAttributes.Count(); j++)
                {
                    Assert.AreEqual(expectedField.Attributes[j], foundAttributes[j].ToString(), "For Field: " + propertyInfo.Name);
                }
            }
        }
        private static string ParseList(this IEnumerable<string> source)
        {
            var rtValue = "";
            foreach (var s in source)
            {
                rtValue = rtValue + "\n" + s;
            }
            return rtValue;
        }
        private static string ParseList(this PropertyInfo[] source)
        {
            var rtValue = "";
            foreach (var s in source)
            {
                rtValue = rtValue + "\n" + s;
            }
            return rtValue;
        }
    }
}
