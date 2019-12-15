using System;

namespace DateApp.DAL
{
    public static class Extensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var age = DateTime.Today.Year - dob.Year;
            if (dob.AddYears(age) > DateTime.Today)
                age--;

            return age;
        }
    }
}
