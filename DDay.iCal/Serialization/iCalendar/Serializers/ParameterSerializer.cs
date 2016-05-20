﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DDay.iCal.Serialization.iCalendar
{
    public class ParameterSerializer :
        SerializerBase
    {
        #region Constructors

        public ParameterSerializer()
        {
        }

        public ParameterSerializer(ISerializationContext ctx) : base(ctx)
        {
        }

        #endregion

        #region Overrides

        public override Type TargetType
        {
            get { return typeof(CalendarParameter); }
        }

        public override string SerializeToString(object obj)
        {
            ICalendarParameter p = obj as ICalendarParameter;
            if (p != null)
            {
                string result = p.Name + "=";
                string value = string.Join(",", p.Values.ToArray());

                // "Section 3.2:  Property parameter values MUST NOT contain the DQUOTE character."
                // Therefore, let's strip any double quotes from the value.                
                value = value.Replace("\"", string.Empty);

                // Surround the parameter value with double quotes, if the value
                // contains any problematic characters.
                if (value.IndexOfAny(new char[] { ';', ':', ',' }) >= 0)
                    value = "\"" + value + "\"";
                return result + value;
            }
            return string.Empty;
        }

        public override object Deserialize(TextReader tr)
        {
            // Create a lexer for our text stream
            iCalLexer lexer = new iCalLexer(tr);
            iCalParser parser = new iCalParser(lexer);

            // Get our serialization context
            ISerializationContext ctx = SerializationContext;

            // Parse the component!
            ICalendarParameter p = parser.parameter(ctx, null);

            // Close our text stream
            tr.Dispose();

            // Return the parsed parameter
            return p;
        } 

        #endregion
    }
}
