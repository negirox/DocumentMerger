using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace MergeHelper
{
    public interface IHelper
    {
        void MergeDocument(string newFilePath, IList<string> documentFiles);
    }
}
