using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Algoritem
{
    
   
  
    class MusicalNote
    {
        public List<Colum> TheColums { get; set; }
        public List<Colum> TheTrueColums { get; set; }
        public bool IfOneE { get; set; }
        public int i { get; set; }
        public int j { get; set; }
        public NoteName name { get; set; }
        public int octave { get; set; }
        public MusicalTimeSpan myLong { get; set; }
        
        public int[,] MusicalNoteMat { get; set; }
        public MusicalNote(int[,] musicalNoteMat,int i,int j)
        {
            this.myLong = MusicalTimeSpan.SixtyFourth;
            TheColums = new List<Colum>();
            TheTrueColums = new List<Colum>();
            this.MusicalNoteMat = musicalNoteMat;
            this.i = i;
            this.j = j;
        }
        public MusicalNote(int[,] mat, MusicalTimeSpan myLong)
        {

            TheTrueColums = new List<Colum>();
            TheColums = new List<Colum>();
            this.MusicalNoteMat = mat;
            this.myLong = myLong;
        }
    }

}
