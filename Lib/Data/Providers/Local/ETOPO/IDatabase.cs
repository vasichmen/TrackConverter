﻿using System;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO
{
    interface IDatabase
    {
        double this[Coordinate coordinate] { get; }
        double this[int i, int j] { get; }
        double CellSize { get; }
        int Columns { get; }
        string DataFile { get; }
        string Folder { get; }
        string HeaderFile { get; }
        int Rows { get; }
        ETOPODBType Type { get; }
        void ExportToSQL(string FileName, Action<string> callback=null);
    }
}