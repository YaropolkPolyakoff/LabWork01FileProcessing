using System;
using System.Collections.Generic;
using System.IO;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Utils
{
    public class FileHelper
    {
        // Ответственность: Низкоуровневое чтение
        private static string[] AcquireTextContent(string location)
        {
            return File.ReadAllLines(location, System.Text.Encoding.UTF8);
        }
        
        // Ответственность: Фильтрация текста
        private static bool ShouldProcessTextLine(string input)
        {
            string clean = input.Trim();
            return clean.Length > 0 && !clean.StartsWith("#");
        }
        
        // Ответственность: Трансформация pipe-delimited в Animal
        private static Animal ParsePipeDelimitedAnimal(string input)
        {
            string[] parts = input.Split('|');
            if (parts.Length < 5)
                throw new FormatException("Не все поля присутствуют: " + input);
            
            return new Animal(parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim(), int.Parse(parts[4].Trim()));
        }
        
        // Ответственность: Проверка наличия префикса ключа
        private static bool HasKeyPrefix(string line, string keyName)
        {
            string prefix = (keyName + "=").ToLower();
            return line.ToLower().StartsWith(prefix);
        }
        
        // Ответственность: Получение значения после знака равенства
        private static string GetValueAfterEqualsSign(string line, int keyLength)
        {
            int skipChars = keyLength + 1;  // +1 для знака '='
            return line.Substring(skipChars).Trim();
        }
        
        // Ответственность: Построение словаря данных покупателя
        private static void PopulateCustomerDataFrom(string line, Dictionary<string, string> storage)
        {
            if (HasKeyPrefix(line, "name"))
                storage["name"] = GetValueAfterEqualsSign(line, 4);
            else if (HasKeyPrefix(line, "age"))
                storage["age"] = GetValueAfterEqualsSign(line, 3);
            else if (HasKeyPrefix(line, "email"))
                storage["email"] = GetValueAfterEqualsSign(line, 5);
            else if (HasKeyPrefix(line, "phone"))
                storage["phone"] = GetValueAfterEqualsSign(line, 5);
        }
        
        // Ответственность: Валидация покупателя
        private static void AssertCustomerDataComplete(Dictionary<string, string> customerData)
        {
            bool hasName = customerData.ContainsKey("name") && !string.IsNullOrWhiteSpace(customerData["name"]);
            bool hasAge = customerData.ContainsKey("age") && !string.IsNullOrWhiteSpace(customerData["age"]);
            bool hasEmail = customerData.ContainsKey("email") && !string.IsNullOrWhiteSpace(customerData["email"]);
            
            if (!hasName)
            {
                throw new ArgumentException("В файле покупателя не найдено поле 'name'. Проверьте формат файла.");
            }
            
            if (!hasAge)
            {
                throw new ArgumentException("В файле покупателя не найдено корректное поле 'age'. Проверьте формат файла.");
            }
            
            if (!hasEmail)
            {
                throw new ArgumentException("В файле покупателя не найдено поле 'email'. Проверьте формат файла.");
            }
        }
        
        public static List<Animal> ReadAnimalsFromFile(string filePath)
        {
            List<Animal> animals = new List<Animal>();
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с животными не найден: " + filePath);
            }
            
            try
            {
                string[] contentLines = AcquireTextContent(filePath);
                
                foreach (string rawLine in contentLines)
                {
                    if (!ShouldProcessTextLine(rawLine))
                        continue;
                    
                    try  
                    {
                        Animal parsedAnimal = ParsePipeDelimitedAnimal(rawLine.Trim());
                        animals.Add(parsedAnimal);
                    }
                    catch (FormatException formatEx)
                    {
                        Console.WriteLine("Предупреждение: " + formatEx.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка: " + ex.Message);
                    }
                }
            }
            catch (IOException ioEx)
            {
                throw new IOException("Ошибка чтения файла: " + ioEx.Message, ioEx);
            }
            
            return animals;
        }

        /// <summary>
        /// Загружает список животных по простому названию списка.
        /// Обычные пользователи используют простые команды вместо путей к файлам.
        /// </summary>
        /// <param name="listType">Тип списка: "zoo", "default" или пусто</param>
        /// <returns>Список животных</returns>
        public static List<Animal> LoadAnimalsByType(string listType)
        {
            List<Animal> animals = new List<Animal>();
            
            // Нормализуем ввод пользователя
            string normalizedType = (listType ?? "").Trim().ToLower();
            
            // Если пользователь указал "zoo" или "full" - загружаем из файла animals.txt
            if (normalizedType == "zoo" || normalizedType == "full" || normalizedType == "зоопарк")
            {
                string filePath = "animals.txt";
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Файл animals.txt не найден. Загружаем животных по умолчанию.");
                    return GetDefaultAnimals();
                }
                
                try
                {
                    animals = ReadAnimalsFromFile(filePath);
                    Console.WriteLine("Загружен полный список животных из зоопарка!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при загрузке списка: " + ex.Message);
                    Console.WriteLine("Загружаем животных по умолчанию.");
                    return GetDefaultAnimals();
                }
            }
            else
            {
                // По умолчанию или если указано "default"
                animals = GetDefaultAnimals();
                Console.WriteLine("Загружены животные по умолчанию.");
            }
            
            return animals;
        }
        
        /// <summary>
        /// Возвращает список животных по умолчанию
        /// </summary>
        private static List<Animal> GetDefaultAnimals()
        {
            List<Animal> animals = new List<Animal>();
            animals.Add(new Animal("Лев Симба", "Лев африканский", "Саванна", "Царь зверей", 5));
            animals.Add(new Animal("Слон Дамбо", "Слон индийский", "Тропический лес", "Умный гигант", 12));
            animals.Add(new Animal("Пингвин Коля", "Пингвин императорский", "Антарктида", "Смешной птиц", 3));
            return animals;
        }

        public static Customer ReadCustomerFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с информацией о покупателе не найден: " + filePath);
            }
            
            try
            {
                string[] contentLines = AcquireTextContent(filePath);
                Dictionary<string, string> customerInfo = new Dictionary<string, string>();
                
                foreach (string rawLine in contentLines)
                {
                    if (!ShouldProcessTextLine(rawLine))
                        continue;
                    
                    PopulateCustomerDataFrom(rawLine.Trim(), customerInfo);
                }
                
                AssertCustomerDataComplete(customerInfo);
                
                Console.WriteLine("Данные покупателя успешно загружены из файла");
                
                return new Customer(customerInfo["name"], int.Parse(customerInfo["age"]), 
                    customerInfo["email"], customerInfo.ContainsKey("phone") ? customerInfo["phone"] : null);
            }
            catch (IOException ioEx)
            {
                throw new IOException("Ошибка чтения файла: " + ioEx.Message, ioEx);
            }
        }

        public static List<string> ReadPurchaseRequestFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с запросом на покупку не найден: " + filePath);
            }
            using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
            
            // Используем StreamWriter для чтения файла
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs, new System.Text.UTF8Encoding(true)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }
                    
                    ticketTypes.Add(line);
                }
            }
            
            return ticketTypes;
        }

        public static void WriteOutputToFile(string filePath, string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);
                // Используем StreamWriter для записи в файл
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    writer.Write(content);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при записи в файл: " + ex.Message);
                throw;
            }
        }

        public static void AppendToOutputFile(string filePath, string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                // Используем StreamWriter для добавления в файл
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(content);
                }
                File.AppendAllText(filePath, content + Environment.NewLine, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при добавлении в файл: " + ex.Message);
                throw;
            }
        }
    }
}

