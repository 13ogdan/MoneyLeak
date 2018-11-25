using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Payment
{
    public class GetPaymentsQuery:IRequest<List<PaymentDTO>>
    {
        public bool WithEmptyCategory { get; set; }
        public DateTime? Date { get; set; }
        public string WithPhraseInDetails { get; set; }
    }
}
