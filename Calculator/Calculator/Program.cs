using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Calculator;

namespace Calculator
{
    /// <summary>
    /// Делегат тип для обработки ошибок.
    /// </summary>
    /// <param name="message"></param>
    public delegate void ErrorNotificationType(string message);

    class Program
    {
        /// <summary>
        /// Пути к файлам.
        /// </summary>
        const string path1 = "expressions.txt";
        const string path2 = "answers.txt";
        const string path3 = "expressions_checker.txt";
        const string path4 = "results.txt";

        /// <summary>
        /// Стрингбилдер ошибок (хз, там так просили).
        /// </summary>
        static StringBuilder errors = new StringBuilder();
        static void Main(string[] args)
        {
            // Заполнение словаря операций.
            Calculator.Operations.Add("+", (a, b) => a + b);
            Calculator.Operations.Add("-", (a, b) => a - b);
            Calculator.Operations.Add("*", (a, b) => a * b);
            Calculator.Operations.Add("/", (a, b) => a / b);
            Calculator.Operations.Add("^", (a, b) => Math.Pow(a, b));

            // Чтение, счет и проверка.
            Calculator.ReadAndWrite(path1, path2);
            Calculator.Check(path3, path2, path4);
        }

        /// <summary>
        /// Метод обработкки ошибки. Выводит в консоль информацию о ней.
        /// </summary>
        /// <param name="message"></param>
        public static void ConsoleErrorHandler(string message)
        {
            Console.WriteLine(message + " " + DateTime.Now);
        }

        /// <summary>
        /// Запись ошибок в стрингбилдер.
        /// </summary>
        /// <param name="message"></param>
        public static void ResultErrorHandler(string message)
        {
            errors.Append(message);
        }
    }

    /// <summary>
    /// Класс, описывающий калькулятор.
    /// </summary>
    public class Calculator
    {
        /// <summary>
        /// Эвент для обработки ошибок.
        /// </summary>
        public static event ErrorNotificationType ErrorNotification = Program.ConsoleErrorHandler;

        /// <summary>
        /// Делегат для вычислений в калькуляторе.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public delegate double MathOperation(double a, double b);

        /// <summary>
        /// Словарь операций.
        /// </summary>
        public static Dictionary<string, MathOperation> Operations = new Dictionary<string, MathOperation>();

        /// <summary>
        /// Статический конструктор, подписывающий еще один метод на событие.
        /// </summary>
        static Calculator()
        {
            ErrorNotification += Program.ResultErrorHandler;
        }

        /// <summary>
        /// Вычисление выражения в строке.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string Calculate(string expr)
        {
            double op1;
            double op2;
            string[] operands = expr.Split(' ');
            // Шаманство и счет.
            if(double.TryParse(operands[0], out op1) & double.TryParse(operands[2], out op2))
            {
                if (Operations.ContainsKey(operands[1]))
                {
                    if(operands[1] == "/" && op2 == 0)
                    {
                        ErrorNotification("Деление на 0");
                        return "bruh";
                    }
                    return $"{Operations[operands[1]](op1, op2):F3}";
                }
                else
                {
                    ErrorNotification("Неверный оператор");
                    return "неверный оператор";
                }
            }
            else
            {
                return (1.0 / 0).ToString();
            }
        }

        /// <summary>
        /// Чтение выражений из файла, счет и запись результатов в другой файл. 
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        public static void ReadAndWrite(string path1, string path2)
        {
            string[] lines = new string[0];
            string output = string.Empty;

            // Стандартная дичь.
            try
            {
                lines = File.ReadAllLines(path1);
            }
            catch(UnauthorizedAccessException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch(System.Security.SecurityException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (IOException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (Exception e)
            {
                ErrorNotification?.Invoke(e.Message);
            }

            // Подсчет значений в строках.
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    output += $"{Calculate(lines[i]):F3}{Environment.NewLine}";
                }
                catch (Exception e)
                {
                    ErrorNotification(e.Message);
                }
            }

            // Стандартная запись в файл.
            try
            {
                File.WriteAllText(path2, output);
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (IOException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (Exception e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Сравнение значений с референсными из файла.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        public static void Check(string path1, string path2, string path3)
        {
            string[] check = new string[0];
            string[] answers = new string[0];
            string output = string.Empty;

            // Счетчик ошибок.
            int errors = 0;

            // Стандартное чтение.
            try
            {
                check = File.ReadAllLines(path1);
                answers = File.ReadAllLines(path2);
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (IOException e)
            {
                ErrorNotification?.Invoke(e.Message);
            }
            catch (Exception e)
            {
                ErrorNotification?.Invoke(e.Message);
            }

            // Сравнение длин массивов.
            if (check.Length == answers.Length)
            {
                // Сравнение значений через жеппу (там какие-то проблемы с парсом "не числа").
                for (int i = 0; i < check.Length; i++)
                {
                    if (answers[i].Contains("число"))
                    {
                        if (check[i].Contains("число"))
                        {
                            output += "OK" + Environment.NewLine;
                        }
                        else
                        {
                            output += "Error" + Environment.NewLine;
                            errors++;
                        }
                    }
                    else
                    {
                        double res;
                        double chk;
                        if (double.TryParse(answers[i], out res) & double.TryParse(check[i], out chk))
                        {
                            if (res == chk)
                            {
                                output += "OK" + Environment.NewLine;
                            }
                            else
                            {
                                output += "Error" + Environment.NewLine;
                                errors++;
                            }
                        }
                        else
                        {
                            if (answers[i] == check[i])
                            {
                                output += "OK" + Environment.NewLine;
                            }
                            else
                            {
                                output += "Error" + Environment.NewLine;
                                errors++;
                            }

                        }
                    }
                }
                // Добавление в конец вывода количества ошибок.
                output += errors;

                try
                {
                    File.WriteAllText(path3, output);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine($"Sizes do not match: answers size is {answers.Length}, check size is {check.Length}" );
            }
        }
    }
}
