using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ReportTextParser.FieldDefinitions
{
    public class TransformerManager : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on the contact changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises a property changed notification for the specified property name.
        /// </summary>
        /// <param name="propName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        protected bool SetValue<T>(ref T property, T value, string propertyName)
        {
            if (Object.Equals(property, value))
            {
                return false;
            }
            property = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public List<TextTransformer> Transformers;

        private String _OutputFile;
        public String OutputFile
        {
            get
            {
                return _OutputFile;
            }
            set
            {
                SetValue(ref this._OutputFile, value, "OutputFile");
                OnPropertyChanged("CanTransform");
            }
        }

        private String _InputFile;
        public String InputFile
        {
            get
            {
                return _InputFile;
            }
            set
            {
                SetValue(ref this._InputFile, value, "InputFile");
                OnPropertyChanged("CanTransform");
            }
        }

        public Boolean CanTransform
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(InputFile)); //&& (!string.IsNullOrWhiteSpace(OutputFile));
            }
        }

        /// <summary>
        /// Collects all the XML files that are named configuration* and loads any objects called "transformer" from them.
        /// </summary>
        public void Initialize()
        {
            //Transformers = new List<TextTransformer>();
            //var files = Directory.GetFiles("configuration");
            //foreach (var conf_file in files)
            //{
            //    IApplicationContext context = new XmlApplicationContext(conf_file);
            //    var transformer = (TextTransformer)context.GetObject("transformer");
            //    Transformers.Add(transformer);
            //}
        }

        public string TransformFileContents()
        {
            return TransformFileContents(InputFile, OutputFile);
        }


        public string TransformContents(string data_contents)
        {
            TextTransformer transformer_to_use = null;

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (var sr = new StringReader(data_contents))
            {
                String first_line;
                first_line = sr.ReadLine();
                //Find the matching regex in the line definitions
                foreach (var transformer in Transformers)
                {
                    if (Regex.IsMatch(first_line, transformer.RegexDetection, RegexOptions.IgnoreCase))
                    {
                        transformer_to_use = transformer;
                        break;
                    }
                }
            }

            string new_file_contents = transformer_to_use.TransformContents(data_contents);
            return new_file_contents;
        }
        public string TransformFileContents(string file_name, string out_filename)
        {
                    TextTransformer transformer_to_use = null;
         
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(file_name))
                {
                    String first_line;
                    first_line = sr.ReadLine();
                    //Find the matching regex in the line definitions
                    foreach (var transformer in Transformers)
                    {
                        if (Regex.IsMatch(first_line, transformer.RegexDetection, RegexOptions.IgnoreCase))
                        {
                            transformer_to_use = transformer;
                            break;
                        }
                    }
                }

                string new_file_contents=  transformer_to_use.TransformFileContents(file_name);
                // Write each directory name to a file.

                if (string.IsNullOrWhiteSpace(out_filename))
                {
                    out_filename = Path.Combine(Path.GetDirectoryName(file_name), transformer_to_use.RegexDetection + ".xls");
                }
                using (StreamWriter sw = new StreamWriter(out_filename))
                {
                    sw.Write(new_file_contents);
                }
                return out_filename;
        }
    }
}
