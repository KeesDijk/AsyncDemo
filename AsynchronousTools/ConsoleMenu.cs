namespace AsynchronousTools
{
    using System;
    using System.Collections.Generic;

    public class ConsoleMenu
    {
        private const string Menuchoices = "0123456789abcdefghijklmnoprstuvw";

        private readonly Dictionary<char, ConsoleMenuItem> menuItems = new Dictionary<char, ConsoleMenuItem>();

        public delegate void MenuCallback();

        public void Add(string text, MenuCallback mc)
        {
            var currentLength = this.menuItems.Count;
            var key = Menuchoices[currentLength];
            this.menuItems.Add(key, new ConsoleMenuItem(text, mc));
        }

        public void Show()
        {
            Console.WriteLine();

            foreach (KeyValuePair<char, ConsoleMenuItem> consoleMenuItem in this.menuItems)
            {
                Console.WriteLine(" [{0}] {1}", consoleMenuItem.Key, consoleMenuItem.Value.Text);
            }

            bool validOption;
            var choosen = this.GetChoice(out validOption);

            if (validOption)
            {
                Console.WriteLine();
                var mi = this.menuItems[choosen];
                if (mi != null)
                {
                    var mc = mi.Mc;
                    mc();
                }
            }
        }

        private char GetChoice(out bool validOption)
        {
            validOption = true;
            var chosenCharacter = Console.ReadKey().KeyChar;

            if (!this.menuItems.ContainsKey(chosenCharacter))
            {
                Console.WriteLine("Invalid option.");
                validOption = false;
            }

            return chosenCharacter;
        }
    }
}