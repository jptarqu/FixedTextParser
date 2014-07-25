using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportTextParser.FieldDefinitions
{
    public class TextTransformer
    {
        public List<LineDefinition> LineDefinitions { get; set; }
        public string RegexDetection { get; set; }

        public TextTransformer()
        {
        }

        public string TransformFileContents(string file_name)
        {
            return TransformContents(new StreamReader(file_name));
        }

        public string TransformContents(string source_data)
        {
            return TransformContents(new StringReader(source_data));
        }

        public string TransformContents(TextReader source_data)
        {
            StringBuilder sb = new StringBuilder();
            
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
            using (TextReader sr = source_data)
                {
                    String line;
                    string header_fields_values = "";
                    // parse every line of the report
                    while ((line = sr.ReadLine()) != null)
                    {
                        LineDefinition matched_line_definition = null;
                        //Find the matching regex in the line definitions
                        foreach (var line_definition in LineDefinitions)
                        {
                            if (Regex.IsMatch(line, line_definition.RegexDetection, RegexOptions.IgnoreCase))
                            {
                                matched_line_definition = line_definition;
                                break;
                            }
                        }
                        if (matched_line_definition != null)
                        {
                            if (!matched_line_definition.IgnoreLine)
                            {
                                for (int blank_line_num = 1; blank_line_num <= matched_line_definition.TotBlankLinesToPrepend; blank_line_num++)
                                {
                                    sb.AppendLine();
                                }
                                var converted_line = matched_line_definition.ConvertLine(line);
                                if (matched_line_definition.IsHeader)
                                {
                                //save the records to append to the other records
                                    header_fields_values = converted_line;
                                }
                            if (!matched_line_definition.NotPrintable) //prevent the headers from repeating in the same line
                                {
                                    if (matched_line_definition.AppendAsColumns)
                                    {
                                        sb.Append(converted_line);
                                    }
                                    else
                                    {
                                    sb.Append(Environment.NewLine + (header_fields_values + converted_line).TrimStart('\t'));
                                    }
                                }
                                
                            }
                        }
                    }
                }
            return sb.ToString();
        }
    }
}
