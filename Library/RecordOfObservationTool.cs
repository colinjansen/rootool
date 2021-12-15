using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using Replication.RooTool.Library.Models;
using System;
using System.IO;
using CoordinateSharp;

namespace Replication.RooTool.Library
{
    public class RecordOfObservationTool
    {
        private XLColor BLANK_FILL = XLColor.Black;
        private XLColor PEACH_FILL = XLColor.FromArgb(225, 213, 180);
        private XLColor BLUE_FILL = XLColor.FromArgb(204, 255, 255);
        private XLColor GREEN_FILL = XLColor.FromArgb(204, 255, 204);
        private XLColor GREY_FILL = XLColor.FromArgb(217, 217, 217);

        private int _currentLine = 1;
        private IXLWorkbook _book;
        private IXLWorksheet _sheet;
        private DataLists _lists;

        private InputInformation _input;
        private Mappings _map;

        public RecordOfObservationTool(InputInformation input, Mappings map)
        {
            _input = input;
            _map = map;
        }

        public void Process()
        {
            _book = new XLWorkbook();

            _lists = new DataLists(_book.AddWorksheet("Data Lists", 99));

            _sheet = AddWorksheet("TP_ToC");
            _sheet.ColumnWidth = 4; 

            _currentLine = 1;

            CreateToC();

            _sheet = AddWorksheet("ASE_Aerial");
            _sheet.ColumnWidth = 3.7;
            _currentLine = 1;

            CreateAseAerialHeaders(_input.Data[1], _map);
            CreateBlackBar();
            // skip the first line - it will be headers
            for (var i = 1; i < _input.Data.Length; i++)
            {
                CreateAseAerialSection(i, _input.Data[i], _map);
                CreateBlackBar();
            }
        }

        private IXLRange CreateDropdown(IXLRange range, string list, string value = null)
        {
            range = range.Merge();
            range.SetDataValidation().List(_lists.GetRangeForList(list));
            range.SetDataValidation().InCellDropdown = true;
            range.Value = string.IsNullOrEmpty(value) ? _lists.GetDisplayNameForList(list) : value;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            range.Style.Fill.BackgroundColor = BLUE_FILL;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            return range;
        }

        private IXLRange CreateLabelCell(IXLRange range, string label)
        {
            range = range.Merge();
            range.Value = label;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            range.Style.Alignment.WrapText = true;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            return range;
        }

        private IXLRange CreateIntegerCell(IXLRange range, int number)
        {
            range = range.Merge();
            range.Value = $"{number}";
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            range.Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Integer);
            range.Style.Fill.BackgroundColor = GREEN_FILL;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            return range;
        }

