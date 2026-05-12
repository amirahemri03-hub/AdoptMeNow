using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.Services
{
    public class RescueStatusFlow
    {
        public static readonly Dictionary<string, List<string>> Flow = new()
        {
             ["Pending"] = new() { "Approved", "Rejected" },

            ["Approved"] = new() { "Rescued", "Recovering", "For Adoption", "Cancelled" },

            ["Rescued"] = new() { "Recovering", "For Adoption" },

            ["Recovering"] = new() { "For Adoption" },

            ["For Adoption"] = new(),

            ["Rejected"] = new(),

            ["Cancelled"] = new()
        };
    }
}