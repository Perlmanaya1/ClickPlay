using System;
using System.Collections.Generic;
using System.Text;

namespace Algoritem
{
    class ImageProccecing
    {
        int[,] mat1;
        List<Row> theAllRows;
        public ImageProccecing(int[,] nisoi2)
        {
            mat1 = nisoi2;
            List<Row> rows;
            List<Staff> staffs;
            rows = GetRows(nisoi2);
            rows = CheckRows(nisoi2, rows);
            staffs = GetStaff(nisoi2, rows);
            // Measures = GetMeasures(staffs);
            foreach (var item in staffs)
            {
                for (int i = 0; i < item.MatStaff.GetLength(0); i++)
                {
                    for (int j = 0; j < item.MatStaff.GetLength(1); j++)
                    {
                        Console.Write(item.MatStaff[i, j]);

                    }
                    Console.WriteLine();
                }
            }
            GetColums(staffs);
            foreach (var item in staffs)
            {
                AddNumberRow(item);

            }
            GetMusicalNote1(staffs);

            foreach (var item in staffs)
            {
                CheckIfMusicalNote(item);
                MusicalNoteTypeStep1(item);
                foreach (var item1 in item.musicalNotes)
                {
                    Console.WriteLine(item1.name);
                }
                //foreach (var item1 in item.musicalNotes)
                //{
                //    for (int i = 0; i < item1.MusicalNoteMat.GetLength(0); i++)
                //    {
                //        for (int j = 0; j < item1.MusicalNoteMat.GetLength(1); j++)
                //        {
                //            Console.Write(item1.MusicalNoteMat[i, j]);
                //        }
                //        Console.WriteLine();
                //    }
                //    Console.WriteLine("@@@@@@@@@@@@@");
                //}
            }
            List<MusicalNote> musicalNotes = AllMusicalNote(staffs);
            Music m = new Music(musicalNotes);
        }

