namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoData
    {
        public static string FormatNear(this DateTime dataHora)
        {
            DateTime data = DateTime.Now;
            return (data - dataHora.Date).Days == 1 ? "1 dia atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days == 2 ? "2 dias atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days == 3 ? "3 dias atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days == 4 ? "4 dias atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days == 5 ? "5 dias atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days == 6 ? "6 dias atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days == 7 ? "7 dias atrás as " + dataHora.ToShortTimeString() :
                   (data - dataHora.Date).Days > 7 ? dataHora.ToString("dd/MM/yyyy HH:mm") :
                   (data - dataHora.Date).Days < 0 ? " " : "Hoje as " + dataHora.ToShortTimeString();
        }

        public static string ToRelativeDate(this DateTime input)
        {
            TimeSpan oSpan = DateTime.Now.Subtract(input);
            double TotalMinutes = oSpan.TotalMinutes;
            string Suffix = " atrás";

            if (TotalMinutes < 0.0)
            {
                TotalMinutes = Math.Abs(TotalMinutes);
                Suffix = " a partir de agora";
            }

            var aValue = new SortedList<double, Func<string>>();
            aValue.Add(0.75, () => "há menos de um minuto");
            aValue.Add(1.5, () => "há um minuto");
            aValue.Add(45, () => string.Format("{0} minutos", Math.Round(TotalMinutes)));
            aValue.Add(90, () => "há cerca de uma hora");
            aValue.Add(1440, () => string.Format("about {0} hours", Math.Round(Math.Abs(oSpan.TotalHours)))); // 60 * 24
            aValue.Add(2880, () => "um dia"); // 60 * 48
            aValue.Add(43200, () => string.Format("{0} dias", Math.Floor(Math.Abs(oSpan.TotalDays)))); // 60 * 24 * 30
            aValue.Add(86400, () => "cerca de um mês"); // 60 * 24 * 60
            aValue.Add(525600, () => string.Format("{0} mesês", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); // 60 * 24 * 365 
            aValue.Add(1051200, () => "cerca de um ano"); // 60 * 24 * 365 * 2
            aValue.Add(double.MaxValue, () => string.Format("{0} anos", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

            return aValue.First(n => TotalMinutes < n.Key).Value.Invoke() + Suffix;
        }

        public static int VerificaDiaUtilData(this DateTime data, int i = 0)
        {
            if (data.AddDays(i).DayOfWeek == DayOfWeek.Saturday || data.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
            {
                i++;
                return data.VerificaDiaUtilData(i);
            }
            else
            {
                return i;
            }
        }

        public static string ToMesAnoString(this DateTime data)
        {
            return data.ToString("MM/yyyy");
        }

        public static DateTime ToCompetencia(this DateTime data)
        {
            DateTime dataRetorno = DateTime.Parse("01/" + data.ToString("MM/yyyy"));
            return dataRetorno;
        }

        public static DateTime ToMesAnoDateTime(string dataString)
        {
            try
            {
                return DateTime.Parse(dataString);
            }
            catch// (Exception e)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        ///   A função <c>dia</c> retorna o dia corrente.
        /// </summary>
        public static int Dia()
        {
            return DateTime.Now.Day;
        }

        /// <summary>
        ///   A função <c>mes</c> retorna o mês corrente.
        /// </summary>
        public static int Mes()
        {
            return DateTime.Now.Month;
        }

        /// <summary>
        ///   A função <c>ano</c> retorna o ano corrente.
        /// </summary>
        public static int Ano()
        {
            return DateTime.Now.Year;
        }


        /// <summary>
        ///   A função <c>diasMes</c> retorna a quantidade de dias do mês definido.
        /// </summary>
        public static int DiasDoMes(this DateTime data)
        {
            return DateTime.DaysInMonth(data.Year, data.Month);
        }

        /// <summary>
        ///   A função <c>inicioDoMes</c> retorna, em formato de data, o primeiro dia do mês corrente.
        /// </summary>
        public static DateTime PrimeiroDiaDoMes()
        {
            return new DateTime(Ano(), Mes(), 1);
        }

        /// <summary>
        ///   A função <c>inicioDoMes</c> retorna, em formato de data, o primeiro dia do mês definido.
        /// </summary>
        public static DateTime PrimeiroDiaDoMes(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }

        /// <summary>
        ///   A função <c>fimDoMes</c> retorna, em formato de data, último dia do mês corrente.
        /// </summary>
        public static DateTime UltimoDiaDoMes()
        {
            return new DateTime(Ano(), Mes(), DiasMes());
        }

        /// <summary>
        ///   A função <c>fimDoMes</c> retorna, em formato de data, último dia do mês definido.
        /// </summary>
        public static DateTime UltimoDiaDoMes(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, data.DiasMes());
        }

        /// <summary>
        ///   A função <c>PrimeiroDiaDaSemana</c> retorna, em formato de data, o primeiro dia da semana
        /// </summary>
        public static DateTime PrimeiroDiaDaSemana(this DateTime data)
        {
            int delta = DayOfWeek.Sunday - data.DayOfWeek;
            DateTime sunday = data.AddDays(delta);
            return sunday;
        }

        /// <summary>
        ///   A função <c>UltimoDiaDaSemana</c> retorna, em formato de data, o ultimo dia da semana
        /// </summary>
        public static DateTime UltimoDiaDaSemana(this DateTime data)
        {
            DateTime saturday = data.PrimeiroDiaDaSemana().AddDays(6);
            return saturday;
        }

        /// <summary>
        ///   A função <c>diasMes</c> retorna a quantidade de dias no mês corrente.
        /// </summary>
        public static int DiasMes()
        {
            return DateTime.DaysInMonth(Ano(), Mes());
        }

        /// <summary>
        ///   A função <c>diasMes</c> retorna a quantidade de dias do mês definido.
        /// </summary>
        public static int DiasMes(this DateTime data)
        {
            return DateTime.DaysInMonth(data.Year, data.Month);
        }

        /// <summary>
        ///   A função <c>inicioDoAno</c> retorna, em formato de data, o primeiro dia do ano corrente.
        /// </summary>
        public static DateTime inicioDoAno()
        {
            return new DateTime(Ano(), 1, 1);
        }


        /// <summary>
        ///   A função <c>diaUtil</c> retorna o próximo dia útil a partir da data definida.
        ///   Ela não leva em consideração feriados.
        /// </summary>
        public static DateTime ProximoDiaUtil(this DateTime data)
        {
            //Feriados devem ser controlados pela aplicação e não pela lib.
            //while (true)
            //{
            if (data.DayOfWeek == DayOfWeek.Saturday)
            {
                data = data.AddDays(2);
                return data.ProximoDiaUtil();
            }
            else if (data.DayOfWeek == DayOfWeek.Sunday)
            {
                data = data.AddDays(1);
                return data.ProximoDiaUtil();
            }
            //else if (Feriado(dt) == true)
            //{
            //    dt = dt.AddDays(1);
            //    return diaUtil(dt);
            //}
            else return data;
            //}
        }

        /// <summary>
        ///   A função <c>diasEntre</c> retorna a quantidade de dias entre duas datas.
        /// </summary>
        public static int DiasEntre(DateTime dataInicial, DateTime dataFinal)
        {
            return (dataFinal - dataInicial).Days;
        }

        public static string FormatarMinutos(int minutos)
        {

            var _horas = Math.Truncate(Convert.ToDecimal(minutos / 60));
            var _minutos = minutos % 60;

            var horas = _horas < 10 ? "0" + _horas.ToString() : _horas.ToString();
            var mins = _minutos < 10 ? "0" + _minutos.ToString() : _minutos.ToString();

            return string.Format("{0}:{1}", horas, mins);

        }

        public static string FormataHora(double total)
        {
            double horas = 0;
            double minutos = 0;

            if (total > 59)
            {
                horas = double.Parse(decimal.Truncate((decimal)total / 60).ToString());
                minutos = total - horas * 60;
            }
            else
            {
                minutos = total;
            }

            minutos = Math.Round(minutos, 0);

            return horas.ToString().PadLeft(2, '0') + ":" + minutos.ToString().PadLeft(2, '0');
        }

        public static string CompletarComZeros(string hora)
        {
            var _hora = "";

            _hora = hora.PadLeft(4, '0').Insert(2, ":");

            return _hora;
        }

        public static string FormataHoraComSinal(double total)
        {
            double horas = 0;
            double minutos = 0;
            double tempTotal = total;

            if (total < 0)
            {
                total = total * -1;
            }

            if (total > 59)
            {
                horas = double.Parse(decimal.Truncate((decimal)total / 60).ToString());
                minutos = total - horas * 60;
            }
            else
            {
                minutos = total;
            }

            minutos = Math.Round(minutos, 0);

            var horaFormatada = horas.ToString().PadLeft(2, '0') + ":" + minutos.ToString().PadLeft(2, '0');

            if (tempTotal > 0)
            {
                horaFormatada = "+" + horaFormatada;
            }
            else if (tempTotal < 0)
            {
                horaFormatada = "-" + horaFormatada;
            }

            return horaFormatada;
        }

        public static bool VerificaSeDataEhFutura(this DateTime? data)
        {
            if (data.HasValue && data > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public static DayOfWeek GetDiaSemana(int diaSemana)
        {
            switch (diaSemana)
            {
                case 1:
                    return DayOfWeek.Sunday;
                case 2:
                    return DayOfWeek.Monday;
                case 3:
                    return DayOfWeek.Tuesday;
                case 4:
                    return DayOfWeek.Wednesday;
                case 5:
                    return DayOfWeek.Thursday;
                case 6:
                    return DayOfWeek.Friday;
                case 7:
                    return DayOfWeek.Saturday;
                default:
                    return DayOfWeek.Monday;
            }
        }
    }
}
