﻿#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json4.Converters;
using NUnit.Framework;
using Newtonsoft.Json4.Serialization;
using Newtonsoft.Json4.Tests.TestObjects;
using System.Data;

namespace Newtonsoft.Json4.Tests.Converters
{
  public class DataSetConverterTests : TestFixtureBase
  {
    [Test]
    public void SerializeAndDeserialize()
    {
      DataSet dataSet = new DataSet("dataSet");
      dataSet.Namespace = "NetFrameWork";
      DataTable table = new DataTable();
      DataColumn idColumn = new DataColumn("id", typeof(int));
      idColumn.AutoIncrement = true;

      DataColumn itemColumn = new DataColumn("item");
      table.Columns.Add(idColumn);
      table.Columns.Add(itemColumn);
      dataSet.Tables.Add(table);

      for (int i = 0; i < 2; i++)
      {
        DataRow newRow = table.NewRow();
        newRow["item"] = "item " + i;
        table.Rows.Add(newRow);
      }

      dataSet.AcceptChanges();

      string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
      
      Assert.AreEqual(@"{
  ""Table1"": [
    {
      ""id"": 0,
      ""item"": ""item 0""
    },
    {
      ""id"": 1,
      ""item"": ""item 1""
    }
  ]
}", json);

      DataSet deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(json);
      Assert.IsNotNull(deserializedDataSet);

      Assert.AreEqual(1, deserializedDataSet.Tables.Count);

      DataTable dt = deserializedDataSet.Tables[0];

      Assert.AreEqual("Table1", dt.TableName);
      Assert.AreEqual(2, dt.Columns.Count);
      Assert.AreEqual("id", dt.Columns[0].ColumnName);
      Assert.AreEqual(typeof(long), dt.Columns[0].DataType);
      Assert.AreEqual("item", dt.Columns[1].ColumnName);
      Assert.AreEqual(typeof(string), dt.Columns[1].DataType);

      Assert.AreEqual(2, dt.Rows.Count);
    }

