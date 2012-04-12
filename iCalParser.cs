// Copyright 2011 Miyako Komooka
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace iCalLibrary // based on rfc5545
{
    using Component;

    public class iCalParser
    {
        iCalendarCollection Collection;
        iCalComponentFactory factory;
        iCalComponent Parent = null;
        iCalComponent Current = null;

        public iCalendarCollection ParseFile( String filename )
        {
            iCalSimpleParser sParser = new iCalSimpleParser();
            sParser.ComponentStart += this.ComponentStartHandler;
            sParser.ComponentEnd += this.ComponentEndHandler;
            sParser.Property += this.PropertyHandler;

            this.Collection = new iCalendarCollection();
            this.Current = this.Collection;
            this.Parent = null;

            this.factory = new iCalComponentFactory();

            sParser.ParseFile( filename );

            return this.Collection;
        }

        public iCalendarCollection ParseStream(TextReader reader) {
            iCalSimpleParser sParser = new iCalSimpleParser();
            sParser.ComponentStart += this.ComponentStartHandler;
            sParser.ComponentEnd += this.ComponentEndHandler;
            sParser.Property += this.PropertyHandler;

            this.Collection = new iCalendarCollection();
            this.Current = this.Collection;
            this.Parent = null;

            this.factory = new iCalComponentFactory();

            sParser.ParseStream( reader );

            return this.Collection;
        }
            
        public void ComponentStartHandler( Object sender,
                                           iCalParserEventArgs args )
        {
            iCalLineContent content = args.Content;

            this.Parent = this.Current;

            this.Current = factory.Create( content.Value );

            this.Current.Parent = this.Parent;
        }

        public void ComponentEndHandler( Object sender,
                                        iCalParserEventArgs args )
        {
            if( this.Parent != null ){
                this.Parent.AddChild( this.Current );
            }
            this.Current = this.Current.Parent;
            this.Parent = this.Current.Parent;
        }
        
        public void PropertyHandler ( Object sender, iCalParserEventArgs args )
        {
            iCalLineContent content = args.Content;
            this.Current.SetProperty( content );
        }
        
    }


    // event based iCal parser
    public class iCalSimpleParser 
    {
        public event EventHandler<iCalParserEventArgs> ComponentStart;
        public event EventHandler<iCalParserEventArgs> ComponentEnd;
        public event EventHandler<iCalParserEventArgs> Property;
        
        public void ParseFile( String filename )
        {
            iCalReader icalReader = new iCalReader( filename );
            this.Parse( icalReader );
        }

        public void ParseStream( TextReader reader )
        {
            iCalReader icalReader = new iCalReader( reader );
            this.Parse( icalReader );
        }

        protected void Parse( iCalReader reader )
        {
            iCalLineContent content = null;
            while( ( content = reader.ReadContent() ) != null ){
                iCalParserEventArgs args = new iCalParserEventArgs( content );

                if( content.Name == "begin" ){
                    String value = content.Value.ToLower();
                    
                    if( this.ComponentStart != null ){
                        this.ComponentStart( this, args );
                    }

                } else if( content.Name == "end" ) {
                    String value = content.Value.ToLower();

                    if( this.ComponentEnd != null ){
                        this.ComponentEnd( this, args );
                    }
                } else {
                    if( this.Property != null ){
                        this.Property( this, args );
                    }
                }
            }
        }
    }

    public class iCalParserEventArgs : EventArgs
    {
        public iCalLineContent Content;

        public iCalParserEventArgs( iCalLineContent content ) {
            this.Content = content;
        }
    }
}