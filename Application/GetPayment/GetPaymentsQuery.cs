using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Entity;

namespace Application.GetPayment
{
    public class GetPaymentsQuery : IRequest<IEnumerable<Payment>>
    {
        public bool WithEmptyCategory { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string WithPhraseInDetails { get; set; }
    }
}
