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
            GetMusical();

        }

        public void GetMusical()
        {
            string[] s = CreateWaveFileArrayAndMerge();
            WaveIO waveIO = new WaveIO();
            waveIO.Merge(s, @"C:\Users\Owner\GitProject\TheProject\Project\oou.wav");
            FileStream fs = new FileStream(@"C:\Users\Owner\GitProject\TheProject\Project\oou.wav", FileMode.Open, FileAccess.Read);
        }

        public String[] CreateWaveFileArrayAndMerge()
        {
            String[] audioFilenames = new String[musicalNotes.Count];
            for (int i = 0; i < musicalNotes.Count; i++)
            {
                audioFilenames[i] = $@"C:\Users\Owner\GitProject\TheProject\Project\תווים/{musicalNotes[i].name}.wav";
            }
            return audioFilenames;

        }
    }
}