        public List<Row> GetRows(int[,] mat)//פונקציה שמקבלת מטריצה ומחזירה את כל השורות הישרות במטריצה
        {
            List<Row> theRowsInMat = new List<Row>();
            int l, jStart, mone = 0;
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 1)//if is not white
                    {
                        jStart = j;//start in the curent j
                        for (l = j; l < mat.GetLength(1) && mat[i, j] == 1; l++, j++, mone++) ;
                        if (mone >= mat.GetLength(1) / 2)//if is row in the staff
                        {
                            theRowsInMat.Add(new Row() { i = i, jStart = jStart, Jfinish = l });
                        }
                        mone = 0;
                    }
                }
            }
            //foreach (var item in theRowsInMat)
            //{
            //    Console.WriteLine("the row:" + item.i + "  the point start:" + item.jStart + "  the point finish:" + item.Jfinish);
            //}
            theAllRows = theRowsInMat;
            // theRowsInMat = CheckRows(mat, theRowsInMat);
            return theRowsInMat;
        }

        public List<Row> CheckRows(int[,] mat, List<Row> rows)//פונקציה שמקבלת את רשימת השורות הישרות ומחזיר רק את רשמת השורות שקשורות לחמשה
        {
            int counter = 0;
            int[] index = new int[5];
            List<Row> NewRowList = new List<Row>();
            for (int i = 0; i < rows.Count;)
            {
                counter = 0;
                index[counter++] = i;
                for (int j = i + 1; j < rows.Count && counter < 5; j++)
                {
                    if (rows[i].jStart == rows[j].jStart && rows[i].Jfinish == rows[j].Jfinish)
                    {
                        if (!(counter == 4 && j == rows.Count - 1))
                        {
                            if (rows[j + 1].i - rows[j].i > 1)
                            {
                                index[counter++] = j;
                            }

                        }
                        else
                        {
                            index[counter++] = j;
                        }


                    }
                }
                if (counter == 5)
                {
                    for (int f = 0; f < 5; f++)
                    {
                        NewRowList.Add(rows[index[f]]);
                        //  rows.RemoveAt(index[f]);
                    }
                    i = index[4] + 1;
                }
                else
                {
                    i++;
                }
            }

            Console.WriteLine(NewRowList.Count);

            return NewRowList;
        }

        public List<Staff> GetStaff(int[,] mat, List<Row> rows)//פונקציה שמקבלת מטריצה וממחזירה רשימה של חמשות
        {
            List<Row> r = new List<Row>();//רשימה שתכיל לכל חמשה את רשימת השורות שלה
            List<Row> temp = rows;//רשימת עזר כדי לא למחוק מהרשימה שהגיעה
            List<Staff> staffs = new List<Staff>();//רשימה של חמשות לתוכה נכניס את החמשות
            int maxLength, j, countStaffs = 0;//
            int[,] newMat;//מטריצת החמשה


            if (rows.Count % 5 != 0)//ליצור שגיאה
            {
                Console.WriteLine("שגיאה,error,מספר השורות לא מתחלק בחמש");//לשנות לשגיאה,exeption
            }
            //for (int i = 0; i < 5; i++)
            //{
            //    r.Add(temp[i]);
            //}
            while (temp.Count > 0)//בגלל שכל 5 שורות שמשתמשים, מוחקים
            {
                maxLength = temp[1].i - temp[0].i;//המרווח הגדול ביותר בין 2 שורות
                for (int i = 1; i < 3; i++)//לולאה שעוברת על כל השורות שקשורות לחמשה ומוצאת את המרווח הגדול ביותר
                {
                    if ((int)(temp[i].i - temp[i + 1].i) > maxLength)
                    {
                        maxLength = temp[i + 1].i - temp[i].i;
                    }
                }
                newMat = new int[(temp[4].i + 5 * maxLength) - (temp[0].i - (5 * maxLength)), temp[0].Jfinish - temp[0].jStart];//מטריצה חדשה בגודל שמתאים לחמשה
                int inew = temp[0].i - (5 * maxLength);
                int jnew = temp[0].jStart;
                for (int i = 0; i < newMat.GetLength(0); i++)
                {
                    for (j = 0; j < newMat.GetLength(1) - 1; j++)
                    {
                        newMat[i, j] = mat[inew + i, jnew + j];

                    }
                }
                staffs.Add(new Staff(newMat, countStaffs) { });

                for (int i = 0; i < 5; i++)
                {

                    //    staffs[countStaffs].Rows.Add((Row)temp[0]);
                    temp.RemoveAt(0);
                }

                AddRowsToStaff(staffs[countStaffs]);
                r.Clear();
                countStaffs++;


            }


            return staffs;
        }

        public void AddRowsToStaff(Staff staff)
        {
            staff.AllRows = GetRows(staff.MatStaff);
            staff.Rows = CheckRows(mat1, staff.AllRows);
        }

        private bool CompareBetweenToRows(Row row1, Row row2)//פונקציה שמשווה בין 2 שורות
        {
            if (row1.i == row2.i && row1.Jfinish == row2.Jfinish && row1.jStart == row2.jStart)
            {
                return true;
            }
            return false;
        }

        public void GetColums(List<Staff> staffs)
        {
            int count = 0;

            foreach (var item in staffs)
            {
                for (int j = 0; j < item.MatStaff.GetLength(1); j++)
                {
                    for (int i = item.AllRows[0].i; i < item.AllRows[item.AllRows.Count - 1].i; i++)
                    {
                        if (item.MatStaff[i, j] == 1)
                        {
                            count++;
                        }
                    }
                    if (count == item.AllRows[item.AllRows.Count - 1].i - item.AllRows[0].i && j > 100)
                    {
                        item.colums.Add(new Colum() { j = j });
                    }
                    count = 0;
                }
            }
        }


        public void GetMusicalNote(List<Staff> staffs)
        {
            int measureSize;
            int jStart;


            foreach (var item in staffs)
            {

                int[,] musicalNoteMat;
                int rowSize = item.Rows[1].i - item.AllRows[0].i;
                // measureSize = (item.colums[1].j) - item.colums[0].j;
                // jStart = item.colums[0].j - measureSize;
                if (item.NumberOfStaff == 0)
                {
                    jStart = 7 * rowSize;
                }
                else
                {
                    jStart = 4 * rowSize;
                }
                jStart += 3;

                musicalNoteMat = new int[item.MatStaff.GetLength(0), rowSize * 2];

                for (int j = jStart; j < item.MatStaff.GetLength(1); j++)
                {
                    for (int i = item.MatStaff.GetLength(0) - 1; i > 0; i--)
                    {

                        if (item.MatStaff[i, j] == 1 & !CheckIfColumOrRow(i, j, item))
                        {

                            for (int l = 0; l < musicalNoteMat.GetLength(0); l++)
                            {
                                for (int m = 0; m < musicalNoteMat.GetLength(1) - 1; m++)
                                {
                                    musicalNoteMat[l, m] = item.MatStaff[l, m + j];
                                    Console.Write(item.MatStaff[l, m + j]);

                                }
                                Console.WriteLine();
                            }
                            Console.WriteLine("@@@@@@@@@@@@@@@@@2");


                            item.musicalNotes.Add(new MusicalNote(musicalNoteMat, i, j));
                            i = -1;
                            j += rowSize * 2;

                        }
                    }
                }

            }
        }


        public void GetMusicalNote1(List<Staff> staffs)
        {
            foreach (var item in staffs)
            {
                int m;
                int count = 0;
                int[,] musicalNoteMat;


                for (int j = item.MatStaff.GetLength(1) - 1; j > 0; j--)
                {
                    for (int i = item.MatStaff.GetLength(0) - 1; i > 0; i--)
                    {

                        if (item.MatStaff[i, j] == 1 & !CheckIfColumOrRow(i, j, item))
                        {
                            count++;
                            for (m = j; m > 0 && count > 0; m--)
                            {
                                count = 0;
                                for (int l = item.MatStaff.GetLength(0) - 1; l > 0; l--)
                                {
                                    if (item.MatStaff[l, m] == 1 & !CheckIfColumOrRow(l, m, item))
                                    {
                                        count++;
                                    }
                                }
                            }
                            musicalNoteMat = new int[item.MatStaff.GetLength(0), j - m];
                            for (m = musicalNoteMat.GetLength(1) - 1; m > 0; m--, j--)
                            {

                                for (int l = musicalNoteMat.GetLength(0) - 1; l > 0; l--)
                                {
                                    musicalNoteMat[l, m] = item.MatStaff[l, j];

                                }

                            }
                            item.musicalNotes.Add(new MusicalNote(musicalNoteMat, i, j));
                            i = -1;

                        }

                    }


                }

            }
        }

        private bool CheckIfColumOrRow(int i, int j, Staff item)//פונקציה שבודקת אם הנקודה הנוכחית היא שורה או עמודה
        {
            foreach (var item1 in item.AllRows)
            {
                if (item1.i == i)
                    return true;
            }
            foreach (var item1 in item.colums)
            {
                if (item1.j == j)

                    return true;

            }
            return false;
        }

        //private void CheckIfKey(Staff staff)
        //{
        //    int iToCheck = 0;


        //    for (int j = 0; j < staff.MatStaff.GetLength(1); j++)
        //    {
        //        for (int i = staff.MatStaff.GetLength(0); i > 0; i--)
        //        {
        //            if (staff.MatStaff[i, j] == 1)
        //            {
        //                if (staff.Rows[3].i == i - 1 || staff.Rows[3].i == i - 2)//לבדוק אם יש משהו יותר מ2
        //                {

        //                }
        //            }
        //        }
        //    }

        //}

        private void AddNumberRow(Staff staff)
        {
            for (int i = 0; i < 5; i++)
            {
                staff.Rows[i].numRow = i + 1;
                for (int j = 0; j < staff.AllRows.Count; j++)
                {
                    if (staff.AllRows[j].i > staff.Rows[i].i || staff.AllRows[j].i == staff.Rows[i].i)
                    {
                        staff.AllRows[j].numRow = i + 1;
                    }
                }
            }


        }

        private void MusicalNoteTypeStep1(Staff staff)
        {
            List<Row>[] lists = new List<Row>[5];//מערך של רשימות של שורה בכל רשימה יהיה את השורות של השורה של האינדקס
            bool[] b = new bool[5];//מערך בוליאני שמאחסן עבור כל תו באלו שורות הוא נוגע
            for (int i = 0; i < 5; i++)
            {
                lists[i] = new List<Row>();//אתחול הרשימות על מנת שיהיה אפשר להוסיף שורה
            }
            foreach (var item in staff.AllRows)//מעבר על כל השורות והוספת השורה לאינדקס המתאים עפ מס השורה
            {
                lists[item.numRow - 1].Add(item);
            }
            int c1 = 0, c2 = 0;//משתנה מונה אחד למעל הורה והשני למתחת השורה
            foreach (var item in staff.musicalNotes)//מעבר על כל התווים
            {
                for (int i = 0; i < 5; i++)
                {
                    b[i] = false;
                }
                for (int i = 0; i < 5; i++)//מעבר בכל תו על חמש שורות
                {
                    c1 = 0; c2 = 0;
                    for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)//מעבר בכל שורה על כל השורה שמעליה בעמודות
                    {
                        if (item.MusicalNoteMat[(lists[i][0].i) - 1, j] == 1)
                        {
                            c1++;
                        }
                        if (item.MusicalNoteMat[(lists[i][(lists[i].Count - 1)].i) + 1, j] == 1)
                        {
                            c2++;
                        }
                    }
                    if (c1 > 2 || c2 > 2)
                    {
                        b[i] = true;

                    }
                }
                MusicalNoteTypeStep2(b, item, staff, lists);

            }

        }

        private void MusicalNoteTypeStep2(bool[] arrBool, MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            bool flag = false;
            List<MusicalNote> musicalNotes = new List<MusicalNote>();
            while (!flag)
            {
                if (arrBool[0] && arrBool[1])
                {
                    musicalNote.name = NoteNameAndOctave.miH;
                    flag = true;
                    continue;

                }
                if (arrBool[1] && arrBool[2])
                {
                    musicalNote.name = NoteNameAndOctave.dooH;
                    flag = true;
                    continue;

                }
                if (arrBool[2] && arrBool[3])
                {
                    musicalNote.name = NoteNameAndOctave.laL;
                    flag = true;
                    continue;
                }
                if (arrBool[3] && arrBool[4])
                {
                    musicalNote.name = NoteNameAndOctave.faL;
                    flag = true;
                    continue;
                }
                if (arrBool[0])
                {

                    musicalNote.name = CheckifFaHOrSolH(musicalNote, staff, lists);
                    flag = true;
                    continue;
                }
                if (arrBool[1])
                {
                    musicalNote.name = NoteNameAndOctave.reH;
                    flag = true;
                    continue;
                }
                if (arrBool[2])
                {
                    musicalNote.name = NoteNameAndOctave.siL;
                    flag = true;
                    continue;
                }
                if (arrBool[3])
                {
                    musicalNote.name = NoteNameAndOctave.solL;
                    flag = true;
                    continue;
                }
                if (arrBool[4])
                {
                    musicalNote.name = CheckifNiLOrReL(musicalNote, staff, lists);
                    flag = true;
                    continue;
                }
                if (flag == false)
                {
                    musicalNote.name = CheckIfDooLOrSolHOrLaH(musicalNote, staff, lists);
                    flag = true;
                    continue;
                }

            }
        }

        private NoteNameAndOctave CheckIfDooLOrSolHOrLaH(MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            return NoteNameAndOctave.dooH;//not correct!!!!!!!!!1
        }

        private NoteNameAndOctave CheckifNiLOrReL(MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            for (int j = musicalNote.MusicalNoteMat.GetLength(1)-1; j > 0; j--)
            {
                for (int i = musicalNote.MusicalNoteMat.GetLength(0)-1; i > 0; i--)
                {
                    if (musicalNote.MusicalNoteMat[i, j] == 1)
                    {
                        foreach (var item in lists[0])
                        {
                            if (item.i == i)
                            {
                                return NoteNameAndOctave.miL;
                            }
                        }
                    }

                }


            }
            return NoteNameAndOctave.reL;
        }

    

    private NoteNameAndOctave CheckifFaHOrSolH(MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            for (int j = musicalNote.MusicalNoteMat.GetLength(1)-1; j > 0; j--)
            {
                for (int i = musicalNote.MusicalNoteMat.GetLength(0) - 1; i > 0; i--)
                {
                    if (musicalNote.MusicalNoteMat[i,j]==1)
                    {
                        foreach (var item in lists[0])
                        {
                            if (item.i==i)
                            {
                                return NoteNameAndOctave.faH;
                            }
                            
                        }
                        j = 0;
                        i = 0;
                    }

                }


            }
            return NoteNameAndOctave.solH;
        }

        //public void GetMeasures(List<Staff> staffs)//פוקציה שמקבלת רשיצה של חמשות ומחזירה רשימה של תיבות
        //{
        //    int count=0,j,i,jlast=0,itempMat=0,jtempMat=0;
        //    List<Measure> measures=new List<Measure>();
        //    int[,] tempMat;
        //    foreach (var item in staffs)
        //    {               
        //        int first = item.MatStaff.GetLength(1) / 4;
        //        for ( j = first; j < item.MatStaff.GetLength(1); j++)
        //        {
        //            for ( i = 0; i < item.MatStaff.GetLength(0); i++)
        //            {
        //                if (item.MatStaff[i,j]==1)
        //                {
        //                    count++;
        //                }
        //            }
        //            if (count==item.MatStaff.GetLength(1))
        //            {

        //                if (!(j-jlast<5))
        //                {
        //                    tempMat = new int[item.MatStaff.GetLength(0), j - jlast];
        //                    for (int l = 0; l < item.MatStaff.GetLength(0); l++)
        //                    {
        //                        for (int m = jlast; m < j; m++)
        //                        {
        //                            tempMat[itempMat++, jtempMat++] = item.MatStaff[l, m];
        //                        }
        //                    }
        //                    measures.Add(new Measure(tempMat) { });
        //                }
        //            }
        //        }

        //    }

        //}

        public void CheckIfMusicalNote(Staff staff)
        {
            List<int> Deleteindex = new List<int>();// רשימת אינדקסים שצריך למחוק 
            int ind = 0;
            List<Row>[] lists = new List<Row>[5];//מערך של רשימות של שורה בכל רשימה יהיה את השורות של השורה של האינדקס
            bool[] b = new bool[5];//מערך בוליאני שמאחסן עבור כל תו באלו שורות הוא נוגע
            for (int i = 0; i < 5; i++)
            {
                lists[i] = new List<Row>();//אתחול הרשימות על מנת שיהיה אפשר להוסיף שורה
            }
            foreach (var item in staff.AllRows)//מעבר על כל השורות והוספת השורה לאינדקס המתאים עפ מס השורה
            {
                lists[item.numRow - 1].Add(item);
            }

            int c1 = 0, c2 = 0;//משתנה מונה אחד למעל הורה והשני למתחת השורה
            foreach (var item in staff.musicalNotes)//מעבר על כל התווים
            {
                for (int i = 0; i < 5; i++)
                {
                    b[i] = false;
                }
                for (int i = 0; i < 5; i++)//מעבר בכל תו על חמש שורות
                {
                    c1 = 0; c2 = 0;
                    for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)//מעבר בכל שורה על כל השורה שמעליה בעמודות
                    {
                        if (item.MusicalNoteMat[(lists[i][0].i) - 1, j] == 1)
                        {
                            c1++;
                        }
                        if (item.MusicalNoteMat[(lists[i][(lists[i].Count - 1)].i) + 1, j] == 1)
                        {
                            c2++;
                        }
                    }
                    if (c1 > 2 || c2 > 2)
                    {
                        b[i] = true;

                    }

                }
                c1 = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (b[i])
                    {
                        c1++;
                    }
                }
                Console.WriteLine(lists[4][lists[4].Count - 1].i - lists[1][0].i);
                if (c1 > 2 || item.MusicalNoteMat.GetLength(1) < (lists[4][lists[4].Count - 1].i - lists[1][0].i) / 2)
                {
                    Deleteindex.Add(ind);
                }
                ind++;
            }


            DeleteNotMusicalNote(staff, Deleteindex);
        }
        public void DeleteNotMusicalNote(Staff staff, List<int> index)//פונקציה שמוחקת תווים לק קשורים
        {
            List<MusicalNote> musicalNotes = new List<MusicalNote>();
            int indexList = 0;
            for (int i = 0; i < staff.musicalNotes.Count - 1; i++)
            {
                while (i < index[indexList])
                {
                    musicalNotes.Add(staff.musicalNotes[i]);
                    i++;
                }
                if (i == index[indexList])
                {

                    indexList++;
                }

            }
            staff.musicalNotes = musicalNotes;
        }
        public void HwLongTheMuicalNote(Staff staff)
        {
            int count = 0;
            foreach (var item in staff.musicalNotes)//לולאה שבודקת האם יש קו ישר בתוך מטריצת התו שזה אומר שהוא באורך ארבע רבעים
            {
                for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
                {
                    for (int i = 0; i < item.MusicalNoteMat.GetLength(0); i++)
                    {
                        if (item.MusicalNoteMat[i, j] == 1)
                        {
                            count++;
                        }
                    }
                    if (count > staff.AllRows.Count)
                    {

                    }
                    count = 0;
                }
            }
        }

        public List<MusicalNote> AllMusicalNote(List<Staff> staffs)
        {
            List<MusicalNote> musicalNotes = new List<MusicalNote>();
            foreach (var item in staffs)
            {
               item.musicalNotes.Reverse();
                foreach (var i1 in item.musicalNotes)
                {
                    musicalNotes.Add(i1);
                }
            }
            return musicalNotes;
        
        }
    }

}
