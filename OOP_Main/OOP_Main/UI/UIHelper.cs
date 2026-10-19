using NOptional;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OOP_Main.UI {
    public class UIHelper {
        public delegate string ConsolePromptHandler(string text); // Func<string, string>

        protected string GetEnumDescription(Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null) return value.ToString();
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        protected static void StatusSpinner(string text, Action actionDuringLoading = null, int loadingTimeInMs = 2400) {
            IOptional<Action> DoSmth = Optional.OfNullable(actionDuringLoading);
            AnsiConsole.Status()
                .Start(text, ctx => {
                    DoSmth.GetValueOrElse(() => { })();
                    Thread.Sleep(loadingTimeInMs);
                });
        }

        protected static T PromptWithDefaultValue<T>(string text, T defaultValue) {
            T value = AnsiConsole.Ask(text, defaultValue);

            return value is string val ? (T)(object)Markup.Escape(val) : value;
        }

        protected static T DefaultTextPrompt<T>(string text) {
            var prompt = new TextPrompt<T>(text);
            T value = AnsiConsole.Prompt(prompt);

            return value is string val ? (T)(object)Markup.Escape(val) : value;
        }

        public static void DefaultMarkupOutput(string text) {
            AnsiConsole.MarkupLine(text);
        }

        public static void DefaultErrorOutput(string message) {
            DefaultMarkupOutput($"[red bold]{message}[/]");
        }

        public static void DefaultSuccessOutput(string message) {
            DefaultMarkupOutput($"[green bold]{message}[/]");
        }

        protected static Action ClearConsole = () => {
            AnsiConsole.Clear();
        };

        protected static ConsolePromptHandler PasswordPrompt = text => {
            var passwordPrompt = new TextPrompt<string>(text)
                .Secret();

            return AnsiConsole.Prompt(passwordPrompt);
        };

        protected static void InternalFlashCard(Dictionary<string, IRenderable> dataForCard) {
            var contentElements = dataForCard.Values.ToList();
            var content = new Rows(contentElements);
            var panel = new Panel(content)
            .BorderColor(Color.Blue)
            .Padding(4, 2);

            AnsiConsole.Write(panel);
        }

        protected static Panel HeaderPanel(string text, Color panelColor) {
            return new Panel(
                new Markup($"[bold]{text}[/]", new Style(foreground: panelColor))
            )
            .BorderColor(panelColor);
        }

        protected static Rule RuleLine() {
            return new Rule();
        }

        protected static Action<string> ShowHeader = text => {
            ClearConsole();
            var logoPanel = HeaderPanel(text, Color.Blue).Padding(2, 2);

            var header = new Padder(logoPanel);

            AnsiConsole.Write(header);
            AnsiConsole.Write(RuleLine());
        };

        protected static bool CheckIfUserIsAdmin(IOptional<User> currentUser) {
            return currentUser.HasValue() && currentUser.Value.IsAdmin();
        }
        protected static bool CheckIfUserIsAdmin(User currentUser) => CheckIfUserIsAdmin(Optional.OfNullable(currentUser));

        protected static string BackToMenuPrompt(string extraOption = "") {
            List<string> options = new List<string> { "Back to menu" };
            if (!string.IsNullOrWhiteSpace(extraOption)) {
                options.Add(extraOption);
            }
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"\nPress ESC or Enter to back to menu{(extraOption != "" ? $" or choose \"{extraOption}\" option" : "")}")
                    .AddCancelResult("Back to menu")
                    .DefaultValue("Back to menu")
                    .AddChoices(options));

            return choice;
        }

        protected bool DefaultConfirm(string text, bool defaultValue = true) {
            return AnsiConsole.Confirm(text, defaultValue);
        }

        protected void RenderProductsTable(IEnumerable<Product> products) {
            DefaultMarkupOutput($"[green]Found products: {products.Count()}[/]");
            var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Blue);
            table.AddColumn("Article");
            table.AddColumn("Name");
            table.AddColumn("Category");
            table.AddColumn("Price ($)");

            foreach (var prod in products) {
                table.AddRow(
                    prod.Article,
                    prod.Name,
                    prod.Category ?? "N/A",
                    prod.Price.ToString("F2")
                );
            }

            table.Columns[0].Header = new Text("Article", new Style(foreground: Color.Yellow));
            table.Columns[1].Header = new Text("Name", new Style(foreground: Color.Yellow));
            table.Columns[2].Header = new Text("Category", new Style(foreground: Color.Yellow));
            table.Columns[3].Header = new Text("Price ($)", new Style(foreground: Color.Yellow));

            AnsiConsole.Write(table);
        }
    }
}
