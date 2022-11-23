using System;
using System.Collections.Generic;
using System.Text;

namespace Algoritem
{
    class Row
    {
        public int i { get; set; }
        public int jStart { get; set; }
        public int Jfinish { get; set; }
        public int numRow { get; set; }
        private double length;

        public Row()
        {
            this.length = Jfinish - jStart;
        }
    }
}
