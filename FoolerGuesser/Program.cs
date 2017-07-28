using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FoolerGuesser
{
    public class OutputCapture : TextWriter, IDisposable
    {
        private TextWriter stdOutWriter;
        public TextWriter Captured { get; private set; }
        public override Encoding Encoding { get { return Encoding.ASCII; } }

        public OutputCapture()
        {
            this.stdOutWriter = Console.Out;
            Console.SetOut(this);
            Captured = new StringWriter();
        }

        override public void Write(string output)
        {
            // Capture the output and also send it to StdOut
            Captured.Write(output);
            stdOutWriter.Write(output);
        }

        override public void WriteLine(string output)
        {
            // Capture the output and also send it to StdOut
            Captured.WriteLine(output);
            stdOutWriter.WriteLine(output);
        }
    }
    class Program
    {
        static List<Password> passwords = new List<Password>();
        static List<string> originList = new List<string>();
        static StreamWriter w = new StreamWriter("table.csv");
        private List<string> obcPass = new List<string>();
        static ProgressBar progress = new ProgressBar();
        private List<int> devs;
        private int total;
        private int correct;
        static void Main(string[] args)
        {
            originList = new List<string>(File.ReadAllLines("toppasswords-100000.txt"));
            foreach (string pass in originList)
            {
                passwords.Add(new Password(pass));
            }

            for (int i = 0; i < 10; i++)
            {
                new Program(i);
            }
            Console.WriteLine("PRESS ENTER TO CLOSE");
            Console.ReadLine();
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private double averg(List<int> x)
        {
            double avg = 0.0;
            foreach (int i in x) avg += i;
            return avg / x.Count;
        }

        public Program(int len)
        {
            obcPass = new List<string>();
            devs = new List<int>();
            if (len == 0) w.WriteLine("Random Insertion Length, Percent Correct, Average Position");
            foreach (string s in originList)
            {
                string x = "";
                for (int i = 0; i < s.Length; i++)
                {
                    //x += s.ToCharArray()[i] + RandomString(random.Next(15) + 6);
                    x += s.ToCharArray()[i] + RandomString(len);
                }
                obcPass.Add(x);
            }
            for (int i = 0; i < passwords.Count; i++)
            {
                runtest(i);
            }
            //Console.WriteLine("Got " + correct + " out of " + total + " for a percentage of " + ((double)(correct)/(double)(total) * 100.0));
            //Console.ReadLine();
            double avg = averg(devs);
            w.WriteLine(len + "," + ((double)(correct) / (double)(total) * 100.0) + "," + avg);
            w.Flush();
            
            progress.Report((double)len / 10);
        }

        public void runtest(int index)
        {
            //foreach (string pass in originList)
            //{
            //    passwords.Add(new Password(pass));
            //}
            String originpass = originList[index];
            String obf = obcPass[index];
            for (int i = 0; i < passwords.Count; i++)
            {
                passwords[i].compare(obf);
            }
            List<Password> sorted = passwords.OrderByDescending(o => o.sharedChars).ToList();
            //sorted.RemoveAll(o => o.sharedChars > originpass.Length);
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(sorted[i]);
            //}
            int pos = sorted.FindIndex(o => o.password == originpass);
            devs.Add(pos);
            //Console.WriteLine("Got the position of " + pos + "with shared chars of " + passwords[pos].sharedChars);
            string topchoice = sorted[0].password;
            //Console.WriteLine("with password " + obf + "top choices were " + sorted[0] + "\n" + sorted[1] + "\n" +
            //sorted[2] + "\n" + sorted[3] + "\n" + sorted[4]);
            if (topchoice.Equals(originpass))
            {
                correct++;
                //Console.WriteLine(sorted[0]);
            }
            //Console.WriteLine("Correct Pos : " + pos + "\n" + sorted[0
            total++;
        }
    }

    class Password
    {
        public string password;
        private bool[] marked;
        public double sharedChars;
        public Password(string password)
        {
            this.password = password;
            sharedChars = 0;
            marked = new bool[password.Length];
            
        }

        private void reset()
        {
            sharedChars = 0.0;
            for (int i = 0; i < marked.Length; i++)
            {
                marked[i] = false;
            }
        }

        public override string ToString()
        {
            return "Password: " + password + "\nShared Chars : " + sharedChars;
        }

        public void compare(string x)
        {
            reset();
            List<int> check = new List<int>();
            int last = 0;
            for (int i = 0; i < password.Length; i++)
            {
                string sub = password.ToCharArray()[i] + "";
                bool found = false;
                for (int j = last; j < x.Length; j++)
                {
                 string p = x.ToCharArray()[j] + "";
                    //if (check.Contains(j)){break;}
                    if (p.Equals(sub))
                        {
                            sharedChars++;
                            
                            //check.Add(j);
                            last = j + 1;
                        found = true;
                            break;
                    }
                }
                if (!found) break;
            }
            sharedChars = sharedChars / password.Length;
        }
    }
}
