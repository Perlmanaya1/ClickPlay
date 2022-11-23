using System;
using System.Collections.Generic;
using System.Text;

namespace Algoritem
{
    class Staff
    {
        public int NumberOfStaff { get; set; }
        public List<Measure> measures { get; set; }
        public List<MusicalNote> musicalNotes { get; set; }
        public List<Row> Rows { get; set; }
        public List<Row> AllRows { get; set; }
        public List<Colum> colums { get; set; }
        public int[,] MatStaff { get; }
        public Staff(int[,] Staff,int countStaff)
        {
            this.MatStaff = Staff;
            this.NumberOfStaff = countStaff;
            colums = new List<Colum>();
            musicalNotes = new List<MusicalNote>();
        }

    }
}

