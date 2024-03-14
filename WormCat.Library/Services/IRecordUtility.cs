﻿using WormCat.Library.Models.Dbo;

namespace WormCat.Library.Services
{
    public interface IRecordUtility
    {
        string GetRecordBase64Image(Record record);
        Container GetRecordFirstContainer(Record record);
        IEnumerable<Container?> GetRecordUniqueContainers(Record record);
        bool IsISBN(string input, out string isbn);
        bool RecordHasAvailableCopy(Record record);
        bool RecordHasContainer(Record record);
    }
}