        private IXLRange CreateDecimalCell(IXLRange range, double number)
        {
            range = range.Merge();
            range.Value = $"{number}";
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            range.Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Precision2WithSeparator);
            range.Style.Fill.BackgroundColor = GREEN_FILL;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            return range;
        }

        private IXLWorksheet AddWorksheet(string name)
        {
            return _book.AddWorksheet(name);
        }

        private void CreateBlackBar()
        {
            _sheet.Row(_currentLine).Height = 5;
            var range = _sheet.Range($"B{_currentLine}:AM{_currentLine}");
            range.Merge().Style.Fill.BackgroundColor = BLANK_FILL;
            _currentLine += 1;
        }

        private void CreateToC()
        {
            IXLRange r;

            for (var i = 0; i < 90; i++)
            {
                _sheet.Row(_currentLine + i).Height = 20;
            }

            r = _sheet.Range($"A{_currentLine + 0}:AE{_currentLine + 52}");
            r.Style.Font.FontName = "Times New Roman";
            r.Style.Font.FontSize = 9;
            r.Style.Fill.BackgroundColor = XLColor.White;

            r = CreateLabelCell(_sheet.Range($"B{_currentLine + 6}:AE{_currentLine + 11}"), "Forested");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Fill.BackgroundColor = GREY_FILL;
            r.Style.Font.FontSize = 40;

            r = CreateLabelCell(_sheet.Range($"B{_currentLine + 13}:AE{_currentLine + 18}"), "Detailed Site Assessment");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Font.FontSize = 40;

            r = _sheet.Range($"B{_currentLine + 22}:F{_currentLine + 25}").Merge();
            r.Value = "Criteria";
            r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            r.Style.Font.FontSize = 18;
            r = CreateDropdown(_sheet.Range($"G{_currentLine + 22}:O{_currentLine + 25}"), DataLists.Names.CRITERIA);
            r.Style.Font.FontSize = 18;
            r = _sheet.Range($"Q{_currentLine + 22}:V{_currentLine + 25}").Merge();
            r.Value = "Application Type";
            r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            r.Style.Font.FontSize = 18;
            r = CreateDropdown(_sheet.Range($"W{_currentLine + 22}:AE{_currentLine + 25}"), DataLists.Names.APPLICATION_TYPE);
            r.Style.Font.FontSize = 18;

            _sheet.Range($"B{_currentLine + 37}:E{_currentLine + 37}").Merge().Value = "Prepared For:";
            _sheet.Range($"F{_currentLine + 37}:J{_currentLine + 37}").Merge().Value = "Company Name:";
            _sheet.Row(_currentLine + 38).Height = 5;
            _sheet.Range($"F{_currentLine + 39}:J{_currentLine + 39}").Merge().Value = "Company Address:";
            _sheet.Row(_currentLine + 40).Height = 5;
            _sheet.Range($"F{_currentLine + 41}:J{_currentLine + 41}").Merge().Value = "Company Contact:";
            _sheet.Row(_currentLine + 42).Height = 5;
            _sheet.Range($"F{_currentLine + 43}:G{_currentLine + 43}").Merge().Value = "Phone:";
            _sheet.Range($"P{_currentLine + 43}:Q{_currentLine + 43}").Merge().Value = "Fax:";
            r = _sheet.Range($"K{_currentLine + 37}:X{_currentLine + 37}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"K{_currentLine + 39}:X{_currentLine + 39}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"K{_currentLine + 41}:X{_currentLine + 41}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"H{_currentLine + 43}:N{_currentLine + 43}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"R{_currentLine + 43}:X{_currentLine + 43}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            _sheet.Range($"B{_currentLine + 46}:E{_currentLine + 46}").Merge().Value = "Prepared By:";
            _sheet.Range($"F{_currentLine + 46}:J{_currentLine + 46}").Merge().Value = "Company Name:";
            _sheet.Row(_currentLine + 47).Height = 5;
            _sheet.Range($"F{_currentLine + 48}:J{_currentLine + 48}").Merge().Value = "Company Address:";
            _sheet.Row(_currentLine + 49).Height = 5;
            _sheet.Range($"F{_currentLine + 50}:J{_currentLine + 50}").Merge().Value = "Company Contact:";
            _sheet.Row(_currentLine + 51).Height = 5;
            _sheet.Range($"F{_currentLine + 52}:G{_currentLine + 52}").Merge().Value = "Phone:";
            _sheet.Range($"P{_currentLine + 52}:Q{_currentLine + 52}").Merge().Value = "Fax:";
            r = _sheet.Range($"K{_currentLine + 46}:X{_currentLine + 46}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"K{_currentLine + 48}:X{_currentLine + 48}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"K{_currentLine + 50}:X{_currentLine + 50}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"H{_currentLine + 52}:N{_currentLine + 52}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            r = _sheet.Range($"R{_currentLine + 52}:X{_currentLine + 52}").Merge();
            r.Style.Fill.BackgroundColor = PEACH_FILL;
            r.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private void CreateAseAerialHeaders(string[] data, Mappings map)
        {
            IXLRange r;

            // set row heights
            for (var i = 0; i < 6; i++)
            {
                _sheet.Row(_currentLine + i).Height = 12;
            }
            _sheet.Row(_currentLine + 2).Height = 2;

            r = _sheet.Range($"A1:ZZ99");
            r.Style.Font.FontName = "Tahoma";
            r.Style.Font.FontSize = 7;
            r.Style.Fill.BackgroundColor = XLColor.White;

            CreateLabelCell(_sheet.Range($"B{_currentLine + 0}:F{_currentLine + 1}"), "OIL SANDS EXPLORATION (OSE) AERIAL ASSESSMENT");
            CreateLabelCell(_sheet.Range($"B{_currentLine + 3}:G{_currentLine + 3}"), "Corehole Location");
            CreateLabelCell(_sheet.Range($"B{_currentLine + 4}:G{_currentLine + 4}"), "Complete Surface Location(s):");
            r = CreateLabelCell(_sheet.Range($"B{_currentLine + 5}:B{_currentLine + 5}"), "Qrt");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"C{_currentLine + 5}:C{_currentLine + 5}"), "LSD");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"D{_currentLine + 5}:D{_currentLine + 5}"), "Sec");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"E{_currentLine + 5}:E{_currentLine + 5}"), "Twp");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"F{_currentLine + 5}:F{_currentLine + 5}"), "Rng");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"G{_currentLine + 5}:G{_currentLine + 5}"), "M");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"H{_currentLine + 0}:N{_currentLine + 0}"), "Operator Name");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"H{_currentLine + 1}:N{_currentLine + 1}"), data[_map.Operator.Offset]);
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Fill.BackgroundColor = GREY_FILL;
            r = CreateLabelCell(_sheet.Range($"O{_currentLine + 0}:W{_currentLine + 0}"), "Unique ID / License No");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"O{_currentLine + 1}:W{_currentLine + 1}"), "Unique ID / License No");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Fill.BackgroundColor = GREY_FILL;
            r = CreateLabelCell(_sheet.Range($"X{_currentLine + 0}:AF{_currentLine + 0}"), "Disposition #");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"X{_currentLine + 1}:AF{_currentLine + 1}"), data[_map.Disposition.Offset]);
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Fill.BackgroundColor = GREY_FILL;

            r = CreateLabelCell(_sheet.Range($"H{_currentLine + 3}:S{_currentLine + 4}"), "Corehole Information");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"H{_currentLine + 5}:L{_currentLine + 5}"), "Parameter");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"M{_currentLine + 5}:S{_currentLine + 5}"), "Description");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"T{_currentLine + 3}:W{_currentLine + 5}"), "Lanscape Comment");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"X{_currentLine + 3}:AA{_currentLine + 5}"), "Vegetation Comments (i.e., Species ID's)");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"AB{_currentLine + 3}:AG{_currentLine + 5}"), "Photo #1");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r = CreateLabelCell(_sheet.Range($"AH{_currentLine + 3}:AM{_currentLine + 5}"), "Photo #2");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            r = CreateLabelCell(_sheet.Range($"AH{_currentLine + 0}:AL{_currentLine + 0}"), "123234");
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Fill.BackgroundColor = GREY_FILL;
            r = CreateLabelCell(_sheet.Range($"AH{_currentLine + 1}:AL{_currentLine + 1}"), data[_map.Criteria.Offset]);
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Fill.BackgroundColor = GREY_FILL;

            _currentLine += 6;
        }

        private void CreateAseAerialSection(int sectionNumber, string[] data, Mappings map)
        {
            // set row heights
            for (var i = 0; i < 12; i++)
            {
                _sheet.Row(_currentLine + i).Height = 12;
            }

            //
            // Legal Location
            //
            var locationPieces = data[_map.LegalLocation.Offset].Split(_map.LegalLocation.Seperators.ToArray());

            CreateDropdown(_sheet.Range($"B{_currentLine + 0}"), DataLists.Names.LAND_QUARTER);
            CreateDropdown(_sheet.Range($"C{_currentLine + 0}"), DataLists.Names.LAND_LEGAL_SUBDIVISION, locationPieces[0]);
            CreateDropdown(_sheet.Range($"D{_currentLine + 0}"), DataLists.Names.LAND_SECTION, locationPieces[1]);
            CreateDropdown(_sheet.Range($"E{_currentLine + 0}"), DataLists.Names.LAND_TOWNSHIP, locationPieces[2]);
            CreateDropdown(_sheet.Range($"F{_currentLine + 0}"), DataLists.Names.LAND_RANGE, locationPieces[3]);
            CreateDropdown(_sheet.Range($"G{_currentLine + 0}"), DataLists.Names.LAND_MERIDIAN, locationPieces[4]);

            //
            // notes
            //
            var notes = "";
            foreach (var note in _map.Notes)
            {
                notes += $"{note.Prefix}{data[note.Offset]}{note.Suffix}\n";
            }
            CreateLabelCell(_sheet.Range($"B{_currentLine + 1}:C{_currentLine + 1}"), "Notes:");
            var noteCell = _sheet.Range($"B{_currentLine + 2}:G{_currentLine + 11}").Merge();
            noteCell.Style.Fill.BackgroundColor = PEACH_FILL;
            noteCell.Style.Alignment.WrapText = true;
            noteCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            noteCell.Value = notes.Trim();

            // labels
            CreateLabelCell(_sheet.Range($"H{_currentLine + 0}:L{_currentLine + 0}"), "Assessment Type & ID #");
            CreateLabelCell(_sheet.Range($"H{_currentLine + 1}:L{_currentLine + 1}"), "Aerial Assessment #");
            CreateLabelCell(_sheet.Range($"H{_currentLine + 2}:L{_currentLine + 2}"), "Natural Sub-region");
            CreateLabelCell(_sheet.Range($"H{_currentLine + 3}:L{_currentLine + 3}"), "Ecosite Phase (EP) Code");
            CreateLabelCell(_sheet.Range($"H{_currentLine + 4}:L{_currentLine + 4}"), "Soil Zone");
            var range = CreateLabelCell(_sheet.Range($"H{_currentLine + 5}:K{_currentLine + 6}"), "Soil Series");
            range.Style.Border.RightBorder = XLBorderStyleValues.None;
            CreateLabelCell(_sheet.Range($"H{_currentLine + 7}:L{_currentLine + 7}"), "Construction Practice:");
            CreateLabelCell(_sheet.Range($"H{_currentLine + 8}:L{_currentLine + 8}"), "Planted / Natural Recovery");
            CreateLabelCell(_sheet.Range($"H{_currentLine + 9}:L{_currentLine + 11}"), "UTM Coordinates (NAD83)");

            var cell = _sheet.Cell($"L{_currentLine + 5}");
            cell.Value = "1 )";
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            cell = _sheet.Cell($"L{_currentLine + 6}");
            cell.Value = "2 )";
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            //
            // more dropdowns
            //
            CreateDropdown(_sheet.Range($"M{_currentLine + 0}:S{_currentLine + 0}"), DataLists.Names.FACILITY_ASSESMENT_TYPE); // ?
            CreateIntegerCell(_sheet.Range($"M{_currentLine + 1}:N{_currentLine + 1}"), sectionNumber);
            CreateDropdown(_sheet.Range($"M{_currentLine + 2}:S{_currentLine + 2}"), DataLists.Names.PROVINCIAL_NATURAL_SUBREGION, data[_map.SubRegion.Offset]);
            CreateDropdown(_sheet.Range($"M{_currentLine + 3}:S{_currentLine + 3}"), DataLists.Names.PROVINCIAL_ECOSITE, data[_map.EcoSite.Offset]);
            CreateDropdown(_sheet.Range($"M{_currentLine + 4}:S{_currentLine + 4}"), DataLists.Names.SOIL_GREAT_GROUP, data[_map.SoilZone.Offset]);
            CreateDropdown(_sheet.Range($"M{_currentLine + 5}:S{_currentLine + 5}"), DataLists.Names.SOIL_SERIES); // ?
            CreateDropdown(_sheet.Range($"M{_currentLine + 6}:S{_currentLine + 6}"), DataLists.Names.SOIL_SERIES); // ?
            CreateDropdown(_sheet.Range($"M{_currentLine + 7}:S{_currentLine + 7}"), DataLists.Names.FACILITY_DISTURBANCE_CONSTRUCTION, data[_map.LevelOfDistruption.Offset]);
            CreateDropdown(_sheet.Range($"M{_currentLine + 8}:S{_currentLine + 8}"), DataLists.Names.FACILITY_REVEGETATION_APPROACH, data[_map.Regeneration.Offset]);


            //
            // Lat and Lon (UTM projection information)
            //
            double.TryParse(data[_map.Latitude.Offset], out var latitude);
            double.TryParse(data[_map.Longitude.Offset], out var longitude);
            var coordinates = new Coordinate(latitude, longitude);
            CreateLabelCell(_sheet.Range($"M{_currentLine + 9}:O{_currentLine + 9}"), "Zone");
            CreateLabelCell(_sheet.Range($"M{_currentLine + 10}:O{_currentLine + 10}"), "Northing");
            CreateLabelCell(_sheet.Range($"M{_currentLine + 11}:O{_currentLine + 11}"), "Easting");
            CreateDropdown(_sheet.Range($"P{_currentLine + 9}:R{_currentLine + 9}"), DataLists.Names.UTM_ZONE, coordinates.UTM.LongZone.ToString());
            CreateDecimalCell(_sheet.Range($"P{_currentLine + 10}:R{_currentLine + 10}"), coordinates.UTM.Northing);
            CreateDecimalCell(_sheet.Range($"P{_currentLine + 11}:R{_currentLine + 11}"), coordinates.UTM.Easting);

            //
            // Landscape Comments
            //
            
            CreateLabelCell(_sheet.Range($"T{_currentLine + 0}:W{_currentLine + 11}"), data[_map.LandscapeComments.Offset]);

            //
            // Vegetation Comments
            //
            var vegetationComments = "";
            foreach (var comment in _map.VegetationComments)
            {
                vegetationComments += $"{comment.Prefix}{data[comment.Offset]}{comment.Prefix}\n";
            }
            CreateLabelCell(_sheet.Range($"X{_currentLine + 0}:AA{_currentLine + 11}"), vegetationComments);

            //
            // Images
            //
            CreateLabelCell(_sheet.Range($"AB{_currentLine + 0}:AG{_currentLine + 11}"), "");
            CreateLabelCell(_sheet.Range($"AH{_currentLine + 0}:AM{_currentLine + 11}"), "");

            var imageData = data[_map.ASEAerialPhotos.Offset].Split(_map.ASEAerialPhotos.Seperators.ToArray());
            if (imageData.Length > 0)
            {
                var key = imageData[0].Trim();
                if (_input.Photos.ContainsKey(key))
                {
                    var imc = _sheet.Cell($"AB{_currentLine + 0}");
                    var pic = _sheet.AddPicture(_input.Photos[key].FullName).MoveTo(imc);
                    var (w, h) = ScaleImage(pic, 180, 190);
                    pic.WithSize(w, h);
                }
            }
            if (imageData.Length > 1)
            {
                var key = imageData[1].Trim();
                if (_input.Photos.ContainsKey(key))
                {
                    var imc = _sheet.Cell($"AH{_currentLine + 0}");
                    var pic = _sheet.AddPicture(_input.Photos[key].FullName).MoveTo(imc);
                    var (w, h) = ScaleImage(pic, 180, 190);
                    pic.WithSize(w, h);
                }
            }

            //
            // fills
            //
            _sheet.Range($"D{_currentLine + 1}:G{_currentLine + 1}").Merge().Style.Fill.BackgroundColor = BLANK_FILL;
            _sheet.Range($"O{_currentLine + 1}:S{_currentLine + 1}").Merge().Style.Fill.BackgroundColor = BLANK_FILL;
            _sheet.Range($"S{_currentLine + 9}:S{_currentLine + 11}").Merge().Style.Fill.BackgroundColor = BLANK_FILL;

            _currentLine += 12;
        }

        public static (int newWidth, int newHeight) ScaleImage(IXLPicture pic, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / pic.OriginalWidth;
            var ratioY = (double)maxHeight / pic.OriginalHeight;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(pic.OriginalWidth * ratio);
            var newHeight = (int)(pic.OriginalHeight * ratio);

            return (newWidth, newHeight);
        }

        public Stream GetStream()
        {
            var stream = new MemoryStream();
            _book.Protect();
            _book.SaveAs(stream);
            return stream;
        }
    }
}
