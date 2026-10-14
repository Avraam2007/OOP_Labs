using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OOP_Main.Utilities {
    internal readonly struct ValidationService {
        /* 
         * ПАТЕРН 1: Валідація Email
         * Пояснення патерну:
         * ^                      - початок рядка
         * [a-zA-Z0-9._%+-]+      - від 1 до багатьох ім'я користувача (букви, цифри, крапка, підкреслення тощо)
         * @                      - обов'язковий символ собачки
         * [a-zA-Z0-9.-]+         - домен першого рівня (наприклад: gmail, nure)
         * \.                     - крапка перед доменною зоною
         * [a-zA-Z]{2,}           - доменна зона мінімум з 2 букв (com, ua, org, net)
         * $                      - кінець рядка
         */
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled
        );

        /* 
         * ПАТЕРН 2: Валідація артикулу товару (підтримує декоратори DISC- / EXTWAR- та базові типи EL, CL, SF, FN, PR)
         * Пояснення патерну:
         * ^                      - початок рядка
         * (DISC-|EXTWAR-)*       - 0 або більше префіксів декораторів
         * (EL|CL|SF|FN|PR|BDL)       - обов'язковий префікс категорії товару (Electronics, Cloth, Sofa, Furniture, Product)
         * -                      - роздільний дефіс
         * \d{4}                  - рівно 4 цифри порядкового номера (наприклад, 0001)
         * $                      - кінець рядка
         */
        private static readonly Regex ArticleRegex = new Regex(
            @"^(?:(?:DISC|EXTWAR|DEC)\d+(?:\.\d+)?-)*(EL|CL|SF|FN|PR|BDL)-\d{4}$",
            RegexOptions.Compiled
        );

        /* 
         * ПАТЕРН 3: Валідація українського номера телефону
         * Пояснення патерну:
         * ^                      - початок рядка
         * (\+?38)?               - бажаний код країни (+38 або 38)
         * 0                      - обов'язковий код оператора, що починається з 0
         * \d{9}                  - рівно 9 цифр номера (наприклад, 671234567)
         * $                      - кінець рядка
         */
        private static readonly Regex UkrPhoneRegex = new Regex(
            @"^(\+?38)?0\d{9}$",
            RegexOptions.Compiled
        );

        public static bool IsValidEmail(string email) => !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

        public static bool IsValidArticle(string article) => !string.IsNullOrWhiteSpace(article) && ArticleRegex.IsMatch(article);

        public static bool IsValidPhone(string phone) => !string.IsNullOrWhiteSpace(phone) && UkrPhoneRegex.IsMatch(phone);
    }
}
