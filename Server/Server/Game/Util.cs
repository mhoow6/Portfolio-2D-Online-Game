using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    public class Util
    {
        public static List<string> GetLinesFromTableFileStream(string filePath)
        {
            string line = string.Empty;
            List<string> lines = new List<string>();

            using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(f, System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines;
        }
    }

    public class SingleTon<T> where T : class, new()
    {
        private static T inst = null;

        public SingleTon() { }

        public static T Instance
        {
            get
            {
                if (inst == null)
                    inst = new T();
                return inst;
            }
        }

    }
}
