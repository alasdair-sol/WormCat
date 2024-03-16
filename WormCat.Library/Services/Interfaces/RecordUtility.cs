﻿using System.Text.RegularExpressions;
using WormCat.Library.Models.Dbo;

namespace WormCat.Library.Services.Interfaces
{
    public class RecordUtility : IRecordUtility
    {
        private string ImageFallbackBase64 = "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";

        /// <summary>
        /// Takes in a potential isbn and verifies if it is valid or not. Returns the validated and formatted ISBN (removed dashes etc)
        /// </summary>
        /// <param name="input">The string to verify</param>
        /// <param name="isbn">Formatted, valid ISBN, or empty string if method returns false</param>
        /// <returns></returns>
        public bool IsISBN(string input, out string isbn)
        {
            isbn = string.Empty;

            if (string.IsNullOrWhiteSpace(input)) return false;

            Regex regex = new Regex("^97[89][0-9]{10}$");
            input = input.Trim();

            bool isMatch = regex.IsMatch(input);

            if (isMatch) isbn = input;
            return isMatch;
        }

        public Container GetRecordFirstContainer(Record record)
        {
            if (!RecordHasContainer(record)) return new Container();
            return record?.Books?.Where(x => x.OnLoan == false).First()?.Container ?? new Container();
        }

        public bool RecordHasContainer(Record record)
        {
            if (record == null) return false;
            if (record.Books == null) return false;
            if (record.Books.Count <= 0) return false;
            return true;
        }

        public bool RecordHasAvailableCopy(Record record)
        {
            if (record == null) return false;
            if (record.Books == null) return false;
            if (record.Books.Count <= 0) return false;
            return record.Books.Any(x => x.OnLoan == false);
        }

        public IEnumerable<Container?> GetRecordUniqueContainers(Record record)
        {
            if (record == null) return default!;
            if (record.Books == null) return default!;
            if (record.Books.Count <= 0) return default!;
            return record.Books.Where(x => x.OnLoan == false).Select(x => x.Container).DistinctBy(x => x.Id);
        }

        public string GetRecordBase64Image(Record record)
        {
            if (string.IsNullOrWhiteSpace(record.Image))
                return ImageFallbackBase64;

            return record.Image;
        }
    }
}