using Data.Interfaces;
using Domain;
using System.Text.RegularExpressions;

namespace Data.InMemory
{
    public class ReaderRepository : IReaderRepository
    {

        private static readonly string PHONE_NUMBER_FORMAT = @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})";

        List<Reader> ReaderList = new List<Reader>();
        private int CurrentId = 0;
        public bool Add(Reader reader)
        {
            if (reader == null) return false;
            reader.Id = ++CurrentId;
            ReaderList.Add(reader);
            return true;
        }

        public bool ContainsReader(Reader reader)
        {
            return ReaderList.Contains(reader);
        }

        public bool Delete(Reader reader)
        {
            if (reader == null) return false;
            ReaderList.Remove(reader);
            return true;
        }

        public List<Reader> GetAll(ReaderFilter filter)
        {
            var result = ReaderList.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.FullName)) result = result
                    .Where(r => r.FullName.Contains(filter.FullName));

            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber)) result = result
                    .Where(r => Regex.Replace(r.PhoneNumber, @"\D", "").Contains(filter.PhoneNumber));

            if(filter.TicketNumber.HasValue) result = result
                    .Where(r => r.Id == filter.TicketNumber);

            

            return result.ToList();
        }

        public Reader GetById(int id)
        {
            return ReaderList.FirstOrDefault(r => r.Id == id);
        }

        public bool Update(Reader reader)
        {
            int index = ReaderList.IndexOf(GetById(reader.Id));

            if (index != -1)
            {
                ReaderList[index] = reader;
                return true;
            }
            return false;
        }

        public string FormatNumber(string number)
        {
            string result = Regex.Replace(number, @"[^\d]", "");
            return Regex.Replace(result, PHONE_NUMBER_FORMAT, "$1 ($2) $3-$4-$5");
        }
    }
}
