using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHEvaluation.Rank
{
    public interface ICalculateRegularAssessmentExtension
    {
        string Title { get; }

        void Calculate(int batchID);
    }
}
