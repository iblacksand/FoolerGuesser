using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FoolerGuesser
{
    class Program
    {
        List<Password> passwords = new List<Password>();
        List<string> originList = new List<string>();
        private List<string> obcPass = new List<string>();
        private int total;
        private int correct;
        static void Main(string[] args)
        {
            new Program();
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Program()
        {
            originList = new List<string>(File.ReadAllLines("toppasswords.txt"));
            foreach (string pass in originList)
            {
                passwords.Add(new Password(pass));
            }
            foreach (string s in originList)
            {
                string x = "";
                for (int i = 0; i < s.Length; i++)
                {
                    x += s.ToCharArray()[i] + RandomString(random.Next(15) + 6);
                }
                obcPass.Add(x);
            }
            for (int i = 0; i < passwords.Count; i++)
            {
                runtest(i);
            }
            Console.WriteLine("Got " + correct + " out of " + total + "for a percentage of " + ((double)(correct)/(double)(total) * 100.0));
            Console.ReadLine();
        }

        public void runtest(int index)
        {
            passwords = new List<Password>();
            foreach (string pass in originList)
            {
                passwords.Add(new Password(pass));
            }
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
            //Console.WriteLine("Got the position of " + pos + "with shared chars of " + passwords[pos].sharedChars);
            string topchoice = sorted[0].password;
            //Console.WriteLine("with password " + obf + "top choices were " + sorted[0] + "\n" + sorted[1] + "\n" +
                              //sorted[2] + "\n" + sorted[3] + "\n" + sorted[4]);
            if (topchoice.Equals(originpass)) correct++;
            total++;
        }
    }

    class Password
    {
        public string password;
        private bool[] marked;
        public int sharedChars;
        public Password(string password)
        {
            this.password = password;
            sharedChars = 0;
            marked = new bool[password.Length];
            
        }

        private void reset()
        {
            sharedChars = 0;
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
                
                for (int j = last; j < x.Length; j++)
                {
                 string p = x.ToCharArray()[j] + "";
                    if (check.Contains(j)){break;}
                    if (p.Equals(sub))
                        {
                            sharedChars++;
                            
                            //check.Add(j);
                            last = j;
                            break;
                    }
                }
            }
        }
    }
}
