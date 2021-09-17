using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Replication.RooTool.Library
{
    public class DataLists
    {
        public static class Names
        {
            public const string CRITERIA = "Criteria";
            public const string APPLICATION_TYPE = "Application_Type";
            public const string APPLICATION_LICENCE_APPROVAL = "Application_License_Approval_No";
            public const string PROVINCIAL_ZONE = "Provincial_Zone";
            public const string PROVINCIAL_TYPE = "Provincial_Type";
            public const string SOIL_SERIES = "Soil_Orders";
            public const string SOIL_GREAT_GROUP = "Soil_Great_Group";
            public const string PROVINCIAL_NATURAL_REGION = "Provincial_Natural_Region";
            public const string PROVINCIAL_NATURAL_SUBREGION = "Provincial_Subregion";
            public const string SOIL_ORDERS = "Soil_Orders";
            public const string PROVINCIAL_ECOSITE = "Provincial_Ecosite";
            public const string UTM_ZONE = "UTM_Zone";
            public const string LAND_QUARTER = "Land_Quarter";
            public const string LAND_MERIDIAN = "Land_Meridian";
            public const string LAND_TOWNSHIP = "Land_Township";
            public const string LAND_SECTION = "Land_Section";
            public const string LAND_RANGE = "Land_Range";
            public const string LAND_LEGAL_SUBDIVISION = "Land_Legal_Subdivision";
            public const string FACILITY_TYPE = "Facility_Type";
            public const string FACILITY_ASSESMENT_TYPE = "Facility_Assessment_type";
            public const string FACILITY_REVEGETATION_APPROACH = "Facility_Revegetation_Approach";
            public const string FACILITY_DISTURBANCE_CONSTRUCTION = "Facility_Disturbance_Construction";
            public const string FACILITY_DISTURBANCE_RECLAMATION = "Facility_Disturbance_Reclamation";
            public const string LIST_SUMMARY = "List_Summary";
            public const string LIST_PASS_FAIL = "List_Pass_Fail";
        }

        internal class DataList
        {
            public string Display { get; set; }
            public List<string> Values { get; set; } = new List<string>();
            public string Index { get; set; } = "A";
            public int LowBoundary { get; set; } = 5;
            public int HighBoundary { get; set; } = 5;
        }

        private readonly Dictionary<string, DataList> _lists;
        private IXLWorksheet _sheet;

        public DataLists(IXLWorksheet sheet)
        {
            _lists = GetLists("data.json");
            _sheet = sheet;
            ExporttoWorksheet(_sheet);
        }

        public string GetFirstItemInList(string name)
        {
            if (!_lists.ContainsKey(name))
            {
                return string.Empty;
            }
            return _lists[name].Values[0];
        }

        public string GetDisplayNameForList(string name)
        {
            if (!_lists.ContainsKey(name))
            {
                return name;
            }
            return _lists[name].Display;
        }

        public IXLRange GetRangeForList(string name)
        {
            if (!_lists.ContainsKey(name))
            {
                return null;
            }
            var list = _lists[name];
            return _sheet.Range($"{list.Index}{list.LowBoundary}:{list.Index}{list.HighBoundary}");
        }

        private void ExporttoWorksheet(IXLWorksheet sheet)
        {
            int i = 1;
            foreach (var list in _lists)
            {
                var x = GetExcelColumnName(i++);
                list.Value.Index = x;
                var y = 1;
                sheet.Cell($"{x}{y++}").Value = list.Key;
                y++;
                list.Value.LowBoundary = y;
                sheet.Cell($"{x}{y++}").Value = list.Value.Display;
                sheet.Cell($"{x}{y++}").Value = "-----";
                foreach (var val in list.Value.Values) {
                    sheet.Cell($"{x}{y++}").Value = val;
                }
                list.Value.HighBoundary = y - 1;
            }
        }

        private Dictionary<string, DataList> GetLists(string filename)
        {
            using var reader = new StreamReader(filename);
            string json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Dictionary<string, DataList>>(json);
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }
    }
}
