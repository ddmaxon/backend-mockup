using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace testapi
{
    public class Helper
    {


        public List<string[]> SplitList(dynamic res)
        {
            List<string[]> splitLine = new List<string[]>();

            if (res is List<string> stringList)
            {
                List<string> tempList = stringList;
                foreach (string line in tempList)
                {
                    string[] tempArray = line.Split(';');
                    splitLine.Add(tempArray);
                }
            }
            else if (res is string singleString)
            {
                string[] tempArray = singleString.Split(';');
                splitLine.Add(tempArray);
            }
            return splitLine;
        }

        public List<object> GetExecutionTime(dynamic res)
        {
            try
            {
                List<object> execTime = new List<object>();

                if (res is List<string> stringList)
                {
                    List<string> tempList = stringList;
                    foreach (string line in tempList)
                    {
                        // 11.07.2023 16:29:07.439;Datenbank.vi;Q 'DB';End: "DB_ErrorLog" [0.130 s] 
                        List<string[]> strArrList = this.SplitList(res);

                        string[] strArr = strArrList.ToArray()[0];
                        Array.Reverse(strArr);

                        // ["End: "DB_ErrorLog" [0.130 s]", "Q 'DB'", "Datenbank.vi", "11.07.2023 16:29:07.439"] 
                        string[] exec_timeArr = strArr[0].Split(" ");
                        Array.Reverse(exec_timeArr);
                        string exec_time = exec_timeArr[2];

                        // [0.130 s]
                        exec_time = exec_time.Replace("[", "");

                        // 0.130
                        if (double.TryParse(exec_time, out _))
                        {
                            execTime.Add(new { res, time = exec_time });
                        }
                    }
                }
                else if (res is string singleString)
                {
                    // 11.07.2023 16:29:07.439;Datenbank.vi;Q 'DB';End: "DB_ErrorLog" [0.130 s] 
                    List<string[]> strArrList = this.SplitList(res);

                    string[] strArr = strArrList.ToArray()[0];
                    Array.Reverse(strArr);

                    // ["End: "DB_ErrorLog" [0.130 s]", "Q 'DB'", "Datenbank.vi", "11.07.2023 16:29:07.439"] 
                    string[] exec_timeArr = strArr[0].Split(" ");
                    Array.Reverse(exec_timeArr);
                    string exec_time = exec_timeArr[2];

                    // [0.130 s]
                    exec_time = exec_time.Replace("[", "");

                    // 0.130
                    if (double.TryParse(exec_time, out _))
                    {
                        execTime.Add(new { res, time = exec_time });
                    }
                }

                if(execTime.Count <= 0)
                {
                    throw new Exception("No data with time found!");
                }

                return execTime;
            }
            catch (Exception err)
            {
                throw new FormatException(err.Message);
            }
        }
    }
}