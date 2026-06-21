using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace SLib.Utility
{
    public static class Utility
    {
        #region Generic

        /// <summary>
        /// Log a message with the time and date
        /// </summary>
        /// <param name="text"></param>
        /// <param name="precise">Whether to include milliseconds in the timestamp</param>
        public static void Log(string text, bool precise = false)
        {
            string time = GetTime(precise);
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            Console.WriteLine($"[{date}][{time}]: {text}");
        }

        /// <summary>
        /// Get the current time as a string, optionally including milliseconds
        /// </summary>
        /// <param name="precise"></param>
        /// <returns></returns>
        public static string GetTime(bool precise = false)
        {
            return precise ? DateTime.Now.ToString("HH:mm:ss:fff") : DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// Get the current second as a string
        /// </summary>
        /// <returns></returns>
        public static string GetSecond()
        {
            return DateTime.Now.ToString("ss");
        }

        /// <summary>
        /// Get the current second as an integer
        /// </summary>
        /// <returns></returns>
        public static int GetSecondInt()
        {
            return DateTime.Now.Second;
        }

        /// <summary>
        /// Get the current minute as a string
        /// </summary>
        /// <returns></returns>
        public static string GetMinute()
        {
            return DateTime.Now.ToString("mm");
        }

        /// <summary>
        /// Get the current minute as an integer
        /// </summary>
        /// <returns></returns>

        public static int GetMinuteInt()
        {
            return DateTime.Now.Minute;
        }

        /// <summary>
        /// Get the current hour as a string
        /// </summary>
        /// <returns></returns>
        public static string GetHour()
        {
            return DateTime.Now.ToString("HH");
        }

        /// <summary>
        /// Get the current hour as an integer
        /// </summary>
        /// <returns></returns>
        public static int GetHourInt()
        {
            return DateTime.Now.Hour;
        }

        /// <summary>
        /// Execute a cmd command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="remain_open"></param>
        /// <param name="show_terminal"></param>
        /// <returns>Cmd output</returns>
        public static string RunCommand(string command, bool remain_open = false, bool show_terminal = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/{(remain_open ? "k" : "c")} {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = !show_terminal
            };

            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    process.WaitForExit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Get the current working directory
        /// </summary>
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        #endregion

        #region Extension Methods

        private static readonly Random _random = new Random();

        #region Conversions

        private static readonly NumberStyles Style = NumberStyles.Any;
        private static readonly IFormatProvider Culture = CultureInfo.InvariantCulture;

        #region Integer
        /// <summary> Throws exception if parsing fails. </summary>
        public static int ToInt(this string value) => int.Parse(value, Style, Culture);

        /// <summary> Returns defaultValue if parsing fails. </summary>
        public static int ToIntOrDefault(this string value, int defaultValue = 0) =>
            int.TryParse(value, Style, Culture, out var result) ? result : defaultValue;
        #endregion

        #region Float
        /// <summary> Throws exception if parsing fails. </summary>
        public static float ToFloat(this string value) => float.Parse(value, Style, Culture);

        /// <summary> Returns defaultValue if parsing fails. </summary>
        public static float ToFloatOrDefault(this string value, float defaultValue = 0f) =>
            float.TryParse(value, Style, Culture, out var result) ? result : defaultValue;
        #endregion

        #region Double
        /// <summary> Throws exception if parsing fails. </summary>
        public static double ToDouble(this string value) => double.Parse(value, Style, Culture);

        /// <summary> Returns defaultValue if parsing fails. </summary>
        public static double ToDoubleOrDefault(this string value, double defaultValue = 0.0) =>
            double.TryParse(value, Style, Culture, out var result) ? result : defaultValue;
        #endregion

        #region Decimal
        /// <summary> Throws exception if parsing fails. </summary>
        public static decimal ToDecimal(this string value) => decimal.Parse(value, Style, Culture);

        /// <summary> Returns defaultValue if parsing fails. </summary>
        public static decimal ToDecimalOrDefault(this string value, decimal defaultValue = 0m) =>
            decimal.TryParse(value, Style, Culture, out var result) ? result : defaultValue;
        #endregion

        #region Boolean
        /// <summary> Throws exception if parsing fails. </summary>
        public static bool ToBool(this string value) => bool.Parse(value);

        /// <summary> Returns defaultValue if parsing fails. </summary>
        public static bool ToBoolOrDefault(this string value, bool defaultValue = false) =>
            bool.TryParse(value, out var result) ? result : defaultValue;
        #endregion

        #endregion

        #region Other utilities

        #region IEnumerables
        /// <summary> List out each individual item in a list </summary>
        public static void ListItems<T>(this IEnumerable<T> collection, bool separateLines = true, string header = "", string separate = ", ")
        {
            int x = collection.Count();

            for (int i = 0; i < x; i++)
            {
                var item = collection.ElementAt(i);
                if (separateLines) Console.WriteLine($"{header}{item}");
                else Console.Write($"{item}{(i == x - 1 ? "" : separate)}");
            }
            if (!separateLines) Console.WriteLine();
        }

        /// <summary>
        /// Sum of integers in an enumerable
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int Total(this IEnumerable<int> items)
        {
            int total = 0;
            foreach (int item in items)
            {
                total += item;
            }
            return total;
        }

        /// <summary>
        /// Sum of doubles in an enumerable
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static double Total(this IEnumerable<double> items)
        {
            double total = 0;
            foreach (double item in items)
            {
                total += item;
            }
            return total;
        }

        /// <summary>
        /// Sum of floats in an enumerable
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static float Total(this IEnumerable<float> items)
        {
            float total = 0;
            foreach (float item in items)
            {
                total += item;
            }
            return total;
        }
        #endregion

        /// <summary>
        /// Is not null, empty, or whitespace
        /// </summary>
        public static bool HasContent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Get random element from a list
        /// </summary>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) return default;
            return list[_random.Next(list.Count)];
        }

        /// <summary>
        /// Is a collection null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// Quickly wrap a text in quotations
        /// </summary>
        public static string InQuotes(this string value) => $"\"{value}\"";

        /// <summary>
        /// Is value even
        /// </summary>
        public static bool IsEven(this int value) => value % 2 == 0;

        /// <summary>
        /// Is value odd
        /// </summary>
        public static bool IsOdd(this int value) => !value.IsEven();

        /// <summary>
        /// Returns true 50% of the time, like a coin toss
        /// </summary>
        public static bool FiftyFifty() => _random.Next(2) == 0;

        /// <summary>
        /// Returns true 50% of the time, like a coin toss
        /// </summary>
        public static bool CoinToss() => FiftyFifty();

        /// <summary>
        /// One in x change of returning true
        /// </summary>
        public static bool OneIn(int value) => _random.Next(0, value) == 0;

        /// <summary>
        /// Outputs the value to the console and returns it. Allowing for injection into expressions without breaking them.
        /// </summary>
        public static T Out<T>(this T value, string prefix = "")
        {
            if (!string.IsNullOrEmpty(prefix))
                Console.Write(prefix);

            if (value == null)
            {
                Console.WriteLine("null");
                return value;
            }

            Type type = value.GetType();

            // Simple types
            if (type.IsPrimitive || value is string || value is decimal)
            {
                Console.WriteLine(value);
                return value;
            }

            var sb = new StringBuilder();

            // Properties
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                try // .GetProperties.GetValue Can throw, so we catch and continue
                {
                    object propValue = prop.GetValue(value);
                    sb.Append($"{prop.Name}: {propValue}, ");
                }
                catch
                {
                    sb.Append($"{prop.Name}: <error>, ");
                }
            }

            // Fields 
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object fieldValue = field.GetValue(value);
                sb.Append($"{field.Name}: {fieldValue}, ");
            }

            if (sb.Length > 2)
                sb.Length -= 2;

            Console.WriteLine(sb.ToString());

            return value;
        }
        #endregion

        #endregion
    }
}