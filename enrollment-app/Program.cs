using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;

public class readCSV
{
    static void Main(string[] args)
    {
        Dictionary<string, List<string>> enrollUser = new Dictionary<string, List<string>>();
        List<string> insCompany = new List<string>();
        var enroll = new readCSV();
        //Using streamreader to read the csv file.
        using (var reader = new StreamReader(Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "enrollment.csv")))
        {
            //Creating dictionary with userid as key and rest of the data as values.
            String key = new String("");
            List<string> data = new List<string>();

            try
            {
                while (!reader.EndOfStream)
                {
                    //read data from csv 
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var values = line.Split(',').ToList();
                    key = values[0];
                    data = values.Skip(1).ToList();

                    //Check if the Userid exists already.
                    if (enrollUser.ContainsKey(key))
                    {
                        enroll.duplicateIdReplaceorNot(enrollUser, key, data);
                        if (!insCompany.Contains(data[2]))
                        {
                            insCompany.Add(data[2]);
                        }
                    }
                    //Add the csv data as a key value pair in dictionary
                    else
                    {
                        enrollUser.Add(key, data);
                        insCompany.Add(data[2]);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //sort the dictionary based on FirstName LastName  
        enrollUser = enroll.sortAsc(enrollUser);

        //Create seperate file for each Insurance Company
        foreach (var comp in insCompany)
        {

            enroll.createInsuranceCompanyFile(enrollUser.Where(x => x.Value.Contains(comp)).ToDictionary(x => x.Key, y => y.Value), comp);

        }

        Console.WriteLine("Finished creating insurance files");
    }

    /// <summary>
    /// Creates an output csv file with the new csv file.
    /// </summary>
    /// <param name="dict">Contains the fields to be printed</param>
    public void createInsuranceCompanyFile(Dictionary<string, List<string>> dict, string insuranceCompany)
    {
    
        try
        {
            if (dict != null)
            {
                using (var writer = new StreamWriter(Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, insuranceCompany + ".csv")))
                {

                    foreach (var item in dict)
                    {
                        writer.WriteLine("{0},{1}", item.Key, String.Join(",", item.Value.ToList()));

                    }
                    //values= new StringBuilder();
                }
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }
    // <summary>
    /// Sorts the dict using LINQ .
    /// </summary>
    /// <param name="dict">Contains the fields to be printed</param>
    public Dictionary<string, List<string>> sortAsc(Dictionary<string, List<string>> dict)
    {
        Dictionary<string, List<string>> sorted = new Dictionary<string, List<string>>();
        try
        {
            sorted = dict.OrderBy(x => x.Value[0]).ToDictionary(x => x.Key, x => x.Value);
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }

        return sorted;
    }
    // <summary>
    /// Replace duplciate id with highest version.
    /// </summary>
    /// <param name="dict">Contains the fields to be printed</param>
    public void duplicateIdReplaceorNot(Dictionary<string, List<string>> dict, string key, List<string> data)
    {
        List<string> tempData = new List<string>();
        try
        {
            if (data != null)
            {
                int version = Convert.ToInt32(data[1]);

                if (dict.TryGetValue(key, out tempData))
                {

                    if (Convert.ToInt32(tempData[1]) > version)
                    {
                        dict.Remove(key);
                        dict.Add(key, data);
                    }
                }
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }
}