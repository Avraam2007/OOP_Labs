using OOP_Main.States;
using OOP_Main.Utilities;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace OOP_Main.UI {
    public class MainScreen: UIHelper {

        private static int GetHeaderHeight(int panelRows, int rules) {
            if (panelRows < 0 || rules < 0) return 0;

            int panelHeight = 7;
            int ruleHeight = 3;

            return panelRows * panelHeight + rules * ruleHeight;
        }

        private static int GetTopIndex(int selectedIndex, int topIndex, int pageSize) {
            if (selectedIndex < topIndex) {
                return selectedIndex;
            }
            else if (selectedIndex >= topIndex + pageSize) {
                return selectedIndex - pageSize + 1;
            }
            return topIndex;
        }

        private static int GetRawTextLength(string markupText) {
            if (string.IsNullOrEmpty(markupText)) return 0;
            return Regex.Replace(markupText, @"\[[^\]]*\]", "").Length;
        }
        public static T ShowMenuWithLiveClock<T>(
            string headerTitle, 
            List<T> options, 
            User user,
            Func<T, string> displayConverter = null, 
            int pageSize = 10
        ) {
            if (Tools.ValidateArray(options)) {
                return default;
            }

            if (displayConverter == null) {
                displayConverter = option => option.ToString();
            }

            int selectedIndex = 0;
            int topIndex = 0;
            T selectedResult = default;
            bool isSelected = false;

            ClearConsole();

            var layout = new Layout("Root")
                .SplitRows(
                    new Layout("Header"),
                    new Layout("Body")
                );

            AnsiConsole.Live(layout)
                .AutoClear(true)
                .Start(ctx => {
                    while (!isSelected) {
                        int horizontalPadding = 2;
                        int verticalPadding = 1;

                        //4, 2

                        string clockText = DateTime.Now.ToString("dddd, d MMMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        string userRoleDesc = UserRoleExtensions.GetDescription(user.Role);
                        string userText = $"{user.Username} - {userRoleDesc}";

                        var logoPanel = HeaderPanel(headerTitle, Color.Blue)
                            .Padding(horizontalPadding, verticalPadding);

                        var clockPanel = HeaderPanel(clockText, Color.Red3)
                            .Padding(horizontalPadding, verticalPadding);

                        var userPanel = HeaderPanel(userText, Color.Green)
                            .Padding(horizontalPadding, verticalPadding);

                        // 3. Обчислюємо реальну ширину кожної панелі (довжина тексту + відступи + рамка)
                        int logoWidth = GetRawTextLength(headerTitle) + 6;
                        int clockWidth = clockText.Length + 6;
                        int userWidth = userText.Length + 6;

                        int totalWidth = logoWidth + clockWidth + userWidth + 4; // +4 на міжколонкові інтервали
                        int twoColWidth = Math.Max(logoWidth + clockWidth + 2, userWidth);

                        // 4. Отримуємо ширину поточного вікна консолі
                        int windowWidth = Console.WindowWidth > 0 ? Console.WindowWidth : AnsiConsole.Console.Profile.Width;

                        IRenderable headerContent;
                        int headerHeight;

                        // --- FLEX-WRAP ЛОГІКА ---
                        if (windowWidth >= totalWidth) {
                            // Стан 1: Усі 3 панелі в 1 рядок (широкий термінал)
                            headerContent = new Rows(
                                new Padder(new Columns(logoPanel, clockPanel, userPanel)),
                                RuleLine()
                            );
                            headerHeight = GetHeaderHeight(1, 1); // 7 рядків панелі + 2 рядок Rule
                        }
                        else if (windowWidth >= twoColWidth) {
                            // Стан 2: 2 рядки (Logo + Clock зверху, User знизу)
                            headerContent = new Rows(
                                new Padder(new Columns(logoPanel, clockPanel)),
                                new Padder(userPanel),
                                RuleLine()
                            );
                            headerHeight = GetHeaderHeight(2, 1); // (7 + 7) рядків панелей + 2 рядок Rule
                        }
                        else {
                            // Стан 3: 3 рядки (Усі панелі вертикально для вузького екрана)
                            headerContent = new Rows(
                                new Padder(logoPanel),
                                new Padder(clockPanel),
                                new Padder(userPanel),
                                RuleLine()
                            );
                            headerHeight = GetHeaderHeight(3, 1); // (7 * 3) рядків панелей + 2 рядок Rule
                        }

                        // Динамічно змінюємо висоту хедера в зависимости від поточного стан
                        layout["Header"].Size(headerHeight);
                        layout["Header"].Update(headerContent);

                        // --- РЕНДЕРИНГ МЕНЮ (BODY) ---

                        topIndex = GetTopIndex(selectedIndex, topIndex, pageSize);

                        var menuTable = new Table().NoBorder().HideHeaders();
                        menuTable.AddColumn("Option");

                        if (topIndex > 0) {
                            menuTable.AddRow("[grey] /\\ (more items above)[/]");
                        }

                        int endIndex = Math.Min(topIndex + pageSize, options.Count);
                        for (int i = topIndex; i < endIndex; i++) {
                            string displayText = displayConverter(options[i]);
                            if (i == selectedIndex) {
                                menuTable.AddRow($"[blue]> {displayText}[/]");
                            }
                            else {
                                menuTable.AddRow($"  {displayText}");
                            }
                        }

                        if (endIndex < options.Count) {
                            menuTable.AddRow("[grey] \\/ (more items below)[/]");
                        }

                        string headerText = $"Choose option ({selectedIndex + 1}/{options.Count})";
                        var mainMenu = new Panel(menuTable).Header(headerText).BorderColor(Color.Grey);

                        layout["Body"].Update(mainMenu);
                        ctx.Refresh();

                        // --- ЗЧИТУВАННЯ ВВОДУ ---
                        if (Console.KeyAvailable) {
                            var key = Console.ReadKey(true).Key;
                            if (key == ConsoleKey.UpArrow) {
                                selectedIndex = (selectedIndex == 0) ? options.Count - 1 : selectedIndex - 1;
                            }
                            else if (key == ConsoleKey.DownArrow) {
                                selectedIndex = (selectedIndex == options.Count - 1) ? 0 : selectedIndex + 1;
                            }
                            else if (key == ConsoleKey.Enter) {
                                selectedResult = options[selectedIndex];
                                isSelected = true;
                            }
                        }

                        Thread.Sleep(50);
                    }
                });

            return selectedResult;
        }
    }
}
