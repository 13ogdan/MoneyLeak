// <copyright>BSP corporation</copyright>

using System;

namespace BankReport
{
    public class ReportAnalyzerFactory
    {
        public IReportAnalyzer CreateAnalyzerByFileHeader(string headerLine)
        {
            if (AvalAnalizer.CanParse(headerLine))
                return new AvalAnalizer();

            throw new ApplicationException("Cannot find proper analyzer for report");
        }
    }
}