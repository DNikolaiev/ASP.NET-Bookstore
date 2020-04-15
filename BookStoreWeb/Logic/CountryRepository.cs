using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
namespace BookStoreWeb.Logic
{
    public class CountryRepository
    {
        public List<string> GetCountries()
        {
            return (CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new RegionInfo(x.LCID).EnglishName)
                .Distinct()
                .OrderBy(x => x).ToList());
        }
    }
}