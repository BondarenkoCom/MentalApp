﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MentalTest.Interfaces
{
        public interface IDatabaseAssetService
        {
            void CopyDatabaseIfNotExists(string dbName, string destinationPath);
        }
}
