using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        /*arrays that help to get control on needed label*/
        int[,] labelIndexes = new int[200, 200];
        int []alphabetIndexes = new int[30];

        char selectedLetter;

        bool giveUp = false;

        /*words in dictionary(file)*/
        List<String> words = new List<String>();
        String[] word; 

        object s;
        EventArgs e1;

        List<String> used = new List<String> (); // used words in crossword

        List<char> hiden = new List<char>();
        List<char> choosed = new List<char>();
        List<char> additional = new List<char>();

        char[,] crossword = new char[20, 20]; // generating crossword
        char[,] crossword1 = new char[20, 20]; // crossword that we choose to play
        char[,] user = new char[20, 20]; // crossword wich player made
        char[,] userReset = new char[20, 20]; // copy for reset

        int max = 0;

        int level = 0;

        int crossSize = 10;
        

        List<List<char> > functionpuzzle(List<string> dict, int difficulty, int size)
        {
            word = dict.ToArray();
            level = difficulty; 
            if (level < 0) level = 0;

            if (size < 0) size = 0;
            if (size > 15) size = 15;

            crossSize = size;

            int count = 0; 
            if (crossSize < 10) count = 20;
            if (crossSize >= 13) count = 2; else count = 5;

            for (int j = 0; j <= count; j++)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int q = 0; q < crossSize; q++)
                        crossword[i, q] = ' ';
                generateCrossword();
            }

            hiden.Clear();
            additional.Clear();

            List<char> symbols = new List<char>();

            for (int i = 0; i < crossSize; i++)
                for (int j = 0; j < crossSize; j++)
                {
                    if (crossword1[i, j] != ' ')
                    {
                        if (!symbols.Contains(crossword1[i, j])) symbols.Add(crossword1[i, j]);
                    }
                }

            int hidenCount = 0;

            if (level == 0) hidenCount = (int)(symbols.Count * 0.4);
            if (level == 1) hidenCount = (int)(symbols.Count * 0.5);
            if (level == 2) hidenCount = (int)(symbols.Count * 0.6);
            if (level >= 3) hidenCount = (int)(symbols.Count * 0.7);


            char[] symb = symbols.ToArray();
            int n = symbols.Count;

            while (hidenCount > 0)
            {
                Random at = new Random();
                char hS = symb[at.Next(n)];
                while (hiden.Contains(hS)) hS = symb[at.Next(n)];
                hiden.Add(hS);
                hidenCount--;
            }

            List<char> y = new List<char>();
            for (char x = 'A'; x <= 'Z'; x++)
            {
                if (!symbols.Contains(x)) y.Add(x);
            }
            char[] sym = y.ToArray();
            int m = y.Count;
            int addSymb = y.Count;
            if (level == 0) addSymb = (int)(addSymb * 0.9);
            if (level == 1) addSymb = (int)(addSymb * 0.7);
            if (level == 2) addSymb = (int)(addSymb * 0.5);
            if (level >= 3) addSymb = (int)(addSymb * 0.3);
            while (addSymb > 0)
            {
                Random rand = new Random();
                int index = rand.Next(m);
                if (y.Contains(sym[index]))
                {
                    addSymb--;
                    additional.Add(sym[index]);
                    y.Remove(sym[index]);
                }
            }

            List<char> list1 = new List<char>();
            List<char> list2 = new List<char>();

            for (int i=0;i<crossSize;i++)
                for (int j=0;j<crossSize;j++)
                {
                    list1.Add(crossword1[i, j]);
                    if (hiden.Contains(crossword1[i, j]) && !list2.Contains(crossword1[i, j])) list2.Add(crossword1[i, j]);
                }
            List<List<char>> result = new List<List<char>>();
            result.Add(list1);
            result.Add(list2);
            return result;
        }

        // hide Letters
        public void hideLetters()
        {

            hiden.Clear();
            additional.Clear();

            List<char> symbols = new List<char>();
            int []count = new int [30];
            
            // get all symbols and their count in our crossword
            for (int i = 0; i < 30; i++) count[i] = 0;

                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] != ' ') 
                        {
                            if (!symbols.Contains(crossword1[i, j])) symbols.Add(crossword1[i, j]);
                            count[crossword1[i, j] - 'A']++;
                        }
                    }

            // calculate number of hiden letters, that depends on level

            int hidenCount = 0;

            if (level == 0) hidenCount = (int)(symbols.Count * 0.4);
            if (level == 1) hidenCount = (int)(symbols.Count * 0.5);
            if (level == 2) hidenCount = (int)(symbols.Count * 0.6);
            if (level >= 3) hidenCount = (int)(symbols.Count * 0.7);


            char[] symb = symbols.ToArray();
            int n = symbols.Count;

            //get hiden symbols using random
            while (hidenCount>0)
            {
                Random at = new Random();
                char hS = symb[at.Next(n)];
                while (hiden.Contains(hS)) hS = symb[at.Next(n)];
                hiden.Add(hS);
                hidenCount--;
            }

            // hide letters in crossword
            for (int i = 0; i < crossSize; i++)
                for (int j = 0; j < crossSize; j++)
                    if (hiden.Contains(crossword1[i, j])) { changeColor(alphabetIndexes[crossword1[i, j] - 'A'], Color.White); changeColor(labelIndexes[i, j], Color.SlateBlue); changeText(labelIndexes[i, j], " "); user[i, j] = ' '; }
                    else
                    {
                        if (crossword1[i, j] != ' ') changeColor(alphabetIndexes[crossword1[i, j] - 'A'], Color.Aqua);
                    }

            //additional symbols to hide
            List<char> y = new List<char>();
            for (char x='A';x<='Z';x++)
            {
                if (!symbols.Contains(x)) y.Add(x);
            }
            char[] sym = y.ToArray();
            int m = y.Count;
            int addSymb = y.Count;
            if (level == 0) addSymb = (int)(addSymb * 0.9);
            if (level == 1) addSymb = (int)(addSymb * 0.7);
            if (level == 2) addSymb = (int)(addSymb * 0.5);
            if (level >= 3) addSymb = (int)(addSymb * 0.3);
            while (addSymb>0)
            {
                Random rand = new Random();
                int index = rand.Next(m);
                if (y.Contains(sym[index])) 
                {
                    addSymb--;
                    changeColor(alphabetIndexes[sym[index] - 'A'], Color.Aqua);
                    additional.Add(sym[index]);
                    y.Remove(sym[index]);
                }
            }
        }
        /*check if user crossword the same as needed*/
        public bool checkCross()
        {
            for (int i=0;i<crossSize;i++)
                for (int j=0;j<crossSize;j++)
                {
                    if (crossword1[i, j] != user[i, j]) return false;
                }
            return true;
        }

        public void NewGame(int lev)
        {
            word = words.ToArray();
            button3.Visible = true;
            button2.Visible = true;
            button1.Visible = true;
            label137.Visible = false;
            label138.Visible = false;
            giveUp = false;
            newReset();
            for (char x = 'A'; x <= 'Z'; x++)
                changeColor(alphabetIndexes[x - 'A'], Color.White);
            selectedLetter = '-'; // put selectedLetter another symbol than letters which means that we don't choose any letter
            reset(); // reset crossword
            size(crossSize); // make needed size
            max = 0; // max number of words in generated crossword
            level = lev; // level
            String r="";
            if (level == 0) r = "Begginer";
            if (level == 1) r = "Easy";
            if (level == 2) r = "Medium";
            if (level > 2) r = "Hard";
            label139.Text = "Level: " + r;
            label139.Visible = true;
            int count = 0; // number of trying to generate
            if (crossSize < 10) count = 20;
            if (crossSize >= 13) count = 2; else count = 5;
            // generating
            for (int j = 0; j <= count; j++)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int q = 0; q < crossSize; q++)
                        crossword[i, q] = ' ';
                generateCrossword();
            }

            //filling crosswords 
            for (int i = 0; i < crossSize; i++)
                for (int q = 0; q < crossSize; q++)
                    changeText(labelIndexes[i, q], crossword1[i, q].ToString());
            for (int i = 0; i < crossSize; i++)
                for (int j = 0; j < crossSize; j++)
                {
                    if (crossword1[i, j] == ' ') changeColor(labelIndexes[i, j], Color.Black);
                    user[i, j] = crossword1[i, j]; 
                    userReset[i, j] = crossword1[i, j];
                }
            // hide letters
            hideLetters();
            newReset();
        }

        public Form1()
        {

            InitializeComponent();
            initArray(s, e1); // initializing arrays which helps to get needed label at crossword

            label139.Text = "Level: Beginner";

            button3.Visible = true;
            button2.Visible = true;
            button1.Visible = true;

            // don't show congrat. and NEXT
            label138.Visible = false;
            label137.Visible = false;

            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                    changeVisibility(labelIndexes[i, j], false);

            size(10); // default size

            // make labels with letters (at right)
            for (char x='A';x<='Z';x++)
            {
                changeText(alphabetIndexes[x - 'A'], x.ToString());
                changeColor(alphabetIndexes[x - 'A'], Color.White);
            }

            selectedLetter = '-';

            // reading words from file
            using (StreamReader fs = new StreamReader(@"Dictionary.txt"))
            {
                while (true)
                {
                    String temp = fs.ReadLine();
                    if (temp == null) break;
                    words.Add(temp);
                }
            }

            word = words.ToArray();

        }

        public bool correct()
        {
            for (int i=0;i<14;i++)
                for (int j=0;j<14;j++)    
            {
                if (crossword[i, j] != ' ' && crossword[i + 1, j] != ' ' && crossword[i, j + 1] != ' ' && crossword[i + 1, j + 1] != ' ') return false;
            }
            return true;
        }

        private void generateCrossword()
        {
            // put first horizontal random word
            Random t = new Random();
            int index = t.Next(words.Count);
            used.Clear();
            while (word[index].Length > crossSize && word[index].Length>3) index = t.Next(words.Count);
            int j = 0;
            for (int i = crossSize - word[index].Length; i < crossSize; i++)
            {
                crossword[crossSize / 2, i] = word[index].ElementAt(j);
                j++;
            }
            int k = 0;
            // put vertical random word than horizontal and do the same until we cann't to add a new word
            while (true)
            {
                bool ok = false;
                for (int i = 0; i < 20; i++) if (putVert()) { k++; ok = true; break; }
                //if (!correct()) { generateCrossword(); break; }
                for (int i = 0; i < 20; i++) if (putHor()) { k++; ok = true; break; }
                //if (!correct()) { generateCrossword(); break; }
                if (!ok) break;
            }

           // if (!correct()) generateCrossword();
            // compare number of words in generated crossword with current maximal number
            if (k > max) 
            {
                max = k;
                for (int i = 0; i < crossSize; i++)
                    for (int q = 0; q < crossSize; q++)
                        crossword1[i, q] = crossword[i, q];
            }
        }
        
        /*try to put words in every position and than take any of them*/

        public bool putVert()
        {
            List<String> suit = new List<String>();
            for (int q = 0; q < words.Count;q++)
            {
                String t = word[q];
                if (t.Length>2 && !used.Contains(t))
                for (int x=0;x<crossSize;x++)
                    for (int y=0;y<crossSize;y++)
                    {
                        if ((x - 1 < 0 || crossword[x - 1, y] == ' ')  && x + t.Length-1 < crossSize && (x + t.Length >= crossSize || crossword[x + t.Length, y] == ' '))
                        {
                            bool ok=false;
                            int k=0;
                            int cnt=0;
                            for (int j = x; j < x + t.Length; j++)
                            {
                                if (y - 1 < 0) continue;
                                if (crossword1[j, y - 1] >= 'A' && crossword1[j, y - 1]<='Z') { cnt++; continue; }
                                if (y + 1 >= crossSize) continue;
                                if (crossword1[j, y + 1] >= 'A' && crossword1[j, y + 1] <= 'Z') { cnt++; continue; }
                            }
                            if (cnt > 1) continue;
                            for (int j = x; j < x + t.Length; j++)
                            {
                                if (crossword[j, y] == t[j - x]) continue;
                                else
                                    if (crossword[j, y] != ' ') { ok = true; break; }
                            }
                            if(!ok)
                                for (int j=x;j<x+t.Length;j++)
                                {
                                    if (crossword[j, y] == ' ' && (y + 1 < crossSize && crossword[j, y + 1] != ' ')) { ok = true; break; }
                                    else
                                        if (crossword[j, y] == ' ' && (y - 1 >0 && crossword[j, y - 1] != ' ')) { ok = true; break; }
                                        else
                                            if (crossword[j, y] == ' ') { k++; continue; }
                                }
                            if (k == t.Length-1 && !ok) suit.Add(t);
                        }
                    }
            }
            
            String[] w = suit.ToArray();
            if (suit.Count==0) return false;
            Random ra = new Random();
            String add = w[ra.Next(suit.Count)];
            for (int x = 0; x < crossSize; x++)
                for (int y = 0; y < crossSize; y++)
                {
                    if ((x - 1 < 0 || crossword[x - 1, y] == ' ') && x + add.Length - 1 < crossSize && (x + add.Length >= crossSize || crossword[x + add.Length, y] == ' '))
                    {
                        bool ok = false;
                        int k = 0;
                        int cnt = 0;
                        for (int j = x; j < x + add.Length; j++)
                        {
                            if (y - 1 < 0) continue;
                            if (crossword1[j, y - 1] >= 'A' && crossword1[j, y - 1] <= 'Z') { cnt++; continue; }
                            if (y + 1 >= crossSize) continue;
                            if (crossword1[j, y + 1] >= 'A' && crossword1[j, y + 1] <= 'Z') { cnt++; continue; }
                        }
                        if (cnt > 1) continue;
                        for (int j = x; j < x + add.Length; j++)
                        {
                            if (crossword[j, y] == add[j - x]) continue;
                            else
                                if (crossword[j, y] != ' ') { ok = true; break; }
                        }
                        if (!ok)
                            for (int j = x; j < x + add.Length; j++)
                            {
                                if (crossword[j, y] == ' ' && (y + 1 < crossSize && crossword[j, y + 1] != ' ')) { ok = true; break; }
                                else
                                    if (crossword[j, y] == ' ' && (y - 1 > 0 && crossword[j, y - 1] != ' ')) { ok = true; break; }
                                    else
                                        if (crossword[j, y] == ' ') { k++; continue; }
                            }
                        if (k == add.Length-1 && !ok)
                        {
                            for (int j = x; j < x + add.Length; j++)
                                crossword[j,y] = add[j - x];
                            used.Add(add);
                            return true;
                        }
                    }
                }
            return true;
        }

        public bool putHor()
        {
            List<String> suit = new List<String>();
            for (int q = 0; q < words.Count; q++)
            {
                String t = word[q];
                if (t.Length > 2 && !used.Contains(t))
                for (int x = 0; x < crossSize; x++)
                    for (int y = 0; y < crossSize; y++)
                    {
                        if ((y - 1 < 0 || crossword[x, y - 1] == ' ') && y + t.Length - 1 < crossSize && (y + t.Length >= crossSize || crossword[x, y + t.Length] == ' ') )
                        {
                            bool ok = false;
                            int k = 0;
                            int cnt = 0;
                            for (int j = y; j < y + t.Length; j++)
                            {
                                if (x - 1 < 0) continue;
                                if (crossword1[x - 1, j] >= 'A' && crossword1[x - 1, j] <= 'Z') { cnt++; continue; }
                                if (x + 1 >= crossSize) continue;
                                if (crossword1[x + 1, j] >= 'A' && crossword1[x + 1, j] <= 'Z') { cnt++; continue; }
                            }
                            if (cnt > 1) continue;
                                for (int j = y; j < y + t.Length; j++)
                                {
                                    if (crossword[x, j] == t[j - y]) continue;
                                    else
                                        if (crossword[x, j] != ' ') { ok = true; break; }
                                }
                            if (!ok)
                                for (int j = y; j < y + t.Length; j++)
                                {
                                    if (crossword[x, j] == ' ' && ((x + 1 < crossSize && crossword[x + 1, j] != ' '))) { ok = true; break; }
                                    else
                                        if (crossword[x, j] == ' ' && ((x - 1 > 0 && crossword[x - 1, j] != ' '))) { ok = true; break; }
                                        else
                                            if (crossword[x, j] == ' ') { k++; continue; }

                                }
                            if (k == t.Length-1 && !ok) suit.Add(t);
                        }
                    }
            }
            //MessageBox.Show(suit.Count.ToString());
            String[] w = suit.ToArray();
            //for (int i = 0; i < suit.Count; i++) MessageBox.Show(w[i]);
                if (suit.Count == 0) return false;
            Random ra = new Random();
            String add = w[ra.Next(suit.Count)];
            for (int x = 0; x < crossSize; x++)
                for (int y = 0; y < crossSize; y++)
                {
                    if ((y - 1 < 0 || crossword[x, y - 1] == ' ') && y + add.Length - 1 < crossSize && (y + add.Length >= crossSize || crossword[x, y + add.Length] == ' '))
                    {
                        bool ok = false;
                        int k = 0;
                        int cnt = 0;
                        for (int j = y; j < y + add.Length; j++)
                        {
                            if (x - 1 < 0) continue;
                            if (crossword1[x - 1, j] != ' ') { cnt++; continue; }
                            if (x + 1 >= crossSize) continue;
                            if (crossword1[x + 1, j] != ' ') { cnt++; continue; }
                        }
                        if (cnt > 1) continue;
                        for (int j = y; j < y + add.Length; j++)
                        {
                            if (crossword[x, j] == add[j - y]) continue;
                            else
                                if (crossword[x, j] != ' ') { ok = true; break; }
                        }
                        if (!ok)
                            for (int j = y; j < y + add.Length; j++)
                            {
                                if (crossword[x, j] == ' ' && ((x + 1 < crossSize && crossword[x + 1, j] != ' '))) { ok = true; break; }
                                else
                                    if (crossword[x, j] == ' ' && ((x - 1 > 0 && crossword[x - 1, j] != ' '))) { ok = true; break; }
                                    else
                                        if (crossword[x, j] == ' ') { k++; continue; }

                            }
                        if (k == add.Length-1 && !ok)
                        {
                            for (int j = y; j < y + add.Length; j++)
                                crossword[x, j] = add[j - y];
                            used.Add(add);
                            return true;
                        }
                    }
                }
            return true;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            s = sender;
            e1 = e;
        }

        private void changeColor(int i, Color c)
        {
            this.Controls[i].BackColor = c;
        }

        private void changeText(int i, String text) 
        {
            this.Controls[i].Text = text;
        }

        private void changeVisibility(int i, bool w)
        {
            this.Controls[i].Visible = w;
        }

        public void initArray(object s, EventArgs e)
        {
            alphabetIndexes[0] = this.Controls.GetChildIndex(label111);
            alphabetIndexes[1] = this.Controls.GetChildIndex(label112);
            alphabetIndexes[2] = this.Controls.GetChildIndex(label113);
            alphabetIndexes[3] = this.Controls.GetChildIndex(label114);
            alphabetIndexes[4] = this.Controls.GetChildIndex(label115);
            alphabetIndexes[5] = this.Controls.GetChildIndex(label116);
            alphabetIndexes[6] = this.Controls.GetChildIndex(label117);
            alphabetIndexes[7] = this.Controls.GetChildIndex(label118);
            alphabetIndexes[8] = this.Controls.GetChildIndex(label119);
            alphabetIndexes[9] = this.Controls.GetChildIndex(label120);
            alphabetIndexes[10] = this.Controls.GetChildIndex(label121);
            alphabetIndexes[11] = this.Controls.GetChildIndex(label122);
            alphabetIndexes[12] = this.Controls.GetChildIndex(label123);
            alphabetIndexes[13] = this.Controls.GetChildIndex(label124);
            alphabetIndexes[14] = this.Controls.GetChildIndex(label125);
            alphabetIndexes[15] = this.Controls.GetChildIndex(label126);
            alphabetIndexes[16] = this.Controls.GetChildIndex(label127);
            alphabetIndexes[17] = this.Controls.GetChildIndex(label128);
            alphabetIndexes[18] = this.Controls.GetChildIndex(label129);
            alphabetIndexes[19] = this.Controls.GetChildIndex(label130);
            alphabetIndexes[20] = this.Controls.GetChildIndex(label131);
            alphabetIndexes[21] = this.Controls.GetChildIndex(label132);
            alphabetIndexes[22] = this.Controls.GetChildIndex(label133);
            alphabetIndexes[23] = this.Controls.GetChildIndex(label134);
            alphabetIndexes[24] = this.Controls.GetChildIndex(label135);
            alphabetIndexes[25] = this.Controls.GetChildIndex(label136);
            labelIndexes[0, 0] = this.Controls.GetChildIndex(label1);
            labelIndexes[0, 1] = this.Controls.GetChildIndex(label2);
            labelIndexes[0, 2] = this.Controls.GetChildIndex(label3);
            labelIndexes[0, 3] = this.Controls.GetChildIndex(label4);
            labelIndexes[0, 4] = this.Controls.GetChildIndex(label5);
            labelIndexes[0, 5] = this.Controls.GetChildIndex(label6);
            labelIndexes[0, 6] = this.Controls.GetChildIndex(label7);
            labelIndexes[0, 7] = this.Controls.GetChildIndex(label8);
            labelIndexes[0, 8] = this.Controls.GetChildIndex(label9);
            labelIndexes[0, 9] = this.Controls.GetChildIndex(label10);
            labelIndexes[0, 10] = this.Controls.GetChildIndex(label11);
            labelIndexes[1, 0] = this.Controls.GetChildIndex(label12);
            labelIndexes[1, 1] = this.Controls.GetChildIndex(label13);
            labelIndexes[1, 2] = this.Controls.GetChildIndex(label14);
            labelIndexes[1, 3] = this.Controls.GetChildIndex(label15);
            labelIndexes[1, 4] = this.Controls.GetChildIndex(label16);
            labelIndexes[1, 5] = this.Controls.GetChildIndex(label17);
            labelIndexes[1, 6] = this.Controls.GetChildIndex(label18);
            labelIndexes[1, 7] = this.Controls.GetChildIndex(label19);
            labelIndexes[1, 8] = this.Controls.GetChildIndex(label20);
            labelIndexes[1, 9] = this.Controls.GetChildIndex(label21);
            labelIndexes[1, 10] = this.Controls.GetChildIndex(label22);
            labelIndexes[2, 0] = this.Controls.GetChildIndex(label23);
            labelIndexes[2, 1] = this.Controls.GetChildIndex(label24);
            labelIndexes[2, 2] = this.Controls.GetChildIndex(label25);
            labelIndexes[2, 3] = this.Controls.GetChildIndex(label26);
            labelIndexes[2, 4] = this.Controls.GetChildIndex(label27);
            labelIndexes[2, 5] = this.Controls.GetChildIndex(label28);
            labelIndexes[2, 6] = this.Controls.GetChildIndex(label29);
            labelIndexes[2, 7] = this.Controls.GetChildIndex(label30);
            labelIndexes[2, 8] = this.Controls.GetChildIndex(label31);
            labelIndexes[2, 9] = this.Controls.GetChildIndex(label32);
            labelIndexes[2, 10] = this.Controls.GetChildIndex(label33);
            labelIndexes[3, 0] = this.Controls.GetChildIndex(label34);
            labelIndexes[3, 1] = this.Controls.GetChildIndex(label35);
            labelIndexes[3, 2] = this.Controls.GetChildIndex(label36);
            labelIndexes[3, 3] = this.Controls.GetChildIndex(label37);
            labelIndexes[3, 4] = this.Controls.GetChildIndex(label38);
            labelIndexes[3, 5] = this.Controls.GetChildIndex(label39);
            labelIndexes[3, 6] = this.Controls.GetChildIndex(label40);
            labelIndexes[3, 7] = this.Controls.GetChildIndex(label41);
            labelIndexes[3, 8] = this.Controls.GetChildIndex(label42);
            labelIndexes[3, 9] = this.Controls.GetChildIndex(label43);
            labelIndexes[3, 10] = this.Controls.GetChildIndex(label44);
            labelIndexes[4, 0] = this.Controls.GetChildIndex(label45);
            labelIndexes[4, 1] = this.Controls.GetChildIndex(label46);
            labelIndexes[4, 2] = this.Controls.GetChildIndex(label47);
            labelIndexes[4, 3] = this.Controls.GetChildIndex(label48);
            labelIndexes[4, 4] = this.Controls.GetChildIndex(label49);
            labelIndexes[4, 5] = this.Controls.GetChildIndex(label50);
            labelIndexes[4, 6] = this.Controls.GetChildIndex(label51);
            labelIndexes[4, 7] = this.Controls.GetChildIndex(label52);
            labelIndexes[4, 8] = this.Controls.GetChildIndex(label53);
            labelIndexes[4, 9] = this.Controls.GetChildIndex(label54);
            labelIndexes[4, 10] = this.Controls.GetChildIndex(label55);
            labelIndexes[5, 0] = this.Controls.GetChildIndex(label56);
            labelIndexes[5, 1] = this.Controls.GetChildIndex(label57);
            labelIndexes[5, 2] = this.Controls.GetChildIndex(label58);
            labelIndexes[5, 3] = this.Controls.GetChildIndex(label59);
            labelIndexes[5, 4] = this.Controls.GetChildIndex(label60);
            labelIndexes[5, 5] = this.Controls.GetChildIndex(label61);
            labelIndexes[5, 6] = this.Controls.GetChildIndex(label62);
            labelIndexes[5, 7] = this.Controls.GetChildIndex(label63);
            labelIndexes[5, 8] = this.Controls.GetChildIndex(label64);
            labelIndexes[5, 9] = this.Controls.GetChildIndex(label65);
            labelIndexes[5, 10] = this.Controls.GetChildIndex(label66);
            labelIndexes[6, 0] = this.Controls.GetChildIndex(label67);
            labelIndexes[6, 1] = this.Controls.GetChildIndex(label68);
            labelIndexes[6, 2] = this.Controls.GetChildIndex(label69);
            labelIndexes[6, 3] = this.Controls.GetChildIndex(label70);
            labelIndexes[6, 4] = this.Controls.GetChildIndex(label71);
            labelIndexes[6, 5] = this.Controls.GetChildIndex(label72);
            labelIndexes[6, 6] = this.Controls.GetChildIndex(label73);
            labelIndexes[6, 7] = this.Controls.GetChildIndex(label74);
            labelIndexes[6, 8] = this.Controls.GetChildIndex(label75);
            labelIndexes[6, 9] = this.Controls.GetChildIndex(label76);
            labelIndexes[6, 10] = this.Controls.GetChildIndex(label77);
            labelIndexes[7, 0] = this.Controls.GetChildIndex(label78);
            labelIndexes[7, 1] = this.Controls.GetChildIndex(label79);
            labelIndexes[7, 2] = this.Controls.GetChildIndex(label80);
            labelIndexes[7, 3] = this.Controls.GetChildIndex(label81);
            labelIndexes[7, 4] = this.Controls.GetChildIndex(label82);
            labelIndexes[7, 5] = this.Controls.GetChildIndex(label83);
            labelIndexes[7, 6] = this.Controls.GetChildIndex(label84);
            labelIndexes[7, 7] = this.Controls.GetChildIndex(label85);
            labelIndexes[7, 8] = this.Controls.GetChildIndex(label86);
            labelIndexes[7, 9] = this.Controls.GetChildIndex(label87);
            labelIndexes[7, 10] = this.Controls.GetChildIndex(label88);
            labelIndexes[8, 0] = this.Controls.GetChildIndex(label89);
            labelIndexes[8, 1] = this.Controls.GetChildIndex(label90);
            labelIndexes[8, 2] = this.Controls.GetChildIndex(label91);
            labelIndexes[8, 3] = this.Controls.GetChildIndex(label92);
            labelIndexes[8, 4] = this.Controls.GetChildIndex(label93);
            labelIndexes[8, 5] = this.Controls.GetChildIndex(label94);
            labelIndexes[8, 6] = this.Controls.GetChildIndex(label95);
            labelIndexes[8, 7] = this.Controls.GetChildIndex(label96);
            labelIndexes[8, 8] = this.Controls.GetChildIndex(label97);
            labelIndexes[8, 9] = this.Controls.GetChildIndex(label98);
            labelIndexes[8, 10] = this.Controls.GetChildIndex(label99);
            labelIndexes[9, 0] = this.Controls.GetChildIndex(label100);
            labelIndexes[9, 1] = this.Controls.GetChildIndex(label101);
            labelIndexes[9, 2] = this.Controls.GetChildIndex(label102);
            labelIndexes[9, 3] = this.Controls.GetChildIndex(label103);
            labelIndexes[9, 4] = this.Controls.GetChildIndex(label104);
            labelIndexes[9, 5] = this.Controls.GetChildIndex(label105);
            labelIndexes[9, 6] = this.Controls.GetChildIndex(label106);
            labelIndexes[9, 7] = this.Controls.GetChildIndex(label107);
            labelIndexes[9, 8] = this.Controls.GetChildIndex(label108);
            labelIndexes[9, 9] = this.Controls.GetChildIndex(label109);
            labelIndexes[9, 10] = this.Controls.GetChildIndex(label110);
            labelIndexes[0, 11] = this.Controls.GetChildIndex(label140);
            labelIndexes[0, 12] = this.Controls.GetChildIndex(label141);
            labelIndexes[0, 13] = this.Controls.GetChildIndex(label142);
            labelIndexes[0, 14] = this.Controls.GetChildIndex(label143);
            labelIndexes[1, 11] = this.Controls.GetChildIndex(label147);
            labelIndexes[1, 12] = this.Controls.GetChildIndex(label148);
            labelIndexes[1, 13] = this.Controls.GetChildIndex(label149);
            labelIndexes[1, 14] = this.Controls.GetChildIndex(label150);
            labelIndexes[2, 11] = this.Controls.GetChildIndex(label154);
            labelIndexes[2, 12] = this.Controls.GetChildIndex(label155);
            labelIndexes[2, 13] = this.Controls.GetChildIndex(label156);
            labelIndexes[2, 14] = this.Controls.GetChildIndex(label157);
            labelIndexes[3, 11] = this.Controls.GetChildIndex(label161);
            labelIndexes[3, 12] = this.Controls.GetChildIndex(label162);
            labelIndexes[3, 13] = this.Controls.GetChildIndex(label163);
            labelIndexes[3, 14] = this.Controls.GetChildIndex(label164);
            labelIndexes[4, 11] = this.Controls.GetChildIndex(label168);
            labelIndexes[4, 12] = this.Controls.GetChildIndex(label169);
            labelIndexes[4, 13] = this.Controls.GetChildIndex(label170);
            labelIndexes[4, 14] = this.Controls.GetChildIndex(label171);
            labelIndexes[5, 11] = this.Controls.GetChildIndex(label175);
            labelIndexes[5, 12] = this.Controls.GetChildIndex(label176);
            labelIndexes[5, 13] = this.Controls.GetChildIndex(label177);
            labelIndexes[5, 14] = this.Controls.GetChildIndex(label178);
            labelIndexes[6, 11] = this.Controls.GetChildIndex(label182);
            labelIndexes[6, 12] = this.Controls.GetChildIndex(label183);
            labelIndexes[6, 13] = this.Controls.GetChildIndex(label184);
            labelIndexes[6, 14] = this.Controls.GetChildIndex(label185);
            labelIndexes[7, 11] = this.Controls.GetChildIndex(label189);
            labelIndexes[7, 12] = this.Controls.GetChildIndex(label190);
            labelIndexes[7, 13] = this.Controls.GetChildIndex(label191);
            labelIndexes[7, 14] = this.Controls.GetChildIndex(label192);
            labelIndexes[8, 11] = this.Controls.GetChildIndex(label196);
            labelIndexes[8, 12] = this.Controls.GetChildIndex(label197);
            labelIndexes[8, 13] = this.Controls.GetChildIndex(label198);
            labelIndexes[8, 14] = this.Controls.GetChildIndex(label199);
            labelIndexes[9, 11] = this.Controls.GetChildIndex(label203);
            labelIndexes[9, 12] = this.Controls.GetChildIndex(label204);
            labelIndexes[9, 13] = this.Controls.GetChildIndex(label205);
            labelIndexes[9, 14] = this.Controls.GetChildIndex(label206);
            labelIndexes[10, 0] = this.Controls.GetChildIndex(label210);
            labelIndexes[10, 1] = this.Controls.GetChildIndex(label211);
            labelIndexes[10, 2] = this.Controls.GetChildIndex(label212);
            labelIndexes[10, 3] = this.Controls.GetChildIndex(label213);
            labelIndexes[10, 4] = this.Controls.GetChildIndex(label214);
            labelIndexes[10, 5] = this.Controls.GetChildIndex(label215);
            labelIndexes[10, 6] = this.Controls.GetChildIndex(label216);
            labelIndexes[10, 7] = this.Controls.GetChildIndex(label217);
            labelIndexes[10, 8] = this.Controls.GetChildIndex(label218);
            labelIndexes[10, 9] = this.Controls.GetChildIndex(label219);
            labelIndexes[10, 10] = this.Controls.GetChildIndex(label220);
            labelIndexes[10, 11] = this.Controls.GetChildIndex(label221);
            labelIndexes[10, 12] = this.Controls.GetChildIndex(label222);
            labelIndexes[10, 13] = this.Controls.GetChildIndex(label223);
            labelIndexes[10, 14] = this.Controls.GetChildIndex(label224);
            labelIndexes[11, 0] = this.Controls.GetChildIndex(label228);
            labelIndexes[11, 1] = this.Controls.GetChildIndex(label229);
            labelIndexes[11, 2] = this.Controls.GetChildIndex(label230);
            labelIndexes[11, 3] = this.Controls.GetChildIndex(label231);
            labelIndexes[11, 4] = this.Controls.GetChildIndex(label232);
            labelIndexes[11, 5] = this.Controls.GetChildIndex(label233);
            labelIndexes[11, 6] = this.Controls.GetChildIndex(label234);
            labelIndexes[11, 7] = this.Controls.GetChildIndex(label235);
            labelIndexes[11, 8] = this.Controls.GetChildIndex(label236);
            labelIndexes[11, 9] = this.Controls.GetChildIndex(label237);
            labelIndexes[11, 10] = this.Controls.GetChildIndex(label238);
            labelIndexes[11, 11] = this.Controls.GetChildIndex(label239);
            labelIndexes[11, 12] = this.Controls.GetChildIndex(label240);
            labelIndexes[11, 13] = this.Controls.GetChildIndex(label241);
            labelIndexes[11, 14] = this.Controls.GetChildIndex(label242);
            labelIndexes[12, 0] = this.Controls.GetChildIndex(label246);
            labelIndexes[12, 1] = this.Controls.GetChildIndex(label247);
            labelIndexes[12, 2] = this.Controls.GetChildIndex(label248);
            labelIndexes[12, 3] = this.Controls.GetChildIndex(label249);
            labelIndexes[12, 4] = this.Controls.GetChildIndex(label250);
            labelIndexes[12, 5] = this.Controls.GetChildIndex(label251);
            labelIndexes[12, 6] = this.Controls.GetChildIndex(label252);
            labelIndexes[12, 7] = this.Controls.GetChildIndex(label253);
            labelIndexes[12, 8] = this.Controls.GetChildIndex(label254);
            labelIndexes[12, 9] = this.Controls.GetChildIndex(label255);
            labelIndexes[12, 10] = this.Controls.GetChildIndex(label256);
            labelIndexes[12, 11] = this.Controls.GetChildIndex(label257);
            labelIndexes[12, 12] = this.Controls.GetChildIndex(label258);
            labelIndexes[12, 13] = this.Controls.GetChildIndex(label259);
            labelIndexes[12, 14] = this.Controls.GetChildIndex(label260);
            labelIndexes[13, 0] = this.Controls.GetChildIndex(label264);
            labelIndexes[13, 1] = this.Controls.GetChildIndex(label265);
            labelIndexes[13, 2] = this.Controls.GetChildIndex(label266);
            labelIndexes[13, 3] = this.Controls.GetChildIndex(label267);
            labelIndexes[13, 4] = this.Controls.GetChildIndex(label268);
            labelIndexes[13, 5] = this.Controls.GetChildIndex(label269);
            labelIndexes[13, 6] = this.Controls.GetChildIndex(label270);
            labelIndexes[13, 7] = this.Controls.GetChildIndex(label271);
            labelIndexes[13, 8] = this.Controls.GetChildIndex(label272);
            labelIndexes[13, 9] = this.Controls.GetChildIndex(label273);
            labelIndexes[13, 10] = this.Controls.GetChildIndex(label274);
            labelIndexes[13, 11] = this.Controls.GetChildIndex(label275);
            labelIndexes[13, 12] = this.Controls.GetChildIndex(label276);
            labelIndexes[13, 13] = this.Controls.GetChildIndex(label277);
            labelIndexes[13, 14] = this.Controls.GetChildIndex(label278);
            labelIndexes[14, 0] = this.Controls.GetChildIndex(label282);
            labelIndexes[14, 1] = this.Controls.GetChildIndex(label283);
            labelIndexes[14, 2] = this.Controls.GetChildIndex(label284);
            labelIndexes[14, 3] = this.Controls.GetChildIndex(label285);
            labelIndexes[14, 4] = this.Controls.GetChildIndex(label286);
            labelIndexes[14, 5] = this.Controls.GetChildIndex(label287);
            labelIndexes[14, 6] = this.Controls.GetChildIndex(label288);
            labelIndexes[14, 7] = this.Controls.GetChildIndex(label289);
            labelIndexes[14, 8] = this.Controls.GetChildIndex(label290);
            labelIndexes[14, 9] = this.Controls.GetChildIndex(label291);
            labelIndexes[14, 10] = this.Controls.GetChildIndex(label292);
            labelIndexes[14, 11] = this.Controls.GetChildIndex(label293);
            labelIndexes[14, 12] = this.Controls.GetChildIndex(label294);
            labelIndexes[14, 13] = this.Controls.GetChildIndex(label295);
            labelIndexes[14, 14] = this.Controls.GetChildIndex(label296);

        }

        private void size(int size)
        {
            reset();
            List<int> r = new List<int>();
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                {
                    if (i < size && j < size) changeVisibility(labelIndexes[i, j], true); else changeVisibility(labelIndexes[i, j], false);
                }
        }

        private void x6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(6);
            crossSize = 6;
            size(6);
        }

        private void x8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(8);
            crossSize = 8;
            size(8);
        }

        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(10);
            crossSize = 10;
            size(10);
        }

        private void x7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(7);
            crossSize = 7;
            size(7);
        }

        private void x9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(9);
            crossSize = 9;
            size(9);
        }

        private void x11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(11);
            crossSize = 11;
            size(11);
        }

        private void x12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(12);
            crossSize = 12;
            size(12);
        }

        private void x13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(13);
            crossSize = 13;
            size(13);
        }

        private void x14ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(14);
            crossSize = 14;
            size(14);
        }

        private void x15ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            size(15);
            crossSize = 15;
            size(15);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewGame(0);
        }

        public void reset()
        {
            // reset crossword
            initArray(s,e1);
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                {
                    crossword[i,j]=crossword1[i,j]=' ';
                    changeText(labelIndexes[i, j], "");
                    changeColor(labelIndexes[i, j], Color.White);
                }
        }
        
        public void newReset()
        {
            if (!giveUp)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                        if (hiden.Contains(crossword1[i, j])) { changeColor(alphabetIndexes[crossword1[i, j] - 'A'], Color.White); changeColor(labelIndexes[i, j], Color.SlateBlue); changeText(labelIndexes[i, j], " "); user[i, j] = ' '; }
                        else
                        {
                            if (crossword1[i, j] != ' ') changeColor(alphabetIndexes[crossword1[i, j] - 'A'], Color.Aqua);
                        }
                char[] sel = additional.ToArray();
                for (int i = 0; i < additional.Count; i++)
                    changeColor(alphabetIndexes[sel[i] - 'A'], Color.Aqua);
                selectedLetter = '-';
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            newReset();
        }

        private void fillYellow()
        {
            // filling hiden fields with yellow
            if (hiden.Contains(selectedLetter))
            for (int i=0;i<crossSize;i++)
                for (int j=0;j<crossSize;j++)
                {
                    if (crossword1[i, j] == selectedLetter)
                    {
                        changeColor(labelIndexes[i, j], Color.Yellow);
                    }
                    else
                        if (hiden.Contains(crossword1[i, j])) changeColor(labelIndexes[i,j],Color.SlateBlue); else
                        if (crossword1[i, j] != ' ') changeColor(labelIndexes[i, j], Color.White);
                }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (label1.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 0];
                fillYellow();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (label2.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 1];
                fillYellow();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (label3.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 2];
                fillYellow();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (label4.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 3];
                fillYellow();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (label5.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 4];
                fillYellow();
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            if (label6.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 5];
                fillYellow();
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            if (label7.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 6];
                fillYellow();
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            if (label8.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 7];
                fillYellow();
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            if (label9.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 8];
                fillYellow();
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            if (label10.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 9];
                fillYellow();
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            if (label11.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 10];
                fillYellow();
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {
            if (label12.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 0];
                fillYellow();
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            if (label13.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 1];
                fillYellow();
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            if (label14.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 2];
                fillYellow();
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {
            if (label15.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 3];
                fillYellow();
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {
            if (label16.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 4];
                fillYellow();
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {
            if (label17.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 5];
                fillYellow();
            }
        }

        private void label18_Click(object sender, EventArgs e)
        {
            if (label18.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 6];
                fillYellow();
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {
            if (label19.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 7];
                fillYellow();
            }
        }

        private void label20_Click(object sender, EventArgs e)
        {
            if (label20.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 8];
                fillYellow();
            }
        }

        private void label21_Click(object sender, EventArgs e)
        {
            if (label21.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 9];
                fillYellow();
            }
        }

        private void label22_Click(object sender, EventArgs e)
        {
            if (label22.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 10];
                fillYellow();
            }
        }

        private void label23_Click(object sender, EventArgs e)
        {
            if (label23.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 0];
                fillYellow();
            }
        }

        private void label24_Click(object sender, EventArgs e)
        {
            if (label24.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 1];
                fillYellow();
            }
        }

        private void label25_Click(object sender, EventArgs e)
        {
            if (label25.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 2];
                fillYellow();
            }
        }

        private void label26_Click(object sender, EventArgs e)
        {
            if (label26.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 3];
                fillYellow();
            }
        }

        private void label27_Click(object sender, EventArgs e)
        {
            if (label27.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 4];
                fillYellow();
            }
        }

        private void label28_Click(object sender, EventArgs e)
        {
            if (label28.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 5];
                fillYellow();
            }
        }

        private void label29_Click(object sender, EventArgs e)
        {
            if (label29.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 6];
                fillYellow();
            }
        }

        private void label30_Click(object sender, EventArgs e)
        {
            if (label30.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 7];
                fillYellow();
            }
        }

        private void label31_Click(object sender, EventArgs e)
        {
            if (label31.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 8];
                fillYellow();
            }
        }

        private void label32_Click(object sender, EventArgs e)
        {
            if (label32.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 9];
                fillYellow();
            }
        }

        private void label33_Click(object sender, EventArgs e)
        {
            if (label33.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 10];
                fillYellow();
            }
        }

        private void label34_Click(object sender, EventArgs e)
        {
            if (label34.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 0];
                fillYellow();
            }
        }

        private void label35_Click(object sender, EventArgs e)
        {
            if (label35.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 1];
                fillYellow();
            }
        }

        private void label36_Click(object sender, EventArgs e)
        {
            if (label36.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 2];
                fillYellow();
            }
        }

        private void label37_Click(object sender, EventArgs e)
        {
            if (label37.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 3];
                fillYellow();
            }
        }

        private void label38_Click(object sender, EventArgs e)
        {
            if (label38.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 4];
                fillYellow();
            }
        }

        private void label39_Click(object sender, EventArgs e)
        {
            if (label39.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 5];
                fillYellow();
            }
        }

        private void label40_Click(object sender, EventArgs e)
        {
            if (label40.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 6];
                fillYellow();
            }
        }

        private void label41_Click(object sender, EventArgs e)
        {
            if (label41.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 7];
                fillYellow();
            }
        }

        private void label42_Click(object sender, EventArgs e)
        {
            if (label42.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 8];
                fillYellow();
            }
        }

        private void label43_Click(object sender, EventArgs e)
        {
            if (label43.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 9];
                fillYellow();
            }
        }

        private void label44_Click(object sender, EventArgs e)
        {
            if (label44.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 10];
                fillYellow();
            }
        }

        private void label45_Click(object sender, EventArgs e)
        {
            if (label45.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 0];
                fillYellow();
            }
        }

        private void label46_Click(object sender, EventArgs e)
        {
            if (label46.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 1];
                fillYellow();
            }
        }

        private void label47_Click(object sender, EventArgs e)
        {
            if (label47.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 2];
                fillYellow();
            }
        }

        private void label48_Click(object sender, EventArgs e)
        {
            if (label48.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 3];
                fillYellow();
            }
        }

        private void label49_Click(object sender, EventArgs e)
        {
            if (label49.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 4];
                fillYellow();
            }
        }

        private void label50_Click(object sender, EventArgs e)
        {
            if (label50.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 5];
                fillYellow();
            }
        }

        private void label51_Click(object sender, EventArgs e)
        {
            if (label51.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 6];
                fillYellow();
            }
        }

        private void label52_Click(object sender, EventArgs e)
        {
            if (label52.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 7];
                fillYellow();
            }
        }

        private void label53_Click(object sender, EventArgs e)
        {
            if (label53.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 8];
                fillYellow();
            }
        }

        private void label54_Click(object sender, EventArgs e)
        {
            if (label54.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 9];
                fillYellow();
            }
        }

        private void label55_Click(object sender, EventArgs e)
        {
            if (label55.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 10];
                fillYellow();
            }
        }

        private void label56_Click(object sender, EventArgs e)
        {
            if (label56.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 0];
                fillYellow();
            }
        }

        private void label57_Click(object sender, EventArgs e)
        {
            if (label57.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 1];
                fillYellow();
            }
        }

        private void label58_Click(object sender, EventArgs e)
        {
            if (label58.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 2];
                fillYellow();
            }
        }

        private void label59_Click(object sender, EventArgs e)
        {
            if (label59.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 3];
                fillYellow();
            }
        }

        private void label60_Click(object sender, EventArgs e)
        {
            if (label60.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 4];
                fillYellow();
            }
        }

        private void label61_Click(object sender, EventArgs e)
        {
            if (label61.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 5];
                fillYellow();
            }
        }

        private void label62_Click(object sender, EventArgs e)
        {
            if (label62.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 6];
                fillYellow();
            }
        }

        private void label63_Click(object sender, EventArgs e)
        {
            if (label63.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 7];
                fillYellow();
            }
        }

        private void label64_Click(object sender, EventArgs e)
        {
            if (label64.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 8];
                fillYellow();
            }
        }

        private void label65_Click(object sender, EventArgs e)
        {
            if (label65.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 9];
                fillYellow();
            }
        }

        private void label66_Click(object sender, EventArgs e)
        {
            if (label66.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 10];
                fillYellow();
            }
        }

        private void label67_Click(object sender, EventArgs e)
        {
            if (label67.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 0];
                fillYellow();
            }
        }

        private void label68_Click(object sender, EventArgs e)
        {
            if (label68.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 1];
                fillYellow();
            }
        }

        private void label69_Click(object sender, EventArgs e)
        {
            if (label69.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 2];
                fillYellow();
            }
        }

        private void label70_Click(object sender, EventArgs e)
        {
            if (label70.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 3];
                fillYellow();
            }
        }

        private void label71_Click(object sender, EventArgs e)
        {
            if (label71.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 4];
                fillYellow();
            }
        }

        private void label72_Click(object sender, EventArgs e)
        {
            if (label72.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 5];
                fillYellow();
            }
        }

        private void label73_Click(object sender, EventArgs e)
        {
            if (label73.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 6];
                fillYellow();
            }
        }

        private void label74_Click(object sender, EventArgs e)
        {
            if (label74.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 7];
                fillYellow();
            }
        }

        private void label75_Click(object sender, EventArgs e)
        {
            if (label75.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 8];
                fillYellow();
            }
        }

        private void label76_Click(object sender, EventArgs e)
        {
            if (label76.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 9];
                fillYellow();
            }
        }

        private void label77_Click(object sender, EventArgs e)
        {
            if (label77.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 10];
                fillYellow();
            }
        }

        private void label78_Click(object sender, EventArgs e)
        {
            if (label78.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 0];
                fillYellow();
            }
        }

        private void label79_Click(object sender, EventArgs e)
        {
            if (label79.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 1];
                fillYellow();
            }
        }

        private void label80_Click(object sender, EventArgs e)
        {
            if (label80.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 2];
                fillYellow();
            }
        }

        private void label81_Click(object sender, EventArgs e)
        {
            if (label81.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 3];
                fillYellow();
            }
        }

        private void label82_Click(object sender, EventArgs e)
        {
            if (label82.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 4];
                fillYellow();
            }
        }

        private void label83_Click(object sender, EventArgs e)
        {
            if (label83.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 5];
                fillYellow();
            }
        }

        private void label84_Click(object sender, EventArgs e)
        {
            if (label84.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 6];
                fillYellow();
            }
        }

        private void label85_Click(object sender, EventArgs e)
        {
            if (label85.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 7];
                fillYellow();
            }
        }

        private void label86_Click(object sender, EventArgs e)
        {
            if (label86.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 8];
                fillYellow();
            }
        }

        private void label87_Click(object sender, EventArgs e)
        {
            if (label87.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 9];
                fillYellow();
            }
        }

        private void label88_Click(object sender, EventArgs e)
        {
            if (label88.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 10];
                fillYellow();
            }
        }

        private void label89_Click(object sender, EventArgs e)
        {
            if (label89.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 0];
                fillYellow();
            }
        }

        private void label90_Click(object sender, EventArgs e)
        {
            if (label90.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 1];
                fillYellow();
            }
        }

        private void label91_Click(object sender, EventArgs e)
        {
            if (label91.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 2];
                fillYellow();
            }
        }

        private void label92_Click(object sender, EventArgs e)
        {
            if (label92.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 3];
                fillYellow();
            }
        }

        private void label93_Click(object sender, EventArgs e)
        {
            if (label93.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 4];
                fillYellow();
            }
        }

        private void label94_Click(object sender, EventArgs e)
        {
            if (label94.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 5];
                fillYellow();
            }
        }

        private void label95_Click(object sender, EventArgs e)
        {
            if (label95.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 6];
                fillYellow();
            }
        }

        private void label96_Click(object sender, EventArgs e)
        {
            if (label96.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 7];
                fillYellow();
            }
        }

        private void label97_Click(object sender, EventArgs e)
        {
            if (label97.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 8];
                fillYellow();
            }
        }

        private void label98_Click(object sender, EventArgs e)
        {
            if (label98.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 9];
                fillYellow();
            }
        }

        private void label99_Click(object sender, EventArgs e)
        {
            if (label99.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 10];
                fillYellow();
            }
        }

        private void label100_Click(object sender, EventArgs e)
        {
            if (label100.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 0];
                fillYellow();
            }
        }

        private void label101_Click(object sender, EventArgs e)
        {
            if (label101.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 1];
                fillYellow();
            }
        }

        private void label102_Click(object sender, EventArgs e)
        {
            if (label102.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 2];
                fillYellow();
            }
        }

        private void label103_Click(object sender, EventArgs e)
        {
            if (label103.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 3];
                fillYellow();
            }
        }

        private void label104_Click(object sender, EventArgs e)
        {
            if (label104.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 4];
                fillYellow();
            }
        }

        private void label105_Click(object sender, EventArgs e)
        {
            if (label105.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 5];
                fillYellow();
            }
        }

        private void label106_Click(object sender, EventArgs e)
        {
            if (label106.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 6];
                fillYellow();
            }
        }

        private void label107_Click(object sender, EventArgs e)
        {
            if (label107.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 7];
                fillYellow();
            }
        }

        private void label108_Click(object sender, EventArgs e)
        {
            if (label108.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 8];
                fillYellow();
            }
        }

        private void label109_Click(object sender, EventArgs e)
        {
            if (label109.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 9];
                fillYellow();
            }
        }

        private void label110_Click(object sender, EventArgs e)
        {
            if (label110.BackColor==Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 10];
                fillYellow();
            }
        }

        private void label140_Click(object sender, EventArgs e)
        {
            if (label140.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 11];
                fillYellow();
            }
        }

        private void label141_Click(object sender, EventArgs e)
        {
            if (label141.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 12];
                fillYellow();
            }
        }

        private void label142_Click(object sender, EventArgs e)
        {
            if (label142.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 13];
                fillYellow();
            }
        }

        private void label143_Click(object sender, EventArgs e)
        {
            if (label143.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[0, 14];
                fillYellow();
            }
        }

        private void label147_Click(object sender, EventArgs e)
        {
            if (label147.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 11];
                fillYellow();
            }
        }

        private void label148_Click(object sender, EventArgs e)
        {
            if (label148.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 12];
                fillYellow();
            }
        }

        private void label149_Click(object sender, EventArgs e)
        {
            if (label149.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 13];
                fillYellow();
            }
        }

        private void label150_Click(object sender, EventArgs e)
        {
            if (label150.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[1, 14];
                fillYellow();
            }
        }

        private void label154_Click(object sender, EventArgs e)
        {
            if (label154.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 11];
                fillYellow();
            }
        }

        private void label155_Click(object sender, EventArgs e)
        {
            if (label155.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 12];
                fillYellow();
            }
        }

        private void label156_Click(object sender, EventArgs e)
        {
            if (label156.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 13];
                fillYellow();
            }
        }

        private void label157_Click(object sender, EventArgs e)
        {
            if (label157.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[2, 14];
                fillYellow();
            }
        }

        private void label161_Click(object sender, EventArgs e)
        {
            if (label161.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 11];
                fillYellow();
            }
        }

        private void label162_Click(object sender, EventArgs e)
        {
            if (label162.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 12];
                fillYellow();
            }
        }

        private void label163_Click(object sender, EventArgs e)
        {
            if (label163.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 13];
                fillYellow();
            }
        }

        private void label164_Click(object sender, EventArgs e)
        {
            if (label164.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[3, 14];
                fillYellow();
            }
        }

        private void label168_Click(object sender, EventArgs e)
        {
            if (label168.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 11];
                fillYellow();
            }
        }

        private void label169_Click(object sender, EventArgs e)
        {
            if (label169.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 12];
                fillYellow();
            }
        }

        private void label170_Click(object sender, EventArgs e)
        {
            if (label170.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 13];
                fillYellow();
            }
        }

        private void label171_Click(object sender, EventArgs e)
        {
            if (label171.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[4, 14];
                fillYellow();
            }
        }

        private void label175_Click(object sender, EventArgs e)
        {
            if (label175.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 11];
                fillYellow();
            }
        }

        private void label176_Click(object sender, EventArgs e)
        {
            if (label176.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 12];
                fillYellow();
            }
        }

        private void label177_Click(object sender, EventArgs e)
        {
            if (label177.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 13];
                fillYellow();
            }
        }

        private void label178_Click(object sender, EventArgs e)
        {
            if (label178.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[5, 14];
                fillYellow();
            }
        }

        private void label182_Click(object sender, EventArgs e)
        {
            if (label182.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 11];
                fillYellow();
            }
        }

        private void label183_Click(object sender, EventArgs e)
        {
            if (label183.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 12];
                fillYellow();
            }
        }

        private void label184_Click(object sender, EventArgs e)
        {
            if (label184.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 13];
                fillYellow();
            }
        }

        private void label185_Click(object sender, EventArgs e)
        {
            if (label185.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[6, 14];
                fillYellow();
            }
        }

        private void label189_Click(object sender, EventArgs e)
        {
            if (label189.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 11];
                fillYellow();
            }
        }

        private void label190_Click(object sender, EventArgs e)
        {
            if (label190.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 12];
                fillYellow();
            }
        }

        private void label191_Click(object sender, EventArgs e)
        {
            if (label191.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 13];
                fillYellow();
            }
        }

        private void label192_Click(object sender, EventArgs e)
        {
            if (label192.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[7, 14];
                fillYellow();
            }
        }

        private void label196_Click(object sender, EventArgs e)
        {
            if (label196.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 11];
                fillYellow();
            }
        }

        private void label197_Click(object sender, EventArgs e)
        {
            if (label197.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 12];
                fillYellow();
            }
        }

        private void label198_Click(object sender, EventArgs e)
        {
            if (label198.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 13];
                fillYellow();
            }
        }

        private void label199_Click(object sender, EventArgs e)
        {
            if (label199.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[8, 14];
                fillYellow();
            }
        }

        private void label203_Click(object sender, EventArgs e)
        {
            if (label203.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 11];
                fillYellow();
            }
        }

        private void label204_Click(object sender, EventArgs e)
        {
            if (label204.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 12];
                fillYellow();
            }
        }

        private void label205_Click(object sender, EventArgs e)
        {
            if (label205.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 13];
                fillYellow();
            }
        }

        private void label206_Click(object sender, EventArgs e)
        {
            if (label206.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[9, 14];
                fillYellow();
            }
        }

        private void label210_Click(object sender, EventArgs e)
        {
            if (label210.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 0];
                fillYellow();
            }
        }

        private void label211_Click(object sender, EventArgs e)
        {
            if (label211.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 1];
                fillYellow();
            }
        }

        private void label212_Click(object sender, EventArgs e)
        {
            if (label212.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 2];
                fillYellow();
            }
        }

        private void label213_Click(object sender, EventArgs e)
        {
            if (label213.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 3];
                fillYellow();
            }
        }

        private void label214_Click(object sender, EventArgs e)
        {
            if (label214.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 4];
                fillYellow();
            }
        }

        private void label215_Click(object sender, EventArgs e)
        {
            if (label215.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 5];
                fillYellow();
            }
        }

        private void label216_Click(object sender, EventArgs e)
        {
            if (label216.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 6];
                fillYellow();
            }
        }

        private void label217_Click(object sender, EventArgs e)
        {
            if (label217.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 7];
                fillYellow();
            }
        }

        private void label218_Click(object sender, EventArgs e)
        {
            if (label218.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 8];
                fillYellow();
            }
        }

        private void label219_Click(object sender, EventArgs e)
        {
            if (label219.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 9];
                fillYellow();
            }
        }

        private void label220_Click(object sender, EventArgs e)
        {
            if (label220.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 10];
                fillYellow();
            }
        }

        private void label221_Click(object sender, EventArgs e)
        {
            if (label221.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 11];
                fillYellow();
            }
        }

        private void label222_Click(object sender, EventArgs e)
        {
            if (label222.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 12];
                fillYellow();
            }
        }

        private void label223_Click(object sender, EventArgs e)
        {
            if (label223.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 13];
                fillYellow();
            }
        }

        private void label224_Click(object sender, EventArgs e)
        {
            if (label224.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[10, 14];
                fillYellow();
            }
        }

        private void label228_Click(object sender, EventArgs e)
        {
            if (label228.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 0];
                fillYellow();
            }
        }

        private void label229_Click(object sender, EventArgs e)
        {
            if (label229.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 1];
                fillYellow();
            }
        }

        private void label230_Click(object sender, EventArgs e)
        {
            if (label230.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 2];
                fillYellow();
            }
        }

        private void label231_Click(object sender, EventArgs e)
        {
            if (label231.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 3];
                fillYellow();
            }
        }

        private void label232_Click(object sender, EventArgs e)
        {
            if (label232.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 4];
                fillYellow();
            }
        }

        private void label233_Click(object sender, EventArgs e)
        {
            if (label233.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 5];
                fillYellow();
            }
        }

        private void label234_Click(object sender, EventArgs e)
        {
            if (label234.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 6];
                fillYellow();
            }
        }

        private void label235_Click(object sender, EventArgs e)
        {
            if (label235.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 7];
                fillYellow();
            }
        }

        private void label236_Click(object sender, EventArgs e)
        {
            if (label236.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 8];
                fillYellow();
            }
        }

        private void label237_Click(object sender, EventArgs e)
        {
            if (label237.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 9];
                fillYellow();
            }
        }

        private void label238_Click(object sender, EventArgs e)
        {
            if (label238.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 10];
                fillYellow();
            }
        }

        private void label239_Click(object sender, EventArgs e)
        {
            if (label239.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 11];
                fillYellow();
            }
        }

        private void label240_Click(object sender, EventArgs e)
        {
            if (label240.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 12];
                fillYellow();
            }
        }

        private void label241_Click(object sender, EventArgs e)
        {
            if (label241.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 13];
                fillYellow();
            }
        }

        private void label242_Click(object sender, EventArgs e)
        {
            if (label242.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[11, 14];
                fillYellow();
            }
        }

        private void label246_Click(object sender, EventArgs e)
        {
            if (label246.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 0];
                fillYellow();
            }
        }

        private void label247_Click(object sender, EventArgs e)
        {
            if (label247.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 1];
                fillYellow();
            }
        }

        private void label248_Click(object sender, EventArgs e)
        {
            if (label248.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 2];
                fillYellow();
            }
        }

        private void label249_Click(object sender, EventArgs e)
        {
            if (label249.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 3];
                fillYellow();
            }
        }

        private void label250_Click(object sender, EventArgs e)
        {
            if (label250.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 4];
                fillYellow();
            }
        }

        private void label251_Click(object sender, EventArgs e)
        {
            if (label251.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 5];
                fillYellow();
            }
        }

        private void label252_Click(object sender, EventArgs e)
        {
            if (label252.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 6];
                fillYellow();
            }
        }

        private void label253_Click(object sender, EventArgs e)
        {
            if (label253.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 7];
                fillYellow();
            }
        }

        private void label254_Click(object sender, EventArgs e)
        {
            if (label254.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 8];
                fillYellow();
            }
        }

        private void label255_Click(object sender, EventArgs e)
        {
            if (label255.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 9];
                fillYellow();
            }
        }

        private void label256_Click(object sender, EventArgs e)
        {
            if (label256.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 10];
                fillYellow();
            }
        }

        private void label257_Click(object sender, EventArgs e)
        {
            if (label257.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 11];
                fillYellow();
            }
        }

        private void label258_Click(object sender, EventArgs e)
        {
            if (label258.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 12];
                fillYellow();
            }
        }

        private void label259_Click(object sender, EventArgs e)
        {
            if (label259.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 13];
                fillYellow();
            }
        }

        private void label260_Click(object sender, EventArgs e)
        {
            if (label260.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[12, 14];
                fillYellow();
            }
        }

        private void label264_Click(object sender, EventArgs e)
        {
            if (label264.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 0];
                fillYellow();
            }
        }

        private void label265_Click(object sender, EventArgs e)
        {
            if (label265.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 1];
                fillYellow();
            }
        }

        private void label266_Click(object sender, EventArgs e)
        {
            if (label266.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 2];
                fillYellow();
            }
        }

        private void label267_Click(object sender, EventArgs e)
        {
            if (label267.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 3];
                fillYellow();
            }
        }

        private void label268_Click(object sender, EventArgs e)
        {
            if (label268.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 4];
                fillYellow();
            }
        }

        private void label269_Click(object sender, EventArgs e)
        {
            if (label269.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 5];
                fillYellow();
            }
        }

        private void label270_Click(object sender, EventArgs e)
        {
            if (label270.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 6];
                fillYellow();
            }
        }

        private void label271_Click(object sender, EventArgs e)
        {
            if (label271.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 7];
                fillYellow();
            }
        }

        private void label272_Click(object sender, EventArgs e)
        {
            if (label272.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 8];
                fillYellow();
            }
        }

        private void label273_Click(object sender, EventArgs e)
        {
            if (label273.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 9];
                fillYellow();
            }
        }

        private void label274_Click(object sender, EventArgs e)
        {
            if (label274.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 10];
                fillYellow();
            }
        }

        private void label275_Click(object sender, EventArgs e)
        {
            if (label275.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 11];
                fillYellow();
            }
        }

        private void label276_Click(object sender, EventArgs e)
        {
            if (label276.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 12];
                fillYellow();
            }
        }

        private void label277_Click(object sender, EventArgs e)
        {
            if (label277.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 13];
                fillYellow();
            }
        }

        private void label278_Click(object sender, EventArgs e)
        {
            if (label278.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[13, 14];
                fillYellow();
            }
        }

        private void label282_Click(object sender, EventArgs e)
        {
            if (label282.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 0];
                fillYellow();
            }
        }

        private void label283_Click(object sender, EventArgs e)
        {
            if (label283.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 1];
                fillYellow();
            }
        }

        private void label284_Click(object sender, EventArgs e)
        {
            if (label284.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 2];
                fillYellow();
            }
        }

        private void label285_Click(object sender, EventArgs e)
        {
            if (label285.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 3];
                fillYellow();
            }
        }

        private void label286_Click(object sender, EventArgs e)
        {
            if (label286.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 4];
                fillYellow();
            }
        }

        private void label287_Click(object sender, EventArgs e)
        {
            if (label287.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 5];
                fillYellow();
            }
        }

        private void label288_Click(object sender, EventArgs e)
        {
            if (label288.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 6];
                fillYellow();
            }
        }

        private void label289_Click(object sender, EventArgs e)
        {
            if (label289.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 7];
                fillYellow();
            }
        }

        private void label290_Click(object sender, EventArgs e)
        {
            if (label290.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 8];
                fillYellow();
            }
        }

        private void label291_Click(object sender, EventArgs e)
        {
            if (label291.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 9];
                fillYellow();
            }
        }

        private void label292_Click(object sender, EventArgs e)
        {
            if (label292.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 10];
                fillYellow();
            }
        }

        private void label293_Click(object sender, EventArgs e)
        {
            if (label293.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 11];
                fillYellow();
            }
        }

        private void label294_Click(object sender, EventArgs e)
        {
            if (label294.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 12];
                fillYellow();
            }
        }

        private void label295_Click(object sender, EventArgs e)
        {
            if (label295.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 13];
                fillYellow();
            }
        }

        private void label296_Click(object sender, EventArgs e)
        {
            if (label296.BackColor == Color.SlateBlue)
            {
                selectedLetter = crossword1[14, 14];
                fillYellow();
            }
        }

        private void label111_Click(object sender, EventArgs e)
        {
            if (selectedLetter!='-' && label111.BackColor==Color.White)
            {
                for (int i=0;i<crossSize;i++)
                    for (int j=0;j<crossSize;j++)
                    {
                        if (crossword1[i, j] == selectedLetter) 
                        {
                            if (user[i,j]!=' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'A'; 
                            changeText(labelIndexes[i, j], "A");
                        }
                    }
                changeColor(alphabetIndexes['A' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label112_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label112.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'B';
                            changeText(labelIndexes[i, j], "B");
                        }
                    }
                changeColor(alphabetIndexes['B' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label113_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label113.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'C';
                            changeText(labelIndexes[i, j], "C");
                        }
                    }
                changeColor(alphabetIndexes['C' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label114_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label114.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'D';
                            changeText(labelIndexes[i, j], "D");
                         }
                     }
                 changeColor(alphabetIndexes['D' - 'A'], Color.Aqua);
                 if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }


        private void label115_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label115.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'E';
                            changeText(labelIndexes[i, j], "E");
                        }
                    }
                changeColor(alphabetIndexes['E' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label116_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label116.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'F';
                            changeText(labelIndexes[i, j], "F");
                        }
                    }
                changeColor(alphabetIndexes['F' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label117_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label117.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'G';
                            changeText(labelIndexes[i, j], "G");
                        }
                    }
                changeColor(alphabetIndexes['G' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label118_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label118.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'H';
                            changeText(labelIndexes[i, j], "H");
                        }
                    }
                changeColor(alphabetIndexes['H' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label119_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label119.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'I';
                            changeText(labelIndexes[i, j], "I");
                        }
                    }
                changeColor(alphabetIndexes['I' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label120_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label120.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'J';
                            changeText(labelIndexes[i, j], "J");
                        }
                    }
                changeColor(alphabetIndexes['J' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label121_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label121.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'K';
                            changeText(labelIndexes[i, j], "K");
                        }
                    }
                changeColor(alphabetIndexes['K' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label122_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label122.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'L';
                            changeText(labelIndexes[i, j], "L");
                        }
                    }
                changeColor(alphabetIndexes['L' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label123_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label123.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'M';
                            changeText(labelIndexes[i, j], "M");
                        }
                    }
                changeColor(alphabetIndexes['M' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label124_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label124.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'N';
                            changeText(labelIndexes[i, j], "N");
                        }
                    }
                changeColor(alphabetIndexes['N' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label125_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label125.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'O';
                            changeText(labelIndexes[i, j], "O");
                        }
                    }
                changeColor(alphabetIndexes['O' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label126_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label126.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'P';
                            changeText(labelIndexes[i, j], "P");
                        }
                    }
                changeColor(alphabetIndexes['P' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label127_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label127.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'Q';
                            changeText(labelIndexes[i, j], "Q");
                        }
                    }
                changeColor(alphabetIndexes['Q' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label128_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label128.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'R';
                            changeText(labelIndexes[i, j], "R");
                        }
                    }
                changeColor(alphabetIndexes['R' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label129_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label129.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'S';
                            changeText(labelIndexes[i, j], "S");
                        }
                    }
                changeColor(alphabetIndexes['S' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label130_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label130.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'T';
                            changeText(labelIndexes[i, j], "T");
                        }
                    }
                changeColor(alphabetIndexes['T' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label131_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label131.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'U';
                            changeText(labelIndexes[i, j], "U");
                        }
                    }
                changeColor(alphabetIndexes['U' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label132_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label132.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'V';
                            changeText(labelIndexes[i, j], "V");
                        }
                    }
                changeColor(alphabetIndexes['V' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label133_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label133.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'W';
                            changeText(labelIndexes[i, j], "W");
                        }
                    }
                changeColor(alphabetIndexes['W' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label134_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label134.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'X';
                            changeText(labelIndexes[i, j], "X");
                        }
                    }
                changeColor(alphabetIndexes['X' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label135_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label135.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'Y';
                            changeText(labelIndexes[i, j], "Y");
                        }
                    }
                changeColor(alphabetIndexes['Y' - 'A'], Color.Aqua);
                if (checkCross()) {label138.Visible=true;label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false;}
            }
        }

        private void label136_Click(object sender, EventArgs e)
        {
            if (selectedLetter != '-' && label136.BackColor == Color.White)
            {
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        if (crossword1[i, j] == selectedLetter)
                        {
                            if (user[i, j] != ' ') changeColor(alphabetIndexes[(int)(user[i, j] - 'A')], Color.White);
                            user[i, j] = 'Z';
                            changeText(labelIndexes[i, j], "Z");
                        }
                    }
                changeColor(alphabetIndexes['Z' - 'A'], Color.Aqua);
                if (checkCross()) { label138.Visible = true; label137.Visible = true; button1.Visible = false; button2.Visible = false; button3.Visible = false; button1.Visible = false; button2.Visible = false; button3.Visible = false; }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            giveUp = true;
            level = 0;
            for (char x = 'A'; x <= 'Z';x++)
                changeColor(alphabetIndexes[x - 'A'], Color.Aqua);
                for (int i = 0; i < crossSize; i++)
                    for (int j = 0; j < crossSize; j++)
                    {
                        changeText(labelIndexes[i, j], crossword1[i, j].ToString());
                    }
        }

        private void label137_Click(object sender, EventArgs e)
        {
            level++;
            label139.Visible = false;
            NewGame(level);
        }

    }
}
