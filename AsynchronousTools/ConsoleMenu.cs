namespace AsynchronousTools
{
    using System;
    using System.Collections;

    public class ConsoleMenu
    {
        private readonly ArrayList menuItems = new ArrayList();

        public delegate void MenuCallback();

        public void Add(string text, MenuCallback mc)
        {
            this.menuItems.Add(new ConsoleMenuItem(text, mc));
        }

        public void Show()
        {
            Console.WriteLine();
            for (var i = 0; i < this.menuItems.Count; ++i)
            {
                var mi = this.menuItems[i] as ConsoleMenuItem;
                Console.WriteLine(" [{0}] {1}", i + 1, mi.Text);
            }

            int choosen;
            var succes = int.TryParse(Console.ReadKey().KeyChar.ToString(), out choosen);

            if (!succes || choosen > this.menuItems.Count || choosen < 1)
            {
                Console.WriteLine("Invalid option.");
            }
            else
            {
                Console.WriteLine();
                var mi = this.menuItems[choosen - 1] as ConsoleMenuItem;
                if (mi != null)
                {
                    var mc = mi.Mc;
                    mc();
                }
            }
        }
    }
}