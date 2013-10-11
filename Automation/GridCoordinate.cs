using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class GridCoordinate
    {
        private readonly int rowIndex;
        private readonly int colIndex;

        public GridCoordinate(
            int rowIndex,
            int colIndex
        )
        {
            if (rowIndex < 0) throw new ArgumentException("rowIndex MUST BE non-negative");
            if (colIndex < 0) throw new ArgumentException("colIndex MUST BE non-negative");

            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        public int RowIndex { get { return this.rowIndex; } }
        public int ColIndex { get { return this.colIndex; } }
    }
}
