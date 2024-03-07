using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ConsoleApp
{
    public class DataReader
    {
        private List<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport)
        {
            ImportedObjects = ImportData(fileToImport);
            AssignNumberOfChildren();
            PrintDatabaseInfo();
            Console.ReadLine();
        }

        private List<ImportedObject> ImportData(string fileToImport)
        {
            ImportedObjects = new List<ImportedObject>();
            var streamReader = new StreamReader(fileToImport);
            
            var importedLines = streamReader.ReadToEnd().Split('\n');

            for (int i = 1; i < importedLines.Length; i++)
            {
                 var importedLine = importedLines[i];
                if (string.IsNullOrWhiteSpace(importedLine))
                {
                    continue;
                }
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject
                {
                    Type = values.ElementAtOrDefault(0)?.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(),
                    Name = values.ElementAtOrDefault(1)?.Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                    Schema = values.ElementAtOrDefault(2)?.Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                    ParentName = values.ElementAtOrDefault(3)?.Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                    ParentType = values.ElementAtOrDefault(4)?.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(),
                    DataType = values.ElementAtOrDefault(5)?.Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                    IsNullable = values.ElementAtOrDefault(6) == "1" ? true : false
                };
                ImportedObjects.Add(importedObject);
            }
            return ImportedObjects;
        }

        private void AssignNumberOfChildren()
        {
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.NumberOfChildren = ImportedObjects.Count(obj =>
                    obj.ParentType == importedObject.Type && obj.ParentName == importedObject.Name);
            }
        }

        private void PrintDatabaseInfo()
        {
            foreach (var database in ImportedObjects.Where(obj => obj.Type == "DATABASE"))
            {
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                foreach (var table in ImportedObjects.Where(obj =>
                    obj.ParentType != null &&
                    obj.ParentName != null &&
                    obj.ParentType.ToUpper() == database.Type &&
                    obj.ParentName == database.Name))
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    foreach (var column in ImportedObjects.Where(obj =>
                        obj.ParentType != null &&
                        obj.ParentName != null &&
                        obj.ParentType.ToUpper() == table.Type &&
                        obj.ParentName == table.Name))
                    {
                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == true ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }
        }

    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public double NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
