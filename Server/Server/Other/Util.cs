using Google.Protobuf.Protocol;
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

        public static string GetLinesWithFileStream(string filePath)
        {
            string line = string.Empty;
            string lines = string.Empty;

            using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(f, System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines += line;
                    }
                }
            }

            return lines;
        }
    }

    public class Vector2Helper
    {
        public static Vector2 Plus(Vector2 lhs, Vector2 rhs)
        {
            Vector2 ret = new Vector2();
            ret.X = lhs.X + rhs.X;
            ret.Y = lhs.Y + rhs.Y;
            return ret;
        }

        public static Vector2 Minus(Vector2 lhs, Vector2 rhs)
        {
            Vector2 ret = new Vector2();
            ret.X = lhs.X - rhs.X;
            ret.Y = lhs.Y - rhs.Y;
            return ret;
        }

        public static float Magnitude(Vector2 vec)
        {
            float ret = 0.0f;

            ret = MathF.Sqrt(MathF.Pow(vec.X, 2) + MathF.Pow(vec.Y, 2));

            return ret;
        }
    }
}
