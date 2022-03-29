namespace Paulsams.MicsUtil
{
    public static class SerializedPropertyUtilities
    {
        public static int GetIndexFromArrayProperty(string dataArray)
        {
            int startIndex = dataArray.IndexOf('[') + 1;
            int endIndex = dataArray.Length - 1;

            string indexInArrayFromString = string.Empty;
            for (int j = startIndex; j < endIndex; ++j)
            {
                indexInArrayFromString += dataArray[j];
            }

            return int.Parse(indexInArrayFromString);
        }
    }
}