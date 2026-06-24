namespace SLib.IO
{
    public static class IO
    {
        public static readonly string punctuation = "!?,.-:;~";

        /// <summary>
        /// Animated type-writer output
        /// </summary>
        /// <param name="message">text to output</param>
        /// <param name="delay">delay in ms between characters displayed</param>
        /// <param name="newLine">new line after output</param>
        /// <param name="punctuationMultiplier">multiplier for punctuation delay, (delay * puncMutliplier)</param>
        public static void Type(string message, int delay = 90, bool newLine = true, bool showCursor = false, float punctuationMultiplier = 1.4f)
        {
            // Store cursor visibilty before changing it so it can be reverted at the end
            bool cursorVisibility = Console.CursorVisible;
            Console.CursorVisible = showCursor;

            // If delay is <= 0, just write the message instantly
            if (delay <= 0) { Console.Write(message); }
            else
            {
                // For each character, write it out
                foreach (char c in message)
                {
                    Console.Write(c);

                    // If current character is in the punctuation list
                    // then sleep longer (base delay * punctuation multiplier), otherwise sleep for regular time
                    if (punctuation.Contains(c)) { Thread.Sleep((int)(delay * punctuationMultiplier)); }
                    else { Thread.Sleep(delay); }
                }
            }
            // Optional new line
            if (newLine) Console.WriteLine();

            // Restore cursor visibility settings
            Console.CursorVisible = cursorVisibility;
        }

        /// <summary>
        /// Get an input from an array of valid options
        /// </summary>
        /// <param name="options">String array of options</param>
        /// <param name="prompt">Option prompt which is output before requesting an input</param>
        /// <returns>selected option as a string</returns>
        public static string GetOption(string[] options, bool isCaseSensitive = false, string prompt = null)
        {
            // Console.CursorVisible = false;

            // Start input off as invalid option
            string input = "";

            // If isCaseSensitive is false, standardise the array by converting each element to lowercase and trim leading or proceeding white spaces 
            if (!isCaseSensitive) options = options.Select(s => s.ToLower().Trim()).ToArray();

            // While input is not in the list (starts off as true, so loop starts)
            while (!options.Contains(input))
            {
                // If user passed an optional prompt, display it before input each time
                if (!(prompt == null || prompt == "")) { Console.WriteLine(prompt); }

                // Get new (standardised) input
                if (!isCaseSensitive) input = Console.ReadLine().ToLower().Trim();
                else input = Console.ReadLine().Trim();
            }

            // Loop ends when inptu is valid, return input
            return input;
        }

        /// <summary>
        /// Display a list of selectable options and return the selected string
        /// </summary>
        /// <param name="prompt">message prompt</param>
        /// <param name="options">options array</param>
        /// <param name="delay">animated display</param>
        /// <param name="selectedIdentifier">selected option identifier</param>
        /// <returns>selected option as a string</returns>
        public static string GetOptionDropdown(string prompt, string[] options, int delay = 90, string selectedIdentifier = ">", bool colored = false, ConsoleColor color = ConsoleColor.Cyan)
        {
            int index = 0;
            bool selected = false;
            int cursorY = Console.CursorTop;
            // Console.CursorVisible = false;
            ConsoleColor baseColor = Console.ForegroundColor;

            Console.SetCursorPosition(0, cursorY);
            if (delay <= 0) { Console.Write($"{prompt}\n"); } else { Type(prompt, delay); }

            for (int i = 0; i < options.Length; i++)
            {
                bool activeElement = i == index;
                string head = activeElement ? $"{selectedIdentifier} " : $"{string.Concat(Enumerable.Repeat(" ", selectedIdentifier.Length))} ";
                string text = $"{head} {options[i]}\n";
                if (colored) Console.ForegroundColor = activeElement ? color : baseColor;
                if (delay <= 0) Console.Write(text);
                else Type(text, delay, false);
            }

            while (!selected)
            {
                if (colored) Console.ForegroundColor = baseColor;
                Console.SetCursorPosition(0, cursorY);
                Console.Write($"{prompt}\n");

                for (int i = 0; i < options.Length; i++)
                {
                    bool activeElement = i == index;
                    string head = activeElement ? $"{selectedIdentifier} " : $"{string.Concat(Enumerable.Repeat(" ", selectedIdentifier.Length))} ";
                    string text = $"{head} {options[i]}\n";
                    if (colored) Console.ForegroundColor = activeElement ? color : baseColor;
                    Console.Write(text);
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                {
                    index--;
                    if (index < 0) { index = options.Length - 1; }
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                {
                    index++;
                    if (index > options.Length - 1) { index = 0; }
                }
                else if (key.Key == ConsoleKey.Enter) { selected = true; }
            }

            return options[index];
        }

        /// <summary>
        /// Get an input from the user
        /// </summary>
        /// <param name="allowEmpty">permit empty responses</param>
        /// <param name="doHeader">text before input, e.g. ">" </param>
        /// <param name="head">text header</param>
        /// <param name="doTrim">strip leading and trailing whitespace</param>
        /// <param name="normalise">convert to lowercase</param>
        /// <returns>user input as a string</returns>
        public static string Input(bool allowEmpty = false, bool doHeader = true, string head = "> ", bool doTrim = true, bool normalise = true)
        {
            bool cursorVisibility = Console.CursorVisible;
            Console.CursorVisible = true;

            string? input = null;

            while(input == null || (!allowEmpty && input == ""))
            {
                if (doHeader) 
                    Console.Write(head);

                input = Console.ReadLine();
            }

            if (doTrim)
                input = input.Trim();

            if (normalise)
                input = input.ToLowerInvariant();

            Console.CursorVisible = cursorVisibility;

            return input;
        }

        /// <summary>
        /// Prompt the user for input with a message
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="allowEmpty"></param>
        /// <param name="doHeader"></param>
        /// <param name="head"></param>
        /// <param name="stripLeadingOrTrailingWhiteSpaces"></param>
        /// <param name="normalise"></param>
        /// <returns>user input</returns>
        public static string PromptIn(string prompt, bool allowEmpty = false, bool doHeader = true, string head = "> ", bool stripLeadingOrTrailingWhiteSpaces = true, bool normalise = true)
        {
            Console.WriteLine(prompt);
            return Input(allowEmpty, doHeader, head, stripLeadingOrTrailingWhiteSpaces, normalise);
        }
    }
}
