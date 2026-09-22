using System;
using System.Collections.Generic;
using System.IO;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Utils
{
    public class FileHelper
    {
        public static List<Animal> ReadAnimalsFromFile(string filePath)
        {
            List<Animal> animals = new List<Animal>();
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с животными не найден: " + filePath);
            }
            
            // Используем StreamReader для чтения файла
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
                    
                    string[] parts = line.Split('|');
                    
                    if (parts.Length < 5)
                    {
                        Console.WriteLine("Предупреждение: пропущена некорректная строка: " + line);
                        continue;
                    }
                    
                    try
                    {
                        string name = parts[0].Trim();
                        string species = parts[1].Trim();
                        string habitat = parts[2].Trim();
                        string description = parts[3].Trim();
                        int age = int.Parse(parts[4].Trim());
                        
                        Animal animal = new Animal(name, species, habitat, description, age);
                        animals.Add(animal);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при чтении животного: " + ex.Message);
                    }
                }
            }
            
            return animals;
        }

        public static Customer ReadCustomerFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с информацией о покупателе не найден: " + filePath);
            }
            
            string name = "";
            int age = 0;
            string email = "";
            string phone = "";
            
            // Используем StreamReader для чтения файла
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
                    
                    if (line.StartsWith("name=") || line.StartsWith("Name="))
                    {
                        name = line.Substring(5).Trim();
                    }
                    else if (line.StartsWith("age=") || line.StartsWith("Age="))
                    {
                        age = int.Parse(line.Substring(4).Trim());
                    }
                    else if (line.StartsWith("email=") || line.StartsWith("Email="))
                    {
                        email = line.Substring(6).Trim();
                    }
                    else if (line.StartsWith("phone=") || line.StartsWith("Phone="))
                    {
                        phone = line.Substring(6).Trim();
                    }
                }
            }
            
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("В файле покупателя не найдено поле 'name'. Проверьте формат файла.");
            }
            
            if (age == 0)
            {
                throw new ArgumentException("В файле покупателя не найдено корректное поле 'age'. Проверьте формат файла.");
            }
            
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("В файле покупателя не найдено поле 'email'. Проверьте формат файла.");
            }
            
            Console.WriteLine("Данные покупателя успешно загружены из файла");
            
            return new Customer(name, age, email, phone);
        }

        public static List<string> ReadPurchaseRequestFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с запросом на покупку не найден: " + filePath);
            }
            
            List<string> ticketTypes = new List<string>();
            
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

