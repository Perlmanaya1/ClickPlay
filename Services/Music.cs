using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Algoritem
{
    class Music
    {
        List<MusicalNote> musicalNotes;
        public Music(List<MusicalNote> musicalNotes)
        {
            this.musicalNotes = musicalNotes;
            
        }
        public string Manage()
        {
           return GetMusical();

        }
        public string GetMusical()
        {


            var trackChunk = new TrackChunk();

            PatternBuilder pattern = new PatternBuilder();

            for (int i = 0; i < musicalNotes.Count; i++)
            {
                pattern.Note(Melanchall.DryWetMidi.MusicTheory.Note.Get(musicalNotes[i].name, musicalNotes[i].octave), musicalNotes[i].myLong);
            }
            MidiFile midiFile1 = pattern.Build().ToFile(TempoMap.Default);
            using (FileStream fs = new FileStream($"F:/TheProject/Project/picture/MySound113.midi", FileMode.Create))
            {
                midiFile1.Write(fs);
               
            }
            return "F:/TheProject/Project/picture/MySound113.midi";
        }
       
    }
}
