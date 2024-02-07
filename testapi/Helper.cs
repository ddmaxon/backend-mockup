using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace testapi
{
    public class Helper
    {


        public List<string> SplitList(dynamic res)
        {
            List<string> splitLine = new List<string>();

            if (res is List<string> stringList)
            {
                List<string> tempList = stringList;
                foreach (string line in tempList)
                {
                    string[] tempArray = line.Split(';');
                    splitLine.AddRange(tempArray);
                }
            }
            else if (res is string singleString)
            {
                string[] tempArray = singleString.Split(';');
                splitLine.AddRange(tempArray);
            }
            return splitLine;
        }
    }
}