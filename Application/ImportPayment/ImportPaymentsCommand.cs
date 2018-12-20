using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Application.ImportPayment
{
    public class ImportPaymentsCommand : IRequest
    {
        public ImportPaymentsCommand(string[] report)
        {
            Report = report;
        }

        public string[] Report { get; }
    }
}
