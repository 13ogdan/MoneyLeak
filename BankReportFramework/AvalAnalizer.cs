﻿using System;

namespace BankReport
{
    public class AvalAnalizer
    {
        private static string AvalHeaderLine = "\"Дата операції/Дата обробки операції\";Номер картки;Тип операції;Деталі операції;Вхідний залишок(у валюті рахунку);Сума у валюті рахунку;Сума у валюті операції;Валюта операції;Вихідний залишок(у валюті рахунку)";

        public Payment ParseReportLine(string line)
        {
            /*
            0- Дата операції/Дата обробки операції;
            1- Номер картки;
            2- Тип операції;
            3- Деталі операції;
            4- Вхідний залишок (у валюті рахунку);
            5- Сума у валюті рахунку;
            6- Сума у валюті операції;
            7- Валюта операції;
            8- Вихідний залишок (у валюті рахунку)
            */
            var items = line.Split(';');
            Payment payment = new Payment();
            payment.Date = ParseDate(items[0]);
            payment.Amout = double.Parse(items[5]);
            payment.Details = items[3];
            return payment;
        }

        /// <summary>
        /// Parses date and time of operation. 
        /// Important is only when operation registereg, not when money went away
        /// </summary>
        /// <param name="input">Date time representation in format like:04.11.2018/04.11.2018</param>
        private DateTime ParseDate(string input)
        {
            var date = input.Substring(0, 10);
            var result = DateTime.ParseExact(date, "dd.MM.yyyy", null);
            return result;
        }

        /// <summary>
        /// Analyze if can parse report by header line in the report
        /// </summary>
        public static bool CanParse(string headerLine)
        {
            return headerLine == AvalHeaderLine;
        }
    }

    public class Payment : IPayment
    {
        public DateTime Date { get;internal set; }
        public double Amout { get; internal set; }
        public string Details { get; internal set; }
    }
}
