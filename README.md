# iCalLibrary

## Introduction 
iCalLibrary is iCalendar format (RFC5545) library
for Microsoft .NET / Silverlight

Tested on .NET framework 4 or silverlight 5

As of April 2012, it is only able to read from iCalendar format file.
Writing to iCalendar file is not yet implemented.

## Usage
    using iCalLibrary;

    iCalParser parser = new iCalParser();  
    iCalendarCollection iColl = parser.ParseStream( <TextReader> );
                                 or
                                parser.ParseFile( <filename> );

    List<iCalEvent> events = iColl.GetEventByDay( year, month, day );
    List<iCalToDo>  todos = iColl.GetToDoByDay( year, month, day, 
                                                <includeNoDateEntryFlag> );

If includeNoDateEntryFlag is set to false, todo entry which does not have
start date or due date is not included
  