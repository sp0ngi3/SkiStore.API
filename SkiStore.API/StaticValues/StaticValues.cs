using System.Reflection;

namespace SkiStore.API.StaticValues
{
    public static class StaticValues
    {
        public static string MODE { get => GetEnvVariable("MODE"); }
        private static string GetEnvVariable(string variableName)
        {
            try
            {
                string value = Environment.GetEnvironmentVariable(variableName)!;

                return value == null ? throw new ArgumentNullException($"{variableName} is null") : value;
            }
            catch
            {
                return null!;
            }
        }

        public static bool IsEnvValid()
        {
            PropertyInfo[] properties = typeof(StaticValues).GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    string value = (string)property.GetValue(null)!;
                    if (value == "" || value == null)
                    {
                        return false;
                    }
                }
            }

            return true;

        }
    }
}
