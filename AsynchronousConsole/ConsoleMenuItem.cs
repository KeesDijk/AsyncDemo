namespace AsynchronousConsole
{
    public class ConsoleMenuItem
    {
        public ConsoleMenuItem(string text, ConsoleMenu.MenuCallback mc)
        {
            this.Mc = mc;
            this.Text = text;
        }

        public ConsoleMenu.MenuCallback Mc { get; set; }

        public string Text { get; set; }
    }
}