using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;


namespace testapi
{
    public class CsvLoader
    {
        private List<string> _csv;

        public CsvLoader(string csvPath = "./")
        {
            this._csv = new List<string>();
            this.loadCsv(csvPath);
        }

        public void loadCsv(string path)
        {
            using (var reader = new StreamReader($"{path}"))
            {
                while (!reader.EndOfStream)
                {
                    var val = reader.ReadLine();

                    if (val == null)
                        continue;

                    this._csv.Add(val);
                }
            }
        }

        public List<string> GetCsvLines()
        {
            return _csv;
        }

        public string GetCsvLine(int lineNr)
        {
            try
            {
                return this._csv[(lineNr - 1)];
            }
            catch (Exception err)
            {
                throw new Exception($"Out of range! Max range is {this._csv.Count()}");
            }
        }

        public object AddCsvLine(string line)
        {
            this._csv.Add(line);
            return new
            {
                state = true,
                message = "line successfully added!"
            };
        }

        public List<string> searchSubstringInCsv(string subStr = " ")
        {
            try
            {
                List<string> result = new List<string>();

                foreach (string str in this._csv)
                {

                    // search subStr in every line
                    if (str.ToLower().Contains(subStr.ToLower()))
                    {
                        result.Add(str);
                    }
                }

                return result;
            }catch(Exception err)
            {
                throw new Exception($"Something went wrong! We're working on it.");
            }
        }

        public object getIndexOfSearch(string subStr)
        {
            List<string> result = new List<string>();
            var indexOfres = 0;

            foreach (string str in this._csv)
            {
                // search subStr in every line and note the index
                if (str.Contains(subStr))
                {
                    result.Add(str);
                    break;
                }
                indexOfres++;
            }


            return new
            {
                indexOf = indexOfres,
                subStr,
                result
            };
        }

        public object getDataBetween(string firstSub, string secondSub)
        {
            List<string> _res = new List<string>();

            bool isBetween = false;

            foreach (string str in this._csv)
            {

                if (str.Contains(firstSub) && !isBetween)
                {
                    _res.Add(str);
                    isBetween = true;
                }

                if (isBetween)
                {
                    _res.Add(str);
                }

                if (str.Contains(secondSub) && isBetween)
                {
                    _res.Add(str);
                    isBetween = false;
                    break;
                }
            }

            return new
            {
                startSub = firstSub,
                endSub = secondSub,
                isBetween,
                result = _res,
                resultCount = _res.Count()
            };
        }
    }
}
