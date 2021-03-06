﻿using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using PrintClassInstanceLib;
using PrintClassInstanceLib.Extensions;
using PrintClassInstanceLib.Messages;

namespace DemoCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var data = TestDataGenerator.GenerateTestData1();

            //Tracking
            var dictTrack = new DictionaryContainer();
            var jsonTrack = new JsonContainer();
            for (var i= 0; i <= 10; i++)
            {
                data.ParentStringTest = i.ToString();
                data.Snapshot(dictTrack);
                data.Snapshot(jsonTrack);
            }
            var snapshots1 = dictTrack.GetSnapshots();
            var snapshots2 = jsonTrack.GetSnapshots();

            //Creater instance with mappings
            var testObject = InstanceUtils.CreateInstance<PrintMe>();
            var mappings = new Mappings<PrintMe>()
                .Map(s => s.DateTimeTest, DateTime.Now)
                .Map(s => s.TestValueTuple, (1, 2, 3, "4"))
                .Map(s => s.PrintMeEnum, PrintMeEnum.PrintMeEnum2);
            var testObject1= InstanceUtils.CreateInstance<PrintMe>(mappings);

            //Dump the object graph to a file
            data.SaveToFile(@"C:\tmp\test.txt");

            //Flatten and combine
            var data2 = TestDataGenerator.GenerateTestData2();
            var x = data.CombineAndFlatten(data2).Result;
            var y = data.CombineAndFlattenedJson(data2).Result;

            //Save to S3
            var contentData = new Dictionary<string, string>
            {
                { "AccessKeyId","your access key"},
                {"SecretAccessKey","your secret access key"}
            };
            var uploadMessage = new S3UploadMessage
            {
                S3Client = new AmazonS3Client(contentData["AccessKeyId"], contentData["SecretAccessKey"],
                        Amazon.RegionEndpoint.USEast1),
                BucketName = "your bucket name",
                Key = "objectgraph.txt"
            };
            var result = data.SaveToS3(uploadMessage).Result;

            //Get member value
            var memberNames = data.MemberNames();
            foreach (var memberName in memberNames)
            {
                var memberVal = data.MemberValue(memberName);
                Console.WriteLine($"Value:{memberVal}-Type:{memberVal.GetType()}");
            }

            //Get null members
            var nullMembers = data.NullMembers();
            foreach (var nullMember in nullMembers)
            {
                Console.WriteLine($"{nullMember}");
            }

            var obj1 = TestDataGenerator.GenerateTestData1();
            var obj2 = TestDataGenerator.GenerateTestData2();

            //Compare two objects 
            var diff1 = obj1.CompareObjects(obj2, "obj1", "obj2");
            Console.WriteLine(diff1.NoMatchList.Any() ? "The objects differ" : "The objects are same");
            diff1.SaveToFile(@"c:\tmp\compare1.txt");
            
            //Compare two objects 
            //Todo: Figure out the names instead of sending them as parameters
            var diff2 = obj1.CompareObjects(obj1, "obj1", "obj1");
            Console.WriteLine(diff2.NoMatchList.Any() ? "The objects differ" : "The objects are same");
            diff2.SaveToFile(@"c:\tmp\compare2.txt");
            
            //Compare two objects
            var simpleObj1 = new Object1 { X = 1, Y = "A", Z = "Z" };
            var simpleObj2 = new Object2 { X = "1", Y = "B", Z = "Z" };
            var diff3 = simpleObj1.CompareObjects(simpleObj2, "simpleObj1", "simpleObj2");
            Console.WriteLine(diff3.NoMatchList.Any() ? "The objects differ" : "The objects are same");
            diff3.SaveToFile(@"c:\tmp\compare3.txt");

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }

    public class Object1
    {
        public int X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
    }

    public class Object2
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
    }
}
