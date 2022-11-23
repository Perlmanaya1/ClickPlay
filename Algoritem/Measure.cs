using System;
using System.Collections.Generic;
using System.Text;



namespace Algoritem
{
    class Measure
    {
        List<MusicalNote> theNotes;
        public int[,] MeasureMat { get; }

        public Measure(int[,] MeasureMat)
        {
            this.MeasureMat = MeasureMat;
        }
        
        //public Measure()
        //{
        //    theNotes = new List<MusicalNote>();
        //}
    }
}