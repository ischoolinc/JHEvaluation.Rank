using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JHEvaluation.Rank
{
    public interface ICalculateRegularAssessmentExtension
    {
        string Title { get; }

        void Calculate(int batchID);

        void AddDGVColumn(DataGridView dataGridView, string scoreType);
    }
}
