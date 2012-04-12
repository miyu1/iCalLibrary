// Copyright 2011 Miyako Komooka
using System;
using System.IO;
using System.Collections.Generic;
// using System.Linq;
using System.Text;

namespace iCalLibrary // based on rfc5545
{
    using Parameter;

    public class iCalReader
    {
        TextReader reader = null;
        String cachedLine = null;

        public iCalReader( String filename ) 
        {
            this.reader = new StreamReader( filename,
                                            Encoding.GetEncoding("UTF-8") );
        }

        public iCalReader( TextReader reader )
        {
            this.reader = reader;
        }

        public String ReadLine()
        {
            String line = null;
            String contentLine = null;

            if( this.cachedLine != null ){
                contentLine = this.cachedLine;
                this.cachedLine = null;
            }
            while( ( line = this.reader.ReadLine() ) != null ){
                if ( line == null || line.Length == 0)
                {
                    continue;
                }
                if( line[0] == ' ' || line[0] == '\t' ){
                    contentLine += line.Substring( 1 );
                } else {
                    if( contentLine == null ){
                        contentLine = line;
                    } else {
                        this.cachedLine = line;
                        break;
                    }
                }
            }
            return contentLine;
        }

        public iCalLineContent ReadContent()
        {

            // contentLine = name *(";" param-name '=' paramvalue, *( "," paramvalue ) ) ":" value

            // name, param-name, is alphabe or digit or '-'
            // param-value is any char except double-quote("), comma(,), colon(:), semi-colon(;)
            // param-value may quoted. in that case, any char except
            // double-quote(") inside.
            // name of property, param, enumed value and param value is case-insensitive

            String contentLine = this.ReadLine();
            if( contentLine == null )
                return null;

            int mode = 1;
            iCalLineContent content = new iCalLineContent();
            content.original = contentLine;
            String paramName = "";
            String paramValue = "";
            iCalParameterFactory factory = new iCalParameterFactory();
            iCalParameter param = null;
            
            for( int i = 0 ; i < contentLine.Length; i++ ){
                Char c = contentLine[i];
                if ( mode == 1 ){ // parsing name
                    if( c == ':' ){
                        mode = 2;
                    } else if ( c == ';' ) {
                        mode = 3;
                        // param = new iCalParameter();
                        paramName = "";
                    } else {
                        content.Name += Char.ToLower( c );
                    }
                } else if ( mode == 2 ) { // value
                    content.Value = contentLine.Substring( i );
                    // end of line parse
                    break;
                } else if ( mode == 3 ) { // parameter name
                    if( c == '=' ){
                        mode = 4;
                        // paramName = param.Name;
                        param = factory.Create( paramName );
                        param.Name = paramName;
                        content.Params[ paramName ] = param;
                        paramValue = "";
                    } else if( c == ';' ) { // irregular
                        param = factory.Create( paramName );
                        content.Params[ paramName ] = param;
                        param = null;
                    } else if( c == ':' ) { // irregular
                        param = factory.Create( paramName );
                        content.Params[ paramName ] = param;
                        mode = 2;
                    } else {
                        paramName += Char.ToLower( c );
                    }
                } else if ( mode == 4 ){ // parameter value
                    if( c == ',' ){
                        content.Params[ paramName ].Values.Add( paramValue );
                        paramValue = "";
                    } else if( c == ';' ) { 
                        content.Params[ paramName ].Values.Add( paramValue );
                        param = new iCalParameter();
                        paramValue = "";
                        paramName = "";
                        mode = 3;
                    } else if( c == ':' ) { 
                        content.Params[paramName].Values.Add(paramValue);
                        mode = 2;
                    } else if( c == '\"' ){
                        mode = 5;
                    } else {
                        paramValue += c;
                    }
                } else if ( mode == 5 ){ // double quoted parameter value
                    if( c == '\"' ){
                        mode = 4;
                    } else {
                        // parameter value in double-quote is case sensitive
                        paramValue += c; 
                    }
                }
            }

            return content;
        }

        public void Close()
        {
            this.reader.Close();
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }
    }

    public class iCalLineContent
    {
        public String Name;
        public Dictionary<String,iCalParameter> Params =
            new Dictionary<String,iCalParameter>();
        public String Value;

        public String original; // just for debug
    }

}
