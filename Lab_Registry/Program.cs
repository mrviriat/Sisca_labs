namespace Lab_Registry
{
    using System;
    using Microsoft.Win32;
    using System.IO;

    class RegistryReaderWriter
    {
        static void Main()
        {
            while (true) {
                Console.WriteLine("1. Чтение реестра");
                Console.WriteLine("2. Запись в реестр");
                Console.Write("Выберите операцию (1 или 2): ");

                int choice = int.Parse(Console.ReadLine());

                if (choice == 1)
                {
                    ReadRegistry();
                }
                else if (choice == 2)
                {
                    WriteRegistry();
                }
                else
                {
                    Console.WriteLine("Неверный выбор операции.");
                }
            }
            
        }

        static void ReadRegistry()
        {
            Console.Write("Введите путь к ключу реестра (например, 'HKEY_CURRENT_USER\\Software'): ");
            string registryPath = Console.ReadLine();

            try
            {
                RegistryKey key = RegistryKey.OpenBaseKey(GetRegistryHive(registryPath), RegistryView.Default);
                string subKey = registryPath.Substring(registryPath.IndexOf("\\") + 1);

                RegistryKey registryKey = key.OpenSubKey(subKey);

                if (registryKey != null)
                {
                    Console.WriteLine("Логическая структура реестра:");

                    // Вывести подключи
                    Console.WriteLine("Подключи:");
                    foreach (string subKeyName in registryKey.GetSubKeyNames())
                    {
                        Console.WriteLine($"- {subKeyName}");
                    }

                    // Вывести параметры
                    Console.WriteLine("Параметры:");
                    foreach (string valueName in registryKey.GetValueNames())
                    {
                        Console.WriteLine($"- {valueName}: {registryKey.GetValue(valueName)}");
                    }

                    // Сохранить информацию в текстовый файл
                    Console.Write("Сохранить информацию в текстовый файл? (y/n) ");
                    string choice = Console.ReadLine();
                    if(choice == "y")
                    {
                        Console.Write("Введите имя файла для сохранения: ");
                        string fileName = Console.ReadLine();
                        SaveRegistryInfoToFile(registryKey, fileName);
                    }
                }
                else
                {
                    Console.WriteLine("Указанный ключ реестра не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении реестра: {ex.Message}");
            }
        }

        static void SaveRegistryInfoToFile(RegistryKey key, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (string subKeyName in key.GetSubKeyNames())
                {
                    writer.WriteLine($"Подключь: {subKeyName}");
                }

                foreach (string valueName in key.GetValueNames())
                {
                    writer.WriteLine($"Параметр: {valueName}, Значение: {key.GetValue(valueName)}");
                }
            }

            Console.WriteLine($"Информация сохранена в файл {fileName}");
        }

        static void WriteRegistry()
        {
            Console.Write("Введите путь к ключу реестра для записи (например, 'HKEY_CURRENT_USER\\Software\\MyApp'): ");
            string registryPath = Console.ReadLine();

            Console.Write("Введите имя параметра: ");
            string paramName = Console.ReadLine();

            Console.Write("Введите тип переменной для хранения (string, int, binary): ");
            string valueType = Console.ReadLine();

            Console.Write("Введите значение параметра: ");
            string paramValue = Console.ReadLine();

            try
            {
                RegistryKey key = RegistryKey.OpenBaseKey(GetRegistryHive(registryPath), RegistryView.Default);
                string subKey = registryPath.Substring(registryPath.IndexOf("\\") + 1);

                RegistryKey registryKey = key.CreateSubKey(subKey);

                if (registryKey != null)
                {
                    if (valueType.ToLower() == "string")
                    {
                        registryKey.SetValue(paramName, paramValue, RegistryValueKind.String);
                    }
                    else if (valueType.ToLower() == "int")
                    {
                        int intValue = int.Parse(paramValue);
                        registryKey.SetValue(paramName, intValue, RegistryValueKind.DWord);
                    }
                    else if (valueType.ToLower() == "binary")
                    {
                        byte[] binaryValue = Convert.FromBase64String(paramValue);
                        registryKey.SetValue(paramName, binaryValue, RegistryValueKind.Binary);
                    }
                    else
                    {
                        Console.WriteLine("Неверный тип переменной.");
                        return;
                    }

                    Console.WriteLine($"Значение {paramName} успешно записано в реестр.");
                }
                else
                {
                    Console.WriteLine("Указанный ключ реестра не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в реестр: {ex.Message}");
            }
        }

        static RegistryHive GetRegistryHive(string registryPath)
        {
            string hive = registryPath.Substring(0, registryPath.IndexOf("\\"));
            switch (hive.ToUpper())
            {
                case "HKEY_CLASSES_ROOT":
                    return RegistryHive.ClassesRoot;
                case "HKEY_CURRENT_USER":
                    return RegistryHive.CurrentUser;
                case "HKEY_LOCAL_MACHINE":
                    return RegistryHive.LocalMachine;
                case "HKEY_USERS":
                    return RegistryHive.Users;
                case "HKEY_CURRENT_CONFIG":
                    return RegistryHive.CurrentConfig;
                default:
                    throw new ArgumentException("Неверный корневой ключ реестра.");
            }
        }
    }

}