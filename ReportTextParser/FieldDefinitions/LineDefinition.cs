using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportTextParser.FieldDefinitions
{
    public class LineDefinition
    {
        public string RegexDetection { get; set; }
        public bool IgnoreLine { get; set; }
        public bool AppendAsColumns { get; set; }
        public int TotBlankLinesToPrepend { get; set; }
        public List<IFieldDefinition> FieldDefinitions { get; set; }
        public bool IsHeader { get; set; }
        public bool NotPrintable { get; set; }

        public LineDefinition()
        {
            FieldDefinitions = new List<IFieldDefinition>();
        }

        /// <summary>
        /// Converts a raw line into a tab delimited line. The fields are taken from the FieldDefinitions
        /// </summary>
        /// <param name="input_line"></param>
        /// <returns></returns>
        public string ConvertLine(string input_line)
        {
            string result = "";
            foreach (var field in FieldDefinitions)
            {
                result += '\t' + field.ConvertField(input_line);
            }
            return result;//.TrimStart('\t');
        }
    }
}
