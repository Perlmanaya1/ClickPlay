using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Algoritem
{
    class ImageProccecing
    {
        static int myI = 0,
         myJ = 0;
        List<Row> rows;
        List<Staff> staffs;
        List<Row> theAllRows;
        public ImageProccecing()
        {

        }
        public string manage(int[,] mat)
        {
            rows = GetRows(mat);// get the rows in the mat
            rows = CheckRows(mat, rows);//check if the row is good row
            rows = DividedRowToStaff(rows);//divided the all rows to staffs
            staffs = GetStaff(mat, rows);// get the staffs
            GetColums(staffs);// get the colums
            foreach (var item in staffs)
            {
                AddNumberRow(item);// add number row to the row
            }
            AddmusicalNote(staffs);//Manages Spot detection
            foreach (var item in staffs)
            {
                foreach (var item1 in item.musicalNotes)
                {
                    AddColumToMusicalNote(item1, item);//add colum to all Note
                }
                CheckNote(item);//Checking the Note

                item.musicalNotes = CutShminit(item.musicalNotes, item);//Cuts connected eighths 
                MusicalNoteTypeStep1(item);//Identify the type of Note
            }
            List<MusicalNote> musicalNotes = AllMusicalNote(staffs);//turn rhe MusicalNoteList            
            musicalNotes = doubleWithToPoint(musicalNotes);
            Music m = new Music(musicalNotes);//Create a music class
            return m.Manage();//Activate the character play function
        }
        private List<MusicalNote> doubleWithToPoint(List<MusicalNote> musicalNotes)
        {

            List<MusicalNote> musicalNotes1 = new List<MusicalNote>();
            int count = 0, myi;
            for (int i = 0; i < musicalNotes.Count; i++)
            {
                if (musicalNotes[i].name != NoteName.CSharp)
                {
                    musicalNotes1.Add(musicalNotes[i]);
                }

                if (musicalNotes[i].name == NoteName.CSharp)
                {

                    if (musicalNotes[i].TheColums[0].j > musicalNotes[i].j && count == 0)
                    {
                        i = 0;
                        while (musicalNotes[i].name != NoteName.CSharp)
                        {
                            musicalNotes1.Add(musicalNotes[i++]);

                        }
                        i++;
                    }
                    else
                    {
                        if (musicalNotes[i].TheColums[0].j < musicalNotes[i].j)
                        {
                            i++;
                            myi = i;
                            while (musicalNotes[i].name != NoteName.CSharp)
                            {
                                musicalNotes1.Add(musicalNotes[i++]);
                            }
                            while (i > myi)
                            {
                                musicalNotes1.Add(musicalNotes[myi++]);
                            }
                            i++;
                        }
                    }
                    count++;
                }
            }
            return musicalNotes1;
        }
        public void CheckNote(Staff staff)//Checking the Nots
        {
            List<MusicalNote> musicalNotes = new List<MusicalNote>();
            foreach (var item in staff.musicalNotes)//Go over all staff nots
            {
                //If the width of the note matrix is standard, and also the number of columns in it is one
                if (item.TheTrueColums.Count == 1 && item.TheColums.Count < 6 && item.MusicalNoteMat.GetLength(1) > 6)
                {
                    if (checkIfOneEighth(item, staff)) //If it is a single eighth, the length is an eighth
                    {
                        item.myLong = MusicalTimeSpan.Eighth;
                        item.IfOneE = true;
                    }
                    musicalNotes.Add(item);//add to the list
                }
                else
                {
                    // If the width of the note matrix is standard, and also the number of columns in it is two
                    if (item.TheTrueColums.Count == 2 && item.TheColums.Count < 6 && item.MusicalNoteMat.GetLength(1) > 6)
                    {
                        item.myLong = MusicalTimeSpan.Eighth;//the lonf is eighth
                        musicalNotes.Add(item);
                    }
                    else
                    {
                        if (item.TheTrueColums.Count == 0)//if dont have a colum
                        {
                            if (CheckIfMusicalNote(item, staff))//check the Note, if yes the long is whole
                            {
                                item.myLong = MusicalTimeSpan.Whole;
                                musicalNotes.Add(item);
                            }
                            else
                            {
                                CheckIfPointOrTwoPoint(item, staff);//check if Repeat marks, or own point that marker long 3/4
                                if (item.name == NoteName.CSharp)
                                {
                                    musicalNotes.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            staff.musicalNotes = musicalNotes;
        }
        private bool checkIfOneEighth(MusicalNote item, Staff staff)
        {
            for (int i = 0; i < item.MusicalNoteMat.GetLength(0); i++)
            {
                for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
                {
                    Console.Write(item.MusicalNoteMat[i, j]);
                }
                Console.WriteLine();
            }
            int c1 = 0, c2 = 0;
            //  int count = 0;
            //  for (int i = 0; i < item.MusicalNoteMat.GetLength(0); i++)
            //   {
            //     if (item.MusicalNoteMat[i,item.TheColums[0].j-1]==1)
            //     {
            //        count++;
            //    }
            //}
            //  if (count > 0)
            if (item.TheColums[0].j == 0)
            {
                for (int i = 0; i < (item.MusicalNoteMat.GetLength(0) / 4) * 3; i++)
                {
                    if (item.MusicalNoteMat[i, item.TheColums[item.TheColums.Count - 1].j + 1] == 1 && !CheckIfColumOrRow(i, item.TheColums[item.TheColums.Count - 1].j + 1, staff))
                    {
                        c1++;
                    }

                }
                for (int i = item.MusicalNoteMat.GetLength(0) / 4 * 3; i < item.MusicalNoteMat.GetLength(0); i++)
                {
                    if (item.MusicalNoteMat[i, item.TheColums[item.TheColums.Count - 1].j + 1] == 1 && !CheckIfColumOrRow(i, item.TheColums[item.TheColums.Count - 1].j + 1, staff))
                    {
                        c2++;
                    }

                }
                if (c1 > 2 && c2 > 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (item.TheColums[item.TheColums.Count - 1].j + 1 == item.MusicalNoteMat.GetLength(1))

                {
                    return false;
                }
                int c1A = 0, c2A = 0, c1B = 0, c2B = 0;
                for (int i = 0; i < item.MusicalNoteMat.GetLength(0) / 2; i++)
                {
                    if (item.MusicalNoteMat[i, item.TheColums[item.TheColums.Count - 1].j + 1] == 1 && !CheckIfColumOrRow(i, item.TheColums[item.TheColums.Count - 1].j + 1, staff))
                    {
                        c1A++;
                    }
                    if (item.MusicalNoteMat[i, item.TheColums[0].j - 1] == 1 && !CheckIfColumOrRow(i, item.TheColums[0].j - 1, staff))
                    {
                        c1B++;
                    }
                }
                for (int i = item.MusicalNoteMat.GetLength(0) / 2; i < item.MusicalNoteMat.GetLength(0); i++)
                {
                    if (item.MusicalNoteMat[i, item.TheColums[item.TheColums.Count - 1].j + 1] == 1 && !CheckIfColumOrRow(i, item.TheColums[item.TheColums.Count - 1].j + 1, staff))
                    {
                        c2A++;
                    }
                    if (item.MusicalNoteMat[i, item.TheColums[0].j - 1] == 1 && !CheckIfColumOrRow(i, item.TheColums[0].j - 1, staff))
                    {
                        c2B++;
                    }
                }
                if (c1A > 2 && c2A > 2 || c1A > 0 && c2B > 2 || c1B > 2 && c2A > 2)
                {
                    return true;
                }
            }
            return false;
        }
        private void CheckIfPointOrTwoPoint(MusicalNote item, Staff staff)
        {
            int c1 = 0, c2 = 0;
            for (int i = staff.Rows[1].i; i < staff.Rows[2].i; i++)//go over the matrix between row num 2 to row num 3
            {
                for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
                {
                    if (item.MusicalNoteMat[i, j] == 1 && !CheckIfColumOrRow(i, j, staff))//Stock of one digit 
                    {
                        c1++;
                    }
                }
            }
            for (int i = staff.Rows[2].i; i < staff.Rows[3].i; i++)//go over the matrix between row num 3 to row num 4
            {
                for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
                {
                    if (item.MusicalNoteMat[i, j] == 1 && !CheckIfColumOrRow(i, j, staff))
                    {
                        c2++;
                    }
                }
            }
            if (c2 > 0 && c1 > 0&&c1==c2)//If both are greater than zero
            {
                item.name = NoteName.CSharp;//two point
                addThenearColum(item, staff);
            }
            else
            {
                item.name = NoteName.DSharp;//one point
            }
        }
        private void addThenearColum(MusicalNote musicalNote, Staff staff)
        {
            int Range = 100;
            int index = 0;
            int i = 0;
            foreach (var item in staff.colums)
            {
                if (item.j > musicalNote.j)
                {
                    if (item.j - musicalNote.j < Range)
                    {
                        Range = item.j - musicalNote.j;
                        index = i;
                    }
                }
                else
                {
                    if (musicalNote.j - item.j < Range)
                    {
                        Range = musicalNote.j - item.j;
                        index = i;
                    }
                }
                i++;
            }
            musicalNote.TheColums.Add(new Colum() { j = index });
        }
        private void AddColumToMusicalNote(MusicalNote musicalNote, Staff staff)
        {
            int count = 0;         
            int jstart = 0;
            for (int j = 0; j < musicalNote.MusicalNoteMat.GetLength(1); j++)//Running on the matrix of Staff
            {
                for (int i = 0; i < musicalNote.MusicalNoteMat.GetLength(0); i++)
                {
                    //Check if one is worth and not a row and column
                    if (musicalNote.MusicalNoteMat[i, j] == 1 && !CheckIfColumOrRow(i, j, staff))
                    {
                        count++;
                    }
                }
                //If the difference is greater than two spaces between lines in five, that is, two characters, add the column
                if (count > (staff.Rows[1].i - staff.Rows[0].i) * 2)
                {
                    musicalNote.TheColums.Add(new Colum()
                    { j = j });
                    jstart = j;
                }
                count = 0;
            }
            //If the number of columns is greater than zero, you will compress columns adjacent to one column.
            if (musicalNote.TheColums.Count > 0)
            {
                CompressColum(musicalNote);
            }
        }
        public List<Row> GetRows(int[,] mat)//A function that receives a matrix and returns all the rows in the matrix
        {
            List<Row> theRowsInMat = new List<Row>();//list to the row
            int l, jStart, mone = 0;
            for (int i = 0; i < mat.GetLength(0); i++)//Go over a matrix to look for a continuous line of unity
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 1)//if is not white
                    {
                        jStart = j;//start in the curent j
                        for (l = j; l < mat.GetLength(1) && mat[i, j] == 1; l++, j++, mone++) ;
                        //A share of the length of the sequence, and progress as long as equal to one
                        if (mone >= mat.GetLength(1) / 2)//if is row in the staff
                        {
                            theRowsInMat.Add(new Row() { i = i, jStart = jStart, Jfinish = l });
                        }
                        mone = 0;
                    }
                }
            }
            theAllRows = theRowsInMat;
            return theRowsInMat;
        }
        public List<Row> DividedRowToStaff(List<Row> rows)//A function that reduces the number of straight lines per line by five to one line
        {
            List<Row> newList = new List<Row>();//the new Row List
            newList.Add(rows[0]);//Add The First Row
            for (int i = 1; i < rows.Count; i++)//Go over the straight lines
            {
                if (rows[i].i - rows[i - 1].i >= 3)
                //If the difference between the two lines can be a space between two lines in five you will add it to the new list.
                {
                    newList.Add(rows[i]);
                }
            }
            return newList;

        }
        public List<Row> CheckRows(int[,] mat, List<Row> rows)//פונקציה שמקבלת את רשימת השורות הישרות ומחזיר רק את רשמת השורות שקשורות לחמשה
        {
            List<Row> NewList = new List<Row>();
            foreach (var item in rows)
            {
                if ((item.Jfinish - item.jStart) < mat.GetLength(1))
                {
                    NewList.Add(item);
                }

            }
            return NewList;

        }
        public List<Staff> GetStaff(int[,] mat, List<Row> rows)//A function that receives a matrix and returns a list of hamsas
        {
            List<Row> r = new List<Row>();// A list that will contain for every five its list of rows
            List<Row> temp = rows;// Auxiliary list so as not to delete from the incoming list
            List<Staff> staffs = new List<Staff>();// A list of fives into which we will insert the fives
            int maxLength, j, countStaffs = 0;//
            int[,] newMat;// a staff Matrix
            if (rows.Count % 5 != 0)
            {
                Console.WriteLine("שגיאה,error,מספר השורות לא מתחלק בחמש");//If not divided by five error
            }
            while (temp.Count > 0)//Because every 5 lines that are used, are deleted
            {
                maxLength = temp[1].i - temp[0].i;// The largest space between 2 rows
                for (int i = 1; i < 3; i++)// A loop that goes through all the rows associated with the five and finds the largest spacing
                {
                    if ((int)(temp[i].i - temp[i + 1].i) > maxLength)
                    {
                        maxLength = temp[i + 1].i - temp[i].i;
                    }
                }
                newMat = new int[(temp[4].i + 3 * maxLength) - (temp[0].i - (3 * maxLength)), temp[0].Jfinish - temp[0].jStart];// A new matrix in a size that fits five
                int inew = temp[0].i - (3 * maxLength);//Start copying with the i of the first line in Staff plus two Nots that can use above
                int jnew = temp[0].jStart;//Start copying with the j of the first line in five
                for (int i = 0; i < newMat.GetLength(0) - 1; i++)//Copy the image matrix to the Staff matrix
                {
                    for (j = 0; j < newMat.GetLength(1) - 1; j++)
                    {
                        newMat[i, j] = mat[inew + i, jnew + j];
                    }
                }
                staffs.Add(new Staff(newMat, countStaffs) { });//Add To The List

                for (int i = 0; i < 5; i++)//Delete lines that have already been used 
                {
                    temp.RemoveAt(0);
                }
                AddRowsToStaff(staffs[countStaffs]);//add the ros to the staff
                r.Clear();
                countStaffs++;
            }
            return staffs;
        }
        public void AddRowsToStaff(Staff staff)
        {
            staff.AllRows = GetRows(staff.MatStaff);
            staff.Rows = DividedRowToStaff(staff.AllRows);
        }
        public void GetColums(List<Staff> staffs)//Search for columns in a matrix
        {
            int count = 0;
            foreach (var item in staffs)//Go over all staff
            {
                for (int j = 0; j < item.MatStaff.GetLength(1); j++)//Running on the matrix of Staff between the first and fifth lines
                {
                    for (int i = item.AllRows[0].i; i < item.AllRows[item.AllRows.Count - 1].i; i++)
                    {
                        if (item.MatStaff[i, j] == 1)
                        {
                            count++;
                        }
                    }
                    if (count == item.AllRows[item.AllRows.Count - 1].i - item.AllRows[0].i && j > 100)//If the numerator is within the limits of staff
                    {
                        item.colums.Add(new Colum() { j = j });//add to the Colum list
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
        private void AddNumberRow(Staff staff)//Adds to each line its line number
        {
            for (int i = 0; i < 5; i++)//run on five staff line
            {
                staff.Rows[i].numRow = i + 1;
                for (int j = 0; j < staff.AllRows.Count; j++)//go on all rows
                {
                    if (staff.AllRows[j].i > staff.Rows[i].i || staff.AllRows[j].i == staff.Rows[i].i)//Is it in the appropriate range the line tax is i
                    {
                        staff.AllRows[j].numRow = i + 1;
                    }
                }
            }


        }
        private void MusicalNoteTypeStep1(Staff staff)
        {
            int counter = 0;
            int i, j;
            List<Row>[] lists = new List<Row>[5];// An array of list lists in each list will be the rows of the index row
            bool[] b = new bool[5];// A Boolean array that stores for each character which lines it touches
            for (i = 0; i < 5; i++)
            {
                lists[i] = new List<Row>();// Initialize the lists so that a row can be added
            }
            foreach (var item in staff.AllRows)// Go through all the rows and add the row to the appropriate index according to the row tax
            {
                lists[item.numRow - 1].Add(item);
            }
            int c1 = 0, c2 = 0;// One numerator variable above a parent and the other below the line
            foreach (var item in staff.musicalNotes)// Go over all the nots
            {              
                for (i = 0; i < 5; i++)
                {
                    b[i] = false;
                }
                for (i = 0; i < 5; i++)// Cross each character on five lines
                {
                    c1 = 0; c2 = 0;
                    for (j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
                    {
                        if (item.MusicalNoteMat[(lists[i][0].i) - 1, j] == 1)//Count the row above
                        {
                            c1++;
                        }
                        if (item.MusicalNoteMat[(lists[i][(lists[i].Count - 1)].i) + 1, j] == 1)//Count the row below
                        {
                            c2++;
                        }
                    }
                    if (item.IfOneE == true)
                    {
                        if (c1 > 0)
                        {
                            if (i == 0)
                            {
                                for (int l = 0; l < lists[i][0].i; l++)
                                {
                                    for (int m = 0; m < item.MusicalNoteMat.GetLength(1); m++)
                                    {
                                        if (item.MusicalNoteMat[l, m] == 1)
                                        {
                                            counter++;
                                        }
                                    }
                                }
                                if (counter > (lists[1][0].i - lists[0][0].i) * 6)
                                {
                                    b[i] = true;
                                }
                            }
                            else
                            {
                                for (int l = lists[i - 1][lists[i - 1].Count - 1].i + 1; l < lists[i][0].i; l++)
                                {
                                    for (int m = 0; m < item.MusicalNoteMat.GetLength(1); m++)
                                    {
                                        if (item.MusicalNoteMat[l, m] == 1)
                                        {
                                            counter++;
                                        }
                                    }
                                }
                                if (counter > (lists[1][0].i - lists[0][0].i) * 6)
                                {
                                    b[i] = true;
                                }
                            }
                        }
                        counter = 0;
                        if (c2 > 0)
                        {
                            if (i == 4)
                            {
                                for (int l = lists[i][lists[i].Count - 1].i + 1; l < item.MusicalNoteMat.GetLength(0); l++)
                                {
                                    for (int m = 0; m < item.MusicalNoteMat.GetLength(1); m++)
                                    {
                                        if (item.MusicalNoteMat[l, m] == 1)
                                        {
                                            counter++;
                                        }
                                    }
                                }
                                if (counter > (lists[1][0].i - lists[0][0].i) * 6)
                                {
                                    b[i] = true;
                                }
                            }
                            else
                            {
                                for (int l = lists[i][lists[i].Count - 1].i + 1; l < lists[i + 1][0].i; l++)
                                {
                                    for (int m = 0; m < item.MusicalNoteMat.GetLength(1); m++)
                                    {
                                        if (item.MusicalNoteMat[l, m] == 1)
                                        {
                                            counter++;
                                        }
                                    }
                                }
                                if (counter > (lists[1][0].i - lists[0][0].i) * 6)
                                {
                                    b[i] = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        //If the number of the unity in the row above is greater than the width of a line of a musical note, 
                        //and also is not an eighth or the number of the unity in the row above is small or equal to the width of the matrix divide by 2,
                        // or
                        // If the number of unity in the row below is greater than the width of a line of a musical note,
                        //and is also not eighth or the number of unity in the row below is small or equal to the width of the matrix divide by 2,
                        if (c1 > 3 && (item.myLong != MusicalTimeSpan.Eighth || c1 - 1 <= (item.MusicalNoteMat.GetLength(1) / 2)) || (c2 > 3 && (item.myLong != MusicalTimeSpan.Eighth || c2 - 1 <= (item.MusicalNoteMat.GetLength(1) / 2))))
                        {
                            b[i] = true;
                        }
                    }
                }
                
                
                MusicalNoteTypeStep2(b, item, staff, lists);//check the name of the Note
            }

        }

        //Receives an array of lines that touches, a note, the list of rows and five, and returns the name of the Note
        private void MusicalNoteTypeStep2(bool[] arrBool, MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {

            bool flag = false;         
            HwLongTheMuicalNote(staff, musicalNote, lists);//check the long the note
            if (musicalNote.name == NoteName.CSharp)
            {
                flag = true;
            }
            while (!flag)//Check which lines it touches and send to the appropriate function
            {
                if (arrBool[0] && arrBool[1])//If touching the first and second line
                {
                    musicalNote.name = NoteName.E;
                    musicalNote.octave = 5;
                    flag = true;
                    continue;

                }
                if (arrBool[1] && arrBool[2])//If touching the second and third lines
                {
                    musicalNote.name = NoteName.C;
                    musicalNote.octave = 5;
                    flag = true;
                    continue;

                }
                if (arrBool[2] && arrBool[3])//If touching the third and fourth row
                {
                    musicalNote.name = NoteName.A;
                    musicalNote.octave = 4;
                    flag = true;
                    continue;
                }
                if (arrBool[3] && arrBool[4])//If touching the fourth and fifth rows
                {
                    musicalNote.name = NoteName.F;
                    musicalNote.octave = 4;
                    flag = true;
                    continue;
                }
                if (arrBool[0])//If touching the first 
                {

                    CheckifFaHOrSolH(musicalNote, staff, lists);
                    flag = true;
                    continue;
                }
                if (arrBool[1])
                {
                    musicalNote.name = NoteName.D;//If touching the second
                    musicalNote.octave = 5;
                    flag = true;
                    continue;
                }
                if (arrBool[2])//If touching the third
                {
                    musicalNote.name = NoteName.B;
                    musicalNote.octave = 4;
                    flag = true;
                    continue;
                }
                if (arrBool[3])//If touching the fourth
                {
                    musicalNote.name = NoteName.G;
                    musicalNote.octave = 4;
                    flag = true;
                    continue;
                }
                if (arrBool[4])//If touching the  fifth 
                {
                    CheckifNiLOrReL(musicalNote, staff, lists);
                    flag = true;
                    continue;
                }
                if (flag == false)//If not touching any line
                {
                    CheckIfDooLOrSolHOrLaH(musicalNote, staff, lists);//דו נמוך רה גבוה סי גבהוה
                    flag = true;
                    continue;
                }


            }
        }
        private void CheckIfDooLOrSolHOrLaH(MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            for (int j = musicalNote.MusicalNoteMat.GetLength(1) - 1; j > 0; j--)//Running on the matrix from the bottom left corner
            {
                for (int i = musicalNote.MusicalNoteMat.GetLength(0) - 1; i > 0; i--)
                {
                    if (musicalNote.MusicalNoteMat[i, j] == 1)
                    {
                        if (i > lists[4][lists[4].Count - 1].i)//If it is big from the bottom row so it do low
                        {
                            musicalNote.name = NoteName.C;
                            musicalNote.octave = 4;
                        }
                        else
                        {
                            int tvach = lists[1][0].i - lists[0][lists[0].Count - 1].i;//Height of note
                            if (i - tvach * 1.5 == 0 || i - tvach * 1.5 == -1 || i - tvach * 1.5 == 1 || i - tvach * 1.5 == -2 || i - tvach * 1.5 == 2)
                            {
                                musicalNote.name = NoteName.A;
                                musicalNote.octave = 5;
                            }
                            else
                            {
                                musicalNote.name = NoteName.B;
                                musicalNote.octave = 5;
                            }
                        }

                    }
                }

            }
        }
        private void CheckifNiLOrReL(MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            for (int j = musicalNote.MusicalNoteMat.GetLength(1) - 1; j > 0; j--)
            {
                for (int i = musicalNote.MusicalNoteMat.GetLength(0) - 1; i > 0; i--)
                {
                    if (musicalNote.MusicalNoteMat[i, j] == 1)
                    {
                        foreach (var item in lists[4])
                        {
                            if (item.i == i)
                            {
                                musicalNote.name = NoteName.E;
                                musicalNote.octave = 4;
                            }
                        }
                    }

                }


            }
            musicalNote.name = NoteName.D;
            musicalNote.octave = 4;
        }
        private void CheckifFaHOrSolH(MusicalNote musicalNote, Staff staff, List<Row>[] lists)
        {
            //bool flag = false;
            //for (int j = musicalNote.MusicalNoteMat.GetLength(1) - 1; j > 0; j--)
            //{
            //    for (int i = musicalNote.MusicalNoteMat.GetLength(0) - 1; i > 0; i--)
            //    {
            //        if (musicalNote.MusicalNoteMat[i, j] == 1)
            //        {
            //            foreach (var item in lists[0])
            //            {
            //                if (item.i == i )
            //                {
            //                    musicalNote.name = NoteName.F;
            //                    musicalNote.octave = 5;
            //                    flag = true;
            //                    j = 0;
            //                    i = 0;
            //                }

            //            }

            //        }

            //    }


            //}
            //if (flag == false)
            //{
            //    musicalNote.name = NoteName.G;
            //    musicalNote.octave = 5;
            //}

            int c2 = 0;
            for (int j = 0; j < musicalNote.MusicalNoteMat.GetLength(1); j++)//מעבר בכל שורה על כל השורה שמעליה בעמודות
            {

                if (musicalNote.MusicalNoteMat[(lists[0][(lists[0].Count - 1)].i) + 1, j] == 1)
                {
                    c2++;
                }
            }
            if (c2 > 3)
            {
                musicalNote.name = NoteName.F;
                musicalNote.octave = 5;
            }
            else
            {
                musicalNote.name = NoteName.G;
                musicalNote.octave = 5;
            }
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

        //public void CheckIfMusicalNote(Staff staff)
        //{
        //    int mone = 0;// מונה למספר המקומות בהם נמצא שברגע שיש יותר משתיים ישר תמחוק 
        //    List<int> Deleteindex = new List<int>();// רשימת אינדקסים שצריך למחוק 
        //    int ind = 0;
        //    List<Row>[] lists = new List<Row>[5];//מערך של רשימות של שורה בכל רשימה יהיה את השורות של השורה של האינדקס
        //    bool[] b = new bool[5];//מערך בוליאני שמאחסן עבור כל תו באלו שורות הוא נוגע
        //    for (int i = 0; i < 5; i++)
        //    {
        //        lists[i] = new List<Row>();//אתחול הרשימות על מנת שיהיה אפשר להוסיף שורה
        //    }
        //    foreach (var item in staff.AllRows)//מעבר על כל השורות והוספת השורה לאינדקס המתאים עפ מס השורה
        //    {
        //        lists[item.numRow - 1].Add(item);
        //    }

        //    int c1 = 0, c2 = 0;//משתנה מונה אחד למעל שורה  והשני למתחת השורה
        //    foreach (var item in staff.musicalNotes)//מעבר על כל התווים
        //    {
        //        for (int i = 0; i < 5; i++)
        //        {
        //            b[i] = false;
        //        }
        //        //for (int i = 0; i < lists[0][0].i-1; i++)
        //        //{
        //        //    for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
        //        //    {
        //        //        if (item.MusicalNoteMat[i,j]==1)
        //        //        {
        //        //            b[0] = true;
        //        //            mone++;
        //        //            i = lists[0][0].i - 1;
        //        //            continue;
        //        //        }
        //        //    }
        //        //}
        //        //for (int i = lists[4][lists[4].Count-1].i+1; i < item.MusicalNoteMat.GetLength(0)-1; i++)
        //        //{
        //        //    for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
        //        //    {
        //        //        if (item.MusicalNoteMat[i, j] == 1)
        //        //        {
        //        //            b[5] = true;
        //        //            mone++;
        //        //            i = item.MusicalNoteMat.GetLength(0);
        //        //            j = item.MusicalNoteMat.GetLength(1);
        //        //            continue;
        //        //        }
        //        //    }
        //        //}
        //        //while (mone < 2)
        //        //{
        //        //    for (int h = 0; h < 4; h++)
        //        //    {
        //        //        for (int i = (lists[h][lists[h].Count - 1].i) + 1; i < (lists[h + 1][0].i) - 1; i++)
        //        //        {
        //        //            for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)
        //        //            {
        //        //                if (item.MusicalNoteMat[i, j] == 1)
        //        //                {
        //        //                    mone++;
        //        //                    b[h + 1] = true;
        //        //                    i = (lists[h + 1][0].i) - 1;
        //        //                    continue;
        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        for (int i = 0; i < 5; i++)//מעבר בכל תו על חמש שורות
        //        {
        //            c1 = 0; c2 = 0;
        //            for (int j = 0; j < item.MusicalNoteMat.GetLength(1); j++)//מעבר בכל שורה על כל השורה שמעליה בעמודות
        //            {
        //                if (item.MusicalNoteMat[(lists[i][0].i) - 1, j] == 1)
        //                {
        //                    c1++;
        //                }
        //                if (item.MusicalNoteMat[(lists[i][(lists[i].Count - 1)].i) + 1, j] == 1)
        //                {
        //                    c2++;
        //                }
        //            }
        //            if (c1 > 2 || c2 > 2)
        //            {
        //                b[i] = true;

        //            }
        //        }
        //            c2 = 0;
        //            for (int k = 0; k < 5; k++)
        //            {
        //                if (b[k])
        //                {
        //                    c2++;
        //                }
        //            }
        //            if ((c2 > 2 && !CheckIfEighth(item, staff) )|| item.MusicalNoteMat.GetLength(1) < (lists[4][lists[4].Count - 1].i - lists[1][0].i) / 2 || mone == 0)
        //            {
        //                Deleteindex.Add(ind);
        //            }
        //            ind++;

        //        }


        //        if (mone == 2)//לבדוק אם זה סימן של חזרה ואם כן להכפיל,,
        //        {
        //            //לבדוק אם לא נוגע וגם בטווח בין שתיים
        //        }

        //        //   Console.WriteLine(lists[4][lists[4].Count - 1].i - lists[1][0].i);




        //    DeleteNotMusicalNote(staff, Deleteindex);
        //}
        public bool CheckIfMusicalNote(MusicalNote musicalNote, Staff staff)
        {
            bool flag = false;
            List<int> Deleteindex = new List<int>();// List of indexes to delete
            int ind = 0;
            List<Row>[] lists = new List<Row>[5];// An array of list lists in each list will be the rows of the index row
            bool[] b = new bool[5];// A Boolean array that stores for each character which lines it touches
            for (int i = 0; i < 5; i++)
            {
                lists[i] = new List<Row>();// Initialize the lists so that a row can be added
            }
            foreach (var item in staff.AllRows)// Go through all the rows and add the row to the appropriate index according to the row tax
            {
                lists[item.numRow - 1].Add(item);
            }
            int c1 = 0, c2 = 0;//One numerator variable is above the line and the other below the line
            for (int i = 0; i < 5; i++)
            {
                b[i] = false;
            }
            for (int i = 0; i < 5; i++)//Beyond each character on five Rows
            {
                c1 = 0; c2 = 0;
                for (int j = 0; j < musicalNote.MusicalNoteMat.GetLength(1); j++)
                {
                    if (musicalNote.MusicalNoteMat[(lists[i][0].i) - 1, j] == 1)//Go through each row on the entire row above it in columns
                    {
                        c1++;
                    }
                    if (musicalNote.MusicalNoteMat[(lists[i][(lists[i].Count - 1)].i) + 1, j] == 1)//Go through each row on the entire row below it in columns
                    {
                        c2++;
                    }
                }
                if (c1 > 3 || c2 > 3)//if big with colum width
                {
                    b[i] = true;//it touch to the row
                }
            }
            c1 = 0;
            for (int i = 0; i < 5; i++)//Counting in a few רם'ד touches
            {
                if (b[i])
                {
                    c1++;
                }
            }
            //If it touches more than 2 rows, or has a width not condemning a character matrix, return false
            if (c1 > 2 || musicalNote.MusicalNoteMat.GetLength(1) < (lists[4][lists[4].Count - 1].i - lists[1][0].i) / 2)
            {
                return false; ;
            }
            return true;
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
        private bool HwLongTheMuicalNote(Staff staff, MusicalNote musicalNote, List<Row>[] lists)
        {
            int tvach = lists[1][0].i - lists[0][lists[0].Count - 1].i;         
            bool flag = false;
            int mone = 0;
            if (musicalNote.myLong == MusicalTimeSpan.SixtyFourth)
            {
                for (int j = 0; j < musicalNote.MusicalNoteMat.GetLength(1); j++)
                {
                    for (int i = 0; i < musicalNote.MusicalNoteMat.GetLength(0); i++)
                    {
                        flag = false;
                        if (musicalNote.MusicalNoteMat[i, j] == 1 && !CheckIfColumOrRow(i, j, staff))
                        {

                            foreach (var item in musicalNote.TheColums)
                            {

                                if (j == item.j)
                                {
                                    flag = true;
                                }
                            }
                            if (flag == false)
                            {
                                mone++;
                            }

                        }

                    }
                    if (mone >= (tvach / 2 + tvach / 3))
                    {
                        musicalNote.myLong = MusicalTimeSpan.Quarter;
                        return true;
                    }
                    mone = 0;
                }
                musicalNote.myLong = MusicalTimeSpan.Half;
                return true;
            }
            return false;
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
        public int[,] CompressionRow(Staff staff)
        {
            int inew = 0;
            //New matrix the size of the original matrix less the rows
            int[,] mat = new int[staff.MatStaff.GetLength(0) - staff.AllRows.Count, staff.MatStaff.GetLength(1)];
            for (int i = 0; i < staff.MatStaff.GetLength(0); i++)//Ran on the matrix
            {
                //If the i value is one of the i values of the rows you will override it, 
                //and if you do not ,copy it to the matrix without the rows.
                foreach (var item in staff.AllRows)

                {
                    if (i == item.i)
                    {
                        i++;
                    }
                }
                for (int j = 0; j < staff.MatStaff.GetLength(1); j++)
                {
                    mat[inew, j] = staff.MatStaff[i, j];
                }
                inew++;
            }
            return mat;
        }
        public void AddmusicalNote(List<Staff> staffs)// A function that tries each spot as the character list character
        {
            foreach (var item in staffs)//Go over all the characters
            {
                int m;
                int count = 0;
                int[,] musicalNoteMat;//a musical note Note
                int[,] mm = item.MatStaff;
                mm = CompressionRow(item);//Compression of the lines so that we do not treat them with a stain         
                for (int j = mm.GetLength(1) - 1; j > 0; j--)//Run on the matrix starting from the bottom right point
                {
                    for (int i = mm.GetLength(0) - 1; i > 0; i--)
                    {
                        if (mm[i, j] == 1 && !CheckIfColumOrRow(i, j, item))
                        {
                            myJ = item.MatStaff.GetLength(1);
                            IdentifyingStainsToNote(mm, i, j);//Sent to the spot detection function
                            musicalNoteMat = new int[item.MatStaff.GetLength(0), j - myJ + 1];//Create a matrix the size of the stain
                            for (m = musicalNoteMat.GetLength(1) - 1; m >= 0; m--, j--)//Copy the source matrix into the character matrix in the desired locations
                            {
                                for (int l = musicalNoteMat.GetLength(0) - 1; l >= 0; l--)
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
        public void IdentifyingStainsToNote(int[,] mat, int i, int j)//Matrix spot detection function
        {
            //If the cell value in the matrix is equal to zero, or equal to two — we were already there, stop the run.
            if (mat[i, j] == 0 || mat[i, j] == 2)
            {
                return;
            }
            //If there is a deviation from the matrix limits
            if (i >= mat.GetLength(0) || i <= 0 || j >= mat.GetLength(1) || j <= 0)
            {
                return;
            }
            if (myJ > j)//If the j value is more left than we have found so far, save it.
            {
                myJ = j;

            }
            mat[i, j] = 2;//Place about the cell that we were two, so that an endless run is not created.

            IdentifyingStainsToNote(mat, i - 1, j);//Summons the function to the cell on the down
            IdentifyingStainsToNote(mat, i + 1, j);//Summons the function to the cell on the up
            IdentifyingStainsToNote(mat, i, j - 1);//Summons the function to the cell on the left
            IdentifyingStainsToNote(mat, i, j + 1);//Summons the function to the cell on the right
        }
        public bool CheckIfPoint(MusicalNote musicalNote)
        {
            for (int i = 0; i < musicalNote.MusicalNoteMat.GetLength(0); i++)
            {
                for (int j = 0; j < musicalNote.MusicalNoteMat.GetLength(1); j++)
                {
                    if (musicalNote.MusicalNoteMat[i, j] == 1)
                    {
                        if (1 == 1)
                        {

                        }
                    }
                }
            }
            return true;
        }
        public bool CheckIfEighth(MusicalNote musicalNote, Staff staff)
        {
            int j, m;


            if (musicalNote.TheTrueColums.Count == 2)
            {
                return true;
            }
            return false;
        }
        public void CompressColum(MusicalNote musicalNote)//compressing colums in musical Note
        {
            List<Colum> newColums = new List<Colum>();//The list of new columns
            for (int i = 0; i < musicalNote.TheColums.Count - 1; i++)//Go through all the columns that Note
            {
                //If the difference between the next character and the current character is greater than one, then you will add the column to the current list
                if (musicalNote.TheColums[i + 1].j - musicalNote.TheColums[i].j > 1)
                {
                    newColums.Add(musicalNote.TheColums[i]);
                }
            }        
            newColums.Add(musicalNote.TheColums[(musicalNote.TheColums.Count - 1)]);  //Add the last column
            musicalNote.TheTrueColums = newColums;//Add 5the list to the Note
        }
        public List<MusicalNote> CutShminit(List<MusicalNote> musicalNotes, Staff staff)//An eighth separator is connected in a line to two single characters of eighth length
        {
            int start;
            int[,] mat1, mat2;//the new matrix to two note
            List<MusicalNote> MyList = new List<MusicalNote>();//a new musicalNote list 
            foreach (var item in musicalNotes)//Go over all the Note
            {

                if (CheckIfEighth(item, staff))//if is eighth 
                {
                    start = item.MusicalNoteMat.GetLength(1) / 3 - 1;//Beginning of the second matrix
                    mat1 = new int[item.MusicalNoteMat.GetLength(0), (item.MusicalNoteMat.GetLength(1) / 3) * 2];//New matrix in size 2/3 of the original
                    mat2 = new int[item.MusicalNoteMat.GetLength(0), (item.MusicalNoteMat.GetLength(1) / 3) * 2];//New matrix in size 2/3 of the original
                    for (int i = 0; i < mat1.GetLength(0); i++) //Running the size of the new matrices
                    {
                        for (int j = 0; j < mat1.GetLength(1); j++)
                        {
                            mat1[i, j] = item.MusicalNoteMat[i, j];
                            mat2[i, j] = item.MusicalNoteMat[i, j + start];
                        }
                    }
                    MyList.Add(new MusicalNote(mat2, MusicalTimeSpan.Eighth));//add the note to the list
                    MyList.Add(new MusicalNote(mat1, MusicalTimeSpan.Eighth));//add the note to the list
                }
                else//if not eoghth
                {
                    MyList.Add(item);//add rhe note
                }
            }
            return MyList;
        }
    }
}
