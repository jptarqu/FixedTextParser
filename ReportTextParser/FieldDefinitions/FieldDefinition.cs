using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportTextParser.FieldDefinitions
{
    public interface IFieldDefinition
    {
        string ConvertField(string original_line);
    }
    public class TextFieldDefinition : IFieldDefinition
    {
        public int StartPos { get; set; }
        public int Lenght { get; set; }
        public bool Trim { get; set; }
        public string ConvertField(string original_line)
        {
            string result = "";
           // if (original_line.Length > (StartPos - 1) + Lenght)
          //  {

            if (original_line.Length > (StartPos - 1))
            {
                int lenght_to_take = Lenght;
                if (original_line.Length < (StartPos - 1) + Lenght)
                    lenght_to_take = original_line.Length - (StartPos - 1);
                result = original_line.Substring(StartPos - 1, lenght_to_take);
                if (Trim)
                    result = result.Trim();
            }
          //  }
            return result;
        }

        public TextFieldDefinition(int start_pos, int lenght, bool trim)
        {
            StartPos = start_pos;
            Lenght = lenght;
            Trim = trim;
        }

    }

    public class NumericFieldDefinition : IFieldDefinition
    {
        public int StartPos { get; set; }
        public int Lenght { get; set; }

        public string ConvertField(string original_line)
        {
            string result = "0.00";
            int chars_to_take = Lenght;

            int length_required = (StartPos - 1) + chars_to_take;
            if (original_line.Length < length_required)
            {
                chars_to_take -= length_required - original_line.Length;
            }

            if (original_line.Length > (StartPos - 1))
            {
                result = original_line.Substring(StartPos - 1, chars_to_take);
                result = result.Trim();
                //handle negatives
                if (result.Length > 0 && result[result.Length - 1] == '-')
                {
                    result = "-" + result.Substring(0,result.Length - 1) ;
                }
            }
            return result;
        }

        public NumericFieldDefinition(int start_pos, int lenght)
        {
            StartPos = start_pos;
            Lenght = lenght;
        }

    }

    public class DefaultFieldDefinition : IFieldDefinition
    {
        public string Text { get; set; }
        public string ConvertField(string original_line)
        {
            return Text;
        }
        public DefaultFieldDefinition(string text)
        {
            Text = text;
        }

    }
    public class IncrementingFieldDefinition : IFieldDefinition
    {
        public int StartNumber { get; set; }
        public int Increment { get; set; }
        public int Size { get; set; }

        public string ConvertField(string original_line)
        {
            StartNumber += Increment;
            return StartNumber.ToString().PadRight(Size, '0');
        }

        public IncrementingFieldDefinition(int start_number, int increment, int size)
        {
            StartNumber = start_number;
            Increment = increment;
            Size = size;
        }

    }
}
