using System;
using System.Collections.Generic;
using System.Text;

namespace Algoritem
{
    [Obsolete("to write do to doo")]
    enum NoteNameAndOctave { reH, miH, faH, solH, laH, siH, dooH, reL, miL, faL, solL, laL, siL, dooL }
    enum Octave {regular, high}
    class MusicalNote
    {
        public int i { get; set; }
        public int j { get; set; }
        public NoteNameAndOctave name { get; set; }
        public double myLong { get; set; }
        
        public int[,] MusicalNoteMat { get; set; }
        public MusicalNote(int[,] musicalNoteMat,int i,int j)
        {
            this.MusicalNoteMat = musicalNoteMat;
            this.i = i;
            this.j = j;
        }
    }

}
