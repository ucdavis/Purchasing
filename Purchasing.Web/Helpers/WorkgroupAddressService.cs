using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Helpers
{
    public interface IWorkgroupAddressService
    {
        int CompareAddress(WorkgroupAddress newAddress, WorkgroupAddress existingAddress);
    }

    public class WorkgroupAddressService : IWorkgroupAddressService
    {
        /// <summary>
        /// Compare 2 workgroupAddresses ignoring case
        /// </summary>
        /// <param name="newAddress">new existingAddress</param>
        /// <param name="existingAddress">existing existingAddress</param>
        /// <returns>0 if no match found, otherwise the id of the existingAddress match</returns>
        public int CompareAddress(WorkgroupAddress newAddress, WorkgroupAddress existingAddress)
        {
            Check.Require(newAddress != null, "New Address may not be null");
            Check.Require(existingAddress != null, "Existing Address may not be null");
            Check.Require(existingAddress.Id > 0, "Exiting Address must have an ID > 0");

            int matchFound = existingAddress.Id;
            if(newAddress.Address.ToLower() != existingAddress.Address.ToLower())
            {
                matchFound = 0;
            }
            if(!string.IsNullOrWhiteSpace(newAddress.Building) && !string.IsNullOrWhiteSpace(existingAddress.Building))
            {
                if(newAddress.Building.ToLower() != existingAddress.Building.ToLower())
                {
                    matchFound = 0;
                }
            }
            if((!string.IsNullOrWhiteSpace(newAddress.Building) && string.IsNullOrWhiteSpace(existingAddress.Building)) ||
                (string.IsNullOrWhiteSpace(newAddress.Building) && !string.IsNullOrWhiteSpace(existingAddress.Building)))
            {
                matchFound = 0;
            }

            if (newAddress.BuildingCode != existingAddress.BuildingCode)
            {
                matchFound = 0;
            }

            if(!string.IsNullOrWhiteSpace(newAddress.Room) && !string.IsNullOrWhiteSpace(existingAddress.Room))
            {
                if(newAddress.Room.ToLower() != existingAddress.Room.ToLower())
                {
                    matchFound = 0;
                }
            }
            if((!string.IsNullOrWhiteSpace(newAddress.Room) && string.IsNullOrWhiteSpace(existingAddress.Room)) ||
                (string.IsNullOrWhiteSpace(newAddress.Room) && !string.IsNullOrWhiteSpace(existingAddress.Room)))
            {
                matchFound = 0;
            }
            if(newAddress.Name.ToLower() != existingAddress.Name.ToLower())
            {
                matchFound = 0;
            }
            if(newAddress.City.ToLower() != existingAddress.City.ToLower())
            {
                matchFound = 0;
            }
            if(newAddress.State.ToLower() != existingAddress.State.ToLower())
            {
                matchFound = 0;
            }
            if(newAddress.Zip.ToLower() != existingAddress.Zip.ToLower())
            {
                matchFound = 0;
            }
            if(!string.IsNullOrWhiteSpace(newAddress.Phone) && !string.IsNullOrWhiteSpace(existingAddress.Phone))
            {
                if(newAddress.Phone.ToLower() != existingAddress.Phone.ToLower())
                {
                    matchFound = 0;
                }
            }
            if((!string.IsNullOrWhiteSpace(newAddress.Phone) && string.IsNullOrWhiteSpace(existingAddress.Phone)) ||
                (string.IsNullOrWhiteSpace(newAddress.Phone) && !string.IsNullOrWhiteSpace(existingAddress.Phone)))
            {
                matchFound = 0;
            }

            return matchFound;
        }
    }
}