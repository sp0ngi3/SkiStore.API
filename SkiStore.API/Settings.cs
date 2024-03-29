﻿
namespace SkiStore.API
{
    public static class Settings
    {
        public static string Mode { get; private set; }

        public static string TokenKey { get; private set; }

        public const string BUYERID = "BuyerId";

        static Settings()
        {
            Mode = GetEnvVariable("MODE");
            TokenKey = GetEnvVariable("TOKEN_KEY");
        }
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

    }
}
