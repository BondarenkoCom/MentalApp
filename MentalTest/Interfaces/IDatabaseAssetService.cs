namespace MentalTest.Interfaces
{
        public interface IDatabaseAssetService
        {
            void CopyDatabaseIfNotExists(string dbName, string destinationPath);
        }
}