    [Test]
    public void SerializeMultiTableDataSet()
    {
      DataSet ds = new DataSet();
      ds.Tables.Add(CreateDataTable("FirstTable", 2));
      ds.Tables.Add(CreateDataTable("SecondTable", 1));

      string json = JsonConvert.SerializeObject(ds, Formatting.Indented, new IsoDateTimeConverter());
      // {
      //   "FirstTable": [
      //     {
      //       "StringCol": "Item Name",
      //       "Int32Col": 1,
      //       "BooleanCol": true,
      //       "TimeSpanCol": "10.22:10:15.1000000",
      //       "DateTimeCol": "2000-12-29T00:00:00Z",
      //       "DecimalCol": 64.0021
      //     },
      //     {
      //       "StringCol": "Item Name",
      //       "Int32Col": 2,
      //       "BooleanCol": true,
      //       "TimeSpanCol": "10.22:10:15.1000000",
      //       "DateTimeCol": "2000-12-29T00:00:00Z",
      //       "DecimalCol": 64.0021
      //     }
      //   ],
      //   "SecondTable": [
      //     {
      //       "StringCol": "Item Name",
      //       "Int32Col": 1,
      //       "BooleanCol": true,
      //       "TimeSpanCol": "10.22:10:15.1000000",
      //       "DateTimeCol": "2000-12-29T00:00:00Z",
      //       "DecimalCol": 64.0021
      //     }
      //   ]
      // }

      DataSet deserializedDs = JsonConvert.DeserializeObject<DataSet>(json, new IsoDateTimeConverter());

      Assert.AreEqual(@"{
  ""FirstTable"": [
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 1,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    },
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 2,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    }
  ],
  ""SecondTable"": [
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 1,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    }
  ]
}", json);

      Assert.IsNotNull(deserializedDs);

    }

    [Test]
    public void DeserializeMultiTableDataSet()
    {
      string json = @"{
  ""FirstTable"": [
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 2147483647,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    }
  ],
  ""SecondTable"": [
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 2147483647,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    }
  ]
}";

      DataSet ds = JsonConvert.DeserializeObject<DataSet>(json, new IsoDateTimeConverter());
      Assert.IsNotNull(ds);

      Assert.AreEqual(2, ds.Tables.Count);
      Assert.AreEqual("FirstTable", ds.Tables[0].TableName);
      Assert.AreEqual("SecondTable", ds.Tables[1].TableName);

      DataTable dt = ds.Tables[0];
      Assert.AreEqual("StringCol", dt.Columns[0].ColumnName);
      Assert.AreEqual(typeof(string), dt.Columns[0].DataType);
      Assert.AreEqual("Int32Col", dt.Columns[1].ColumnName);
      Assert.AreEqual(typeof(long), dt.Columns[1].DataType);
      Assert.AreEqual("BooleanCol", dt.Columns[2].ColumnName);
      Assert.AreEqual(typeof(bool), dt.Columns[2].DataType);
      Assert.AreEqual("TimeSpanCol", dt.Columns[3].ColumnName);
      Assert.AreEqual(typeof(string), dt.Columns[3].DataType);
      Assert.AreEqual("DateTimeCol", dt.Columns[4].ColumnName);
      Assert.AreEqual(typeof(string), dt.Columns[4].DataType);
      Assert.AreEqual("DecimalCol", dt.Columns[5].ColumnName);
      Assert.AreEqual(typeof(double), dt.Columns[5].DataType);

      Assert.AreEqual(1, ds.Tables[0].Rows.Count);
      Assert.AreEqual(1, ds.Tables[1].Rows.Count);
    }

    private DataTable CreateDataTable(string dataTableName, int rows)
    {
      // create a new DataTable.
      DataTable myTable = new DataTable(dataTableName);

      // create DataColumn objects of data types.
      DataColumn colString = new DataColumn("StringCol");
      colString.DataType = typeof(string);
      myTable.Columns.Add(colString);

      DataColumn colInt32 = new DataColumn("Int32Col");
      colInt32.DataType = typeof(int);
      myTable.Columns.Add(colInt32);

      DataColumn colBoolean = new DataColumn("BooleanCol");
      colBoolean.DataType = typeof(bool);
      myTable.Columns.Add(colBoolean);

      DataColumn colTimeSpan = new DataColumn("TimeSpanCol");
      colTimeSpan.DataType = typeof(TimeSpan);
      myTable.Columns.Add(colTimeSpan);

      DataColumn colDateTime = new DataColumn("DateTimeCol");
      colDateTime.DataType = typeof(DateTime);
      colDateTime.DateTimeMode = DataSetDateTime.Utc;
      myTable.Columns.Add(colDateTime);

      DataColumn colDecimal = new DataColumn("DecimalCol");
      colDecimal.DataType = typeof(decimal);
      myTable.Columns.Add(colDecimal);

      for (int i = 1; i <= rows; i++)
      {
        DataRow myNewRow = myTable.NewRow();

        myNewRow["StringCol"] = "Item Name";
        myNewRow["Int32Col"] = i;
        myNewRow["BooleanCol"] = true;
        myNewRow["TimeSpanCol"] = new TimeSpan(10, 22, 10, 15, 100);
        myNewRow["DateTimeCol"] = new DateTime(2000, 12, 29, 0, 0, 0, DateTimeKind.Utc);
        myNewRow["DecimalCol"] = 64.0021;
        myTable.Rows.Add(myNewRow);
      }

      return myTable;
    }

    public class DataSetAndTableTestClass
    {
      public string Before { get; set; }
      public DataSet Set { get; set; }
      public string Middle { get; set; }
      public DataTable Table { get; set; }
      public string After { get; set; }
    }

    [Test]
    public void SerializeWithCamelCaseResolver()
    {
      DataSet ds = new DataSet();
      ds.Tables.Add(CreateDataTable("FirstTable", 2));
      ds.Tables.Add(CreateDataTable("SecondTable", 1));

      string json = JsonConvert.SerializeObject(ds, Formatting.Indented, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

      Assert.AreEqual(@"{
  ""firstTable"": [
    {
      ""stringCol"": ""Item Name"",
      ""int32Col"": 1,
      ""booleanCol"": true,
      ""timeSpanCol"": ""10.22:10:15.1000000"",
      ""dateTimeCol"": ""\/Date(978048000000)\/"",
      ""decimalCol"": 64.0021
    },
    {
      ""stringCol"": ""Item Name"",
      ""int32Col"": 2,
      ""booleanCol"": true,
      ""timeSpanCol"": ""10.22:10:15.1000000"",
      ""dateTimeCol"": ""\/Date(978048000000)\/"",
      ""decimalCol"": 64.0021
    }
  ],
  ""secondTable"": [
    {
      ""stringCol"": ""Item Name"",
      ""int32Col"": 1,
      ""booleanCol"": true,
      ""timeSpanCol"": ""10.22:10:15.1000000"",
      ""dateTimeCol"": ""\/Date(978048000000)\/"",
      ""decimalCol"": 64.0021
    }
  ]
}", json);
    }

    [Test]
    public void SerializeDataSetProperty()
    {
      DataSet ds = new DataSet();
      ds.Tables.Add(CreateDataTable("FirstTable", 2));
      ds.Tables.Add(CreateDataTable("SecondTable", 1));

      DataSetAndTableTestClass c = new DataSetAndTableTestClass
        {
          Before = "Before",
          Set = ds,
          Middle = "Middle",
          Table = CreateDataTable("LoneTable", 2),
          After = "After"
        };

      string json = JsonConvert.SerializeObject(c, Formatting.Indented, new IsoDateTimeConverter());

      Assert.AreEqual(@"{
  ""Before"": ""Before"",
  ""Set"": {
    ""FirstTable"": [
      {
        ""StringCol"": ""Item Name"",
        ""Int32Col"": 1,
        ""BooleanCol"": true,
        ""TimeSpanCol"": ""10.22:10:15.1000000"",
        ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
        ""DecimalCol"": 64.0021
      },
      {
        ""StringCol"": ""Item Name"",
        ""Int32Col"": 2,
        ""BooleanCol"": true,
        ""TimeSpanCol"": ""10.22:10:15.1000000"",
        ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
        ""DecimalCol"": 64.0021
      }
    ],
    ""SecondTable"": [
      {
        ""StringCol"": ""Item Name"",
        ""Int32Col"": 1,
        ""BooleanCol"": true,
        ""TimeSpanCol"": ""10.22:10:15.1000000"",
        ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
        ""DecimalCol"": 64.0021
      }
    ]
  },
  ""Middle"": ""Middle"",
  ""Table"": [
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 1,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    },
    {
      ""StringCol"": ""Item Name"",
      ""Int32Col"": 2,
      ""BooleanCol"": true,
      ""TimeSpanCol"": ""10.22:10:15.1000000"",
      ""DateTimeCol"": ""2000-12-29T00:00:00Z"",
      ""DecimalCol"": 64.0021
    }
  ],
  ""After"": ""After""
}", json);

      DataSetAndTableTestClass c2 = JsonConvert.DeserializeObject<DataSetAndTableTestClass>(json, new IsoDateTimeConverter());

      Assert.AreEqual(c.Before, c2.Before);
      Assert.AreEqual(c.Set.Tables.Count, c2.Set.Tables.Count);
      Assert.AreEqual(c.Middle, c2.Middle);
      Assert.AreEqual(c.Table.Rows.Count, c2.Table.Rows.Count);
      Assert.AreEqual(c.After, c2.After);
    }
  }
}
#endif