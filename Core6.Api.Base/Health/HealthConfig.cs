using System;

namespace Core6.Api.Base.Health
{
    public static class HealthConfig
    {
        private static DateTime? _dataInicio { get; set; }

        public static void Configure()
        {
            _dataInicio = DateTime.Now;
        }

        public static DateTime? DataInicio
        {
            get { return _dataInicio; }
        }
    }
}
