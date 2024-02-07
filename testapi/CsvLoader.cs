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
                throw new IndexOutOfRangeException($"Out of range! Max range is {this._csv.Count()}");
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

        public List<object> searchSubstringInCsv(string subStr = " ")
        {
            try
            {
                List<object> result = new List<object>();

                foreach (string str in this._csv)
                {

                    // search subStr in every line
                    if (str.ToLower().Contains(subStr.ToLower()))
                    {
                        dynamic item = this.getIndexOfSearch(str);
                        result.Add(new { indexOf = item.indexOf, data = item.subStr });
                    }
                }

                if (result.Count == 0)
                {
                    throw new Exception("422");
                }

                return result;
            }
            catch (Exception err)
            {
                if (err.Message == "422")
                {
                    throw new($"No sources found for {subStr}");
                }

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
                if (str.ToLower().Contains(subStr.ToLower()))
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

                if (str.ToLower().Contains(firstSub.ToLower()) && !isBetween)
                {
                    _res.Add(str);
                    isBetween = true;
                }

                if (isBetween)
                {
                    _res.Add(str);
                }

                if (str.ToLower().Contains(secondSub.ToLower()) && isBetween)
                {
                    _res.Add(str);
                    break;
                }
            }

            return new
            {
                start = new { firstSub, index = getIndexOfSearch(firstSub) },
                endSub = new { secondSub, index = getIndexOfSearch(secondSub) },
                isBetween,
                result = _res,
                resultCount = _res.Count()
            };
        }


        public object getAllTestsData()
        {
            string _testdata_startStr = "Start: \"DB_SaveResult\"";
            string _testdata_endStr = "End: \"DB_SaveResult\"";

            List<List<object>> _res = new List<List<object>>();
            List<object> _temp = new List<object>();

            bool isBetween = false;


            foreach (string str in this._csv)
            {

                if (str.ToLower().Contains(_testdata_startStr.ToLower()) && !isBetween)
                {
                    isBetween = true;
                    _temp = new List<object>();

                    dynamic item = this.getIndexOfSearch(str);

                    _temp.Add(new { indexOf = item.indexOf, data = item.subStr });
                }

                if (isBetween)
                {
                    dynamic item = this.getIndexOfSearch(str);
                    _temp.Add(new { indexOf = item.indexOf, data = item.subStr });
                }

                if (str.ToLower().Contains(_testdata_endStr.ToLower()) && isBetween)
                {
                    isBetween = false; 
                    
                    dynamic item = this.getIndexOfSearch(str);
                    _temp.Add(new { indexOf = item.indexOf, data = item.subStr });

                    _res.Add(_temp);
                }
            }


            return new
            {
                data = _res,
                testCount = _res.Count
            };
        }
    }
}
